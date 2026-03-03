using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Handler;

namespace WebApi.DependecyInjection;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var ssoConfig = configuration.GetSection("SSOConfig");
        var enableSSO = ssoConfig.GetValue<bool>("Enable");
        var authority = ssoConfig["AuthorityUrl"];
        var audience = ssoConfig["Audience"];

        if (enableSSO)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;

                options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new CustomOpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{authority}/",
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        }
        else
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };
            });
        }

        return services;
    }
}
