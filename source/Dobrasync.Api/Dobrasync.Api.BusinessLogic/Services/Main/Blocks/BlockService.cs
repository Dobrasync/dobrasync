using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Shared.Exceptions.Userspace;
using Dobrasync.Api.Shared.Util;
using Dobrasync.Common.Util;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Blocks;

public class BlockService(IRepoWrapper repo, IAppsettingsProviderService asp, ILibraryService libraryService) : IBlockService
{
    public async Task<Block?> TryDeleteOrphanBlockAsync(Guid blockId)
    {
        #region Load
        Block? block = await repo.BlockRepo
            .QueryAll()
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == blockId);

        if (block == null) throw new NotFoundUSException();
        #endregion
        #region Return if not orphaned
        if (block.Versions.Count > 0) return null;
        #endregion
        #region Delete
        await repo.BlockRepo.DeleteAsync(block);
        #endregion
        
        return block;
    }

    public async Task<List<Block>> DeleteAllOrphanBlocksAsync()
    {
        #region Load
        List<Block> orphans = repo.BlockRepo
            .QueryAll()
            .Where(x => x.Versions.Count == 0)
            .ToList();
        #endregion

        List<Block> deleted = [];
        foreach (Block orphan in orphans)
        {
            Block? res = await TryDeleteOrphanBlockAsync(orphan.Id);
            if (res != null) deleted.Add(res);
        }

        return deleted;
    }
    
    public async Task<Block> CreateBlockAsync(byte[] payload, string checksum, Guid libraryId)
    {
        #region Load library
        Library library = await libraryService.GetLibraryByIdAsync(libraryId);
        #endregion
        #region Integrity check
        bool match = ChecksumUtil.VerifyBlockChecksum(payload, checksum);
        if (!match) throw new ChecksumMismatchUSException();
        #endregion
        #region Abort if exists
        Block? existingBlock = await repo.BlockRepo
            .QueryAll()
            .FirstOrDefaultAsync(x => x.Checksum == checksum);
        
        if (existingBlock != null) return existingBlock;
        #endregion
        #region Create in file system
        string blockpath = Pathing.GetPathToBlock(asp.GetAppsettings(), library.Id, checksum);
        await File.WriteAllBytesAsync(blockpath, payload);
        #endregion
        #region Create in DB
        Block block = new()
        {
            Checksum = checksum
        };
        await repo.BlockRepo.InsertAsync(block);
        #endregion
        
        return block;
    }
}