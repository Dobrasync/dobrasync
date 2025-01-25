namespace Dobrasync.Api.BusinessLogic.Services.Core.Logger;

public interface ILoggerService
{
    void LogDebug(string msg);
    void LogInfo(string msg);
    void LogWarn(string msg);
    void LogError(string msg);
    void LogFatal(string msg);
}