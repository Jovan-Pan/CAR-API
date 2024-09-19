using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Repository.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.CAR
{
    public class ErrorLogRepository(DbContext dbContext) : IErrorLogRepository
    {
        private readonly DbContext _dbContext = dbContext;
        public async Task<(IEnumerable<ErrorLogModel> data, int totalCount)> ShowDataErrorLog(GlobalParam param, ConditionParams ConditionParams)
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
            const string query = "INSERT INTO ERRORLOG (Description,ErrSource, UserID, DateOccured) VALUES (@Description, @ErrSource,@UserID, CURRENT_TIMESTAMP)";
            await using var conn = _dbContext.CARConnection();

            return await conn.ExecuteAsync(query, new { Description = ErrorLogData.ErrDescription, ErrSource=ErrorLogData.ErrSource, UserID = ErrorLogData.AddedBy });
        }

        public async Task<IEnumerable<ErrorLogModel>> GetDataReport(string? startdate, string? enddate)
        {
            string query;
            if (string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(enddate))
            {
                query = "SELECT ID,  Description AS ErrDescription,  UserID AS AddedBy, DateOccured AS AddedOn FROM  ERRORLOG WHERE DateOccured >= CAST(GETDATE() AS DATE) ORDER BY ID DESC";
            }   
            else 
            {
                query = "SELECT ID, Description AS ErrDescription, UserID AS AddedBy, DateOccured AS AddedOn FROM ERRORLOG WHERE DateOccured BETWEEN @startdate AND @enddate";
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ErrorLogModel>(query, new { startdate = startdate, enddate = enddate });
        }
    }
}
