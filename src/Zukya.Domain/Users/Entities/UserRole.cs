using Zukya.Domain.Shared.Entities;

namespace Zukya.Domain.Users.Entities;

public class UserRole : AggregateRoot
{
    public string UserId { get; private set; }
    public string RoleId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public UserRole(string userId, string roleId)
    {
        UserId = userId;
        RoleId = roleId;
        CreatedAt = DateTimeOffset.Now;
        UpdatedAt = DateTimeOffset.Now;
    }
}
