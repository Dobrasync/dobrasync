namespace Dobrasync.Api.BusinessLogic.Dtos.Diff;

public class DiffDto
{
    public List<DiffFileResultDescriptionDto> Result { get; set; } = new();
    public Guid? TransactionId { get; set; }
}