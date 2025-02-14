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
    public interface IDynamicNewFormService
    {
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> GetDynamicFormConfiguration(bool delflag);
        Task<ApiResponse<string>> ProcessSubmit(DynamicFormParameterDTO data);
        Task<ApiResponse<string>> issuerVoid(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> issuerUpdate(DynamicFormParameterDTO mydata);
        Task<DirectoryCredentials> GetDirectoryAuth(string Domain, string UserID, string Password, string BasePath);
        Task<ApiResponse<string>> issuerMgrVoid(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> issuerMgrReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> issuerMngUpdate(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> pdaDecision(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> pdaDecisionUpdate(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> pdaApproval(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> pdaActionReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverAction(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverActionUpdate(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverActionAppeal(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverIssueReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverApproval(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverMngReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReceiverApprovalToReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> PDAReviewerVoid(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> PDAReviewerReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> PDAReviewerAprove(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReviewerSubmit(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> ReviewerReject(DynamicFormParameterDTO mydata);
        Task<ApiResponse<string>> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param);
    }
}
