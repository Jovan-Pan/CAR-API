using Entities.CAR;
using Entities.ParamRequest;
using Entities;
using Microsoft.Data.SqlClient;
using Services.Helper;
using Services.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Services.Contracts.CAR;
using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using Contracts.Repository;
using Contracts;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;
using Services.Contracts;
using System.Transactions;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Entities.MasterData;
using OfficeOpenXml.Style;
using System.IO;

namespace Services.CAR
{
    internal sealed class IssueFeedbackReportService(IDataManager data, IServiceManager SM, IMDMRepository mdm, ICacheManager memCache, ILocalizationService localization) : IIssueFeedbackReportService
    {
        public async Task<ApiResponse<IssueFeedbackResultDto>> GetDataReport(GlobalParam param)
        {
            Dictionary<string, string> keyValuePair;
            
            var skip = (param.PageNumber - 1) * param.PageSize;
            
            string whereCondition = DisplayDataCondition.GenerateWhereCondition(param);
            string orderByCondition = DisplayDataCondition.GenerateOrderByCondition(param.order);
            int totalRecords = 0;
            int take = param.PageSize;
            ConditionParams Cpr = new ConditionParams();
            Cpr.take = take;
            Cpr.skip = skip;
            Cpr.ExtraWhereCondition = whereCondition;
            Cpr.OrderByCondition = orderByCondition;

            var totrecord = await data.IFR.GetTotalRecord(param, Cpr);

            #region generate condition query for advance filter
            string Condquery = @"";
            if (param.formType != null)
            {
                if(param.formType.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND FormType IN @formType ";
                }
            }
            if (param.formNumber != null)
            {
                if (param.formNumber.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND FormNo IN @formNumber ";
                }
            }
            if (param.status != null)
            {
                if (param.status.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += @" AND status IN @status ";
                }
            }
            if (param.dept != null)
            {
                if (param.dept.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND Dept IN @dept ";
                }
            }
            if (param.processGrp != null)
            {
                if (param.processGrp.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND procecessGrpCode IN @processGrp ";
                }
            }
            if (param.product != null)
            {
                if (param.product.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND Product IN @product ";
                }
            }
            if (param.model != null)
            {
                if (param.model.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND Model IN @model ";
                }
            }
            if (param.mattype != null)
            {
                if (param.mattype.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND MaterialType IN @mattype ";
                }
            }
            if (param.material != null)
            {
                if (param.material.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND MaterialCode IN @material ";
                }
            }
            if (param.vendor != null)
            {
                if (param.vendor.Count() > 0)
                {
                    Condquery += Environment.NewLine;
                    Condquery += " AND isnull(VendorCode,'NA') IN @vendor ";
                }
            }
            if (param.datetype != null)
            {
                if (param.fromdate != null && param.todate != null) {
                    if (param.datetype == "DetectionDate")
                    {
                        Condquery += Environment.NewLine;
                        Condquery += " AND format(DetectionDate,'yyyy-MM-dd') between @fromdate and @todate ";
                    }
                    else if (param.datetype == "EffectiveDate")
                    {
                        Condquery += Environment.NewLine;
                        Condquery += " AND format(effectivedate,'yyyy-MM-dd') between @fromdate and @todate ";
                    }
                }
            }

            if (Condquery.Length > 0) {
                Cpr.ExtraWhereCondition += Environment.NewLine;
                Cpr.ExtraWhereCondition += Condquery;
            }
            #endregion

            var maindata = await data.IFR.GetMaindata(param, Cpr);
            var formNoList = maindata.Select(data => data.FormNo);
            var dataAtch = await data.IFR.GetDataAttchment(param.Plant, formNoList,null);

            var mdmMattype = await mdm.GetMatType(param.Plant);
            GetMaterialParam mparam = new GetMaterialParam();
            IEnumerable<string> materialCodes = maindata.Select(data => data.MaterialCode);
            mparam.plant = param.Plant;
            mparam.MaterialList = materialCodes;
            var mdmMaterial = await mdm.GetMaterialWoProdAut(mparam);
            var mdmDept = await mdm.GetSystemDeptVsUser(param.Plant, "CAR");
            var mdmCurency = await mdm.GetCurrency(mparam.plant);
            var mdmProcGrp = await mdm.getProcessGrp(mparam.plant);

            var joinedData = from main in maindata
                             join mattype in mdmMattype on main.MaterialType equals mattype.MaterialType into matGroup
                             from matTy in matGroup.DefaultIfEmpty()
                             join mats in mdmMaterial on main.MaterialCode equals mats.Material into mat
                             from mats in mat.DefaultIfEmpty()
                             join depts in mdmDept on main.Dept equals depts.dept into dept
                             from depts in dept.DefaultIfEmpty()
                             join cur in mdmCurency on main.Curency equals cur.CurrencyCode into cry
                             from cur in cry.DefaultIfEmpty()
                             join prg in mdmProcGrp on main.procecessGrpCode equals prg.procecessGrpCode into prgs
                             from prg in prgs.DefaultIfEmpty()
                             select new IssueFeedbackDto
                             {
                                 Plant = main.Plant,
                                 FormType = main.FormType,
                                 FormNo = main.FormNo,
                                 DetectionDate = main.DetectionDate,
                                 Product = main.Product,
                                 Model = main.Model,
                                 MaterialType = main.MaterialType,
                                 MESDesc = matTy != null ? matTy.MESDesc : main.MESDesc,
                                 MaterialCode = main.MaterialCode,
                                 MaterialDesc = mats != null ? mats.MaterialDesc : main.MaterialDesc,
                                 NcQty = main.NcQty,
                                 SamplingCheck = main.SamplingCheck,
                                 NcRatio = main.NcRatio,
                                 Dept = main.Dept,
                                 deptName = depts != null ? depts.deptName : main.deptName,
                                 VendorCode = main.VendorCode,
                                 VendorDesc = main.VendorDesc,
                                 TttlQty = main.TttlQty,
                                 TttlQtyUOM = main.TttlQtyUOM,
                                 AffectedCavity = main.AffectedCavity,
                                 IssueType = main.IssueType,
                                 NCCode = main.NCCode,
                                 NCCategory = main.NCCategory,
                                 NCReason = main.NCReason,
                                 NCDescription = main.NCDescription,
                                 Status = main.Status,
                                 IssueBy = main.IssueBy,
                                 IssueByName = main.IssueByName,
                                 IssueDate = main.IssueDate,
                                 IssueByComment = main.IssueByComment,
                                 IssueUpdatedBy = main.IssueUpdatedBy,
                                 IssueUpdatedByName = main.IssueUpdatedByName,
                                 IssueUpdatedDate = main.IssueUpdatedDate,
                                 AcknowledgeBy = main.AcknowledgeBy,
                                 AcknowledgeByname = main.AcknowledgeByname,
                                 AcknowledgeByDate = main.AcknowledgeByDate,
                                 AcknowledgeByComment = main.AcknowledgeByComment,
                                 ReceiveActionRejectReason = main.ReceiveActionRejectReason,
                                 PDAActionBy = main.PDAActionBy,
                                 PDAActionByName = main.PDAActionByName,
                                 PDAActionDate = main.PDAActionDate,
                                 PDAActionImmAct = main.PDAActionImmAct,
                                 PDAActionComment = main.PDAActionComment,
                                 PDAActionUpdatedBy = main.PDAActionUpdatedBy,
                                 PDAActionUpdatedByName = main.PDAActionUpdatedByName,
                                 PDAActionUpdatedDate = main.PDAActionUpdatedDate,
                                 PDAAprovalBy = main.PDAAprovalBy,
                                 PDAAprovalByName = main.PDAAprovalByName,
                                 PDAAprovalDate = main.PDAAprovalDate,
                                 pdaAprovalComment = main.pdaAprovalComment,
                                 ImmActRecDetail = main.ImmActRecDetail,
                                 CostPC = main.CostPC,
                                 Curency = main.Curency,
                                 CurrencyDescription = cur != null ? cur.CurrencyDescription : main.CurrencyDescription,
                                 ActionResult = main.ActionResult,
                                 ReceiveActionRootCause = main.ReceiveActionRootCause,
                                 RootCauseDetail = main.RootCauseDetail,
                                 procecessGrpCode = main.procecessGrpCode,
                                 procecessGrpDesc = prg != null ? prg.procecessGrpDesc : main.procecessGrpDesc,
                                 ReceiveCorrectiveAct = main.ReceiveCorrectiveAct,
                                 effectivedate = main.effectivedate,
                                 ReceiveActionComment = main.ReceiveActionComment,
                                 ReceiveActionBy = main.ReceiveActionBy,
                                 ReceiveActionByName = main.ReceiveActionByName,
                                 ReceiveActionDate = main.ReceiveActionDate,
                                 ReceiveActionUpdatedBy = main.ReceiveActionUpdatedBy,
                                 ReceiveActionUpdatedByName = main.ReceiveActionUpdatedByName,
                                 ReceiveActionUpdatedDate = main.ReceiveActionUpdatedDate,
                                 ReceiveAprovalBy = main.ReceiveAprovalBy,
                                 ReceiveAprovalByName = main.ReceiveAprovalByName,
                                 ReceiveAprovalDate = main.ReceiveAprovalDate,
                                 receiveAprovalComment = main.receiveAprovalComment,

                                 PDAReviewBy = main.PDAReviewBy,
                                 PDAReviewByName = main.PDAReviewByName,
                                 PDAReviewDate = main.PDAReviewDate,
                                 ReviewDate = main.ReviewDate,
                                 PDAReviewComment = main.PDAReviewComment,

                                 ReviewBy = main.ReviewBy,
                                 ReviewByName = main.ReviewByName,
                                 ReviewSubmitDate = main.ReviewSubmitDate,
                                 ReviewComment = main.ReviewComment,
                                 ReviewMethod = main.ReviewMethod,
                             };
            maindata = joinedData;
            

            IssueFeedbackResultDto result = new IssueFeedbackResultDto ();
            
            if (maindata != null) {
                if (maindata.Count() > 0)
                {
                    var maindataList = maindata.ToList();

                    for (int i = 0; i < maindataList.Count(); i++)
                    {
                        string formnumber = maindataList[i].FormNo;
                        var atch = dataAtch.Where(dto => dto.FormNo == formnumber);
                        maindataList[i].dataAtch = atch;
                    }

                    maindata = maindataList;
                }
            }
            result.maindata = maindata;
            result.totrecord = totrecord;
            return ApiResponse<IssueFeedbackResultDto>.SuccessResponse(result);
        }

        public async Task<ApiResponse<TotalRecordForEachSttsDto>> GetTotalRecordForEachStts(GetTotalRecordForEachSttsParam param)
        {
            string condition = " AND Dept IN @DeptList AND Product IN @ProductList ";
            if (param.vendorcode != null)
            {
                condition = " AND vendorcode = @vendorcode ";
            }
            if (param.formType != null)
            {
                condition += " AND (FormType IN @formType or '' IN @formType) ";
            }
            var result  = await data.IFR.GetTotalRecordForEachStts(param, condition);
            return ApiResponse<TotalRecordForEachSttsDto>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetFormNumberListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var result = await data.IFR.GetFormNumberListFilter(plant, deptAuthList, productAuthList);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetDeptListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var result = await data.IFR.GetDeptListFilter(plant, deptAuthList, productAuthList);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<ProcessGroupDto>>> GetprocecessGrpCodeFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var AvlPrcGrpData = await data.IFR.GetprocecessGrpCodeFilter(plant, deptAuthList, productAuthList);
            var procGrpmaster = await mdm.getProcessGrp(plant);
            var result = from key in AvlPrcGrpData
                         join dto in procGrpmaster on key equals dto.procecessGrpCode into joined
                         from dto in joined.DefaultIfEmpty()
                         select new ProcessGroupDto
                         {
                             procecessGrpCode = key,
                             procecessGrpDesc = dto?.procecessGrpDesc
                         };
            return ApiResponse<IEnumerable<ProcessGroupDto>>.SuccessResponse(result);
        }


        public async Task<ApiResponse<IEnumerable<string>>> GetproductListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var result = await data.IFR.GetproductListFilter(plant, deptAuthList, productAuthList);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetModelListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var result = await data.IFR.GetModelListFilter(plant, deptAuthList, productAuthList);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetMatTypeListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var result = await data.IFR.GetMatTypeListFilter(plant, deptAuthList, productAuthList);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<TMATERIALDto>>> GetMaterialListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList, string searchTerm)
        {
            var mat = await data.IFR.GetMaterialListFilter(plant, deptAuthList, productAuthList);
            GetMaterialParam matparam = new GetMaterialParam();
            matparam.plant = plant;
            matparam.productAuthList = productAuthList;
            matparam.searchTerm = searchTerm;
            matparam.MaterialList = mat;
            var matmaster = await mdm.GetMaterialWoProdAut(matparam);

            return ApiResponse<IEnumerable<TMATERIALDto>>.SuccessResponse(matmaster);
        }

        public async Task<ApiResponse<IEnumerable<VendorDto>>> GetVendorListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            var result = await data.IFR.GetVendorListFilter(plant, deptAuthList, productAuthList);
            return ApiResponse<IEnumerable<VendorDto>>.SuccessResponse(result);
        }

        public async Task<(Stream FileStream, string MimeType, string FileName)> GetFilePreviewAsync(GetAttachmentParam request)
        {
            var basepathconfig = await mdm.getBasePathConfig(request.Plant);
            if (!basepathconfig.Any())
            {
                throw new InvalidOperationException("Master Data Base Path For Attachment Not Found");
            }
            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            var credentials = await SM.IssueSubmission.GetDirectoryAuth(domain, windowsuser, pwd, basePath);
            if (credentials.Success)
            {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        var filePath = Path.Combine(request.filepath);

                        if (!System.IO.File.Exists(filePath))
                        {
                            throw new FileNotFoundException("File not found.");
                        }

                        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                        var mimeType = GetMimeType(filePath);
                        var fileName = $"{request.filename}.{request.fileExt}";

                        return (fileStream, mimeType, fileName);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Failed to authenticate with UNC path.");
                    }
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Failed to authenticate with UNC path.");
            }
        }

        public async Task<List<(string Base64Content, string MimeType, string FileName)>> GetFilesAttchment(IEnumerable<GetAttachmentParam> request)
        {
            var resultFiles = new List<(string Base64Content, string MimeType, string FileName)>();

            foreach (var fileRequest in request)
            {
                var basepathconfig = await mdm.getBasePathConfig(fileRequest.Plant);
                if (!basepathconfig.Any())
                {
                    throw new InvalidOperationException("Master Data Base Path For Attachment Not Found");
                }

                string domain = basepathconfig.First().domain;
                string windowsuser = basepathconfig.First().userID;
                string pwd = basepathconfig.First().password;
                string basePath = basepathconfig.First().basePath;

                var credentials = await SM.IssueSubmission.GetDirectoryAuth(domain, windowsuser, pwd, basePath);

                if (credentials.Success)
                {
                    using (UNCFileManager unc = new())
                    {
                        if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                        {
                            var filePath = Path.Combine(fileRequest.filepath);
                            if (!System.IO.File.Exists(filePath))
                            {
                                throw new FileNotFoundException($"File not found: {fileRequest.filepath}");
                            }

                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, useAsync: true))
                            {
                                using (var ms = new MemoryStream())
                                {
                                    await fileStream.CopyToAsync(ms);
                                    var base64Content = Convert.ToBase64String(ms.ToArray());
                                    var mimeType = GetMimeType(filePath);
                                    var fileName = $"{fileRequest.filename}.{fileRequest.fileExt}";

                                    resultFiles.Add((base64Content, mimeType, fileName));
                                    ms.Close();
                                }
                                fileStream.Close();
                            }
                        }
                        else
                        {
                            throw new UnauthorizedAccessException("Failed to authenticate with UNC path.");
                        }
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("Failed to authenticate with UNC path.");
                }
            }

            return resultFiles;
        }

        private string GetMimeType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpg",
                _ => "application/octet-stream",
            };
        }
    }
}
