using Contracts;
using Contracts.Infrastructure;
using Entities;
using Services.Contracts;
using Services.Contracts.Account;
using Entities.Account;
using Entities.Account.Dto;

namespace Services.Account;

internal sealed class AccountService(
    IServiceManager business,
    IMasterDataApi mesMasterApi,
    ILocalizationService localization) : IAccountService
{
    public async Task<(ApiResponse<UserInfo>, TokenResponse)> Login(LoginDto user)
    {
        var result = await mesMasterApi.Login<TokenResponse>(user, false);
        
        return (await HandleOtpDisabled(result), result);
    }

    public async Task<(ApiResponse<UserInfo>, TokenResponse)> RegenerateTokenForUser(RegenerateTokenForUserDto user)
    {
        var newToken = await mesMasterApi.RefreshToken(user);

        var userInfoOnly = new UserInfo(newToken);

        return (ApiResponse<UserInfo>.SuccessResponse(userInfoOnly), newToken);
    }

    private async Task<ApiResponse<UserInfo>> HandleOtpDisabled(TokenResponse userInfo)
    {
        if (!await UserHaveAccessToCRCUSystem(userInfo.UseID))
            return ApiResponse<UserInfo>.FailResponse(localization.GetLocalizedString("A002", LocaleResourcesEnum.Account));

        var userInfoOnly =
            new UserInfo(userInfo); //to exclude jwt token information for data that is returned to client

        return ApiResponse<UserInfo>.SuccessResponse(userInfoOnly);
    }

    private async Task<bool> UserHaveAccessToCRCUSystem(string userId)
    {
        var menuList = await business.MasterData.GetMenuSetting(userId);

        return menuList.Success;
    }
}