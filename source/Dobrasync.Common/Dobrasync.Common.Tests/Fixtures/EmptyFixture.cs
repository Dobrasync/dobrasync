using Dobrasync.Common.Clients.BusinessLogic.Services;
using Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Common.Tests.Fixtures;

public class EmptyFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; }
    
    public EmptyFixture()
    {
        var services = new ServiceCollection();
        
        ServiceRegister.RegisterCommonServices(services);
        TestServiceRegister.RegisterTestServices(services);
        
        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        
    }
}