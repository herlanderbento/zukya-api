using System.Security.Cryptography;
using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Users.Enums;

namespace Zukya.Domain.Users.Entities;

public class VerificationCode : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; } = null!;
    public VerificationType Type { get; private set; }
    public int Attempts { get; private set; }
    public int MaxAttempts { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    protected VerificationCode()
    {
    }

    public static VerificationCode Create(
        Guid userId,
        VerificationType type,
        TimeSpan validityDuration)
    {
        return new VerificationCode
        {
            UserId = userId,
            Code = GenerateCode(),
            Type = type,
            Attempts = 0,
            MaxAttempts = 3,
            ExpiresAt = DateTimeOffset.UtcNow.Add(validityDuration),
            IsUsed = false,
            UsedAt = null,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public bool IsValid() => !IsUsed && DateTimeOffset.UtcNow <= ExpiresAt && Attempts < MaxAttempts;

    public void RegisterFailedAttempt() => Attempts++;

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTimeOffset.UtcNow;
    }

    private static string GenerateCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);

        var result = new char[length];
        for (var i = 0; i < length; i++)
            result[i] = chars[bytes[i] % chars.Length];

        return new string(result);
    }
}
