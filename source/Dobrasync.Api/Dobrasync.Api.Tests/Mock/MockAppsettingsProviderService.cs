using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.Shared.Appsettings;

namespace Dobrasync.Api.Tests.Mock;

public class MockAppsettingsProviderService : IAppsettingsProviderService
{
    public AppsettingsAS GetAppsettings()
    {
        return new()
        {
            Storage = new()
            {
                DataDirectory = "./tmp"
            }
        };
    }
}