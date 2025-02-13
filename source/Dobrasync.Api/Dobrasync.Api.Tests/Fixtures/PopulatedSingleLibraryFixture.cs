using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.BusinessLogic.Services;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Tests.Util;
using Microsoft.Extensions.DependencyInjection;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.Tests.Fixtures;

public class PopulatedSingleLibraryFixture : IAsyncLifetime
{
    private readonly string _testFileSourcePath = "Data/Testfile.txt";
    private readonly string _testFileNewerSourcePath = "Data/TestfileNewer.txt";
    public static readonly string TestFileNewerOnlylocalSourcePath = "Data/TestfileNewerOnlyLocal.txt";
    
    public static readonly string LibraryName = "TestLibrary";
    public static readonly string TestFilePath = "testdir/testfile.txt";
    public static readonly string TestFileContent = "This is the testfiles content.";
    
    public static Library CreatedLibrary = null!;
    public static VersionCreateResultDto InitialFileVersion = null!;
    public static VersionCreateResultDto CurrentFileVersion = null!;
    
    public IServiceProvider ServiceProvider { get; }
    
    public PopulatedSingleLibraryFixture()
    {
        var services = new ServiceCollection();
        
        ServiceRegister.RegisterCommonServices(services);
        TestServiceRegister.RegisterTestServices(services);
        
        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        ILibraryService libraryService = ServiceProvider.GetRequiredService<ILibraryService>();
        IVersionService versionService = ServiceProvider.GetRequiredService<IVersionService>();

        CreatedLibrary = await libraryService.CreateLibraryAsync(LibraryName);
        
        #region create initial version
        InitialFileVersion = await TestUtil.EnsureFileCreatedInLibrary(TestFilePath, _testFileSourcePath, CreatedLibrary.Id, versionService);
        #endregion
        #region create newer version
        CurrentFileVersion = await TestUtil.EnsureFileCreatedInLibrary(TestFilePath, _testFileNewerSourcePath, CreatedLibrary.Id, versionService);
        #endregion

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}