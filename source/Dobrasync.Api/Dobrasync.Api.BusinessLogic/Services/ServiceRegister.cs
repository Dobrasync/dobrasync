using Dobrasync.Api.BusinessLogic.Mapper;
using Dobrasync.Api.BusinessLogic.Services.Core.AccessControl;
using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Core.Invoker;
using Dobrasync.Api.BusinessLogic.Services.Core.Logger;
using Dobrasync.Api.BusinessLogic.Services.Main.Blocks;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Libraries;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.DB;
using Dobrasync.Api.Database.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Api.BusinessLogic.Services;

public static class ServiceRegister
{
    public static void RegisterCommonServices(ServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<FileMappingProfile>();
            cfg.AddProfile<VersionMappingProfile>();
            cfg.AddProfile<LibraryMappingProfile>();
        });
        #region Core
        serviceCollection.AddScoped<IRepoWrapper, RepoWrapper>();
        serviceCollection.AddScoped<IAccessControlService, AccessControlService>();
        serviceCollection.AddScoped<IAppsettingsProviderService, AppsettingsProviderService>();
        serviceCollection.AddScoped<IInvokerService, InvokerService>();
        serviceCollection.AddScoped<ILoggerService, LoggerService>();
        #endregion
        #region Main
        serviceCollection.AddScoped<ILibraryService, LibraryService>();
        serviceCollection.AddScoped<IFileService, FileService>();
        serviceCollection.AddScoped<IBlockService, BlockService>();
        serviceCollection.AddScoped<IVersionService, VersionService>();
        #endregion
    }
}