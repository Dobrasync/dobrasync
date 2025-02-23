using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryClone;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryCreate;
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
        var created = await libraryService.CreateLibraryAsync(randomLibraryName, new Progress<LibraryCreatePR>(), CancellationToken.None);
        
        await libraryService.CloneLibraryAsync(created.CreatedLibrary.Id, LibraryDirectory, new Progress<LibraryClonePR>(), CancellationToken.None);
        
        Assert.NotNull(repo.LibraryRepo.QueryAll().FirstOrDefault(x => x.RemoteId == created.CreatedLibrary.Id));
        FileInfo directory = new FileInfo(Path.Join(LibraryDirectory, randomLibraryName));
        Assert.True(directory.Directory!.Exists);
    }
    
    [Fact]
    public async Task CreateClonePushPullEmptyLibraryTest()
    {
        string randomLibraryName = $"testlib-{Guid.NewGuid().ToString()}";
        var created = await libraryService.CreateLibraryAsync(randomLibraryName, new Progress<LibraryCreatePR>(), CancellationToken.None);
        
        await libraryService.CloneLibraryAsync(created.CreatedLibrary.Id, LibraryDirectory, new Progress<LibraryClonePR>(), CancellationToken.None);
        
        IProgress<LibrarySyncPR> progress = new Progress<LibrarySyncPR>();
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        Library lib = repo.LibraryRepo.QueryAll().First(x => x.RemoteId == created.CreatedLibrary.Id);
        var result = await libraryService.SyncLibraryAsync(lib.Id, progress, token);
        
        Assert.Empty(result.PushedFiles);
        Assert.Empty(result.FailedFiles);
        Assert.Empty(result.PulledFiles);
        Assert.Empty(result.UndecidedFiles);
    }
    
    [Fact]
    public async Task CreateClonePushPullWithContentLibraryTest()
    {
        string randomLibraryName = $"testlib-{Guid.NewGuid().ToString()}";
        var created = await libraryService.CreateLibraryAsync(randomLibraryName, new Progress<LibraryCreatePR>(), CancellationToken.None);
        
        await libraryService.CloneLibraryAsync(created.CreatedLibrary.Id, LibraryDirectory, new Progress<LibraryClonePR>(), CancellationToken.None);
        
        IProgress<LibrarySyncPR> progress = new Progress<LibrarySyncPR>();
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        Library lib = repo.LibraryRepo.QueryAll().First(x => x.RemoteId == created.CreatedLibrary.Id);
        
        #region add content to library dir
        string smallFilePath = "SmallFile.txt";
        string bigFilePath = "BigFile.txt";
        await System.IO.File.WriteAllBytesAsync(Path.Join(lib.Path, bigFilePath), File.ReadAllBytes(TestData.TestDataLargeTestfilePath));
        await System.IO.File.WriteAllBytesAsync(Path.Join(lib.Path, smallFilePath), File.ReadAllBytes(TestData.TestDataTestfilePath));
        
        var result = await libraryService.SyncLibraryAsync(lib.Id, progress, token);
        Assert.NotEmpty(result.PushedFiles);
        Assert.Equal(2, result.PushedFiles.Count);
        Assert.Empty(result.FailedFiles);
        Assert.Empty(result.PulledFiles);
        Assert.Empty(result.UndecidedFiles);
        #endregion
    }
}