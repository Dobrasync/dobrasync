using System.ComponentModel.DataAnnotations;
using Dobrasync.Api.Shared.Appsettings.Database;
using Dobrasync.Api.Shared.Appsettings.Deployment;
using Dobrasync.Api.Shared.Appsettings.Dev;
using Dobrasync.Api.Shared.Appsettings.Storage;

namespace Dobrasync.Api.Shared.Appsettings;

public class AppsettingsAS
{
    /// <summary>
    /// Settings related to storage management.
    /// </summary>
    [Required]
    public StorageAS Storage { get; set; } = null!;
    
    /// <summary>
    /// Database related settings.
    /// </summary>
    [Required]
    public DatabaseAS Database { get; set; } = null!;
    
    /// <summary>
    /// Deployment options.
    /// </summary>
    [Required]
    public DeploymentAS Deployment { get; set; } = null!;
    
    /// <summary>
    /// Development options.
    /// </summary>
    [Required]
    public DevAS Dev { get; set; } = null!;
}