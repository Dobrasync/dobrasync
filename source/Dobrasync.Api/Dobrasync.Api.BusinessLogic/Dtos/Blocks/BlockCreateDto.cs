namespace Dobrasync.Api.BusinessLogic.Dtos;

public class BlockCreateDto
{
    public string Checksum { get; set; }
    public byte[] Payload { get; set; }
}