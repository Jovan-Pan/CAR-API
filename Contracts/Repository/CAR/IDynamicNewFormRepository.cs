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
    public interface IDynamicNewFormRepository
    {
        Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration(bool delflag);
        Task<SqlConnection> OpenConnectionAsync();
        Task<string> GenerateNewFormNo(int plant, string FormType, SqlTransaction transaction);

        Task<string> GenerateNewFormNoWithVer(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> CreateNewIssueFeedBcakWithVers(string OldFormNumber, string NewFormNumber, string UserId, string UserName, SqlTransaction transaction);

        Task<int> InsertDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> InsertDataAtchIssuer(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> issuerVoid(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> issuerUpdateDataIssueFeedback(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> deleteDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction);
        Task<int> issuerMgrVoid(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> issuerMgrReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> issuerMngUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> pdaDecision(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> pdaDecisionUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> pdaApproval(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> pdaActionReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverAction(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverActionUpdate(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverActionAppeal(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverIssueReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverApproval(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverMngReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReceiverApprovalToReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> PDAReviewerVoid(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> PDAReviewerReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> PDAReviewerAprove(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReviewerSubmit(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<int> ReviewerReject(IssueSubmissionParameters mydata, SqlTransaction transaction);
        Task<string> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param);


    }
}
