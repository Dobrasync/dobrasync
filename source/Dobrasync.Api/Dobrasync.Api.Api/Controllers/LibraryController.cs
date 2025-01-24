using Dobrasync.Api.Api.Controllers.Base;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dobrasync.Api.Api.Controllers;

[SwaggerTag("Library")]
public class LibraryController(ILibraryService libraryService) : BaseController
{
    [HttpGet("{libraryId}")]
    [SwaggerOperation(
        OperationId = nameof(GetLibraryByIdAsync)
    )]
    public async Task<ActionResult<LibraryDto>> GetLibraryByIdAsync(Guid libraryId)
    {
        LibraryDto library = await libraryService.GetLibraryByIdMappedAsync(libraryId);
        
        return Ok(library);
    }
    
    [HttpGet("by-name/{libraryName}")]
    [SwaggerOperation(
        OperationId = nameof(GetLibraryByNameAsync)
    )]
    public async Task<ActionResult<LibraryDto>> GetLibraryByNameAsync(string libraryName)
    {
        LibraryDto library = await libraryService.GetLibraryByNameMappedAsync(libraryName);
        
        return Ok(library);
    }
    
    [HttpDelete("{libraryId}")]
    [SwaggerOperation(
        OperationId = nameof(DeleteLibraryByIdAsync)
    )]
    public async Task<ActionResult<LibraryDto>> DeleteLibraryByIdAsync(Guid libraryId)
    {
        LibraryDto library = await libraryService.DeleteLibraryMappedAsync(libraryId);
        
        return Ok(library);
    }
    
    [HttpGet("{libraryId}/file")]
    [SwaggerOperation(
        OperationId = nameof(GetLibraryFileAsync)
    )]
    public async Task<ActionResult<FileDto>> GetLibraryFileAsync(Guid libraryId, [FromQuery] string path)
    {
        FileDto file = await libraryService.GetLibraryFileMappedAsync(libraryId, path);

        return Ok(file);
    }
}