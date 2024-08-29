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
                                 FormNo = main.FormNo,
                                 DetectionDate = main.DetectionDate,
                                 Product = main.Product,
                                 Model = main.Model,
                                 MaterialType = main.MaterialType,
                                 MESDesc = matTy != null ? matTy.MESDesc : main.MESDesc,
                                 MaterialCode = main.MaterialCode,
                                 MaterialDesc = mats != null ? mats.MaterialDesc : main.MaterialDesc,
                                 SamplingCheck = main.SamplingCheck,
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
                                 IssueAcknowledgeBy = main.IssueAcknowledgeBy,
                                 IssueAcknowledgeByname = main.IssueAcknowledgeByname,
                                 IssueAcknowledgeByDate = main.IssueAcknowledgeByDate,
                                 IssueAcknowledgeByComment = main.IssueAcknowledgeByComment,
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
                                 ImmActRecDetail = main.ImmActRecDetail,
                                 CostPC = main.CostPC,
                                 Curency = main.Curency,
                                 CurrencyDescription = cur != null ? cur.CurrencyDescription : main.CurrencyDescription,
                                 ActionResult = main.ActionResult,
                                 IssueReceiveActionRootCause = main.IssueReceiveActionRootCause,
                                 RootCauseDetail = main.RootCauseDetail,
                                 procecessGrpCode = main.procecessGrpCode,
                                 procecessGrpDesc = prg != null ? prg.procecessGrpDesc : main.procecessGrpDesc,
                                 IssueReceiveActionCorrectiveAct = main.IssueReceiveActionCorrectiveAct,
                                 effectivedate = main.effectivedate,
                                 IssueReceiveActionComment = main.IssueReceiveActionComment,
                                 IssueReceiveActionBy = main.IssueReceiveActionBy,
                                 IssueReceiveActionByName = main.IssueReceiveActionByName,
                                 IssueReceiveActionDate = main.IssueReceiveActionDate,
                                 IssueReceiveActionUpdatedBy = main.IssueReceiveActionUpdatedBy,
                                 IssueReceiveActionUpdatedByName = main.IssueReceiveActionUpdatedByName,
                                 IssueReceiveActionUpdatedDate = main.IssueReceiveActionUpdatedDate,
                                 IssueReceiveAprovalBy = main.IssueReceiveAprovalBy,
                                 IssueReceiveAprovalByName = main.IssueReceiveAprovalByName,
                                 IssueReceiveAprovalDate = main.IssueReceiveAprovalDate,
                                 IssueReviewBy = main.IssueReviewBy,
                                 IssueReviewByName = main.IssueReviewByName,
                                 IssueReviewDate = main.IssueReviewDate,
                                 IssueReviewComment = main.IssueReviewComment,
                                 IssueReviewMethod = main.IssueReviewMethod,
                                 IssueReviewUpdatedBy = main.IssueReviewUpdatedBy,
                                 IssueReviewUpdatedByName = main.IssueReviewUpdatedByName,
                                 IssueReviewUpdatedDate = main.IssueReviewUpdatedDate,
                                 IssueReviewAprovalBy = main.IssueReviewAprovalBy,
                                 IssueReviewAprovalByName = main.IssueReviewAprovalByName,
                                 IssueReviewAprovalDate = main.IssueReviewAprovalDate,
                                 IssueReviewAprovalComment = main.IssueReviewAprovalComment,
                             };
            maindata = joinedData;
            IssueSbmsAtchParam formFiles = new IssueSbmsAtchParam();
            var basepathconfig = await mdm.getBasePathConfig(param.Plant);
            if (!basepathconfig.Any())
            {
                return ApiResponse<IssueFeedbackResultDto>.FailResponse("Master Data Base Path For Attachment Not Found");
            }
            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            var credentials = await SM.IssueSubmission.GetDirectoryAuth(domain, windowsuser, pwd, basePath);
            if (credentials.Success) {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        List<IFormFile>? nCCategoryImgFiles = new List<IFormFile>();
                        List<IFormFile>? nCCategoryFiles = new List<IFormFile>();

                        var NCCategorydataAtch = dataAtch.Where(x => x.ActionType == "NCCategory");

                        if (NCCategorydataAtch.Any()) {
                            foreach (var attachment in NCCategorydataAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    using (var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    {
                                        var formFile = new FormFile(fileStream, 0, fileStream.Length, attachment.OriFileName, attachment.FileName)
                                        {
                                            Headers = new HeaderDictionary(),
                                            ContentType = GlobalFunction.GetContentType("." + attachment.FileExt)
                                        };

                                        if (formFile.ContentType.StartsWith("image/"))
                                        {
                                            nCCategoryImgFiles.Add(formFile);
                                        }
                                        else
                                        {
                                            nCCategoryFiles.Add(formFile);
                                        }
                                    }

                                }
                            }
                            formFiles.NCCategoryImgFiles = nCCategoryImgFiles.AsEnumerable();
                            formFiles.NCCategoryFiles = nCCategoryFiles.AsEnumerable();
                        }
                    }
                }
            }
            else
            {
                return ApiResponse<IssueFeedbackResultDto>.FailResponse(credentials.Message);
            }

            IssueFeedbackResultDto result = new IssueFeedbackResultDto ();
            result.maindata = maindata;
            result.dataAtch = dataAtch;
            result.formFiles = formFiles;
            result.totrecord = totrecord;
            return ApiResponse<IssueFeedbackResultDto>.SuccessResponse(result);
        }
    }
}
