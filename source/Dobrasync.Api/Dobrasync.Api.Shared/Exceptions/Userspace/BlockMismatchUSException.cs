namespace Dobrasync.Api.Shared.Exceptions.Userspace;

public class BlockMismatchUSException() : UserspaceException(400, "Block mismatch", "Expected and received blocks do not match.")
{
    
}