using Entities;
using Entities.CAR;
using Entities.ParamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.CAR
{
    public interface IIssueSubmissionService
    {
        Task<ApiResponse<string>> ProcessSubmit(IssueSubmissionParameters data);
        Task<ApiResponse<string>> issuerUpdate(IssueSubmissionParameters mydata);
        Task<DirectoryCredentials> GetDirectoryAuth(string Domain, string UserID, string Password, string BasePath);
        Task<ApiResponse<string>> issuerMgrVoid(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> issuerMgrReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> issuerMngUpdate(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaDecision(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaDecisionUpdate(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaApproval(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaActionReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverAction(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverActionUpdate(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverActionAppeal(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverIssueReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverApproval(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverMngReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverApprovalToReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> PDAReviewerVoid(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> PDAReviewerReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> PDAReviewerAprove(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReviewerSubmit(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReviewerReject(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param);
    }
}
