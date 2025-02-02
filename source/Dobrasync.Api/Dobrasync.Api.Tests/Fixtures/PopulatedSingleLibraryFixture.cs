using Dobrasync.Api.BusinessLogic.Services;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Tests.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Api.Tests.Fixtures;

public class PopulatedSingleLibraryFixture : IAsyncLifetime
{
    public static readonly string LibraryName = "TestLibrary";
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

        CreatedLibrary = await libraryService.CreateLibraryAsync(LibraryName);
        TestUtil.EnsureFileCreatedInLibrary();

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}