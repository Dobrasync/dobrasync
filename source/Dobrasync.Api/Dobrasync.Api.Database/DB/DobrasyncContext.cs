using Dobrasync.Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.Database.DB;

public class DobrasyncContext(DbContextOptions<DobrasyncContext> options) : DbContext(options)
{
    public virtual DbSet<Block> Block { get; set; }
    public virtual DbSet<File> File { get; set; }
    public virtual DbSet<Library> Library { get; set; }
    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
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
    }
}