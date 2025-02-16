using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Common.Clients.Database.Repos;
using Dobrasync.Common.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

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
        string randomLibraryName = Guid.NewGuid().ToString();
        LibraryDto created = await libraryService.CreateLibraryAsync(randomLibraryName);
        
        await libraryService.CloneLibraryAsync(created.Id, LibraryDirectory);
    }
}