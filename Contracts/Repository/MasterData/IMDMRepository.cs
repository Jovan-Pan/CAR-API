namespace Contracts.Repository.MasterData;

public interface IMDMRepository
{
    Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId);
}
