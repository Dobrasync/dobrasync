using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryClone;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryCreate;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryUnclone;
using Dobrasync.Common.Clients.Database.DB.Entities;
using Dobrasync.Common.Clients.Database.Repos;
using Dobrasync.Common.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
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
        
        #region itermediate sync
        var result2 = await libraryService.SyncLibraryAsync(lib.Id, progress, token);
        Assert.Empty(result2.FailedFiles);
        Assert.Empty(result2.PulledFiles);
        Assert.Empty(result2.PushedFiles);
        Assert.Empty(result2.UndecidedFiles);
        #endregion
        
        #region change file
        await System.IO.File.WriteAllBytesAsync(Path.Join(lib.Path, bigFilePath), File.ReadAllBytes(TestData.TestDataMediumTestfilePath));
        var result3 = await libraryService.SyncLibraryAsync(lib.Id, progress, token);
        
        Assert.Single(result3.PushedFiles);
        Assert.Empty(result3.FailedFiles);
        Assert.Empty(result3.PulledFiles);
        Assert.Empty(result3.UndecidedFiles);
        #endregion
        
        #region Unclone library
        var uncloneResult = await libraryService.UncloneLibraryAsync(lib.Id, new Progress<LibraryUnclonePR>(), CancellationToken.None);
        Assert.False(Directory.Exists(lib.Path));
        Assert.Equal(0, repo.LibraryRepo.QueryAll().Count(x => x.RemoteId == lib.RemoteId));
        #endregion
        
        #region clone again

        var recloneResult =
            await libraryService.CloneLibraryAsync(lib.RemoteId, LibraryDirectory, new Progress<LibraryClonePR>(),
                CancellationToken.None);

        #endregion
        #region sync / pull from remote
        var resyncResult = await libraryService.SyncLibraryAsync(lib.Id, new Progress<LibrarySyncPR>(), CancellationToken.None);
        
        Assert.Empty(resyncResult.PushedFiles);
        Assert.Empty(resyncResult.FailedFiles);
        Assert.Empty(resyncResult.UndecidedFiles);
        Assert.Equal(2, resyncResult.PulledFiles.Count);
        #endregion
    }
}