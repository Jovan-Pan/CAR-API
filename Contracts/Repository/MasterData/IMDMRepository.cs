using Entities.Account.Dto;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using System.Threading.Tasks;

namespace Contracts.Repository.MasterData;

public interface IMDMRepository
{
    Task<bool> AllowAllDataToAcc(string Userid);
    Task<UserVendorInfoDto> GetUserVendorInfo(int plant, string userid);
    Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId);
    Task<FormAuthorizeInfoDto> GetFormAuthorize(FormAuthorizeParam param);
    Task<IEnumerable<tGlobalSettingDto>> GetDataGlobalSetting(int plant, string system, string settingID);
    Task<IEnumerable<TproductVsSmnProdPICDto>> GetTPRODUCT(int plant, string Userid);
    Task<IEnumerable<MatGroupDto>> GetMatGrp(int plant, string product, IEnumerable<string> productAuthList);
    Task<IEnumerable<TMATERIALTYPEDto>> GetMatType(int plant);
    Task<IEnumerable<TMATERIALDto>> GetMaterial(GetMaterialParam request);
    Task<IEnumerable<TMATERIALDto>> GetMaterialWoProdAut(GetMaterialParam request);
    Task<IEnumerable<NcCategoryDto>> GetNCCategory();
    Task<IEnumerable<SystemDeptVsUserDto>> GetSystemDeptVsUser(int plant, string Userid);
    Task<IEnumerable<SystemDeptVsUserDto>> GetSystemDeptVsUserDynamic(int plant, string Userid);
    Task<IEnumerable<VendorDto>> GetVendor(int plant);
    Task<IEnumerable<BasePathConfigDto>> getBasePathConfig(int plant);
    Task<IEnumerable<UserFormAuthorizeDto>> getUserFormAuthorize(int plant, string Userid, string FormName);
    Task<IEnumerable<string>> getUserStatusAuthorize(int plant, string Userid);
    Task<IEnumerable<CurrencyDto>> GetCurrency(int plant);
    Task<IEnumerable<ProcessGroupDto>> getProcessGrp(int plant);
    Task<IEnumerable<string>> getReason(int plant, string reasontype);
    Task<IEnumerable<TGlobalEmailSettingModel>> GetTGlobalEmailSetting(int plant, string wStatus);
    Task<IEnumerable<SystemvsUservsEmailSubscribeForm>> GetSystemvsUservsEmailSubscribeForm(int plant, string group, string dept);
    Task<IEnumerable<SystemvsUservsEmailSubscribeForm>> GetSystemvsUservsEmailSubscribeFormVendor(string VendorCode, int plant, string group, string dept);
    Task<IEnumerable<string>> GetissuerEmail(int plant, IEnumerable<string> UseID);
    Task<bool> GetisSpAdmin(int plant, string UseID);
    Task<IEnumerable<UsrDto>> GetUsr(DynamicFormParameterDTO data);
    Task<IEnumerable<UsrDto>> CheckUserVSVend(DynamicFormParameterDTO data);
    Task<IEnumerable<PlantDto>> GetPlant(int plant);
    Task<IEnumerable<string>> GetSourceOfSupply(int plant);
}
