using Entities.Account;
using Entities.Account.Dto;
using Entities.Infrastructure;
using Entities.MasterData;

namespace Contracts.Infrastructure;

public interface IMasterDataApi
{
    #region auth
    Task<T> Login<T>(LoginDto user, bool enableOtp);
    Task<TokenResponse> RefreshToken(RegenerateTokenForUserDto user);
    #endregion

    Task<IEnumerable<MenuItem>> GetMenuSetting(string userId);
    Task<UserInfo> GetUserInfoByUserId(string userId);
    Task SendEmail(SendEmailParam param);
}
