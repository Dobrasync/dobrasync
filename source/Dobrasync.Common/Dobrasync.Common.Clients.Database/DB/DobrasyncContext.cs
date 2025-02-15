using Dobrasync.Common.Clients.Database.DB.Entities;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Common.Clients.Database.DB.Entities.File;
using Version = Dobrasync.Common.Clients.Database.DB.Entities.Version;

namespace Dobrasync.Common.Clients.Database.DB;

public class DobrasyncContext(DbContextOptions<DobrasyncContext> options) : DbContext(options)
{
    public virtual DbSet<Setting> Block { get; set; }
    public virtual DbSet<Version> Version { get; set; }
    public virtual DbSet<File> File { get; set; }
    public virtual DbSet<Library> Library { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region Model
        builder.Entity<Setting>()
            .HasIndex(e => e.Key)
            .IsUnique();
        #endregion
        #region Seed
        //Seeder.SeedUniversal(builder);
        #endregion
    }
}