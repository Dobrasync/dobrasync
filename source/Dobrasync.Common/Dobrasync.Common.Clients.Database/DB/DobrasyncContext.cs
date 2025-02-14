using Dobrasync.Common.Clients.Database.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dobrasync.Common.Clients.Database.DB;

public class DobrasyncContext(DbContextOptions<DobrasyncContext> options) : DbContext(options)
{
    public virtual DbSet<Setting> Block { get; set; }

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