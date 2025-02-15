using File = Dobrasync.Common.Clients.Database.DB.Entities.File;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class FileTreeBuildResult
{
    /// <summary>
    /// Files that haven't changed since last build
    /// </summary>
    public List<File> FilesUntouched { get; set; } = new();
    
    /// <summary>
    /// Files that were deleted since last build
    /// </summary>
    public List<File> FilesDeleted { get; set; } = new();
    
    /// <summary>
    /// Files that were added since last build
    /// </summary>
    public List<File> FilesCreated { get; set; } = new();
    
    /// <summary>
    /// Files that were changed since last build
    /// </summary>
    public List<File> FilesChanged { get; set; } = new();
}