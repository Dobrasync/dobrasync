using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Diff;
using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Tests.Fixtures;
using Dobrasync.Api.Tests.Util;
using Dobrasync.Common.Util;
using Microsoft.Extensions.DependencyInjection;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.Tests.Tests;

[Collection("Sync")]
public class VersionTest : IClassFixture<PopulatedSingleLibraryFixture>
{
    private readonly IAppsettingsProviderService apps;
    private readonly IVersionService versionService;
    private readonly IBlockService blockService;
    private readonly IRepoWrapper repo;
    private readonly IFileService fileService;
    private readonly ILibraryService libraryService;

    public VersionTest(PopulatedSingleLibraryFixture fixture)
    {
        versionService = fixture.ServiceProvider.GetRequiredService<IVersionService>();
        repo = fixture.ServiceProvider.GetRequiredService<IRepoWrapper>();
        apps = fixture.ServiceProvider.GetRequiredService<IAppsettingsProviderService>();
        fileService = fixture.ServiceProvider.GetRequiredService<IFileService>();
        libraryService = fixture.ServiceProvider.GetRequiredService<ILibraryService>();
        blockService = fixture.ServiceProvider.GetRequiredService<IBlockService>();
    }

    [Fact]
    public async Task CreateVersionNewFileTest()
    {
        Guid targetLibraryId = PopulatedSingleLibraryFixture.CreatedLibrary.Id;
        TestFile testFile = new TestFile("Data/LargeTestfile.txt", "subdir1/subdir2/TestFile");
        
        VersionCreateResultDto res = await versionService.CreateVersionMappedAsync(new()
        {
            FileChecksum = testFile.Checksum,
            FilePath = testFile.Path,
            ExpectedBlocks = testFile.Blocks.Select(ChecksumUtil.CalculateFileChecksum).ToList(),
            IsDirectory = false,
            LibraryId = targetLibraryId,
            FilePermissionsOctal = 0,
            FileCreatedOnUtc = testFile.FileInfo.CreationTimeUtc,
            FileModifiedOnUtc = testFile.FileInfo.LastWriteTimeUtc, 
        });
        
        Assert.NotNull(res);
        Assert.NotEmpty(res.RequiredBlocks);
        Assert.NotEqual(Guid.Empty, res.CreatedVersion.Id);
    }
    
    [Fact]
    public async Task CreateVersionExistingFileTest()
    {
        string libraryFilePath = "non/existing/path/File.txt";
        Guid targetLibraryId = PopulatedSingleLibraryFixture.CreatedLibrary.Id;
        TestFile testFile = new TestFile("Data/Testfile.txt", libraryFilePath);
        
        #region Create first version
        VersionCreateResultDto res = await versionService.CreateVersionMappedAsync(new()
        {
            FileChecksum = testFile.Checksum,
            FilePath = testFile.Path,
            ExpectedBlocks = testFile.Blocks.Select(ChecksumUtil.CalculateFileChecksum).ToList(),
            IsDirectory = false,
            LibraryId = targetLibraryId,
            FilePermissionsOctal = 0,
            FileCreatedOnUtc = testFile.FileInfo.CreationTimeUtc,
            FileModifiedOnUtc = testFile.FileInfo.LastWriteTimeUtc, 
        });

        foreach (string requiredBlock in res.RequiredBlocks)
        {
            byte[] blockMatch = testFile.Blocks.Find(x =>
                ChecksumUtil.CalculateBlockChecksum(x) == requiredBlock)!;

            await blockService.CreateBlockAsync(blockMatch, ChecksumUtil.CalculateBlockChecksum(blockMatch), targetLibraryId);
        }
        
        await versionService.CompleteMappedAsync(res.CreatedVersion.Id);
        #endregion
        #region Create second version
        TestFile testFile2 = new TestFile("Data/LargeTestfile.txt", libraryFilePath);
        VersionCreateResultDto res2 = await versionService.CreateVersionMappedAsync(new()
        {
            FileChecksum = testFile2.Checksum,
            FilePath = testFile2.Path,
            ExpectedBlocks = testFile2.Blocks.Select(ChecksumUtil.CalculateFileChecksum).ToList(),
            IsDirectory = false,
            LibraryId = targetLibraryId,
            FilePermissionsOctal = 0,
            FileCreatedOnUtc = testFile2.FileInfo.CreationTimeUtc,
            FileModifiedOnUtc = testFile2.FileInfo.LastWriteTimeUtc, 
        });

        foreach (string requiredBlock in res2.RequiredBlocks)
        {
            byte[] blockMatch = testFile2.Blocks.Find(x =>
                ChecksumUtil.CalculateBlockChecksum(x) == requiredBlock)!;

            await blockService.CreateBlockAsync(blockMatch, ChecksumUtil.CalculateBlockChecksum(blockMatch), targetLibraryId);
        }
        
        await versionService.CompleteMappedAsync(res2.CreatedVersion.Id);
        #endregion
        
        #region Get file info
        FileDto fileDto = await libraryService.GetLibraryFileMappedAsync(targetLibraryId, libraryFilePath);
        #endregion
        
        Assert.Equal(res2.CreatedVersion.Id, fileDto.CurrentVersionId);
        Assert.Equal(libraryFilePath, fileDto.Path);
    }
}