namespace Contracts.Infrastructure;

public interface ILoggerManager
{
    bool IsLogDebug();
    bool IsLogInfo();
    bool IsLogWarn();
    bool IsLogError();
    void LogInfo(string message);
    void LogWarn(string message);
    void LogDebug(string message);
    void LogError(string message);
}
