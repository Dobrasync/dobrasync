using Dobrasync.Api.Database.Entities;

namespace Dobrasync.Api.BusinessLogic.Services.Core.Invoker;

public interface IInvokerService
{
    public Task<User> GetInvokerRequiredAsync();
}