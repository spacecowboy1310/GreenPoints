using Microsoft.EntityFrameworkCore;

namespace GreenPointsAPI.Data;

public class GreenPointsContext : DbContext
{
    public GreenPointsContext(DbContextOptions<GreenPointsContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GreenPoint>()
            .HasMany(g => g.Properties)
            .WithOne(p => p.GreenPoint)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EditGreenPoint>()
            .HasMany(g => g.Properties)
            .WithOne(p => p.EditGreenPoint)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TemporalUser> TemporalUsers => Set<TemporalUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<GreenPoint> GreenPoints => Set<GreenPoint>();
    public DbSet<DescriptionProperty> DescriptionProperties => Set<DescriptionProperty>();
    public DbSet<EditGreenPoint> EditGreenPoints => Set<EditGreenPoint>();
    public DbSet<DescriptionProperty> EditDescriptionProperties => Set<DescriptionProperty>();
}
