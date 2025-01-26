using Dobrasync.Api.Database.Entities;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Blocks;

public interface IBlockService
{
    /// <summary>
    /// Attempts to delete orphan block only. If the block is still ref.
    /// by any file, the deletion will be aborted.
    /// </summary>
    /// <param name="blockId"></param>
    /// <returns>Block if deleted, null if unchanged</returns>
    public Task<Block?> TryDeleteOrphanBlockAsync(Guid blockId);
    
    /// <summary>
    /// Scans database for orphaned blocks and deletes them.
    /// </summary>
    /// <returns>List of deleted blocks</returns>
    public Task<List<Block>> DeleteAllOrphanBlocksAsync();
    
    /// <summary>
    /// Creates a new block in the file system and db without parent.
    ///
    /// If block already exists, abort creation and return existing.
    /// </summary>
    /// <param name="checksum">Checksum of the blocks payload</param>
    /// <param name="payload">Block payload</param>
    /// <param name="libraryId">Library this block belongs to</param>
    /// <returns></returns>
    public Task<Block> CreateBlockAsync(byte[] payload, byte[] checksum, Guid libraryId);
}