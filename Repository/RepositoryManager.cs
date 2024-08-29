using Contracts.Repository;
using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;
using Repository.CAR;
using Repository.MasterData;

namespace Repository;

public sealed class RepositoryManager(DbContext dbContext) : IDataManager
{
    private readonly Lazy<IMDMRepository> _mdmRepo = new(() => new MDMRepository(dbContext));
    private readonly Lazy<IImmidateActionRepository> _immediteActrepo = new(() => new ImmidateActionRepository(dbContext));
    private readonly Lazy<IRootCauseRepository> _rootcauserepo = new(() => new RootCauseRepository(dbContext));
    private readonly Lazy<IErrorLogRepository> _ErrorLog = new(() => new ErrorLogRepository(dbContext));
    private readonly Lazy<IIssueSubmissionRepository> _ISM = new(() => new IssueSubmissionRepository(dbContext));
    private readonly Lazy<IIssueFeedbackReportRepository> _IFR = new(() => new IssueFeedbackReportRepository(dbContext));
    public IMDMRepository MDM => _mdmRepo.Value;
    public IImmidateActionRepository ImmAct => _immediteActrepo.Value;
    public IRootCauseRepository RootCause => _rootcauserepo.Value;
    public IErrorLogRepository ErrorLog => _ErrorLog.Value;
    public IIssueSubmissionRepository ISM => _ISM.Value;
    public IIssueFeedbackReportRepository IFR => _IFR.Value;
}
