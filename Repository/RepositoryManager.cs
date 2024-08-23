using Contracts.Repository;
using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;
using Repository.CAR;
using Repository.IssueSubmission;
using Repository.MasterData;

namespace Repository;

public sealed class RepositoryManager(DbContext dbContext) : IDataManager
{
    private readonly Lazy<IMDMRepository> _mdmRepo = new(() => new MDMRepository(dbContext));
    private readonly Lazy<IErrorLogRepository> _ErrorLog = new(() => new ErrorLogRepository(dbContext));
    private readonly Lazy<IIssueSubmissionRepository> _ISM = new(() => new IssueSubmissionRepository(dbContext));
    public IMDMRepository MDM => _mdmRepo.Value;
    public IErrorLogRepository ErrorLog => _ErrorLog.Value;
    public IIssueSubmissionRepository ISM => _ISM.Value;
}
