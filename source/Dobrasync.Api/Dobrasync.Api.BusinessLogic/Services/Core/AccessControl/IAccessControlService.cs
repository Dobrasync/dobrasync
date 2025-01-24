using Dobrasync.Api.Database.Entities;

namespace Dobrasync.Api.BusinessLogic.Services.Core.AccessControl;

public interface IAccessControlService
{
    /// <summary>
    /// Receives a set of challenges to verify. If any challenge fails, an <see cref="UnauthorzedUSException"/> is thrown.
    /// </summary>
    /// <param name="configure">Challenge-set</param>
    /// <returns>Invoker user object</returns>
    public Task<User> VerifyAsync(Action<AccessControlProps> configure);
}