using Contracts;
using Contracts.Infrastructure;
using Infrastructure;
using Services;
using Services.Contracts;
using Services.Locale;
using System.Net.Http.Headers;

namespace WebApi.DependecyInjection;

public static class BusinessLogicExtensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();

        services.AddScoped<IMasterDataApi, MasterDataAPI>();
        services.AddScoped<ILocalizationService, LocalizationService>();

        return services;
    }

    public static IServiceCollection AddMasterApiHandler(this IServiceCollection services, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("MesMasterApi:ApiUrl");
        var apiToken = configuration.GetValue<string>("MesMasterApi:ApiToken");
        var apiSecret = configuration.GetValue<string>("MesMasterApi:ApiSecretKey");

        services.AddHttpClient("MasterDataAPI", c =>
        {
            c.BaseAddress = new Uri(apiUrl!);
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiToken!);
        });

        return services;
    }
}
