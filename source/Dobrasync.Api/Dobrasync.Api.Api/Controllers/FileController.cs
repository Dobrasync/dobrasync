using Dobrasync.Api.Api.Controllers.Base;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dobrasync.Api.Api.Controllers;

[SwaggerTag("File")]
public class FileController(IFileService fileService, IVersionService versionService) : BaseController
{
    [HttpGet("{fileId}")]
    [SwaggerOperation(
        OperationId = nameof(GetFileByIdAsync)
    )]
    public async Task<ActionResult<FileDto>> GetFileByIdAsync(Guid fileId)
    {
        FileDto file = await fileService.GetFileByIdMappedAsync(fileId);
        
        return Ok(file);
    }
    
    [HttpDelete("{fileId}")]
    [SwaggerOperation(
        OperationId = nameof(DeleteFileByIdAsync)
    )]
    public async Task<ActionResult<LibraryDto>> DeleteFileByIdAsync(Guid fileId)
    {
        FileDto file = await fileService.DeleteFileMappedAsync(fileId);
        
        return Ok(file);
    }
    
    [HttpGet("{fileId}/versions-newer-than-date")]
    [SwaggerOperation(
        OperationId = nameof(GetFileVersionsNewerThanDateAsync)
    )]
    public async Task<ActionResult<List<VersionDto>>> GetFileVersionsNewerThanDateAsync(Guid fileId, DateTimeOffset dateUtc)
    {
        List<VersionDto> v = await versionService.GetFileVersionsNewerThanDateMappedAsync(fileId, dateUtc);
        
        return Ok(v);
    }
}