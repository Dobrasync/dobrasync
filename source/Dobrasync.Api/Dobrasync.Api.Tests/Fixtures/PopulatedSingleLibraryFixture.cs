using Dobrasync.Api.BusinessLogic.Services;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Tests.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Api.Tests.Fixtures;

public class PopulatedSingleLibraryFixture : IAsyncLifetime
{
    private readonly string _testFileSourcePath = "Data/Testfile.txt";
    
    public static readonly string LibraryName = "TestLibrary";
    public static readonly string TestFilePath = "testdir/testfile.txt";
    public static readonly string TestFileContent = "This is the testfiles content.";
    public static Library CreatedLibrary = null!;
    
    public IServiceProvider ServiceProvider { get; }
    
    public PopulatedSingleLibraryFixture()
    {
        var services = new ServiceCollection();
        ServiceRegister.RegisterCommonServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        ILibraryService libraryService = ServiceProvider.GetRequiredService<ILibraryService>();
        IVersionService versionService = ServiceProvider.GetRequiredService<IVersionService>();

        CreatedLibrary = await libraryService.CreateLibraryAsync(LibraryName);
        await TestUtil.EnsureFileCreatedInLibrary(TestFilePath, _testFileSourcePath, CreatedLibrary.Id, versionService);

        
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}