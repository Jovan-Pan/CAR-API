using Contracts.Infrastructure;
using Microsoft.Extensions.Options;
using NLog;
using Services;

namespace Infrastructure;

public class LoggerManager(IOptions<AppSettings> appSettings) : ILoggerManager
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public bool IsLogDebug()
    {
        return appSettings.Value.NLogSettings.LogDebug;
    }

    public bool IsLogInfo()
    {
        return appSettings.Value.NLogSettings.LogInfo;
    }

    public bool IsLogWarn()
    {
        return appSettings.Value.NLogSettings.LogWarning;
    }

    public bool IsLogError()
    {
        return appSettings.Value.NLogSettings.LogError;
    }

    public void LogDebug(string message)
    {
        if (!appSettings.Value.NLogSettings.LogDebug) return;
        logger.Debug(message);
    }
    public void LogError(string message)
    {
        if (!appSettings.Value.NLogSettings.LogError) return;
        logger.Error(message);
    }
    public void LogInfo(string message)
    {
        if (!appSettings.Value.NLogSettings.LogInfo) return;
        logger.Info(message);
    }
    public void LogWarn(string message)
    {
        if (!appSettings.Value.NLogSettings.LogWarning) return;
        logger.Warn(message);
    }
}
