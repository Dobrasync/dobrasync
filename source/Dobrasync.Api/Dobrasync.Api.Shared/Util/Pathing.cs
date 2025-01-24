using Dobrasync.Api.Shared.Appsettings;

namespace Dobrasync.Api.Shared.Util;

public static class Pathing
{
    public static string GetPathToBlock(AppsettingsAS appsettings, Guid libraryId, string checksum)
    {
        string libraryDataPath = GetPathToLibraryData(appsettings, libraryId);
        
        return Path.Join(libraryDataPath, checksum);
    }
    
    public static string GetPathToLibraryData(AppsettingsAS appsettings, Guid libraryId)
    {
        return Path.Join(appsettings.Storage.DataDirectory, libraryId.ToString());
    }
}