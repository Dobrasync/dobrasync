namespace Dobrasync.Api.Shared.Exceptions.Userspace;

public class LibraryNameConflictUSException() : UserspaceException(419, "A library with this name already exists.", "A library with the same name already exists.")
{
    
}