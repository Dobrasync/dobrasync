using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.Services.Core;
using Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Common.Clients.Database.DB;
using Dobrasync.Common.Clients.Database.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Common.Tests;

public class TestServiceRegister
{
    public static void RegisterTestServices(IServiceCollection services)
    {
        services.AddDbContext<DobrasyncContext>(opt => { opt.UseInMemoryDatabase($"Test-{Guid.NewGuid()}"); });
        services.AddSingleton<IApiClient>(new ApiClient("http://localhost:5178/", new()
        {
            
        }));
        services.AddSingleton<IRepoWrapper, RepoWrapper>();
        services.AddSingleton<ISettingService, SettingService>();
        services.AddSingleton<ILibraryService, LibraryService>();
    }
}