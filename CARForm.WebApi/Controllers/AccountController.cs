using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Entities.Account;
using Services.Contracts;
using Services;
using Entities.Account.Dto;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("DefaultCors")]
public class AccountController(IServiceManager business, IOptions<AppSettings> appSettings) : ControllerBase
{
    private readonly CookieSettings _cookieSettings = appSettings.Value.CookieSettings;

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto user)
    {
        var result = await business.Account.Login(user);

        if (result.Item2 is not null)
        {
            SetJwtTokenCookies(result.Item2);
            result.Item1.Content.vendorcode = result.Item2.vendorcode;
            result.Item1.Content.vendorname = result.Item2.vendorname;
        }

        return Ok(result.Item1);
    }
    [AllowAnonymous]
    [HttpPost(nameof(RegenerateTokenForUser))]
    public async Task<IActionResult> RegenerateTokenForUser()
    {
        var userId = Request.Cookies[CookieName.UserId];
        var refreshToken = Request.Cookies[CookieName.RefreshToken];

        var regenerateTokenDto = new RegenerateTokenForUserDto
        {
            UserId = userId,
            RefreshToken = refreshToken
        };

        var result = await business.Account.RegenerateTokenForUser(regenerateTokenDto);

        if (result.Item2 is not null)
        {
            SetJwtTokenCookies(result.Item2);
        }

        return Ok(result.Item1);
    }
    [AllowAnonymous]
    [HttpPost(nameof(Logout))]
    public IActionResult Logout()
    {
        ClearCookies();
        return Ok();
    }

    private void ClearCookies()
    {
        Response.Cookies.Delete(CookieName.UserId);
        Response.Cookies.Delete(CookieName.JwtToken);
        Response.Cookies.Delete(CookieName.RefreshToken);
    }

    private void SetJwtTokenCookies(TokenResponse token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            SameSite = _cookieSettings.SamSiteMode,
            Expires = DateTime.Now.AddDays(_cookieSettings.CookieExpiryInDays)
        };

        Response.Cookies.Append(CookieName.UserId, token.UseID, cookieOptions);

        Response.Cookies.Append(CookieName.JwtToken, token.Token, cookieOptions);

        Response.Cookies.Append(CookieName.RefreshToken, token.RefreshToken, cookieOptions);
    }
}
