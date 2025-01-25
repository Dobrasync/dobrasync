namespace Dobrasync.Api.BusinessLogic.Dtos.Error;

public class ApiErrorDto
{
    public string Message { get; set; } = string.Empty;
    public DateTime DateTimeUtc { get; set; } = DateTime.UtcNow;
    public int HttpStatusCode { get; set; } = 500;
}