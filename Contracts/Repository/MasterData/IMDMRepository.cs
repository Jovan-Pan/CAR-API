using Entities.MasterData;
using Entities.ParamRequest;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData;

public interface IMDMRepository
{
    Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId);
    Task<IEnumerable<tGlobalSettingDto>> GetDataGlobalSetting(int plant, string system, string settingID);
    Task<IEnumerable<TproductVsSmnProdPICDto>> GetTPRODUCT(int plant, string Userid);
    Task<IEnumerable<MatGroupDto>> GetMatGrp(int plant, string product, IEnumerable<string> productAuthList);
    Task<IEnumerable<TMATERIALTYPEDto>> GetMatType(int plant);
    Task<IEnumerable<TMATERIALDto>> GetMaterial(GetMaterialParam request);
}
