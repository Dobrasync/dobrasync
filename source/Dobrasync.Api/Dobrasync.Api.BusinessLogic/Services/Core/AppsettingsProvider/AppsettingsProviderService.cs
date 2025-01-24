using Dobrasync.Api.Shared.Appsettings;
using Microsoft.Extensions.Options;

namespace Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;

public class AppsettingsProviderService(IOptions<AppsettingsAS> appsettings) : IAppsettingsProviderService
{
    public AppsettingsAS GetAppsettings()
    {
        return appsettings.Value;
    }
}