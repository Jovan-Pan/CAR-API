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
        Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration(string userid, string Language, string Plant, bool delflag);
        Task<SqlConnection> OpenConnectionAsync();
        Task<string> GenerateNewFormNo(int plant, string FormType, SqlTransaction transaction);

        Task<string> GenerateNewFormNoWithVer(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> CreateNewIssueFeedBcakWithVers(string OldFormNumber, string NewFormNumber, string UserId, string UserName, SqlTransaction transaction);

        Task<int> InsertDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> InsertDataAtchIssuer(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> issuerVoid(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> issuerUpdateDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> deleteDataAtchIssuer(IssueFeedbackAtchmentDto mydata, SqlTransaction transaction);
        Task<int> issuerMgrVoid(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> issuerMgrReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> issuerMngUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> pdaDecision(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> pdaDecisionUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> pdaApproval(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> pdaActionReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverAction(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverActionUpdate(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverActionAppeal(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverIssueReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverApproval(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverMngReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReceiverApprovalToReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> PDAReviewerVoid(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> PDAReviewerReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> PDAReviewerAprove(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReviewerSubmit(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> ReviewerReject(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<string> cekAvailableCompletePastIssue(cekAvailableCompletePastIssueParam param);


    }
}
