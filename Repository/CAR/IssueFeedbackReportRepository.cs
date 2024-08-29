using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;
using Repository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Repository.CAR
{
    internal sealed class IssueFeedbackReportRepository(DbContext dbContext) : IIssueFeedbackReportRepository
    {
        public async Task<int> GetTotalRecord(GlobalParam param, ConditionParams ConditionParams) {
            string query = string.Format(IssueFeedbackReportQuery.GetTotalRecord);
            query += Environment.NewLine;
            query += ConditionParams.ExtraWhereCondition;
            query += Environment.NewLine;


            await using var conn = dbContext.CARConnection();
            return await conn.ExecuteScalarAsync<int>(query, new { plant = param.Plant });
        }

        public async Task<IEnumerable<IssueFeedbackDto>> GetMaindata(GlobalParam param, ConditionParams ConditionParams)
        {
            string query = string.Format(IssueFeedbackReportQuery.GetMainData);
            query += Environment.NewLine;
            query += ConditionParams.ExtraWhereCondition;
            query += Environment.NewLine;
            query += ConditionParams.OrderByCondition;
            query += Environment.NewLine;
            query += IssueFeedbackReportQuery.skiprow;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<IssueFeedbackDto>(query, new { plant = param.Plant, skip = ConditionParams.skip, take = ConditionParams.take });
        }

        public async Task<IEnumerable<IssueFeedbackAtchmentDto>> GetDataAttchment(int plant, IEnumerable<string> FormNoList, SqlTransaction? transaction)
        {
            string query = string.Format(IssueFeedbackReportQuery.GetDataAttchment);
            if (transaction is null)
            {
                await using var conn = dbContext.CARConnection();
                return await conn.QueryAsync<IssueFeedbackAtchmentDto>(query, new { plant = plant, FormNo = FormNoList });
            }
            else
            {
                var conn = transaction.Connection;
                return await conn.QueryAsync<IssueFeedbackAtchmentDto>(query, new { plant = plant, FormNo = FormNoList }, transaction);
            }
            
        }
    }
}
