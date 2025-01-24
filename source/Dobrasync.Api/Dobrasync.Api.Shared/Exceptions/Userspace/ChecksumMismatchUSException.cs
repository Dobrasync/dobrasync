namespace Dobrasync.Api.Shared.Exceptions.Userspace;

public class ChecksumMismatchUSException() : UserspaceException(400, "Checksum mismatch", "Checksum does not match data.")
{
    
}