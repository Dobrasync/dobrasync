namespace Dobrasync.Api.BusinessLogic.Services.Core.Logger;

public class LoggerService : ILoggerService
{
    private static readonly NLog.Logger logger = NLog.LogManager.GetLogger("Main");
    
    public void LogDebug(string msg)
    {
        logger.Debug(msg);
    }

    public void LogInfo(string msg)
    {
        logger.Info(msg);
    }

    public void LogWarn(string msg)
    {
        logger.Warn(msg);
    }

    public void LogError(string msg)
    {
        logger.Error(msg);
    }

    public void LogFatal(string msg)
    {
        logger.Fatal(msg);
    }
}