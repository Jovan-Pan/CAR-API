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
        Task<ApiResponse<string>> issuerMngUpdate(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaDecision(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaDecisionUpdate(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> pdaApproval(IssueSubmissionParameters mydata);
        Task<ApiResponse<string>> ReceiverAction(IssueSubmissionParameters mydata);
    }
}
