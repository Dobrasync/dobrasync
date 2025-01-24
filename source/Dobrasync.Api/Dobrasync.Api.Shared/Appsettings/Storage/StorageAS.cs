using System.ComponentModel.DataAnnotations;

namespace Dobrasync.Api.Shared.Appsettings.Storage;

public class StorageAS
{
    /// <summary>
    /// Where library data (blocks, files) are stored.
    /// </summary>
    [Required]
    public string DataDirectory { get; set; } = null!;
}