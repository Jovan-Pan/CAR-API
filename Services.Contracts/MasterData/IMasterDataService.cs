using Entities;
using Entities.Account.Dto;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;

namespace Services.Contracts.MasterData;

public interface IMasterDataService
{
    Task<ApiResponse<bool>> AllowAllDataToAcc(string userId);
    Task<ApiResponse<IEnumerable<MenuItem>>> GetMenuSetting(string userId);
    Task<ApiResponse<IEnumerable<string>>> GetPlantListForCRCUSystemByUserId(string userId);
    Task<ApiResponse<FormAuthorizeInfoDto>> GetFormAuthorize(FormAuthorizeParam param);
    Task<ApiResponse<UserVendorInfoDto>> GetUserVendorInfo(int plant, string userId);
    Task<ApiResponse<IEnumerable<tGlobalSettingDto>>> GetDataGlobalSetting(int plant, string system, string settingID);
    Task<ApiResponse<IEnumerable<TproductVsSmnProdPICDto>>> GetTPRODUCT(int plant, string Userid);
    Task<ApiResponse<IEnumerable<MatGroupDto>>> GetMatGrp(int plant, string product, IEnumerable<string> productAuthList);
    Task<ApiResponse<IEnumerable<TMATERIALTYPEDto>>> GetMatType(int plant);
    Task<ApiResponse<IEnumerable<TMATERIALDto>>> GetMaterial(GetMaterialParam request);
    Task<ApiResponse<IEnumerable<NcCategoryDto>>> GetNCCategory();
    Task<ApiResponse<IEnumerable<SystemDeptVsUserDto>>> GetSystemDeptVsUser(int plant, string Userid);
    Task<ApiResponse<IEnumerable<SystemDeptVsUserDto>>> GetSystemDeptVsUserDynamic(int plant, string Userid);
    Task<ApiResponse<IEnumerable<VendorDto>>> GetVendor(int plant);
    Task<ApiResponse<IEnumerable<UserFormAuthorizeDto>>> getUserFormAuthorize(int plant, string Userid, string FormName);
    Task<ApiResponse<IEnumerable<string>>> getUserStatusAuthorize(int plant, string Userid);
    Task<ApiResponse<IEnumerable<CurrencyDto>>> GetCurrency(int plant);
    Task<ApiResponse<IEnumerable<ProcessGroupDto>>> getProcessGrp(int plant);
    Task<ApiResponse<IEnumerable<string>>> getReason(int plant, string reasontype);
    Task<ApiResponse<bool>> GetisSpAdmin(int plant, string UseID);
    Task<ApiResponse<bool>> GetisSpAdminVoid(int plant, string UseID);
    Task<ApiResponse<IEnumerable<UsrDto>>> GetUsr(DynamicFormParameterDTO data);
    Task<ApiResponse<IEnumerable<PlantDto>>> GetPlant(int plant);
    Task<ApiResponse<IEnumerable<string>>> GetSourceOfSupply(int plant);
}
