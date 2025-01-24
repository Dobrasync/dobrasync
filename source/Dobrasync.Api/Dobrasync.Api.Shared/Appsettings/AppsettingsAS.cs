using System.ComponentModel.DataAnnotations;
using Dobrasync.Api.Shared.Appsettings.Storage;

namespace Dobrasync.Api.Shared.Appsettings;

public class AppsettingsAS
{
    /// <summary>
    /// Settings related to storage management.
    /// </summary>
    [Required]
    public StorageAS Storage { get; set; } = null!;
}