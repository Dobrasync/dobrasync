namespace Dobrasync.Api.Shared.Appsettings.Deployment;

public class DeploymentAS
{
    /// <summary>
    /// If automatic https redirection is enabled.
    ///
    /// Might not be needed if using a reverse proxy.
    /// </summary>
    public bool EnableHttpsRedirection { get; set; } = false;

    /// <summary>
    /// Allowed cors origins.
    /// </summary>
    public List<string> CorsOrigins { get; set; } = new();
}