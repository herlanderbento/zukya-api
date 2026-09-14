using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zukya.Infra.Persistence.Modules.SystemSetting.Configurations;

using DomainEntity = Domain.SystemSetting.Entities;

public class SystemSettingConfiguration : IEntityTypeConfiguration<DomainEntity.SystemSetting>
{
    public void Configure(EntityTypeBuilder<DomainEntity.SystemSetting> builder)
    {
        builder.ToTable("system_settings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Key).HasColumnName("key").IsRequired();
        builder.Property(s => s.Value).HasColumnName("value").IsRequired();
        builder.Property(s => s.Type).HasColumnName("type").IsRequired();
        builder.Property(s => s.Description).HasColumnName("description");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");

        builder.HasData(
            new
            {
                Id = 1,
                Key = "security.auto_banned_on_failed_login.enabled",
                Value = "true",
                Type = "boolean",
                Description = "Habilita/Desabilita banimento automatico por tentativas excessivas de login",
                CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new
            {
                Id = 2,
                Key = "security.max_failed_login_attempts",
                Value = "5",
                Type = "int",
                Description = "Numero maximo de tentativas falhadas antes de banir a conta",
                CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}
