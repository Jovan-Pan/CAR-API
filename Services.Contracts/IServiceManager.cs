using Services.Contracts.Account;
using Services.Contracts.CAR;
using Services.Contracts.MasterData;

namespace Services.Contracts;

public interface IServiceManager
{
    IAccountService Account { get; }
    IMasterDataService MasterData { get; }
    IImmidateActionService ImmAct { get; }
    IRootCauseService RootCause { get; }
    IErrorLogService ErrorLog { get; }
    IIssueSubmissionService IssueSubmission { get; }
    IIssueFeedbackReportService IFR { get; }
    ISendMailSettingService sendmailsetting { get; }
}
