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
                                Datadetails = Datadetails.Replace("@Formlink", globalmailMaster.FirstOrDefault().Emaillink + $"/pages/issueSubmission?formnumber={mydata.FormNumber}&useraction={mydata.userAction}");
                            }
                            else
                            {
                                Datadetails = Datadetails.Replace("@Formlink", globalmailMaster.FirstOrDefault().Emaillink + $"/pages/DynamicNewForm?formnumber={mydata.FormNumber}&useraction={mydata.userAction}");
                            }
                            Datadetails = Datadetails.Replace("@Status", mydata.IssueStatus?.Replace("PDA-DECISION", "CAR ISSUING"));
                            Datadetails = Datadetails.Replace("@DetectionDate", mydata.DetectionDate?.ToString("dd-MM-yyyy"));
                            Datadetails = Datadetails.Replace("@Product", mydata.Product);
                            //Datadetails = Datadetails.Replace("@Model", mydata.Model);
                            //Datadetails = Datadetails.Replace("@Materialtype", mydata.MaterialType);
                            Datadetails = Datadetails.Replace("@MaterialCode", mydata.MaterialCode);
                            Datadetails = Datadetails.Replace("@MaterialDesc", mydata.MaterialDesc == null ? "" : mydata.MaterialDesc.ToString());
                            Datadetails = Datadetails.Replace("@SamplingCheck", mydata.NcRatio == null ? "" : mydata.NcRatio.ToString());
                            Datadetails = Datadetails.Replace("@Dept", mydata.Dept);
                            Datadetails = Datadetails.Replace("@Vendor", mydata.VendorCode == null ? "" : (mydata.VendorCode.ToString() + "-" + mydata.VendorDesc));
                            Datadetails = Datadetails.Replace("@TotalQty", mydata.TttlQty == null ? "" : mydata.TttlQty.ToString());
                            Datadetails = Datadetails.Replace("@AffectedCavity", mydata.AffectedCavity == null ? "" : mydata.AffectedCavity.ToString());
                            Datadetails = Datadetails.Replace("@NCCategory", mydata.NCCategory == null ? "" : mydata.NCCategory.ToString());
                            Datadetails = Datadetails.Replace("@NCDescription", mydata.NCDescription == null ? "" : mydata.NCDescription.ToString());
                            body = body.Replace("@Data", Datadetails);
                            body = body.Replace("@UserId", mydata.UserName);

                            body += globalmailMaster.FirstOrDefault().EmailFooter;
                            body = body.Replace("@Emaillink", globalmailMaster.FirstOrDefault().Emaillink);
                            mailparam.Body = body;
                            mailparam.CreateUser = mydata.UserId;
                            mailparam.CopyRecipient = string.Join(";", MailtoccList);
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
