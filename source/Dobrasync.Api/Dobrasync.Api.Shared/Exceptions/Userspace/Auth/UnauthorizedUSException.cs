namespace Dobrasync.Api.Shared.Exceptions.Userspace.Auth;

public class UnauthorizedUSException() : UserspaceException(403, "Unauthorized", "User does not have access to this resource.")
{
    
}