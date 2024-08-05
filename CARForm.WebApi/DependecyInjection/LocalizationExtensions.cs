using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace WebApi.DependecyInjection;

public static class LocalizationExtensions
{
    public static IServiceCollection AddLocatiozation(this IServiceCollection services)
    {
        const string defaultCulture = "en";

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        var supportedCultures = new[]
        {
            new CultureInfo(defaultCulture),
            new CultureInfo("zh")
        };

        services.Configure<RequestLocalizationOptions>(options => {
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }
}
