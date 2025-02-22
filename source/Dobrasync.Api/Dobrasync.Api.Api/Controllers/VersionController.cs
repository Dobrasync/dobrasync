using Dobrasync.Api.Api.Controllers.Base;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dobrasync.Api.Api.Controllers;

[SwaggerTag("Version")]
public class VersionController(IVersionService versionService) : BaseController
{
    [HttpGet("{versionId}")]
    [SwaggerOperation(
        OperationId = nameof(GetVersionRequiredAsync)
    )]
    public async Task<ActionResult<VersionDto>> GetVersionRequiredAsync(Guid versionId)
    {
        VersionDto v = await versionService.GetVersionRequiredMappedAsync(versionId);
        
        return Ok(v);
    }
    
    [HttpGet("{versionId}/blocks")]
    [SwaggerOperation(
        OperationId = nameof(GetVersionBlocksAsync)
    )]
    public async Task<ActionResult<List<string>>> GetVersionBlocksAsync(Guid versionId)
    {
        List<string> v = await versionService.GetVersionBlocksAsync(versionId);
        
        return Ok(v);
    }
    
    [HttpPost]
    [SwaggerOperation(
        OperationId = nameof(CreateVersionAsync)
    )]
    public async Task<ActionResult<VersionCreateResultDto>> CreateVersionAsync([FromBody] VersionCreateDto dto)
    {
        VersionCreateResultDto v = await versionService.CreateVersionMappedAsync(dto);
        
        return Ok(v);
    }
    
    [HttpPost("{versionId}/complete")]
    [SwaggerOperation(
        OperationId = nameof(CompleteVersionAsync)
    )]
    public async Task<ActionResult<VersionDto>> CompleteVersionAsync(Guid versionId)
    {
        VersionDto v = await versionService.CompleteMappedAsync(versionId);
        
        return Ok(v);
    }
}