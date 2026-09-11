using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zukya.Domain.Roles.Entities;

namespace Zukya.Infra.Persistence.Modules.Roles;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();


        builder.HasMany(x => x.Permissions)
            .WithMany(p => p.Roles) // <-- Aponta para a coleção inversa em Permission
            .UsingEntity<Dictionary<string, object>>(
                "permissions_roles",
                j => j.HasOne<Permission>().WithMany().HasForeignKey("permission_id").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Role>().WithMany().HasForeignKey("role_id").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("permissions_roles");
                    j.Property<string>("Id").HasColumnName("id").HasMaxLength(26);
                    j.HasKey("Id");
                });
    }
}
