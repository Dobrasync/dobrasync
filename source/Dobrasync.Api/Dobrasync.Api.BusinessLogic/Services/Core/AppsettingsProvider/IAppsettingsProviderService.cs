using Dobrasync.Api.Shared.Appsettings;

namespace Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;

public interface IAppsettingsProviderService
{
    public AppsettingsAS GetAppsettings();
}