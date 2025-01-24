using System.ComponentModel.DataAnnotations;

namespace Dobrasync.Api.Shared.Appsettings.Database;

public class DatabaseAS
{
    /// <summary>
    /// Database connection information.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = null!;
}