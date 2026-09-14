using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Zukya.Domain.Roles.Entities;
using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Shared.Events;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Enums;
using DomainEntity = Zukya.Domain.SystemSetting.Entities;
namespace Zukya.Infra.Persistence;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<VerificationCode> VerificationCode => Set<VerificationCode>();
    public DbSet<Permission> Permission => Set<Permission>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<DomainEntity.SystemSetting> SystemSettings => Set<DomainEntity.SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString()!.ToLower(),
                v => Enum.Parse<UserStatus>(v, true)
            );
        
        modelBuilder.Ignore<DomainEvent>();

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                modelBuilder.Entity(entityType.ClrType)
                    .Property("Id")
                    .ValueGeneratedNever();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        NormalizeDateTimes();
        return base.SaveChanges();
    }

    private void NormalizeDateTimes()
    {
        IEnumerable<EntityEntry> entries = ChangeTracker
            .Entries()
            .Where(e =>
            {
                if (e.State == EntityState.Added) return true;
                return e.State == EntityState.Modified;
            });

        foreach (EntityEntry entry in entries)
        foreach (PropertyEntry property in entry.Properties)
        {
            if (property.Metadata.ClrType != typeof(DateTime) || property.CurrentValue == null) continue;
            var dateTime = (DateTime)property.CurrentValue;
            property.CurrentValue = dateTime.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => property.CurrentValue
            };
        }
    }
}
