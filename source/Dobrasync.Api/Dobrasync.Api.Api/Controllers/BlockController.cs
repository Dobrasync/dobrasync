using Dobrasync.Api.Api.Controllers.Base;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Diff;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dobrasync.Api.Api.Controllers;

[SwaggerTag("Block")]
public class BlockController(IBlockService blockService) : BaseController
{
    [HttpGet]
    [SwaggerOperation(
        OperationId = nameof(GetBlockAsync)
    )]
    public async Task<ActionResult<BlockDto>> GetBlockAsync(Guid libraryId, string blockChecksum)
    {
        BlockDto dto = await blockService.GetBlockMappedAsync(libraryId, blockChecksum);
        
        return Ok(dto);
    }
    
    [HttpPost("{libraryId}")]
    [SwaggerOperation(
        OperationId = nameof(CreateBlockAsync)
    )]
    public async Task<ActionResult> CreateBlockAsync(Guid libraryId, BlockCreateDto dto)
    {
        await blockService.CreateBlockAsync(dto.Payload, dto.Checksum, libraryId);
        
        return Ok();
    }
}