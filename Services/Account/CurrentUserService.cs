using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Services.Contracts.Account;
using System.Security.Claims;

namespace Services.Account;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings) : ICurrentUserService
{
    public string? UserId =>
        appSettings.Value.SSOConfig.Enable
        ? httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        : httpContextAccessor.HttpContext?.User?.FindFirst("UseID")?.Value;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
