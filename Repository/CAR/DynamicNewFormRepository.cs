using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Repository.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace Repository.CAR
{
    internal sealed class DynamicNewFormRepository(DbContext dbContext) :IDynamicNewFormRepository
    {
        public async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = dbContext.CARConnection();
            await connection.OpenAsync();
            return connection;
        }

        public async Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration(string userid, string Language, string Plant, bool delflag)
        {

            string query = DynamicNewFormQuery.GetDynamicFormConfiguration;
           
            if (!delflag)
            {
                query += " and dfc.DelFlag = 0 ";
            }
            query += "),\r\nDefaultLanguage AS(\r\n    SELECT\r\n        dfc.Id,\r\n        dfc.plant,\r\n        dfc.FormType,\r\n        tbfn.UIDisplay AS FieldName_EN,\r\n        dfc.FieldName AS FieldNameForModel,\r\n        tbfn.language,\r\n        dfc.FieldType,\r\n        dfc.FieldLength,\r\n        dfc.Mandatory,\r\n        dfc.FieldElement,\r\n        dfc.OptionDataResource,\r\n        dfc.DBResource,\r\n        dfc.Query,\r\n        dfc.DataOption,\r\n        dfc.Sequence,\r\n        dfc.CreatedBy,\r\n        dfc.CreatedByName,\r\n        dfc.CreatedDate,\r\n        dfc.UpdatedBy,\r\n        dfc.UpdatedByName,\r\n        dfc.UpdatedDate,\r\n        dfc.delflag\r\n    FROM DynamicFormConfiguration dfc\r\n    INNER JOIN TableMappingFieldName tbfn\r\n        ON dfc.FieldName = tbfn.FieldName\r\n        AND dfc.plant = tbfn.plant\r\n    WHERE dfc.Plant = @plant\r\n    AND tbfn.DelFlag = 0\r\n    AND tbfn.language = 'EN'";

            if (!delflag)
            {
                query += " and dfc.DelFlag = 0 ";
            }
            query += ")\r\nSELECT\r\n    COALESCE(ld.Id, dl.Id) AS Id,\r\n    COALESCE(ld.plant, dl.plant) AS plant,\r\n    COALESCE(ld.FormType, dl.FormType) AS FormType,\r\n    COALESCE(ld.FieldName_ZH, dl.FieldName_EN) AS FieldName,\r\n    COALESCE(ld.FieldNameForModel, dl.FieldNameForModel) AS FieldNameForModel,\r\n    COALESCE(ld.language, dl.language) AS language,\r\n    COALESCE(ld.FieldType, dl.FieldType) AS FieldType,\r\n    COALESCE(ld.FieldLength, dl.FieldLength) AS FieldLength,\r\n    COALESCE(ld.Mandatory, dl.Mandatory) AS Mandatory,\r\n    COALESCE(ld.FieldElement, dl.FieldElement) AS FieldElement,\r\n    COALESCE(ld.OptionDataResource, dl.OptionDataResource) AS OptionDataResource,\r\n    COALESCE(ld.DBResource, dl.DBResource) AS DBResource,\r\n    COALESCE(ld.Query, dl.Query) AS Query,\r\n    COALESCE(ld.DataOption, dl.DataOption) AS DataOption,\r\n    COALESCE(ld.Sequence, dl.Sequence) AS Sequence,\r\n    COALESCE(ld.CreatedBy, dl.CreatedBy) AS CreatedBy,\r\n    COALESCE(ld.CreatedByName, dl.CreatedByName) AS CreatedByName,\r\n    COALESCE(ld.CreatedDate, dl.CreatedDate) AS CreatedDate,\r\n    COALESCE(ld.UpdatedBy, dl.UpdatedBy) AS UpdatedBy,\r\n    COALESCE(ld.UpdatedByName, dl.UpdatedByName) AS UpdatedByName,\r\n    COALESCE(ld.UpdatedDate, dl.UpdatedDate) AS UpdatedDate,\r\n    COALESCE(ld.delflag, dl.delflag) AS delflag\r\nFROM DefaultLanguage dl\r\nLEFT JOIN LanguageData ld ON dl.Id = ld.Id;";
            await using var conn = dbContext.CARConnection();
            //return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new {
            var result = await conn.QueryAsync<DynamicFormConfigurationDto>(query, new
            {
                Language = Language,
                Plant = Plant,
                userid = userid
            });


            foreach (var item in result)
            {
                if (item.OptionDataResource == "Execute Query")
                {
                    string queryLowerCase = item.Query.ToLower();
                    string queryWithPlantReplaced;

                    if (queryLowerCase.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
                    {
                        string takeParameterDB = "SELECT AllowParameters FROM AllowParameters";
                        var executedParameters = await conn.QueryAsync<string>(takeParameterDB);

                        HashSet<string> allowedParams = executedParameters
                                              .Select(p => p.ToLower())
                                              .ToHashSet();

                        var regex = new Regex(@"@\w+", RegexOptions.IgnoreCase);
                        var foundParams = regex.Matches(item.Query)
                                               .Select(match => match.Value.ToLower())
                                               .ToHashSet();

                        var invalidParams = foundParams.Except(allowedParams);
                        if (invalidParams.Any())
                        {
                            return new List<DynamicFormConfigurationDto>
                            {

                            };
                        }

                        queryWithPlantReplaced = item.Query.ToLower()
                                                    .Replace("@plant", Plant)
                                                    .Replace("@userid", $"'{userid}'");

                    }
                    else
                    {
                        queryWithPlantReplaced = item.Query;
                    }
                    string executedQuery = "USE " + item.DBResource + "; " + queryWithPlantReplaced;
                    var executedQueryResult = await conn.QueryAsync<dynamic>(executedQuery);

                    // Initialize the list if it is null
                    if (item.ExecutedQueryResult == null)
                    {
                        item.ExecutedQueryResult = new List<Dictionary<string, object>>();
                    }

                    // Accumulate rows from the executed query result
                    foreach (var row in executedQueryResult)
                    {
                        var executedQueryResultDict = new Dictionary<string, object>();

                        foreach (var kvp in (IDictionary<string, object>)row)
                        {
                            if (kvp.Key != null)
                            {
                                executedQueryResultDict[kvp.Key] = kvp.Value;
                            }
                        }

                        item.ExecutedQueryResult.Add(executedQueryResultDict);
                    }
                }
            }

            return result;
        }
        public async Task<string> GenerateNewFormNo(int plant, string FormType, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.GenerateNewFormNo;
            var conn = transaction.Connection;
            return await conn.QueryFirstOrDefaultAsync<string>(query, new { plant = plant, FormType = FormType }, transaction);
        }

        public async Task<string> GenerateNewFormNoWithVer(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.GenerateNewFormNoWithVer;
            var conn = transaction.Connection;
            return await conn.QueryFirstOrDefaultAsync<string>(query, mydata, transaction);
        }

        public async Task<int> CreateNewIssueFeedBcakWithVers(string OldFormNumber, string NewFormNumber, string UserId, string UserName, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.CreateNewIssueFeedBcakWithVers;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, new { OldFormNumber = OldFormNumber, NewFormNumber = NewFormNumber, UserId = UserId, UserName = UserName }, transaction);
        }

        public async Task<int> InsertDataAtchIssuer(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.InsertDataAtchIssuer;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> InsertIssueFeedBackEmailRecipient(DynamicFormParameterDTO mydata, IEnumerable<UsrDto> userList, SqlTransaction transaction)
        {
            string insertQuery = DynamicNewFormQuery.InsertIssueFeedBackEmailRecipient;
            string deleteQueryAll = "DELETE FROM IssueFeedBackEmailRecipient WHERE FormNo = @FormNumber";
            string deleteQueryByUserLevel = "DELETE FROM IssueFeedBackEmailRecipient WHERE FormNo = @FormNumber AND UserLevel = @UserLevel";
            string deleteQueryPartial = "DELETE FROM IssueFeedBackEmailRecipient WHERE FormNo = @FormNumber AND UserLevel = @UserLevel AND UseID NOT IN @ExistingUserIDs";
            string selectQuery = "SELECT UseID, UserLevel FROM IssueFeedBackEmailRecipient WHERE FormNo = @FormNumber";

            var conn = transaction.Connection;

            if (mydata.IssueFeedBackEmailRecipients == null)
            {
                await conn.ExecuteAsync(deleteQueryAll, new { FormNumber = mydata.FormNumber }, transaction);
                return 0;
            }

            var existingUsers = (await conn.QueryAsync<(string UseID, string UserLevel)>(
                selectQuery, new { FormNumber = mydata.FormNumber }, transaction
            )).ToList();

            var existingUserDict = existingUsers.ToDictionary(u => $"{u.UseID}-{u.UserLevel}");

            var newUsersByLevel = mydata.IssueFeedBackEmailRecipients
                .Where(r => !string.IsNullOrEmpty(r.UseID))
                .GroupBy(r => r.UserLevel)
                .ToDictionary(g => g.Key, g => g.Select(u => u.UseID).ToList());


            var existingUserLevels = existingUsers.Select(u => u.UserLevel).Distinct().ToList();


            foreach (var userLevel in existingUserLevels)
            {
                if (!newUsersByLevel.ContainsKey(userLevel))
                {
                    await conn.ExecuteAsync(deleteQueryByUserLevel, new { FormNumber = mydata.FormNumber, UserLevel = userLevel }, transaction);
                }
            }

            foreach (var kvp in newUsersByLevel)
            {
                var userLevel = kvp.Key;
                var userIds = kvp.Value;

                await conn.ExecuteAsync(deleteQueryPartial, new
                {
                    FormNumber = mydata.FormNumber,
                    UserLevel = userLevel,
                    ExistingUserIDs = userIds.Any() ? userIds : new List<string> { "-1" }
                }, transaction);
            }

            var affectedRows = 0;
            foreach (var recipient in mydata.IssueFeedBackEmailRecipients)
            {
                string key = $"{recipient.UseID}-{recipient.UserLevel}";

                if (!existingUserDict.ContainsKey(key))
                {
                    var dParams = new Dapper.DynamicParameters();
                    dParams.Add("@FormNumber", mydata.FormNumber);
                    dParams.Add("@UseID", recipient.UseID);
                    dParams.Add("@UseNam", recipient.UseNam);
                    dParams.Add("@UseEmail", recipient.UseEmail);
                    dParams.Add("@UserLevel", recipient.UserLevel);

                    affectedRows += await conn.ExecuteAsync(insertQuery, dParams, transaction);
                }
            }

            if (!string.IsNullOrEmpty(mydata.VendorCode))
            {
                foreach (var user in userList)
                {
                    string key = $"{user.UseID}-IssueUserVendor";

                    if (!existingUserDict.ContainsKey(key))
                    {
                        var dParams = new Dapper.DynamicParameters();
                        dParams.Add("@FormNumber", mydata.FormNumber);
                        dParams.Add("@UseID", user.UseID);
                        dParams.Add("@UseNam", user.UseNam);
                        dParams.Add("@UseEmail", user.UseEmail);
                        dParams.Add("@UserLevel", "IssueUserVendor");

                        affectedRows += await conn.ExecuteAsync(insertQuery, dParams, transaction);
                    }
                }
            }

            return affectedRows;
        }

        public async Task<int> InsertDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string checkExistDataQuery = "SELECT FormNo From ISSUEFEEDBACK Where FORMNO =@FormNo";
            var conn = transaction.Connection;
            string? existingCount = await conn.ExecuteScalarAsync<string>(checkExistDataQuery, new { FormNo = mydata.FormNumber }, transaction);
            var parameters = GetSqlParametersFromEntity(mydata);
            var query = BuildInsertQuery("IssueFeedback", parameters, existingCount);
            var dParams = new Dapper.DynamicParameters();
            foreach (var param in parameters)
            {
                dParams.Add(param.ParameterName, param.Value);
            }

            return await conn.ExecuteAsync(query, dParams, transaction);
        }

        public static List<SqlParameter> GetSqlParametersFromEntity(object entity)
        {
            var sqlParameters = new List<SqlParameter>();
            //string queryMappingColumnList = "SELECT DISTINCT FieldName, UIDisplay From TableMappingFieldName Where Delflag = 0";
            

            foreach (var prop in entity.GetType().GetProperties())
            {
                var value = prop.GetValue(entity);

                if (value is Microsoft.AspNetCore.Http.IFormFile)
                {
                    continue;
                    
                }
                if (value is IEnumerable<Microsoft.AspNetCore.Http.IFormFile> fileCollection)
                {
                    continue;
                }
                if (prop.Name == nameof(DynamicFormParameterDTO.IssueFeedBackEmailRecipients))
                {
                    continue;
                }
                //if (value != null)
                //{
                    if (prop.Name.Equals("DynamicParameters", StringComparison.InvariantCultureIgnoreCase) &&
                        value is IEnumerable<DynamicParameter> dynamicParams)
                    {
                        int index = 0;
                        foreach (var dynParam in dynamicParams)
                        {
                            sqlParameters.Add(new SqlParameter($"@{dynParam.FieldName}", dynParam.FieldValue));
                             //sqlParameters.Add(new SqlParameter(dynParam.FieldName, dynParam.FieldValue));
                            index++;
                            //if (sqlColumnList.Contains(dynParam.FieldName))
                            //{
                            //    sqlParameters.Add(new SqlParameter($"@{dynParam.FieldName}", dynParam.FieldValue ?? DBNull.Value));
                            //}
                            //var matchingKey = columnMappings.FirstOrDefault(x => x.Value.Equals(dynParam.FieldName, StringComparison.InvariantCultureIgnoreCase));
                            //if (!string.IsNullOrEmpty(matchingKey.Key))
                            //{
                            //    sqlParameters.Add(new SqlParameter($"@{matchingKey.Key}", dynParam.FieldValue));
                            //}
                            //else
                            //{
                            //    sqlParameters.Add(new SqlParameter($"@{dynParam.FieldName}", dynParam.FieldValue));
                            //}

                        }
                    }
                    else
                    {
                        sqlParameters.Add(new SqlParameter($"@{prop.Name}", value));
                    }
                //}
            }

            return sqlParameters;
        }
        public static string BuildInsertQuery(string tableName, List<SqlParameter> parameters, string existingCount)
        {
            var columnReplacements = new Dictionary<string, string>
            {
                { "FormNumber", "FormNo" },
                { "Comment", "IssueByComment" }

            };

            // List of parameters to exclude
            var excludedParameters = new HashSet<string>
            {
                "@IssueStatus", "@UserId", "@UserName", "@mailWStatus", "@mailactionType", "@sendmailUserAction","@UserPlant","@MainStatus","@NCCategoryImgFiles","@NCCategoryFiles","@DetectionDate","@rejectReason", "@PDAImmediteAct", "@rootcause", "@correctiveAct",
    "@immediteActReceiverImgFiles", "@immediteActReceiverFiles",
    "@rootCauseReceiverImgFiles", "@rootCauseReceiverFiles",
    "@correctiveActReceiverImgFiles", "@correctiveActReceiverFiles",
    "@reviewerImgFiles", "@reviewerFiles"
            };
            var excludedParametersForUpdate = new HashSet<string>
            {
                "@IssueStatus", "@UserId", "@UserName", "@mailWStatus", "@mailactionType", "@sendmailUserAction","@UserPlant","@MainStatus","@NCCategoryImgFiles","@NCCategoryFiles","@FormNumber","@FormType","@rejectReason", "@PDAImmediteAct", "@rootcause", "@correctiveAct",
    "@immediteActReceiverImgFiles", "@immediteActReceiverFiles",
    "@rootCauseReceiverImgFiles", "@rootCauseReceiverFiles",
    "@correctiveActReceiverImgFiles", "@correctiveActReceiverFiles",
    "@reviewerImgFiles", "@reviewerFiles"
            };

            //var filteredParameters = parameters.Where(p => !excludedParameters.Contains(p.ParameterName)).ToList();

            var detectionDateParam = parameters.FirstOrDefault(p => p.ParameterName == "@DetectionDate");
            string detectionDateValue = detectionDateParam == null || detectionDateParam.Value == null ||
                                        string.IsNullOrEmpty(detectionDateParam.Value.ToString())
                                        ? "GETDATE()" // Use current date if missing or empty
                                        : "@DetectionDate"; // Use provided value if exists

            if (existingCount == null)
            {
                var filteredParameters = parameters.Where(p => !excludedParameters.Contains(p.ParameterName)).ToList();

                var columns = string.Join(", ", filteredParameters.Select(p =>
                {
                    var paramName = p.ParameterName.Substring(1);  // Remove '@' from parameter names
                    return columnReplacements.ContainsKey(paramName) ? columnReplacements[paramName] : paramName;
                }));
                var values = string.Join(", ", filteredParameters.Select(p => p.ParameterName));

                return $"INSERT INTO {tableName} (Plant,DetectionDate, {columns}, Status, MainStatus, IssueBy, IssueByName, IssueDate) VALUES (@UserPlant,{detectionDateValue}, {values},@IssueStatus,@MainStatus,@UserId,@UserName,GETDATE())";
            }
            else
            {
                var filteredParameters = parameters.Where(p => !excludedParametersForUpdate.Contains(p.ParameterName)).ToList();

                var setClause = string.Join(", ", filteredParameters.Select(p =>
                {
                    var paramName = p.ParameterName.Substring(1); // Hilangkan '@'
                    var columnName = columnReplacements.ContainsKey(paramName) ? columnReplacements[paramName] : paramName;
                    //return $"{columnName} = {p.ParameterName}";

                    var value = p.Value == DBNull.Value || p.Value == null
                    ? "NULL"
                    : p.ParameterName;

                    return $"{columnName} = {value}";
                }));

                return $"Update {tableName} set {setClause}, Status = @IssueStatus, MainStatus =@MainStatus, IssueBy =@UserId, IssueByName =@UserName, IssueDate = GETDATE() WHERE FORMNO =@FormNumber";

            }
        }

        public static string BuildUpdateQuery(string tableName, List<SqlParameter> parameters)
        {
            var columnReplacements = new Dictionary<string, string>
            {
                 { "FormNumber", "FormNo" },
                 { "Comment", "IssueByComment" }
            };

            // List of parameters yang tidak ingin diupdate (misalnya, kolom sistem atau yang hanya untuk insert)
            var excludedParameters = new HashSet<string>
            {
                 "@UserPlant", "@IssueStatus", "@MainStatus", "@UserId", "@UserName",
                 "@mailWStatus", "@mailactionType", "@sendmailUserAction", "@NCCategoryImgFiles", "@NCCategoryFiles","@FormType","@FormNumber","@IssueType"
            };

            // Buat list parameter yang akan diupdate
            var filteredParameters = parameters.Where(p => !excludedParameters.Contains(p.ParameterName)).ToList();

            // Bangun SET clause: col1 = @param1, col2 = @param2, dst.
            var setClause = string.Join(", ", filteredParameters.Select(p =>
            {
                var paramName = p.ParameterName.Substring(1); // Hilangkan '@'
                var columnName = columnReplacements.ContainsKey(paramName) ? columnReplacements[paramName] : paramName;
                return $"{columnName} = {p.ParameterName}";
            }));

            // Asumsikan primary key adalah FormNo
            return $"UPDATE {tableName} SET {setClause}, IssueUpdatedBy = @UserId, IssueUpdatedByName = @UserName, IssueUpdatedDate = GETDATE(), Status = @IssueStatus, MainStatus = @MainStatus WHERE FormNo = @FormNumber";
        }

        public async Task<int> issuerUpdateDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            //string query = DynamicNewFormQuery.issuerUpdateDataIssueFeedback;
            var conn = transaction.Connection;
            var parameters = GetSqlParametersFromEntity(mydata);
            var query = BuildUpdateQuery("IssueFeedback", parameters);
            var dParams = new Dapper.DynamicParameters();
            foreach (var param in parameters)
            {
                dParams.Add(param.ParameterName, param.Value);
            }
            return await conn.ExecuteAsync(query, dParams, transaction);
        }

        public async Task<int> issuerVoid(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.issuerVoid;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> deleteDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.deleteDataAtchIssuer;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> issuerMgrVoid(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.issuerMgrVoid;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> issuerMgrReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.issuerMgrReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> issuerMngUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
             string query = DynamicNewFormQuery.issuerMngUpdate;
            string queryMappingColumnList = "SELECT DISTINCT FieldName, UIDisplay From TableMappingFieldName Where Delflag = 0";
            var conn = transaction.Connection;
            //var columnMappings = (await conn.QueryAsync<(string FieldName, string UIDisplay)>(queryMappingColumnList, transaction: transaction))
            //            .ToDictionary(x => x.FieldName, x => x.UIDisplay);
            var parameters = GetSqlParametersFromEntity(mydata);
            var dParams = new Dapper.DynamicParameters();
            foreach (var param in parameters)
            {
                dParams.Add(param.ParameterName, param.Value);
            }
            if(!string.IsNullOrEmpty(mydata.Comment))
            {
                query += ",AcknowledgeByComment = @Comment ";
            }
            if (mydata.Dept == "VEND" )
            {
                query += ",VendorCode = @VendorCode,VendorDesc = @VendorDesc ";
            }
            else if (!string.IsNullOrEmpty(mydata.Dept))
            {
                query += ",Dept = @Dept ";
            }
            query += "where FormNo = @FormNumber";
            return await conn.ExecuteAsync(query, dParams, transaction);
            //return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaDecision(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.pdaDecision;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaDecisionUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.pdaDecisionUpdate;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaApproval(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.pdaApproval;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaActionReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.pdaActionReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverAction(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverAction;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverActionUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverActionUpdate;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverActionAppeal(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverActionAppeal;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverIssueReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverIssueReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverApproval(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverApproval;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverMngReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverMngReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverApprovalToReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReceiverApprovalToReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> PDAReviewerVoid(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.PDAReviewerVoid;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> PDAReviewerReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.PDAReviewerReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }


        public async Task<int> PDAReviewerAprove(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.PDAReviewerAprove;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReviewerSubmit(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReviewerSubmit;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReviewerReject(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.ReviewerReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<string> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param)
        {
            string query = DynamicNewFormQuery.cekAvailableCompletePastIssue;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryFirstOrDefaultAsync<string>(query, param);
        }
    }
}
