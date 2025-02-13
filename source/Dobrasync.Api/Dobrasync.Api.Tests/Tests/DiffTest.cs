using Dobrasync.Api.BusinessLogic.Dtos.Diff;
using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Tests.Fixtures;
using Dobrasync.Common.Util;
using Microsoft.Extensions.DependencyInjection;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.Tests.Tests;

[Collection("Sync")]
public class DiffTest : IClassFixture<PopulatedSingleLibraryFixture>
{
    private readonly IAppsettingsProviderService apps;
    private readonly ILibraryService libraryService;
    private readonly IRepoWrapper repo;

    public DiffTest(PopulatedSingleLibraryFixture fixture)
    {
        libraryService = fixture.ServiceProvider.GetRequiredService<ILibraryService>();
        repo = fixture.ServiceProvider.GetRequiredService<IRepoWrapper>();
        apps = fixture.ServiceProvider.GetRequiredService<IAppsettingsProviderService>();
    }

    [Fact]
    public async Task GenerateDiffNoLocalFilesTest()
    {
        await libraryService.MakeDiffMappedAsync(PopulatedSingleLibraryFixture.CreatedLibrary.Id, new()
        {
            FilesOnLocal = []
        });
    }
    
    /// <summary>
    /// This test checks how to api handles diff requests of clients which have the same file
    /// the api has, same version but different content.
    ///
    /// In this case we assume the file on local is newer and API requires this new change / content.
    /// </summary>
    [Fact]
    public async Task GenerateDiffNewerLocalFileTest()
    {
        string localFileChecksum = ChecksumUtil.CalculateFileChecksum(
            await System.IO.File.ReadAllBytesAsync(PopulatedSingleLibraryFixture.TestFileNewerOnlylocalSourcePath)
        );
        
        List<string> difflist = await libraryService.MakeDiffMappedAsync(PopulatedSingleLibraryFixture.CreatedLibrary.Id, new()
        {
            FilesOnLocal = [
                new() {
                    Path = PopulatedSingleLibraryFixture.TestFilePath,
                    FileChecksum = localFileChecksum,
                    LatestVersionId = PopulatedSingleLibraryFixture.CurrentFileVersion.CreatedVersion.Id
                }
            ]
        });
        
        // we expect the diff not to be empty as our file is newer on local
        Assert.NotEmpty(difflist);
        // we expect only our initial test file shows up
        Assert.Single(difflist);
    }
    
    [Fact]
    public async Task GenerateDiffOlderLocalFileTest()
    {
        await libraryService.MakeDiffMappedAsync(PopulatedSingleLibraryFixture.CreatedLibrary.Id, new()
        {
            FilesOnLocal = []
        });
    }
}