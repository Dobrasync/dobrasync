using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

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
}