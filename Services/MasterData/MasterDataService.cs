using Entities.MasterData;
using Entities;
using Contracts;
using Services.Contracts.MasterData;
using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using System.Net.Http;
using System.Numerics;
using System.Drawing;
using Entities.ParamRequest;
using Entities.Account.Dto;
using Entities.CAR;

namespace Services.MasterData;

internal sealed class MasterDataService(IMasterDataApi mesMasterApi, IMDMRepository mdm
    , ICacheManager memCache, ILocalizationService localization) : IMasterDataService
{
    public async Task<ApiResponse<bool>> AllowAllDataToAcc(string userId)
    {
        var result = await mdm.AllowAllDataToAcc(userId);
        return ApiResponse<bool>.SuccessResponse(result);
    }
    public async Task<ApiResponse<IEnumerable<MenuItem>>> GetMenuSetting(string userId)
    {
        var menuList = await mesMasterApi.GetMenuSetting(userId);

        if (!menuList.Any())
            return ApiResponse<IEnumerable<MenuItem>>
                .FailResponse(localization.GetLocalizedString("A002", LocaleResourcesEnum.Account));
        
        return ApiResponse<IEnumerable<MenuItem>>.SuccessResponse(menuList);
    }

    public async Task<ApiResponse<IEnumerable<string>>> GetPlantListForCRCUSystemByUserId(string userId)
    {
        var plants = await mdm.GetPlantListForCRCUSystemByUserId(userId);
        return ApiResponse<IEnumerable<string>>.SuccessResponse(plants);
    }

    public async Task<ApiResponse<FormAuthorizeInfoDto>> GetFormAuthorize(FormAuthorizeParam param)
    {
        var result = await mdm.GetFormAuthorize(param);
        return ApiResponse<FormAuthorizeInfoDto>.SuccessResponse(result);
    }

    public async Task<ApiResponse<UserVendorInfoDto>> GetUserVendorInfo(int plant,string userId)
    {
        var result = await mdm.GetUserVendorInfo(plant, userId);
        return ApiResponse<UserVendorInfoDto>.SuccessResponse(result);
    }


    public async Task<ApiResponse<IEnumerable<tGlobalSettingDto>>> GetDataGlobalSetting(int plant, string system, string settingID)
    {
        var result = await mdm.GetDataGlobalSetting(plant, system, settingID);
        return ApiResponse<IEnumerable<tGlobalSettingDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<TproductVsSmnProdPICDto>>> GetTPRODUCT(int plant, string Userid)
    {
        var result = await mdm.GetTPRODUCT(plant, Userid);
        return ApiResponse<IEnumerable<TproductVsSmnProdPICDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<MatGroupDto>>> GetMatGrp(int plant, string product, IEnumerable<string> productAuthList)
    {
        var result = await mdm.GetMatGrp(plant, product, productAuthList);
        return ApiResponse<IEnumerable<MatGroupDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<TMATERIALTYPEDto>>> GetMatType(int plant)
    {
        var result = await mdm.GetMatType(plant);
        return ApiResponse<IEnumerable<TMATERIALTYPEDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<TMATERIALDto>>> GetMaterial(GetMaterialParam request)
    {
        var result = await mdm.GetMaterial(request);
        return ApiResponse<IEnumerable<TMATERIALDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<NcCategoryDto>>> GetNCCategory()
    {
        var result = await mdm.GetNCCategory();
        return ApiResponse<IEnumerable<NcCategoryDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<SystemDeptVsUserDto>>> GetSystemDeptVsUser(int plant, string Userid)
    {
        var result = await mdm.GetSystemDeptVsUser(plant, Userid);
        return ApiResponse<IEnumerable<SystemDeptVsUserDto>>.SuccessResponse(result);
    }
    public async Task<ApiResponse<IEnumerable<SystemDeptVsUserDto>>> GetSystemDeptVsUserDynamic(int plant, string Userid)
    {
        var result = await mdm.GetSystemDeptVsUserDynamic(plant, Userid);
        return ApiResponse<IEnumerable<SystemDeptVsUserDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<VendorDto>>> GetVendor(int plant)
    {
        var result = await mdm.GetVendor(plant);
        return ApiResponse<IEnumerable<VendorDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<UserFormAuthorizeDto>>> getUserFormAuthorize(int plant, string Userid, string FormName)
    {
        var result = await mdm.getUserFormAuthorize(plant, Userid, FormName);
        return ApiResponse<IEnumerable<UserFormAuthorizeDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<string>>> getUserStatusAuthorize(int plant, string Userid)
    {
        var result = await mdm.getUserStatusAuthorize(plant, Userid);
        return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<CurrencyDto>>> GetCurrency(int plant)
    {
        var result = await mdm.GetCurrency(plant);
        return ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<ProcessGroupDto>>> getProcessGrp(int plant)
    {
        var result = await mdm.getProcessGrp(plant);
        return ApiResponse<IEnumerable<ProcessGroupDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<IEnumerable<string>>> getReason(int plant, string reasontype)
    {
        var result = await mdm.getReason(plant, reasontype);
        return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<bool>> GetisSpAdmin(int plant, string UseID)
    {
        var result = await mdm.GetisSpAdmin(plant, UseID);
        return ApiResponse<bool>.SuccessResponse(result);
    }
    public async Task<ApiResponse<IEnumerable<UsrDto>>> GetUsr(DynamicFormParameterDTO data)
    {
        var result = await mdm.GetUsr(data);
        return ApiResponse<IEnumerable<UsrDto>>.SuccessResponse(result);
    }
    public async Task<ApiResponse<IEnumerable<PlantDto>>> GetPlant(int plant)
    {
        var result = await mdm.GetPlant(plant);
        return ApiResponse<IEnumerable<PlantDto>>.SuccessResponse(result);
    }
    public async Task<ApiResponse<IEnumerable<string>>> GetSourceOfSupply(int plant)
    {
        var result = await mdm.GetSourceOfSupply(plant);
        return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
    }

}
