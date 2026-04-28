using Microsoft.Extensions.Options;
using Services;

namespace WebApi.Middleware;

public class JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
{
    private readonly RequestDelegate _next = next;
    private readonly bool _enableSSO = appSettings.Value.SSOConfig.Enable;

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies[CookieName.JwtToken];
        var authHeaderExists = context.Request.Headers["Authorization"].Count > 0;

        // When SSO is enabled, never fall back to the legacy cookie JWT.
        // Doing so would let a stale non-SSO cookie authenticate the request
        // before the OIDC token has been established (e.g. after a browser restart).
        if (!_enableSSO && token is not null && !authHeaderExists)
            context.Request.Headers.Append("Authorization", $"Bearer {token}");

        await _next(context);
    }
}