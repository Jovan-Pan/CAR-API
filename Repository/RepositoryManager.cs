using Contracts.Repository;
using Contracts.Repository.MasterData;
using Repository.MasterData;

namespace Repository;

public sealed class RepositoryManager(DbContext dbContext) : IDataManager
{
    private readonly Lazy<IMDMRepository> _mdmRepo = new(() => new MDMRepository(dbContext));
    public IMDMRepository MDM => _mdmRepo.Value;
}
