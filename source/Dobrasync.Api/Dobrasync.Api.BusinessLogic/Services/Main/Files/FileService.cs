using AutoMapper;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Shared.Exceptions.Userspace;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Files;

public class FileService(IRepoWrapper repo, IBlockService blockService, IMapper mapper) : IFileService
{
    public async Task<File> GetFileByIdAsync(Guid fileId)
    {
        #region Load
        File? file = await repo.FileRepo.QueryAll().FirstOrDefaultAsync(x => x.Id == fileId);
        if (file == null) throw new NotFoundUSException();
        #endregion

        return file;
    }

    public async Task<FileDto> GetFileByIdMappedAsync(Guid fileId)
    {
        File file = await GetFileByIdAsync(fileId);
        
        return mapper.Map<FileDto>(file);
    }
    
    public async Task<File> DeleteFileAsync(Guid fileId)
    {
        #region Load file
        File? file = repo.FileRepo.QueryAll().Include(x => x.Blocks).FirstOrDefault(x => x.Id == fileId);
        if (file == null)
        {
            throw new NotFoundUSException();
        }
        #endregion
        #region Delete orphan blocks
        foreach (var block in file.Blocks)
        {
            await blockService.TryDeleteOrphanBlockAsync(block.Id);
        }
        #endregion
        #region Delete file
        await repo.FileRepo.DeleteAsync(file);
        #endregion

        return file;
    }

    public async Task<FileDto> DeleteFileMappedAsync(Guid fileId)
    {
        File file = await DeleteFileAsync(fileId);
        
        return mapper.Map<FileDto>(file);
    }

    public Task<File> WriteFileAsync(Guid libraryId, string path, List<string> blockChecksums)
    {
        throw new NotImplementedException();
    }
    
    
}