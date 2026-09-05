using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Shared.Exceptions;

namespace Zukya.Domain.Roles.Entities;

public class Permission : AggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles;

    public Permission(string name, string? description = null)
    {
        Name = name;
        Description = description;
        CreatedAt = DateTimeOffset.Now;
        UpdatedAt = DateTimeOffset.Now;

        Validate();
    }

    public void Update(string? name, string? description)
    {
        Name = name ?? Name;
        Description = description ?? Description;
        UpdatedAt = DateTimeOffset.Now;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new EntityValidationException("Permission name is required");
    }
}
