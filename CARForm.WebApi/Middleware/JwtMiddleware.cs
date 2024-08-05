namespace WebApi.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies[CookieName.JwtToken];
        var authHeaderExists = context.Request.Headers["Authorization"].Count > 0;

        if (token is not null && !authHeaderExists)
            context.Request.Headers.Append("Authorization", $"Bearer {token}");

        await _next(context);
    }
}