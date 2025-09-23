using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Repository.CAR;
using Entities;
using Services.Contracts.CAR;
using Contracts.Repository;
using Entities.MasterData;
using Entities.ParamRequest;
using Entities.CAR;
using Newtonsoft.Json;
using Services.Helper;
using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Entities.Infrastructure;
using System.Xml.Linq;

namespace Services.CAR
{
    internal sealed class IssueSubmissionService(IDataManager data, IMDMRepository mdm, IMasterDataApi mdmP, ICacheManager memCache, ILocalizationService localization): IIssueSubmissionService
    {

        public async Task<ApiResponse<string>>ProcessSubmit(IssueSubmissionParameters mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);
            

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            string newformno = mydata.FormNumber;
            if (string.IsNullOrEmpty(mydata.FormNumber))
            {
                newformno = await data.ISM.GenerateNewFormNo(mydata.UserPlant, mydata.FormType, transaction);
                mydata.FormNumber = newformno;
            }

            var chkExsData = await data.ISM.ChkExitsFormno(mydata.FormNumber, transaction);
            if (!chkExsData) {
                await data.ISM.InsertDataIssueFeedback(mydata, transaction);
            }
            else
            {
                await data.ISM.SaveAsDraftDataIssueFeedback(mydata, transaction);
            }
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            DirectoryCredentials credentials = await GetDirectoryAuth(domain, windowsuser, pwd, basePath);

            if (credentials.Success)
            {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        if (mydata.NCCategoryImgFiles != null) {
                            var NCCategoryImgFiles = mydata.NCCategoryImgFiles.ToList();
                            if (NCCategoryImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in NCCategoryImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = newformno + "_" + mydata.NCCategory + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = newformno;
                                            dtaAtch.ActionType = "NC Category";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.NCCategoryFiles != null) {
                            var NCCategoryFiles = mydata.NCCategoryFiles.ToList();
                            if (NCCategoryFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in NCCategoryFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = newformno + "_" + mydata.NCCategory + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = newformno;
                                            dtaAtch.ActionType = "NC Category";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(newformno, "Data Submit Succesfully, New Form No : " + newformno);
        }

        public async Task<ApiResponse<string>> issuerUpdate(IssueSubmissionParameters mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.ISM.issuerUpdateDataIssueFeedback(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);
            #region get old data attachment
            List<string> formNoList = new List<string>();
            formNoList.Add(mydata.FormNumber);
            var dataAtch = await data.IFR.GetDataAttchment(mydata.UserPlant, formNoList, transaction);
            #endregion

            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            DirectoryCredentials credentials = await GetDirectoryAuth(domain, windowsuser, pwd, basePath);

            if (credentials.Success)
            {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        #region delete old Atch
                        var NCCategorydataAtch = dataAtch.Where(x => x.ActionType == "NC Category");
                        if (NCCategorydataAtch.Any())
                        {
                            foreach (var attachment in NCCategorydataAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "NC Category";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }
                        #endregion

                        if (mydata.NCCategoryImgFiles != null) {
                            var NCCategoryImgFiles = mydata.NCCategoryImgFiles.ToList();
                            if (NCCategoryImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in NCCategoryImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "NC Category";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if(mydata.NCCategoryFiles != null)
                        {
                            var NCCategoryFiles = mydata.NCCategoryFiles.ToList();
                            if (NCCategoryFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in NCCategoryFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "NC Category";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();

            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> issuerVoid(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.WorkFlowHistory.InsertNewData(mydata, transaction);
            await data.ISM.issuerVoid(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data VOID Succesfully");
        }

        public async Task<DirectoryCredentials> GetDirectoryAuth(string Domain, string UserID, string Password, string BasePath)
        {
            try
            {
                DirectoryCredentials result = new DirectoryCredentials();

                result = SharedFolderValidator.Validate(Domain, UserID, Password, BasePath);

                if (result.Success)
                {
                    result.Domain = Domain;
                    result.UserID = UserID;
                    result.Password = Password;
                    result.BasePath = BasePath;
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Unable to obtain the authorization, please contact your Administrator.";
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<string>> issuerMgrVoid(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.issuerMgrVoid(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data VOID Succesfully");
        }

        public async Task<ApiResponse<string>> issuerMgrReject(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.issuerMgrReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> issuerMngUpdate(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.issuerMngUpdate(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> pdaDecision(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.pdaDecision(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> pdaDecisionUpdate(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.pdaDecisionUpdate(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> pdaApproval(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.pdaApproval(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve Succesfully");
        }

        public async Task<ApiResponse<string>> pdaActionReject(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.pdaActionReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverAction(IssueSubmissionParameters mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.ISM.ReceiverAction(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);
            #region get old data attachment
            List<string> formNoList = new List<string>();
            formNoList.Add(mydata.FormNumber);
            var dataAtch = await data.IFR.GetDataAttchment(mydata.UserPlant, formNoList, transaction);
            #endregion

            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            DirectoryCredentials credentials = await GetDirectoryAuth(domain, windowsuser, pwd, basePath);

            if (credentials.Success)
            {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        #region delete old Atch
                        var immediteActAtch = dataAtch.Where(x => x.ActionType == "RECEIVER IMMIDIATE ACTION");
                        if (immediteActAtch.Any())
                        {
                            foreach (var attachment in immediteActAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "RECEIVER IMMIDIATE ACTION";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }

                        var rootCauseAtch = dataAtch.Where(x => x.ActionType == "ROOT CAUSE");
                        if (rootCauseAtch.Any())
                        {
                            foreach (var attachment in rootCauseAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "ROOT CAUSE";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }

                        var correctiveActAtch = dataAtch.Where(x => x.ActionType == "CORRECTIVE ACTION");
                        if (correctiveActAtch.Any())
                        {
                            foreach (var attachment in correctiveActAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "CORRECTIVE ACTION";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }
                        #endregion

                        if (mydata.immediteActReceiverImgFiles != null) {
                            var immediteActReceiverImgFiles = mydata.immediteActReceiverImgFiles.ToList();
                            if (immediteActReceiverImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in immediteActReceiverImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "ImmActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "RECEIVER IMMIDIATE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.immediteActReceiverFiles != null) {
                            var immediteActReceiverFiles = mydata.immediteActReceiverFiles.ToList();
                            if (immediteActReceiverFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in immediteActReceiverFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "ImmActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "RECEIVER IMMIDIATE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        if (mydata.rootCauseReceiverImgFiles != null) {
                            var rootCauseReceiverImgFiles = mydata.rootCauseReceiverImgFiles.ToList();
                            if (rootCauseReceiverImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in rootCauseReceiverImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "RootCauseAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "ROOT CAUSE";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        if (mydata.rootCauseReceiverFiles != null) {
                            var rootCauseReceiverFiles = mydata.rootCauseReceiverFiles.ToList();
                            if (rootCauseReceiverFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in rootCauseReceiverFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "RootCauseAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "ROOT CAUSE";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.correctiveActReceiverImgFiles != null) {
                            var correctiveActReceiverImgFiles = mydata.correctiveActReceiverImgFiles.ToList();
                            if (correctiveActReceiverImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in correctiveActReceiverImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "CorrectivActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "CORRECTIVE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.correctiveActReceiverFiles != null) {
                            var correctiveActReceiverFiles = mydata.correctiveActReceiverFiles.ToList();
                            if (correctiveActReceiverFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in correctiveActReceiverFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "CorrectivActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "CORRECTIVE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverActionUpdate(IssueSubmissionParameters mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.ISM.ReceiverActionUpdate(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);
            #region get old data attachment
            List<string> formNoList = new List<string>();
            formNoList.Add(mydata.FormNumber);
            var dataAtch = await data.IFR.GetDataAttchment(mydata.UserPlant, formNoList, transaction);
            #endregion

            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            DirectoryCredentials credentials = await GetDirectoryAuth(domain, windowsuser, pwd, basePath);

            if (credentials.Success)
            {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        #region delete old Atch
                        var immediteActAtch = dataAtch.Where(x => x.ActionType == "RECEIVER IMMIDIATE ACTION");
                        if (immediteActAtch.Any())
                        {
                            foreach (var attachment in immediteActAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "RECEIVER IMMIDIATE ACTION";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }

                        var rootCauseAtch = dataAtch.Where(x => x.ActionType == "ROOT CAUSE");
                        if (rootCauseAtch.Any())
                        {
                            foreach (var attachment in rootCauseAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "ROOT CAUSE";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }

                        var correctiveActAtch = dataAtch.Where(x => x.ActionType == "CORRECTIVE ACTION");
                        if (correctiveActAtch.Any())
                        {
                            foreach (var attachment in correctiveActAtch)
                            {
                                string relativeFilePath = attachment.FilePath.Replace(credentials.BasePath, "");
                                var fullFilePath = Path.Combine(credentials.BasePath, relativeFilePath.TrimStart('\\'));

                                if (File.Exists(fullFilePath))
                                {
                                    IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                    dtaAtch.FormNo = mydata.FormNumber;
                                    dtaAtch.ActionType = "CORRECTIVE ACTION";
                                    dtaAtch.OriFileName = attachment.OriFileName;
                                    dtaAtch.FileName = attachment.FileName;
                                    dtaAtch.FileExt = attachment.FileExt;
                                    dtaAtch.FilePath = attachment.FilePath;

                                    await data.ISM.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }
                        #endregion

                        if (mydata.immediteActReceiverImgFiles != null)
                        {
                            var immediteActReceiverImgFiles = mydata.immediteActReceiverImgFiles.ToList();
                            if (immediteActReceiverImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in immediteActReceiverImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "ImmActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "RECEIVER IMMIDIATE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.immediteActReceiverFiles != null)
                        {
                            var immediteActReceiverFiles = mydata.immediteActReceiverFiles.ToList();
                            if (immediteActReceiverFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in immediteActReceiverFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "ImmActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "RECEIVER IMMIDIATE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.rootCauseReceiverImgFiles != null)
                        {
                            var rootCauseReceiverImgFiles = mydata.rootCauseReceiverImgFiles.ToList();
                            if (rootCauseReceiverImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in rootCauseReceiverImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "RootCauseAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "ROOT CAUSE";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.rootCauseReceiverFiles != null)
                        {
                            var rootCauseReceiverFiles = mydata.rootCauseReceiverFiles.ToList();
                            if (rootCauseReceiverFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in rootCauseReceiverFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "RootCauseAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "ROOT CAUSE";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.correctiveActReceiverImgFiles != null)
                        {
                            var correctiveActReceiverImgFiles = mydata.correctiveActReceiverImgFiles.ToList();
                            if (correctiveActReceiverImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in correctiveActReceiverImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "CorrectivActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "CORRECTIVE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.correctiveActReceiverFiles != null)
                        {
                            var correctiveActReceiverFiles = mydata.correctiveActReceiverFiles.ToList();
                            if (correctiveActReceiverFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in correctiveActReceiverFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "CorrectivActAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "CORRECTIVE ACTION";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverActionAppeal(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.ReceiverActionAppeal(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Appeal Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverIssueReject(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.ReceiverIssueReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverApproval(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.ReceiverApproval(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverMngReject(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.ReceiverMngReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverApprovalToReject(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.ReceiverApprovalToReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve to Reject Succesfully");
        }

        public async Task<ApiResponse<string>> PDAReviewerVoid(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.PDAReviewerVoid(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Void Succesfully");
        }

        public async Task<ApiResponse<string>> PDAReviewerReject(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.PDAReviewerReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);
            //string newformno = await data.ISM.GenerateNewFormNoWithVer(mydata, transaction);
            //await data.ISM.CreateNewIssueFeedBcakWithVers(mydata.FormNumber, newformno, mydata.UserId, mydata.UserName, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> PDAReviewerAprove(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.PDAReviewerAprove(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> ReviewerSubmit(IssueSubmissionParameters mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.ISM.ReviewerSubmit(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            string domain = basepathconfig.First().domain;
            string windowsuser = basepathconfig.First().userID;
            string pwd = basepathconfig.First().password;
            string basePath = basepathconfig.First().basePath;
            DirectoryCredentials credentials = await GetDirectoryAuth(domain, windowsuser, pwd, basePath);

            if (credentials.Success)
            {
                using (UNCFileManager unc = new())
                {
                    if (unc.NetUseWithCredentials(credentials.BasePath, credentials.UserID, credentials.Domain, credentials.Password))
                    {
                        if (mydata.reviewerImgFiles != null) {
                            var reviewerImgFiles = mydata.reviewerImgFiles.ToList();
                            if (reviewerImgFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in reviewerImgFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "ReviwerAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "REVIEW";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mydata.reviewerFiles != null) {
                            var reviewerFiles = mydata.reviewerFiles.ToList();
                            if (reviewerFiles.Count() > 0)
                            {
                                if (Directory.Exists(basePath))
                                {
                                    foreach (var file in reviewerFiles)
                                    {
                                        if (file.Length > 0)
                                        {
                                            string FilenameandExt = Path.GetFileName(file.FileName);
                                            string extensionFile = Path.GetExtension(file.FileName);
                                            string Flnameonly = Path.GetFileNameWithoutExtension(file.FileName);
                                            string newFileName = mydata.FormNumber + "_" + "ReviwerAtch" + "_" + Flnameonly;
                                            string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                            IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                            dtaAtch.FormNo = mydata.FormNumber;
                                            dtaAtch.ActionType = "REVIEW";
                                            dtaAtch.OriFileName = Flnameonly;
                                            dtaAtch.FileName = newFileName;
                                            dtaAtch.FileExt = extensionFile.Substring(extensionFile.LastIndexOf('.') + 1);
                                            if (!basePath.EndsWith("\\"))
                                            {
                                                basePath += "\\";
                                            }
                                            dtaAtch.FilePath = basePath + newFileName + extensionFile;

                                            await data.ISM.InsertDataAtchIssuer(dtaAtch, transaction);

                                            using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve Succesfully");
        }

        public async Task<ApiResponse<string>> ReviewerReject(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.ReviewerReject(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);

            string newformno = await data.ISM.GenerateNewFormNoWithVer(mydata, transaction);
            await data.ISM.CreateNewIssueFeedBcakWithVers(mydata.FormNumber, newformno, mydata.UserId, mydata.UserName, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully, New Form No Created : " + newformno);
        }
        public async Task<ApiResponse<string>> pdaActionVoid(IssueSubmissionParameters mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.ISM.pdaActionVoid(mydata, transaction);
            await data.WorkFlowHistory.InsertNewData(mydata, transaction);
            //string newformno = await data.ISM.GenerateNewFormNoWithVer(mydata, transaction);
            //await data.ISM.CreateNewIssueFeedBcakWithVers(mydata.FormNumber, newformno, mydata.UserId, mydata.UserName, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Void Succesfully");
        }
        public async Task<ApiResponse<string>> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param)
        {
            var result = await data.ISM.cekAvailableCompletePastIssue(param);
            return ApiResponse<string>.SuccessResponse(result);
        }
        public async Task<byte[]> GetPptTemplate()
        {
            var result = await data.ISM.GetPptTemplate();
            return result;
        }
    }
}
