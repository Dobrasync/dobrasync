using Dobrasync.Api.Database.Entities;

namespace Dobrasync.Api.BusinessLogic.Services.Core.AccessControl;

public class AccessControlProps
{
    public Library? RequireLibraryRead { get; set; }
    public Library? RequireLibraryWrite { get; set; }
    public Library? RequireLibraryOwnership { get; set; }
}