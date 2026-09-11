using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zukya.Domain.Users.Entities;

namespace Zukya.Infra.Persistence.Modules.Users.Configurations;

public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("verification_codes");
        builder.Ignore(x => x.Events);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Attempts)
            .HasColumnName("attempts")
            .HasDefaultValue(0);

        builder.Property(x => x.MaxAttempts)
            .HasColumnName("max_attempts")
            .HasDefaultValue(3);

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .HasColumnName("is_used")
            .HasDefaultValue(false);

        builder.Property(x => x.UsedAt)
            .HasColumnName("used_at");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.Type, x.IsUsed })
            .HasDatabaseName("idx_verification_user_type");
    }
}
