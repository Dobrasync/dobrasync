using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Libraries owned by user.
    /// </summary>
    public virtual HashSet<Library> Libraries { get; set; } = new();

    /// <summary>
    /// Clients registered to this user.
    /// </summary>
    public virtual HashSet<Client> Clients { get; set; } = new();
}