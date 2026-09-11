using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zukya.Domain.Roles.Entities;
using Zukya.Domain.Shared.ValueObjects;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Enums;

namespace Zukya.Infra.Persistence.Modules.Users.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.Password)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50);

        builder.Property(x => x.Avatar)
            .HasColumnName("avatar_url")
            .HasConversion(
                avatar => avatar != null ? avatar.Path : null,
                path => !string.IsNullOrEmpty(path) ? new Image(path) : null);

        builder.Property(x => x.AccessLevel)
            .HasColumnName("access_level")
            .HasDefaultValue(0);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(UserStatus.Pending);

        builder.Property(x => x.TaxId)
            .HasColumnName("tax_id")
            .HasMaxLength(50);

        builder.Property(x => x.ReputationScore)
            .HasColumnName("reputation_score")
            .HasColumnType("numeric(3,2)")
            .HasDefaultValue(0.00m);

        builder.Property(x => x.WarehouseCode)
            .HasColumnName("warehouse_code")
            .HasMaxLength(50)
            .HasDefaultValue(null);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasMany(x => x.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>(
                "role_users",
                j => j.HasOne<Role>().WithMany().HasForeignKey("role_id").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("user_id").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("role_users");
                    j.Property<string>("Id").HasColumnName("id").HasMaxLength(26);
                    j.HasKey("Id");
                });
    }
}
