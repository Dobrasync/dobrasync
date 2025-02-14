using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Shared.Exceptions.Userspace;
using Dobrasync.Api.Tests.Fixtures;
using Dobrasync.Common.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Api.Tests.Tests;

[Collection("Sync")]
public class NormalFlowExampleTest : IClassFixture<PopulatedSingleLibraryFixture>
{
    private readonly IAppsettingsProviderService apps;
    private readonly ILibraryService libraryService;
    private readonly IFileService fileService;
    private readonly IVersionService versionService;
    private readonly IBlockService blockService;
    private readonly IRepoWrapper repo;

    public NormalFlowExampleTest(PopulatedSingleLibraryFixture fixture)
    {
        libraryService = fixture.ServiceProvider.GetRequiredService<ILibraryService>();
        repo = fixture.ServiceProvider.GetRequiredService<IRepoWrapper>();
        apps = fixture.ServiceProvider.GetRequiredService<IAppsettingsProviderService>();
        fileService = fixture.ServiceProvider.GetRequiredService<IFileService>();
        versionService = fixture.ServiceProvider.GetRequiredService<IVersionService>();
        blockService = fixture.ServiceProvider.GetRequiredService<IBlockService>();
    }

    /// <summary>
    /// This test is meant to represent the normal flow of event a client
    /// would perform on the API.
    /// </summary>
    [Fact]
    public async Task NormalFlowTest()
    {
        #region Create a new library
        LibraryDto libraryDto = await libraryService.CreateLibraryMappedAsync("My test library");
        #endregion
        #region Make first diff
        List<string> initialDiff = await libraryService.MakeDiffMappedAsync(libraryDto.Id, new()
        {
            FilesOnLocal = []
        }); 
        
        Assert.Empty(initialDiff);
        #endregion
        #region Make diff with first file on client
        Guid? sourceFileVersion = null;
        string sourceFilePath = "Data/Testfile.txt";
        FileInfo sourceFileInfo = new FileInfo(sourceFilePath);
        byte[] sourceFileBytes = File.ReadAllBytes(sourceFilePath);
        string sourceFileChecksum = ChecksumUtil.CalculateFileChecksum(sourceFileBytes);
        List<byte[]> sourceFileBlocks = Chunker.ContentToBlocks(sourceFileBytes);
        
        List<string> firstDiff = await libraryService.MakeDiffMappedAsync(libraryDto.Id, new()
        {
            FilesOnLocal = [
                new()
                {
                    Path = sourceFilePath,
                    FileChecksum = sourceFileChecksum,
                    LatestVersionId = sourceFileVersion,
                }
            ]
        }); 
        
        Assert.NotEmpty(firstDiff);
        Assert.Single(firstDiff);
        #endregion
        #region Iterate over diff list
        foreach (string path in firstDiff)
        {
            try
            {
                FileDto file = await libraryService.GetLibraryFileMappedAsync(libraryDto.Id, path);
            }
            catch (NotFoundUSException)
            {
                VersionCreateResultDto createdResult = await versionService.CreateVersionMappedAsync(new()
                {
                    FilePath = path,
                    FileChecksum = sourceFileChecksum,
                    ExpectedBlocks = sourceFileBlocks.Select(ChecksumUtil.CalculateFileChecksum).ToList(),
                    IsDirectory = false,
                    FilePermissionsOctal = 0,
                    FileModifiedOnUtc = sourceFileInfo.LastWriteTimeUtc,
                    FileCreatedOnUtc = sourceFileInfo.CreationTimeUtc,
                    LibraryId = libraryDto.Id,
                });
                
                sourceFileVersion = createdResult.CreatedVersion.Id;
                
                foreach (string requiredBlockChecksum in createdResult.RequiredBlocks)
                {
                    byte[]? blockContentMatch = sourceFileBlocks.Find(b => ChecksumUtil.CalculateBlockChecksum(b).SequenceEqual(requiredBlockChecksum));
                    if (blockContentMatch == null)
                    {
                        Assert.Fail("Unable to locate required block");
                    }

                    await blockService.CreateBlockAsync(blockContentMatch, ChecksumUtil.CalculateBlockChecksum(blockContentMatch), libraryDto.Id);
                }

                await versionService.CompleteMappedAsync(createdResult.CreatedVersion.Id);
            }
        }
        #endregion
        
        #region Make diff where we expect no difference
        List<string> noDiffExpected = await libraryService.MakeDiffMappedAsync(libraryDto.Id, new()
        {
            FilesOnLocal = [new()
            {
                Path = sourceFilePath,
                FileChecksum = sourceFileChecksum,
                LatestVersionId = sourceFileVersion,
            }]
        });
        
        Assert.Empty(noDiffExpected);
        #endregion
    }
}