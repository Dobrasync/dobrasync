using AutoMapper;
using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Shared.Enums;
using Dobrasync.Api.Shared.Exceptions.Userspace;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Transactions;

public class VersionService(IRepoWrapper repo, IBlockService blockService, IMapper mapper) : IVersionService
{
    public async Task<Version> GetVersionRequiredAsync(Guid id)
    {
        Version? version = await repo.VersionRepo.QueryAll().FirstOrDefaultAsync(x => x.Id == id);
        if (version == null)
        {
            throw new NotFoundUSException();
        }
        
        return version;
    }

    public async Task<VersionDto> GetVersionRequiredMappedAsync(Guid id)
    {
        return mapper.Map<VersionDto>(await GetVersionRequiredAsync(id));
    }

    public async Task<VersionCreateResultDto> CreateVersionAsync(VersionCreateDto createDto)
    {
        #region Load library
        Library? library = repo.LibraryRepo
            .QueryAll()
            .FirstOrDefault(x => x.Id == createDto.LibraryId);
        
        if (library == null)
        {
            throw new NotFoundUSException();
        }
        #endregion
        #region Load or create file this version belongs to
        File? targetFile = await repo.FileRepo
            .QueryAll()
            .Include(x => x.Library)
            .Where(x => x.Library.Id == createDto.LibraryId)
            .FirstOrDefaultAsync(x => x.Path == createDto.FilePath);

        if (targetFile == null)
        {
            targetFile = await CreateNewFile(library, createDto.FilePath);
        }
        #endregion
        
        Version version = new()
        {
            FileCreatedOnUtc = createDto.FileCreatedOnUtc,
            FileModifiedOnUtc = createDto.FileModifiedOnUtc,
            ExpectedBlocks = createDto.ExpectedBlocks,
            IsDirectory = createDto.IsDirectory,
            CreatedUtc = DateTimeOffset.UtcNow,
            FileChecksum = createDto.FileChecksum,
            FilePermissionsOctal = createDto.FilePermissionsOctal,
            File = targetFile,
        };

        #region Determine required blocks
        List<Block> blocksAlreadyOnRemote =
            await repo.BlockRepo
                .QueryAll()
                .Where(x => x.LibraryId == createDto.LibraryId)
                .Where(x => createDto.ExpectedBlocks
                    .Any(b => b.Equals(x.Checksum)))
                .ToListAsync();

        
        List<string> requiredBlocks = createDto.ExpectedBlocks.Except(blocksAlreadyOnRemote.Select(x => x.Checksum)).ToList();
        #endregion
        
        await repo.VersionRepo.InsertAsync(version);
        
        return new()
        {
            CreatedVersion = mapper.Map<VersionDto>(version),
            RequiredBlocks = requiredBlocks,
        };
    }

    public async Task<VersionCreateResultDto> CreateVersionMappedAsync(VersionCreateDto createDto)
    {
        return await CreateVersionAsync(createDto);
    }

    private async Task<File> CreateNewFile(Library library, string path)
    {
        File newFile = new()
        {
            Library = library,
            Path = path,
        };
        
        await repo.FileRepo.InsertAsync(newFile);

        return newFile;
    }

    public async Task<Version> CompleteAsync(Guid transactionId)
    {
        Version? transaction = await GetVersionRequiredAsync(transactionId);
        
        var expectedBLockIndices = transaction.ExpectedBlocks
            .Select((checksum, index) => new { checksum, index })
            .ToList();
        
        // debug
        List<Block> allBLocks = await repo.BlockRepo.QueryAll().ToListAsync();
        
        List<Block> blocks = await repo.BlockRepo
            .QueryAll()
            .Where(x => transaction.ExpectedBlocks.Any(b => b == x.Checksum))
            .ToListAsync();
        
        blocks = blocks
            .OrderBy(x => expectedBLockIndices.FindIndex(
                ci => ci.checksum.SequenceEqual(x.Checksum)))
            .ToList();
            
        #region verify block order
        for (int i = 0; i < transaction.ExpectedBlocks.Count(); i++)
        {
            string expectedBlock = transaction.ExpectedBlocks[i];
            string actualBlock = blocks[i].Checksum;

            if (actualBlock != expectedBlock)
            {
                throw new BlockMismatchUSException();
            }
        }
        #endregion
        
        transaction.Blocks = blocks;
        await repo.VersionRepo.UpdateAsync(transaction);
        
        return transaction;
    }

    public async Task<VersionDto> CompleteMappedAsync(Guid transactionId)
    {
        return mapper.Map<VersionDto>(await CompleteAsync(transactionId));
    }

    public async Task<Version> DeleteVersionAsync(Guid versionId)
    {
        #region load
        Version? version = repo.VersionRepo
            .QueryAll()
            .Include(x => x.Blocks)
            .FirstOrDefault(x => x.Id == versionId);

        if (version == null) throw new NotFoundUSException();
        #endregion
        
        await repo.VersionRepo.DeleteAsync(version);

        foreach (Block block in version.Blocks)
        {
            await blockService.TryDeleteOrphanBlockAsync(block.Id);
        }
        
        return version;
    }
}