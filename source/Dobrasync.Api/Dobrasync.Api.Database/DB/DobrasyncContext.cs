using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Database.Seed;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.Database.DB;

public class DobrasyncContext(DbContextOptions<DobrasyncContext> options) : DbContext(options)
{
    public virtual DbSet<Block> Block { get; set; }
    public virtual DbSet<File> File { get; set; }
    public virtual DbSet<Library> Library { get; set; }
    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region Model
        builder.Entity<File>()
            .HasIndex(e => e.Path)
            .IsUnique();

        builder.Entity<Block>()
            .HasIndex(e => e.Checksum)
            .IsUnique();
        
        builder.Entity<Library>()
            .HasIndex(e => e.Name)
            .IsUnique();
        
        builder.Entity<User>()
            .HasIndex(e => e.Username)
            .IsUnique();
        
        builder.Entity<Version>()
            .Property(e => e.ExpectedBlocks)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v)!
            );
        #endregion
        #region Seed
        Seeder.SeedUniversal(builder);
        #endregion
    }
}