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

        public async Task<string> GenerateNewFormNo(int plant, string FormType,SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.GenerateNewFormNo;
            var conn = transaction.Connection;
            return await conn.QueryFirstOrDefaultAsync<string>(query, new { plant = plant , FormType = FormType }, transaction);
        }

        public async Task<string> GenerateNewFormNoWithVer(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.GenerateNewFormNoWithVer;
            var conn = transaction.Connection;
            return await conn.QueryFirstOrDefaultAsync<string>(query, mydata, transaction);
        }

        public async Task<int> InsertDataIssueFeedback(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.InsertDataIssueFeedback;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> CreateNewIssueFeedBcakWithVers(string OldFormNumber, string NewFormNumber,string UserId,string UserName, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.CreateNewIssueFeedBcakWithVers;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, new { OldFormNumber = OldFormNumber, NewFormNumber = NewFormNumber, UserId = UserId, UserName = UserName }, transaction);
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

        public async Task<int> issuerMgrVoid(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.issuerMgrVoid;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> issuerMgrReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.issuerMgrReject;
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

        public async Task<int> pdaActionReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.pdaActionReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverAction(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverAction;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverActionUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverActionUpdate;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverActionAppeal(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverActionAppeal;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverIssueReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverIssueReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverApproval(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverApproval;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverMngReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverMngReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReceiverApprovalToReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReceiverApprovalToReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> PDAReviewerVoid(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.PDAReviewerVoid;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> PDAReviewerReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.PDAReviewerReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }


        public async Task<int> PDAReviewerAprove(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.PDAReviewerAprove;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReviewerSubmit(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReviewerSubmit;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<int> ReviewerReject(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = IssueSubmissionQuery.ReviewerReject;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<string> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param)
        {
            string query = IssueSubmissionQuery.cekAvailableCompletePastIssue;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryFirstOrDefaultAsync<string>(query, param);
        }
    }
}
