using Entities.MasterData;
using Entities;
using Contracts;
using Services.Contracts.MasterData;
using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using System.Net.Http;
using System.Numerics;

namespace Services.MasterData;

internal sealed class MasterDataService(IMasterDataApi mesMasterApi, IMDMRepository mdm, ICacheManager memCache, ILocalizationService localization) : IMasterDataService
{
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


    public async Task<ApiResponse<IEnumerable<tGlobalSettingDto>>> GetDataGlobalSetting(int plant, string system, string settingID)
    {
        var result = await mdm.GetDataGlobalSetting(plant, system, settingID);
        return ApiResponse<IEnumerable<tGlobalSettingDto>>.SuccessResponse(result);
    }
}