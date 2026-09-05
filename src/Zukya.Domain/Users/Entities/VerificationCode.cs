using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Users.Enums;

namespace Zukya.Domain.Users.Entities;

public class VerificationCode : AggregateRoot
{
    public string UserId { get; private set; }
    public string Code { get; private set; }
    public VerificationType Type { get; private set; }
    public int Attempts { get; private set; }
    public int MaxAttempts { get; }
    public DateTimeOffset ExpiresAt { get; }
    public bool IsUsed { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public VerificationCode(string userId,
        string code,
        VerificationType type,
        TimeSpan validityDuration)
    {
        UserId = userId;
        Code = code;
        Type = type;
        Attempts = 0;
        MaxAttempts = 3;
        ExpiresAt = DateTimeOffset.UtcNow.Add(validityDuration);
        IsUsed = false;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsValid() => !IsUsed && DateTimeOffset.UtcNow <= ExpiresAt && Attempts < MaxAttempts;

    public void RegisterFailedAttempt() => Attempts++;

    public void MarkAsUsed() => IsUsed = true;
}
