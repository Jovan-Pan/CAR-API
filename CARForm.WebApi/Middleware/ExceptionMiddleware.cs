using Contracts.Infrastructure;
using Entities;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace WebApi.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var token = await context.GetTokenAsync("access_token");
        var loggedUserId = string.Empty;

        if (token != null)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ReadJwtToken(token).Payload.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "UseID");

            if (userIdClaim != null)
            {
                loggedUserId = userIdClaim.Value;
            }
        }

        string errorMessage = "Error By: " + loggedUserId + ", Error Description: " + exception.ToString();
        logger.LogError(errorMessage);

        var apiResponse = ApiResponse<string>.FailResponse(exception.Message);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var apiResponseJson = JsonSerializer.Serialize(apiResponse, options);

        await context.Response.WriteAsync(apiResponseJson);
    }
}