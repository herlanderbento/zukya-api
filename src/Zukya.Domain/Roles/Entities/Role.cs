using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Shared.Exceptions;
using Zukya.Domain.Users.Entities;

namespace Zukya.Domain.Roles.Entities;

public class Role : AggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<User> users = new();
    public IReadOnlyCollection<User> Users => users.AsReadOnly();

    private readonly List<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public Role(string name, string? description = null)
    {
        Name = name;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;

        Validate();
    }

    public void Update(string? name, string? description)
    {
        Name = name ?? Name;
        Description = description ?? Description;
    }

    public void AddPermission(Permission permission)
    {
        if (!_permissions.Contains(permission)) _permissions.Add(permission);
    }

    public void RemovePermission(Permission permission)
    {
        if (_permissions.Contains(permission)) _permissions.Remove(permission);
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new EntityValidationException("Role name is required");
    }
}
