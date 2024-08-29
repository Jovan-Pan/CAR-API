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

namespace Services.CAR
{
    internal sealed class IssueSubmissionService(IDataManager data, IMDMRepository mdm, ICacheManager memCache, ILocalizationService localization): IIssueSubmissionService
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

            string newformno = await data.ISM.GenerateNewFormNo(transaction);
            mydata.FormNumber = newformno;

            await data.ISM.InsertDataIssueFeedback(mydata, transaction);

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
                                        dtaAtch.ActionType = "NCCategory";
                                        dtaAtch.ActionCode = mydata.NCCategory;
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
                                        dtaAtch.ActionType = "NCCategory";
                                        dtaAtch.ActionCode = mydata.NCCategory;
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
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
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
                        var NCCategorydataAtch = dataAtch.Where(x => x.ActionType == "NCCategory");
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
                                    dtaAtch.ActionType = "NCCategory";
                                    dtaAtch.ActionCode = mydata.NCCategory;
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
                                        dtaAtch.ActionType = "NCCategory";
                                        dtaAtch.ActionCode = mydata.NCCategory;
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
                                        dtaAtch.ActionType = "NCCategory";
                                        dtaAtch.ActionCode = mydata.NCCategory;
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
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
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

        public async Task<ApiResponse<string>> issuerMngUpdate(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.issuerMngUpdate(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> pdaDecision(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.pdaDecision(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> pdaDecisionUpdate(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.pdaDecisionUpdate(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> pdaApproval(IssueSubmissionParameters mydata)
        {
            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.ISM.pdaApproval(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve Succesfully");
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
                                        string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                        string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                        IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                        dtaAtch.FormNo = mydata.FormNumber;
                                        dtaAtch.ActionType = "RecImmAct";
                                        dtaAtch.ActionCode = mydata.NCCategory;
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
                                        string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                        string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                        IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                        dtaAtch.FormNo = mydata.FormNumber;
                                        dtaAtch.ActionType = "RecImmAct";
                                        dtaAtch.ActionCode = mydata.NCCategory;
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
                                        string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                        string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                        IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                        dtaAtch.FormNo = mydata.FormNumber;
                                        dtaAtch.ActionType = "ROOTCAUSE";
                                        dtaAtch.ActionCode = mydata.rootcause;
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
                                        string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                        string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                        IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                        dtaAtch.FormNo = mydata.FormNumber;
                                        dtaAtch.ActionType = "ROOTCAUSE";
                                        dtaAtch.ActionCode = mydata.rootcause;
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
                                        string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                        string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                        IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                        dtaAtch.FormNo = mydata.FormNumber;
                                        dtaAtch.ActionType = "CORRECTIVEACTION";
                                        dtaAtch.ActionCode = mydata.correctiveAct;
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
                                        string newFileName = mydata.FormNumber + "_" + mydata.NCCategory + "_" + Flnameonly;
                                        string destinationPath = Path.Combine(basePath, (newFileName + extensionFile));

                                        IssueFeedbackAtchmentDto dtaAtch = new IssueFeedbackAtchmentDto();
                                        dtaAtch.FormNo = mydata.FormNumber;
                                        dtaAtch.ActionType = "CORRECTIVEACTION";
                                        dtaAtch.ActionCode = mydata.correctiveAct;
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
            else
            {
                return ApiResponse<string>.FailResponse(credentials.Message);
            }

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }
    }
}
