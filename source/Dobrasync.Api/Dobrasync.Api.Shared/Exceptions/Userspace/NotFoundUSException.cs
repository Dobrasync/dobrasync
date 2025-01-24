namespace Dobrasync.Api.Shared.Exceptions.Userspace;

public class NotFoundUSException() : UserspaceException(404, "Not found", "The requested resource was not found.")
{
    
}