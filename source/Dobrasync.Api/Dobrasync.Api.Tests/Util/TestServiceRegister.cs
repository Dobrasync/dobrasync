using Dobrasync.Api.Database.DB;
using Dobrasync.Api.Tests.Mock;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dobrasync.Api.Tests.Util;

public class TestServiceRegister
{
    public static void RegisterTestServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<DobrasyncContext>(opt => { opt.UseInMemoryDatabase($"Test-{Guid.NewGuid()}"); });
        serviceCollection.AddScoped<IHttpContextAccessor, HttpContextAccessorMock>();
    }
}