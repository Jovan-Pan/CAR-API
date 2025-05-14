using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using Contracts.Repository;
using Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.ParamRequest;
using Entities.CAR;
using Services.Contracts.CAR;
using Entities.Infrastructure;
using Services.Helper;
using Services.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.SqlServer.Server;
using Entities.MasterData;
using System.Collections;
using System.Net.Mail;
using System.Drawing.Imaging;
using System.Drawing;

namespace Services.CAR
{
    internal sealed class SendMailSettingService(IDataManager data, IMDMRepository mdm, IMasterDataApi mdmP, ICacheManager memCache, ILocalizationService localization): ISendMailSettingService
    {
        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> GetaData(SendMailSettingParam param)
        {
            var result = await data.SMS.GetaData(param);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<string>> sendemail(IssueSubmissionParameters mydata)
        {
            string mailmsg = "";
            SendMailSettingParam mlPram = new SendMailSettingParam();
            mlPram.plant = mydata.UserPlant;
            mlPram.ActionType = mydata.mailactionType;
            var SendMlSet = await data.SMS.GetaData(mlPram);
            if (SendMlSet != null)
            {
                if (SendMlSet.Any())
                {
                    bool issendemail = SendMlSet.FirstOrDefault().isSendEmail;
                    if (issendemail == true)
                    {
                        SendEmailParam mailparam = new SendEmailParam();
                        var globalmailMaster = await mdm.GetTGlobalEmailSetting(mydata.UserPlant, mydata.mailWStatus);
                        var userSubsFormMaster = await mdm.GetSystemvsUservsEmailSubscribeForm(mydata.UserPlant, mydata.mailWStatus, mydata.Dept);
                        List<string> recipentList = userSubsFormMaster.Select(form => form.UseEmail).ToList();

                        var MailToCC = await data.IFR.GetMailtocc(mydata.FormNumber);
                        List<string> MailtoccList = MailToCC.ToList();

                        if (mydata.Dept == "VEND")
                        {
                           var userSubsFormMasterVendor = await mdm.GetSystemvsUservsEmailSubscribeFormVendor(mydata.VendorCode, mydata.UserPlant, mydata.mailWStatus, mydata.Dept);
                           recipentList.AddRange(userSubsFormMasterVendor.Select(form => form.UseEmail).ToList());
                        }

                        var IssuerIds = await data.IFR.getIssuerId(mydata.UserPlant, mydata.FormNumber);
                        var GetissuerEmail = await mdm.GetissuerEmail(mydata.UserPlant, IssuerIds);
                        //List<string> recipentList = userSubsFormMaster.Select(form => form.UseEmail).ToList();
                        //if (mydata.Dept == "VEND")
                        //{
                        //    recipentList.AddRange(userSubsFormMasterVendor.Select(form => form.UseEmail).ToList());
                        //}
                        recipentList.AddRange(GetissuerEmail);
                        recipentList = recipentList.Distinct().ToList();

                        if (globalmailMaster.Count() == 0)
                        {
                            mailmsg = "Data Maill Content Not Maintain";
                        }
                        else if (recipentList.Count() == 0)
                        {
                            mailmsg = "no email recipients are set up";
                        }
                        if (mailmsg.Length == 0)
                        {
                            mailparam.FromName = "CAR System";
                            mailparam.FromAddress = globalmailMaster.FirstOrDefault().FromMailaddress;

                            string recipent = string.Join(";", recipentList);
                            if (!string.IsNullOrEmpty(globalmailMaster.FirstOrDefault()?.ReplyMailid))
                            {
                                MailtoccList.Add(globalmailMaster.FirstOrDefault().ReplyMailid);
                            }



                            mailparam.Recipient = recipent;

                            mailparam.Subject = globalmailMaster.FirstOrDefault().EmailSubject;

                            string body = globalmailMaster.FirstOrDefault().Emailbody;
                            body = body.Replace("@UserAction", mydata.sendmailUserAction);

                            string Datadetails = MailBodyContentDetail.mailBodyContentDet;
                            Datadetails = Datadetails.Replace("@Plant", mydata.UserPlant == null ? "" : mydata.UserPlant.ToString());
                            Datadetails = Datadetails.Replace("@FormType", mydata.FormType);
                            Datadetails = Datadetails.Replace("@FormNumber", mydata.FormNumber);
                            if (mydata.FormType == "NCR" || mydata.FormType == "QFR")
                            {
                                Datadetails = Datadetails.Replace("@Formlink", globalmailMaster.FirstOrDefault().Emaillink + $"/pages/issueSubmission?formnumber={mydata.FormNumber}&amp;useraction={mydata.userAction}");
                            }
                            else
                            {
                                Datadetails = Datadetails.Replace("@Formlink", globalmailMaster.FirstOrDefault().Emaillink + $"/pages/DynamicNewForm?formnumber={mydata.FormNumber}&amp;useraction={mydata.userAction}");
                            }
                            Datadetails = Datadetails.Replace("@Status", mydata.IssueStatus?.Replace("PDA-DECISION", "CAR ISSUING"));
                            Datadetails = Datadetails.Replace("@DetectionDate", mydata.DetectionDate?.ToString("dd-MM-yyyy"));
                            Datadetails = Datadetails.Replace("@Product", mydata.Product);
                            //Datadetails = Datadetails.Replace("@Model", mydata.Model);
                            //Datadetails = Datadetails.Replace("@Materialtype", mydata.MaterialType);
                            Datadetails = Datadetails.Replace("@MaterialCode", mydata.MaterialCode);
                            Datadetails = Datadetails.Replace("@MaterialDesc", mydata.MaterialDesc == null ? "N.A." : mydata.MaterialDesc.ToString());
                            Datadetails = Datadetails.Replace("@SamplingCheck", mydata.NcRatio == null ? "0" : mydata.NcRatio.ToString());
                            Datadetails = Datadetails.Replace("@Dept", mydata.Dept == null 
                            ? "<td style=\"border: 1px solid black; padding: 8px;\">Dept</td ><td style=\"border: 1px solid black; padding: 8px;\">N.A.</td>" 
                            : "<td style=\"border: 1px solid black; padding: 8px;\"><b>Dept</b></td><td style=\"border: 1px solid black; padding: 8px;\"><b>"
                            + mydata.Dept + "</b></td>");
                            Datadetails = Datadetails.Replace("@Vendor", mydata.VendorCode == null
                            ? "<td style=\"border: 1px solid black; padding: 8px;\">Vendor</td><td style=\"border: 1px solid black; padding: 8px;\">N.A.</td>"
                            : "<td style=\"border: 1px solid black; padding: 8px;\"><b>Vendor</b></td><td style=\"border: 1px solid black; padding: 8px;\"><b>"
                            + mydata.VendorCode.ToString() + " - " + mydata.VendorDesc + "</b></td>");
                            Datadetails = Datadetails.Replace("@TotalQty", mydata.TttlQty == null ? "0" : mydata.TttlQty.ToString());
                            Datadetails = Datadetails.Replace("@AffectedCavity", mydata.AffectedCavity == null ? "0" : mydata.AffectedCavity.ToString());
                            Datadetails = Datadetails.Replace("@NCCategory", mydata.NCCategory == null ? "N.A" : mydata.NCCategory.ToString());
                            Datadetails = Datadetails.Replace("@NCDescription", mydata.NCDescription == null ? "N.A" : mydata.NCDescription.ToString());

                            var attachments = await data.ISM.GetAttachmentsByFormNo(mydata.FormNumber);
                            var linkedFiles = new List<LinkedFile>();
                            if (attachments != null && attachments.Any())
                            {
                                string imagesHtml = "";
                                foreach (var attachment in attachments)
                                {
                                    if (!string.IsNullOrEmpty(attachment.FilePath))
                                    {
                                        string contentId = Guid.NewGuid().ToString();

                                        linkedFiles.Add(new LinkedFile
                                        {
                                            FilePath = attachment.FilePath,
                                            ContentId = contentId
                                        });

                                        using var originalImg = Image.FromFile(attachment.FilePath);
                                        using var img = ResizeAndPadImage(originalImg, 200, 200);
                                        using var ms = new MemoryStream();

                                        var encoderParams = new EncoderParameters(1);
                                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
                                        var jpegCodec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                                        if (jpegCodec != null)
                                            img.Save(ms, jpegCodec, encoderParams);
                                        else
                                            img.Save(ms, ImageFormat.Jpeg);

                                        string base64 = Convert.ToBase64String(ms.ToArray());
                                        string ext = Path.GetExtension(attachment.FilePath).ToLower();
                                        string mime = ext switch
                                        {
                                            ".png" => "image/png",
                                            ".gif" => "image/gif",
                                            ".jpg" or ".jpeg" => "image/jpeg",
                                            _ => "application/octet-stream"
                                        };

                                        imagesHtml += $"<img src='data:{mime};base64,{base64}' style='height:200pt; width:200pt; object-fit:contain; display:inline-block; margin-right:10pt; border:1pt solid #ddd;' />";
                                        //imagesHtml += $"<img src='cid:{contentId}' style='height:200px; width:200px; object-fit:contain; display:inline-block; margin-right:10px; border:1px solid #ddd;' />";
                                    }
                                }

                                Datadetails = Datadetails.Replace("@NCPicture", "Refer to Attachment.");
                            }

                            body = body.Replace("@Data", Datadetails);
                            body = body.Replace("@UserId", mydata.UserName);

                            body += globalmailMaster.FirstOrDefault().EmailFooter;
                            body = body.Replace("@Emaillink", globalmailMaster.FirstOrDefault().Emaillink);
                            mailparam.Body = body;
                            mailparam.CreateUser = mydata.UserId;
                            mailparam.CopyRecipient = string.Join(";", MailtoccList);
                            mailparam.LinkedFiles = linkedFiles;
                            mailparam.AttachmentsPath = linkedFiles;
                            await mdmP.SendEmail(mailparam);
                        }
                    }
                }
                else
                {
                    mailmsg = "no email Setting are set up";
                }
            }
            else
            {
                mailmsg = "no email Setting are set up";
            }
            return ApiResponse<string>.SuccessResponse(null, (mailmsg.Length == 0 ? "" : " Send Mail Fail : " + mailmsg));
        }

        private static Image ResizeAndPadImage(Image originalImage, int targetWidth = 200, int targetHeight = 200)
        {
            float ratioX = (float)targetWidth / originalImage.Width;
            float ratioY = (float)targetHeight / originalImage.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalImage.Width * ratio);
            int newHeight = (int)(originalImage.Height * ratio);

            int posX = (targetWidth - newWidth) / 2;
            int posY = (targetHeight - newHeight) / 2;

            var canvas = new Bitmap(targetWidth, targetHeight);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White); 
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, posX, posY, newWidth, newHeight);
            }

            return canvas;
        }


        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> GetSendMailSetting(GETMailSettings GETMailSettings)
        {
            var result = await data.SMS.GetSendMailSetting(GETMailSettings);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> InsertNewSendMailSetting(string plant, string actiontype, string actiontypedesc, bool issendemail, string userId)
        {
            var result = await data.SMS.InsertNewSendMailSetting(plant,actiontype, actiontypedesc, issendemail,userId);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> UpdateSendMailSetting(string actiontype, string actiontypedesc, bool issendemail, string userId)
        {
            var result = await data.SMS.UpdateSendMailSetting(actiontype, actiontypedesc, issendemail,userId);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> DataDelete(string actiontype, string userId)
        {
            var result = await data.SMS.DataDelete(actiontype,userId);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> DataPermDelete(string actiontype)
        {
            var result = await data.SMS.DataPermDelete(actiontype);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<MailSetiingDto>>> DataRecover(string actiontype, string userId)
        {
            var result = await data.SMS.DataRecover(actiontype,userId);
            return ApiResponse<IEnumerable<MailSetiingDto>>.SuccessResponse(result);
        }
        public async Task<byte[]> Template()
        {
            var result = await data.SMS.Template();
            return result;
        }
        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.SMS.Import(filePath, userId);

            //System.IO.File.Delete(filePath);

            //if (result.Contains("Invalid Data structure, please follow template format"))
            //{
            //    return new ApiResponse<IEnumerable<string>>
            //    {
            //        Success = false,
            //        Message = "Invalid Data structure, please follow template format",
            //        Content = null
            //    };
            //}
            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
            //return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
    }
}
