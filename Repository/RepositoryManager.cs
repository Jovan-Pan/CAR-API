using Contracts.Repository;
using Contracts.Repository.CAR;
using Contracts.Repository.MasterData;
using Repository.IssueSubmission;
using Repository.MasterData;

namespace Repository;

public sealed class RepositoryManager(DbContext dbContext) : IDataManager
{
    private readonly Lazy<IMDMRepository> _mdmRepo = new(() => new MDMRepository(dbContext));
    private readonly Lazy<IIssueSubmissionRepository> _ISM = new(() => new IssueSubmissionRepository(dbContext));
    public IMDMRepository MDM => _mdmRepo.Value;
    public IIssueSubmissionRepository ISM => _ISM.Value;
}
