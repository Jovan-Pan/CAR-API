using Contracts;
using Contracts.Infrastructure;
using Infrastructure;
using Services;
using Services.Account;
using Services.Contracts;
using Services.Contracts.Account;
using Services.Locale;
using System.Net.Http.Headers;
using WebApi.Handler;

namespace WebApi.DependecyInjection;

public static class BusinessLogicExtensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IServiceManager, ServiceManager>();

        services.AddScoped<IMasterDataApi, MasterDataAPI>();
        services.AddScoped<ILocalizationService, LocalizationService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IServiceCollection AddMasterApiHandler(this IServiceCollection services, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("MesMasterApi:ApiUrl");
        var apiToken = configuration.GetValue<string>("MesMasterApi:ApiToken");

        services.AddTransient<HMACDelegatingHandler>();

        services.AddHttpClient("MasterDataAPI", c =>
        {
            c.BaseAddress = new Uri(apiUrl!);
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiToken!);
        })
        .AddHttpMessageHandler<HMACDelegatingHandler>();

        return services;
    }
}
