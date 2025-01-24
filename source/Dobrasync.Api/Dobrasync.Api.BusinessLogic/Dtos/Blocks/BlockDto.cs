namespace Dobrasync.Api.BusinessLogic.Dtos;

public class BlockDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Checksum { get; set; } = string.Empty;
    public byte[] Payload { get; set; } = [];
}