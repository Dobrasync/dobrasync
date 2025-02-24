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
        Version? version = await repo.VersionRepo
            .QueryAll()
            .Include(x => x.File)
            .FirstOrDefaultAsync(x => x.Id == id);
        
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
        List<File> files = await repo.FileRepo.QueryAll().Include(x => x.Library).ToListAsync();
        
        File? targetFile = await repo.FileRepo
            .QueryAll()
            .Include(x => x.Library)
            .Where(x => x.LibraryId == createDto.LibraryId)
            .FirstOrDefaultAsync(x => x.Path == createDto.FilePath);
        
        bool initialVersion = targetFile == null;
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
            Status = initialVersion ? EVersionStatus.Success : EVersionStatus.Pending,
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

    public async Task<Version> CompleteAsync(Guid versionId)
    {
        Version? version = await GetVersionRequiredAsync(versionId);
        
        var expectedBLockIndices = version.ExpectedBlocks
            .Select((checksum, index) => new { checksum, index })
            .ToList();
        
        List<Block> receivedBlocks = await repo.BlockRepo
            .QueryAll()
            .Where(x => x.LibraryId == version.File.LibraryId)
            .Where(x => version.ExpectedBlocks.Any(b => b == x.Checksum))
            .ToListAsync();
        
        receivedBlocks = receivedBlocks
            .OrderBy(x => expectedBLockIndices.FindIndex(
                ci => ci.checksum.SequenceEqual(x.Checksum)))
            .ToList();
            
        #region verify block order

        if (version.ExpectedBlocks.Count != receivedBlocks.Count)
        {
            throw new BlockMismatchUSException();
        }
        
        for (int i = 0; i < version.ExpectedBlocks.Count(); i++)
        {
            string expectedBlock = version.ExpectedBlocks[i];
            string actualBlock = receivedBlocks[i].Checksum;

            if (actualBlock != expectedBlock)
            {
                throw new BlockMismatchUSException();
            }
        }
        #endregion
        
        version.OrderedBlocks = receivedBlocks.Select((x,i) => new OrderedBlock()
        {
            OrderIndex = i,
            Version = version,
            Block = x,
        }).ToList();
        
        await repo.OrderedBlockRepo.InsertRangeAsync(version.OrderedBlocks);
        
        version.Status = EVersionStatus.Success;
        await repo.VersionRepo.UpdateAsync(version);
        
        return version;
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
            .Include(x => x.OrderedBlocks)
            .ThenInclude(x => x.Block)
            .FirstOrDefault(x => x.Id == versionId);

        if (version == null) throw new NotFoundUSException();
        #endregion
        
        await repo.VersionRepo.DeleteAsync(version);

        foreach (OrderedBlock orderedBlock in version.OrderedBlocks)
        {
            await blockService.TryDeleteOrphanBlockAsync(orderedBlock.Block.Id);
        }
        
        return version;
    }

    public async Task<List<string>> GetVersionBlocksAsync(Guid id)
    {
        #region load version
        // for 404
        await GetVersionRequiredAsync(id);
        #endregion
        #region load
        List<string> blockChecksums = repo.VersionRepo
            .QueryAll()
            .Include(x => x.OrderedBlocks)
            .ThenInclude(x => x.Block)
            .First(x => x.Id == id)
            .OrderedBlocks.Select(x => x.Block.Checksum).ToList();
        #endregion
        
        return blockChecksums;
    }

    public async Task<List<Version>> GetFileVersionsNewerThanDateAsync(Guid fileId, DateTimeOffset dateUtc)
    {
        List<Version> newerVersions = await repo.VersionRepo
            .QueryAll()
            .Where(x => x.FileId == fileId)
            .Where(x => x.CreatedUtc > dateUtc)
            .ToListAsync();
        
        return newerVersions;
    }
    
    public async Task<List<VersionDto>> GetFileVersionsNewerThanDateMappedAsync(Guid fileId, DateTimeOffset dateUtc)
    {
        return mapper.Map<List<VersionDto>>(await GetFileVersionsNewerThanDateAsync(fileId, dateUtc));
    }
}