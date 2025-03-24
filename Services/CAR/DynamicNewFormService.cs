using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts.CAR;
using Entities.CAR;
using Entities;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Services.Helper;
using Services.Resources;
using Contracts.Repository.MasterData;
using Microsoft.IdentityModel.Tokens;
using Entities.MasterData;

namespace Services.CAR
{
    internal sealed class DynamicNewFormService(IDataManager data, IMDMRepository mdm, IMasterDataApi mdmP, ICacheManager memCache, ILocalizationService localization) : IDynamicNewFormService
    {
        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> GetDynamicFormConfiguration(string userid, string Language, string Plant, bool delflag)
        {
            var result = await data.DynamicNewForm.GetDynamicFormConfiguration(userid, Language, Plant, delflag);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<string>> ProcessSubmit(DynamicFormParameterDTO mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            IEnumerable<UsrDto> userList = Enumerable.Empty<UsrDto>();

            if (!string.IsNullOrEmpty(mydata.VendorCode))
            {
                userList = await data.MDM.CheckUserVSVend(mydata) ?? Enumerable.Empty<UsrDto>();
                if (!userList.Any())
                {
                    return ApiResponse<string>.FailResponse("Please Maintain Vendor data in MDM USERS Form");
                }

            }

            string newformno = await data.DynamicNewForm.GenerateNewFormNo(mydata.UserPlant, mydata.FormType, transaction);
            mydata.FormNumber = newformno;

            //await data.ISM.InsertDataIssueFeedback(mydata, transaction);
            await data.DynamicNewForm.InsertDataIssueFeedback(mydata, transaction);
            await data.DynamicNewForm.InsertIssueFeedBackEmailRecipient(mydata,userList, transaction);
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
                        if (mydata.NCCategoryImgFiles != null)
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

                        if (mydata.NCCategoryFiles != null)
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
        public async Task<ApiResponse<string>> issuerUpdate(DynamicFormParameterDTO mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);

            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.DynamicNewForm.issuerUpdateDataIssueFeedback(mydata, transaction);

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

                                    await data.DynamicNewForm.deleteDataAtchIssuer(dtaAtch, transaction);

                                    File.Delete(fullFilePath);
                                }
                            }
                        }
                        #endregion

                        if (mydata.NCCategoryImgFiles != null)
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

                        if (mydata.NCCategoryFiles != null)
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

        public async Task<ApiResponse<string>> issuerVoid(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.issuerVoid(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data VOID Succesfully");
        }

        public async Task<DirectoryCredentials> GetDirectoryAuth(string Domain, string UserID, string Password, string BasePath)
        {
            try
            {
                DirectoryCredentials result = new DirectoryCredentials();

                result = SharedFolderValidator.Validate(Domain, UserID, Password, BasePath);

                //if (result.Success)
                //{
                result.Domain = Domain;
                result.UserID = UserID;
                result.Password = Password;
                result.BasePath = BasePath;
                result.Success = true;
                //}
                //else
                //{
                //    result.Success = false;
                //    result.Message = "Unable to obtain the authorization, please contact your Administrator.";
                //}

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<string>> issuerMgrVoid(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.issuerMgrVoid(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data VOID Succesfully");
        }

        public async Task<ApiResponse<string>> issuerMgrReject(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.issuerMgrReject(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> issuerMngUpdate(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.issuerMngUpdate(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> pdaDecision(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.pdaDecision(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> pdaDecisionUpdate(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.pdaDecisionUpdate(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Update Succesfully");
        }

        public async Task<ApiResponse<string>> pdaApproval(DynamicFormParameterDTO mydata)
        {
            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();


            await data.DynamicNewForm.pdaApproval(mydata, transaction);


            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve Succesfully");
        }

        public async Task<ApiResponse<string>> pdaActionReject(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.pdaActionReject(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverAction(DynamicFormParameterDTO mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.DynamicNewForm.ReceiverAction(mydata, transaction);

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
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverActionUpdate(DynamicFormParameterDTO mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.DynamicNewForm.ReceiverActionUpdate(mydata, transaction);

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

        public async Task<ApiResponse<string>> ReceiverActionAppeal(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.ReceiverActionAppeal(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Appeal Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverIssueReject(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.ReceiverIssueReject(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverApproval(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.ReceiverApproval(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverMngReject(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.ReceiverMngReject(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> ReceiverApprovalToReject(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.ReceiverApprovalToReject(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Approve to Reject Succesfully");
        }

        public async Task<ApiResponse<string>> PDAReviewerVoid(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.PDAReviewerVoid(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Void Succesfully");
        }

        public async Task<ApiResponse<string>> PDAReviewerReject(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.PDAReviewerReject(mydata, transaction);
            //string newformno = await data.ISM.GenerateNewFormNoWithVer(mydata, transaction);
            //await data.ISM.CreateNewIssueFeedBcakWithVers(mydata.FormNumber, newformno, mydata.UserId, mydata.UserName, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully");
        }

        public async Task<ApiResponse<string>> PDAReviewerAprove(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.PDAReviewerAprove(mydata, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Submit Succesfully");
        }

        public async Task<ApiResponse<string>> ReviewerSubmit(DynamicFormParameterDTO mydata)
        {
            var basepathconfig = await mdm.getBasePathConfig(mydata.UserPlant);


            await using var conn = await data.ISM.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            if (!basepathconfig.Any())
            {
                return ApiResponse<string>.FailResponse("Master Data Base Path For Attachment Not Found");
            }

            await data.DynamicNewForm.ReviewerSubmit(mydata, transaction);

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
                        if (mydata.reviewerImgFiles != null)
                        {
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

                        if (mydata.reviewerFiles != null)
                        {
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

        public async Task<ApiResponse<string>> ReviewerReject(DynamicFormParameterDTO mydata)
        {

            await using var conn = await data.DynamicNewForm.OpenConnectionAsync();
            await using SqlTransaction transaction = conn.BeginTransaction();

            await data.DynamicNewForm.ReviewerReject(mydata, transaction);

            string newformno = await data.DynamicNewForm.GenerateNewFormNoWithVer(mydata, transaction);
            await data.ISM.CreateNewIssueFeedBcakWithVers(mydata.FormNumber, newformno, mydata.UserId, mydata.UserName, transaction);

            await transaction.CommitAsync();
            return ApiResponse<string>.SuccessResponse(null, "Data Reject Succesfully, New Form No Created : " + newformno);
        }

        public async Task<ApiResponse<string>> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param)
        {
            var result = await data.ISM.cekAvailableCompletePastIssue(param);
            return ApiResponse<string>.SuccessResponse(result);
        }
    }
}
