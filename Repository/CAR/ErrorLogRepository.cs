using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.CAR
{
    public class ErrorLogRepository(DbContext dbContext) : IErrorLogRepository
    {
        private readonly DbContext _dbContext = dbContext;
        public async Task<(IEnumerable<ErrorLogModel> data, int totalCount)> ShowDataErrorLog(ErrorLogParam param, ConditionParams ConditionParams)
        {
            string query = "select ID,Description as ErrDescription,UserID as AddedBy,DateOccured AddedOn from ERRORLOG ";
            if ((ConditionParams.ExtraWhereCondition == null ? "" : ConditionParams.ExtraWhereCondition) != "")
            {
                query += Environment.NewLine;
                query += @" AND " + ConditionParams.ExtraWhereCondition;
            }
            if ((ConditionParams.OrderByCondition == null ? "" : ConditionParams.OrderByCondition) == "")
            {
                query += Environment.NewLine;
                query += "order by ID DESC ";
            }
            else
            {
                query += Environment.NewLine;
                query += "order by " + ConditionParams.OrderByCondition;
            }

            query += Environment.NewLine;
            query += "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;";


            await using var conn = _dbContext.CARConnection();
            var data = await conn.QueryAsync<ErrorLogModel>(query, new { skip = ConditionParams.skip, take = ConditionParams.take });

            string countQuery = "SELECT COUNT(*) FROM ERRORLOG ";
            if (!string.IsNullOrEmpty(ConditionParams.ExtraWhereCondition))
            {
                countQuery += " AND " + ConditionParams.ExtraWhereCondition;
            }
            var totalCount = await conn.ExecuteScalarAsync<int>(countQuery);
            return (data, totalCount);
        }

        public async Task<int> InsertDataToTERRORLOG(ErrorLogModel ErrorLogData, SqlTransaction? transaction = null)
        {
            const string query = "INSERT INTO ERRORLOG (Description, UserID, DateOccured) VALUES (@Description, @UserID, CURRENT_TIMESTAMP)";
            await using var conn = _dbContext.CARConnection();

            return await conn.ExecuteAsync(query, new { Description = ErrorLogData.ErrDescription, UserID = ErrorLogData.AddedBy });
        }
    }
}
