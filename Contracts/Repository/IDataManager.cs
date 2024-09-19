using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;

namespace Contracts.Repository;

public interface IDataManager
{
    IMDMRepository MDM {  get; }
    IImmidateActionRepository ImmAct { get; }
    IRootCauseRepository RootCause { get; }
    IErrorLogRepository ErrorLog { get; }
    IIssueSubmissionRepository ISM { get; }
    IIssueFeedbackReportRepository IFR { get; }
    ISendMailSettingRepository SMS { get; }
}
