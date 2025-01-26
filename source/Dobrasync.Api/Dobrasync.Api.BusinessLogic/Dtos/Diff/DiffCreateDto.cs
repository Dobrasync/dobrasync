using Dobrasync.Api.Shared.Enums;

namespace Dobrasync.Api.BusinessLogic.Dtos.Diff;

public class DiffCreateDto
{
    public List<DiffFileDescriptionDto> FilesOnLocal { get; set; } = new();
}