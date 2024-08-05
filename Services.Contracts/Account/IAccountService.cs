using Entities;
using Entities.Account;
using Entities.Account.Dto;

namespace Services.Contracts.Account;

public interface IAccountService
{
    Task<(ApiResponse<UserInfo>, TokenResponse)> Login(LoginDto user);
    Task<(ApiResponse<UserInfo>, TokenResponse)> RegenerateTokenForUser(RegenerateTokenForUserDto user);
}
