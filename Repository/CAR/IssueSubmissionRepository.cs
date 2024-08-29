using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Repository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.CAR
{
    internal sealed class IssueSubmissionRepository(DbContext dbContext) : IIssueSubmissionRepository
    {
        public async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = dbContext.CARConnection();
            await connection.OpenAsync();
            return connection;
        }

        public async Task<string> GenerateNewFormNo(SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.GenerateNewFormNo;
            var conn = transaction.Connection;
            return await conn.QueryFirstOrDefaultAsync<string>(query, null, transaction);
        }

        public async Task<int> InsertDataIssueFeedback(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.InsertDataIssueFeedback;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> InsertDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.InsertDataAtchIssuer;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> issuerUpdateDataIssueFeedback(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.issuerUpdateDataIssueFeedback;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> deleteDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.deleteDataAtchIssuer;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> issuerMngUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.issuerMngUpdate;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaDecision(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.pdaDecision;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaDecisionUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.pdaDecisionUpdate;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> pdaApproval(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.pdaApproval;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverAction(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverAction;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }
    }
}
