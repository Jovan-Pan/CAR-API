using Entities.MasterData;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData;

public interface IMDMRepository
{
    Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId);
    Task<IEnumerable<tGlobalSettingDto>> GetDataGlobalSetting(int plant, string system, string settingID);
}
