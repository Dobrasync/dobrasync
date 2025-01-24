using Dobrasync.Api.BusinessLogic.Services.Core.Invoker;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Shared.Exceptions.Userspace.Auth;

namespace Dobrasync.Api.BusinessLogic.Services.Core.AccessControl;

public class AccessControlService(IInvokerService invokerService) : IAccessControlService
{
    public async Task<User> VerifyAsync(Action<AccessControlProps> configure)
    {
        #region Load info and prep
        User? invoker = await invokerService.GetInvokerRequiredAsync();
        
        AccessControlProps props = new AccessControlProps();
        configure(props);
        #endregion
        #region Evaluate challenges
        List<bool> challengeResults =
        [
            props.RequireLibraryOwnership == null || invoker.Libraries.Contains(props.RequireLibraryOwnership)
        ];
        #endregion

        return challengeResults.Contains(false) ? throw new UnauthorizedUSException() : invoker;
    }
}