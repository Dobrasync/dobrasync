using Dobrasync.Api.BusinessLogic.Services.Core.Logger;

namespace Dobrasync.Api.BusinessLogic.Services.Jobs.Base;

public abstract class BaseJob(string jobName, ILoggerService logger)
{
    #region logging
    protected void LogFatal(string message)
    {
        logger.LogFatal(GetLogMessage(message));
    }
    
    protected void LogError(string message)
    {
        logger.LogError(GetLogMessage(message));
    }
    
    protected void LogWarn(string message)
    {
        logger.LogWarn(GetLogMessage(message));
    }
    
    protected void LogInfo(string message)
    {
        logger.LogInfo(GetLogMessage(message));
    }
    
    protected void LogDebug(string message)
    {
        logger.LogDebug(GetLogMessage(message));
    }

    private string GetLogMessage(string message)
    {
        return $"[JOB {jobName}] {message}";
    }
    #endregion
}