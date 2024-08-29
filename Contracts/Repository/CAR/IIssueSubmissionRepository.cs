using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface IIssueSubmissionRepository
    {
        Task<SqlConnection> OpenConnectionAsync();
        Task<string> GenerateNewFormNo(SqlTransaction transaction);
        Task<int> InsertDataIssueFeedback(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> InsertDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction);
        Task<int> issuerUpdateDataIssueFeedback(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> deleteDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction);
        Task<int> issuerMngUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> pdaDecision(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> pdaDecisionUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> pdaApproval(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverAction(IssueSubmissionParameters mydata, SqlTransaction transaction);
    }
}
