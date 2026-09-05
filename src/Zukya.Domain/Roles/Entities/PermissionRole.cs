using Zukya.Domain.Shared.Entities;

namespace Zukya.Domain.Roles.Entities;

public class PermissionRole : AggregateRoot
{
    public string RoleId { get; set; }
    public string PermissionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }


    public PermissionRole(string roleId, string permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;

        CreatedAt = DateTimeOffset.Now;
        UpdatedAt = DateTimeOffset.Now;
    }
}
