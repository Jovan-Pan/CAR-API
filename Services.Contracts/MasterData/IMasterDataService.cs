using Entities;
using Entities.MasterData;

namespace Services.Contracts.MasterData;

public interface IMasterDataService
{
    Task<ApiResponse<IEnumerable<MenuItem>>> GetMenuSetting(string userId);
    Task<ApiResponse<IEnumerable<string>>> GetPlantListForCRCUSystemByUserId(string userId);
}
