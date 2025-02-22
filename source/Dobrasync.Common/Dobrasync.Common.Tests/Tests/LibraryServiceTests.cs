using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Common.Clients.Database.DB.Entities;
using Dobrasync.Common.Clients.Database.Repos;
using Dobrasync.Common.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using File = System.IO.File;

namespace Dobrasync.Common.Tests.Tests;

public class LibraryServiceTests : IClassFixture<EmptyFixture>
{
    public static readonly string LibraryDirectory = "./tmp/libraries";
    private readonly ILibraryService libraryService;
    private readonly IRepoWrapper repo;
    
    public LibraryServiceTests(EmptyFixture fixture)
    {
        libraryService = fixture.ServiceProvider.GetRequiredService<ILibraryService>();
        repo = fixture.ServiceProvider.GetRequiredService<IRepoWrapper>();
    }
    
    [Fact]
    public async Task CreateAndCloneLibraryTest()
    {
        string randomLibraryName = $"testlib-{Guid.NewGuid().ToString()}";
        LibraryDto created = await libraryService.CreateLibraryAsync(randomLibraryName);
        
        await libraryService.CloneLibraryAsync(created.Id, LibraryDirectory);
        
        Assert.NotNull(repo.LibraryRepo.QueryAll().FirstOrDefault(x => x.RemoteId == created.Id));
        FileInfo directory = new FileInfo(Path.Join(LibraryDirectory, randomLibraryName));
        Assert.True(directory.Directory!.Exists);
    }
    
    [Fact]
    public async Task CreateClonePushPullEmptyLibraryTest()
    {
        string randomLibraryName = $"testlib-{Guid.NewGuid().ToString()}";
        LibraryDto created = await libraryService.CreateLibraryAsync(randomLibraryName);
        
        await libraryService.CloneLibraryAsync(created.Id, LibraryDirectory);
        
        IProgress<SyncProgressUpdateBase> progress = new Progress<SyncProgressUpdateBase>();
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        Library lib = repo.LibraryRepo.QueryAll().First(x => x.RemoteId == created.Id);
        SyncResult result = await libraryService.SyncLibraryAsync(progress, token, lib.Id);
        
        Assert.Empty(result.PushedFiles);
        Assert.Empty(result.FailedFiles);
        Assert.Empty(result.PulledFiles);
        Assert.Empty(result.UndecidedFiles);
    }
    
    [Fact]
    public async Task CreateClonePushPullWithContentLibraryTest()
    {
        string randomLibraryName = $"testlib-{Guid.NewGuid().ToString()}";
        LibraryDto created = await libraryService.CreateLibraryAsync(randomLibraryName);
        
        await libraryService.CloneLibraryAsync(created.Id, LibraryDirectory);
        
        IProgress<SyncProgressUpdateBase> progress = new Progress<SyncProgressUpdateBase>();
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        Library lib = repo.LibraryRepo.QueryAll().First(x => x.RemoteId == created.Id);
        
        #region add content to library dir
        string smallFilePath = "SmallFile.txt";
        string bigFilePath = "BigFile.txt";
        await System.IO.File.WriteAllBytesAsync(Path.Join(lib.Path, bigFilePath), File.ReadAllBytes(TestData.TestDataLargeTestfilePath));
        await System.IO.File.WriteAllBytesAsync(Path.Join(lib.Path, smallFilePath), File.ReadAllBytes(TestData.TestDataTestfilePath));
        
        SyncResult result = await libraryService.SyncLibraryAsync(progress, token, lib.Id);
        Assert.NotEmpty(result.PushedFiles);
        Assert.Equal(2, result.PushedFiles.Count);
        Assert.Empty(result.FailedFiles);
        Assert.Empty(result.PulledFiles);
        Assert.Empty(result.UndecidedFiles);
        #endregion
    }
}