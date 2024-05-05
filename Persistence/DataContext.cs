using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Activity> Activities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            modelBuilder.Entity(entityType.Name).Property<DateTime>("CreatedAt");
            modelBuilder.Entity(entityType.Name).Property<DateTime?>("UpdatedAt");
            modelBuilder.Entity(entityType.Name).Property<DateTime?>("DeletedAt");
        }
    }

    public override int SaveChanges()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
            .Select(x => x.Entity);

        var currentTime = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            var entry = Entry(entity);
            if (entry.State == EntityState.Added && entry.Metadata.FindProperty("CreatedAt") != null)
            {
                entry.Property("CreatedAt").CurrentValue = currentTime;
            }
            if (entry.Metadata.FindProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = currentTime;
            }
        }

        return base.SaveChanges();
    }

}
