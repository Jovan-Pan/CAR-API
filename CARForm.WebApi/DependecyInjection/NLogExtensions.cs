using Contracts.Infrastructure;
using Infrastructure;
using NLog;

namespace WebApi.DependecyInjection;

public static class NLogExtensions
{
    public static void AddNLogConfigurations(this IServiceCollection services)
    {
        string configPath = $"{Directory.GetCurrentDirectory()}/nlog.config";
        LogManager.Setup().LoadConfigurationFromFile(configPath);

        services.AddSingleton<ILoggerManager, LoggerManager>();
    }
}
