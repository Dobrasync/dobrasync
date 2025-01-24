namespace Dobrasync.Api.Shared.Exceptions.Userspace;

public abstract class UserspaceException(int httpHttpStatusCode, string userMsg, string technicalDescription)
    : Exception
{
    #region ctor

    public int HttpStatusCode { get; set; } = httpHttpStatusCode;
    public string UserMessage { get; set; } = userMsg;
    public string TechnicalDescription { get; set; } = technicalDescription;

    #endregion
}