using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Repository.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
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

        public async Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration( bool delflag)
        {
            string query;

            query = DynamicFormConfigurationQuery.GetDynamicFormConfiguration;
         
            if (!delflag)
            {
                query += " and DelFlag = 0";
            }
            await using var conn = dbContext.CARConnection();
            //return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new {
            var result = await conn.QueryAsync<DynamicFormConfigurationDto>(query);


            foreach (var item in result)
            {
                if (item.OptionDataResource == "Execute Query")
                {
                    string queryLowerCase = item.Query.ToLower();
                    string queryWithPlantReplaced;

                    if (queryLowerCase.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
                    {
                        string takeParameterDB = "SELECT UsePlant, UseUserId FROM AllowParameters";
                        var executedParameters = await conn.QueryFirstOrDefaultAsync<UsingParameter>(takeParameterDB);

                        string usePlant = executedParameters.UsePlant ?? string.Empty;
                        string useUserId = executedParameters.UseUserId ?? string.Empty;

                        queryWithPlantReplaced = item.Query.ToLower()
                                                    .Replace("@plant", usePlant)
                                                    .Replace("@userid", $"'{useUserId}'");

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
        public async Task<int> InsertDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string queryMappingColumnList = "SELECT DISTINCT FieldName, UIDisplay From TableMappingFieldName Where Delflag = 0";
            var conn = transaction.Connection;
            var columnMappings = (await conn.QueryAsync<(string FieldName, string UIDisplay)>(queryMappingColumnList, transaction: transaction))
                        .ToDictionary(x => x.FieldName, x => x.UIDisplay);
            var parameters = GetSqlParametersFromEntity(mydata, columnMappings);
            var query = BuildInsertQuery("IssueFeedback", parameters);
            var dParams = new Dapper.DynamicParameters();
            foreach (var param in parameters)
            {
                dParams.Add(param.ParameterName, param.Value);
            }
            return await conn.ExecuteAsync(query, dParams, transaction);
        }

        public static List<SqlParameter> GetSqlParametersFromEntity(object entity, Dictionary<string, string> columnMappings)
        {
            var sqlParameters = new List<SqlParameter>();
            //string queryMappingColumnList = "SELECT DISTINCT FieldName, UIDisplay From TableMappingFieldName Where Delflag = 0";
            

            foreach (var prop in entity.GetType().GetProperties())
            {
                var value = prop.GetValue(entity);
                if (value != null)
                {
                    if (prop.Name.Equals("DynamicParameters", StringComparison.InvariantCultureIgnoreCase) &&
                        value is IEnumerable<DynamicParameter> dynamicParams)
                    {
                        int index = 0;
                        foreach (var dynParam in dynamicParams)
                        {
                            //sqlParameters.Add(new SqlParameter($"@{dynParam.FieldName}", value));
                            //// sqlParameters.Add(new SqlParameter(dynParam.FieldName, dynParam.FieldValue));
                            //index++;
                            //if (sqlColumnList.Contains(dynParam.FieldName))
                            //{
                            //    sqlParameters.Add(new SqlParameter($"@{dynParam.FieldName}", dynParam.FieldValue ?? DBNull.Value));
                            //}
                            var matchingKey = columnMappings.FirstOrDefault(x => x.Value.Equals(dynParam.FieldName, StringComparison.InvariantCultureIgnoreCase));
                            if (!string.IsNullOrEmpty(matchingKey.Key))
                            {
                                sqlParameters.Add(new SqlParameter($"@{matchingKey.Key}", dynParam.FieldValue));
                            }
                            else
                            {
                                sqlParameters.Add(new SqlParameter($"@{dynParam.FieldName}", dynParam.FieldValue));
                            }

                        }
                    }
                    else
                    {
                        sqlParameters.Add(new SqlParameter($"@{prop.Name}", value));
                    }
                }
            }

            return sqlParameters;
        }
        public static string BuildInsertQuery(string tableName, List<SqlParameter> parameters)
        {
            var columnReplacements = new Dictionary<string, string>
            {
                { "FormNumber", "FormNo" },
                { "Comment", "IssueByComment" }

            };

            // List of parameters to exclude
            var excludedParameters = new HashSet<string>
            {
                "@IssueStatus", "@UserId", "@UserName", "@mailWStatus", "@mailactionType", "@sendmailUserAction","@UserPlant","@Comment"
            };

            var filteredParameters = parameters.Where(p => !excludedParameters.Contains(p.ParameterName)).ToList();
            var columns = string.Join(", ", filteredParameters.Select(p =>
            {
                var paramName = p.ParameterName.Substring(1);  // Remove '@' from parameter names
                return columnReplacements.ContainsKey(paramName) ? columnReplacements[paramName] : paramName;
            }));
            var values = string.Join(", ", filteredParameters.Select(p => p.ParameterName));

            return $"INSERT INTO {tableName} (Plant, {columns}, Status, MainStatus, IssueBy, IssueByName, IssueDate) VALUES (@UserPlant, {values},'SUBMITED','CAR RAISE',@UserId,@UserName,GETDATE())";
        }

        public async Task<int> issuerUpdateDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction)
        {
            string query = DynamicNewFormQuery.issuerUpdateDataIssueFeedback;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
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
            var columnMappings = (await conn.QueryAsync<(string FieldName, string UIDisplay)>(queryMappingColumnList, transaction: transaction))
                        .ToDictionary(x => x.FieldName, x => x.UIDisplay);
            var parameters = GetSqlParametersFromEntity(mydata, columnMappings);
            var dParams = new Dapper.DynamicParameters();
            foreach (var param in parameters)
            {
                dParams.Add(param.ParameterName, param.Value);
            }
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
