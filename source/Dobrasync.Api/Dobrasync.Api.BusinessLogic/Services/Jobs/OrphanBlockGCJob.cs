using Dobrasync.Api.BusinessLogic.Services.Core.Logger;
using Dobrasync.Api.BusinessLogic.Services.Jobs.Base;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.Database.Entities;
using Quartz;

namespace Dobrasync.Api.BusinessLogic.Services.Jobs;

public class OrphanBlockGCJob(ILoggerService logger, IBlockService blockService) : BaseJob("Orphan Block GC", logger)
{
    public async Task Execute(IJobExecutionContext context)
    {
        LogInfo("Scanning database...");
        List<Block> deletedBlocks = await blockService.DeleteAllOrphanBlocksAsync();
        LogInfo($"Deleted {deletedBlocks.Count} orphan blocks.");
    }
}