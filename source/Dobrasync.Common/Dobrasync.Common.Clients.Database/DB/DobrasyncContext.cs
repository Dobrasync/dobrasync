using Dobrasync.Common.Clients.Database.DB.Entities;
using Dobrasync.Common.Clients.Database.DB.Entities.Base;
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
        
        builder.Entity<Library>()
            .HasMany(l => l.Files)
            .WithOne(b => b.Library)
            .HasForeignKey(b => b.LibraryId)
            .OnDelete(DeleteBehavior.Cascade); 
        
        builder.Entity<File>()
            .HasMany(l => l.Versions)
            .WithOne(b => b.File)
            .HasForeignKey(b => b.FileId)
            .OnDelete(DeleteBehavior.Cascade); 
        #endregion
        #region Seed
        //Seeder.SeedUniversal(builder);
        #endregion
    }
}