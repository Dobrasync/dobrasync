using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    
    /// <summary>
    /// All Files that were interated over in the build process
    /// </summary>
    public List<File> FilesAll => FilesUntouched.Concat(FilesCreated).Concat(FilesChanged).ToList();
    
    /// <summary>
    /// FileInfo objs of all files
    /// </summary>
    public List<FileInfo> FilesInfo { get; set; } = new();
}