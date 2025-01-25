using Dobrasync.Api.BusinessLogic.Mapper;
using Dobrasync.Api.BusinessLogic.Services.Core.AccessControl;
using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Core.Invoker;
using Dobrasync.Api.BusinessLogic.Services.Core.Logger;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Api.BusinessLogic.Services;

public static class ServiceRegister
{
    public static void RegisterCommonServices(ServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<FileMappingProfile>();
        });
        #region Core
        serviceCollection.AddScoped<IAccessControlService, AccessControlService>();
        serviceCollection.AddScoped<IAppsettingsProviderService, AppsettingsProviderService>();
        serviceCollection.AddScoped<IInvokerService, InvokerService>();
        serviceCollection.AddScoped<ILoggerService, LoggerService>();
        #endregion
        #region Main
        serviceCollection.AddScoped<ILibraryService, LibraryService>();
        serviceCollection.AddScoped<IFileService, FileService>();
        serviceCollection.AddScoped<IBlockService, BlockService>();
        #endregion
    }
}