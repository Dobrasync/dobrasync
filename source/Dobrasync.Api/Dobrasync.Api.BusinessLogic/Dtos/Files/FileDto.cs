namespace Dobrasync.Api.BusinessLogic.Dtos;

public class FileDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsDirectory { get; set; } = false;
}