namespace Dobrasync.Api.BusinessLogic.Dtos;

public class FileDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Path { get; set; } = string.Empty;
    public Guid CurrentVersionId { get; set; }
    
}