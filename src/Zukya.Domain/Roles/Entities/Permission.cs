using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Shared.Exceptions;

namespace Zukya.Domain.Roles.Entities;

public class Permission : AggregateRoot
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string Category { get; private set; }
    public string? Description { get; private set; }

    public string BackendEndpoints { get; private set; }
    public string FrontendRoutes { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles;

    public Permission(
        string name,
        string slug,
        string category,
        string? description = null,
        string backendEndpoints = "[]",
        string frontendRoutes = "[]")
    {
        Name = name;
        Slug = slug;
        Category = category;
        Description = description;
        BackendEndpoints = backendEndpoints;
        FrontendRoutes = frontendRoutes;
        CreatedAt = DateTimeOffset.Now;
        UpdatedAt = DateTimeOffset.Now;

        Validate();
    }

    public void Update(
        string? name,
        string? slug,
        string? category,
        string? description,
        string? backendEndpoints,
        string? frontendRoutes)
    {
        Name = name ?? Name;
        Slug = slug ?? Slug;
        Category = category ?? Category;
        Description = description ?? Description;
        BackendEndpoints = backendEndpoints ?? BackendEndpoints;
        FrontendRoutes = frontendRoutes ?? FrontendRoutes;
        UpdatedAt = DateTimeOffset.Now;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new EntityValidationException("Permission name is required");
        if (string.IsNullOrWhiteSpace(Slug)) throw new EntityValidationException("Permission slug is required");
        if (string.IsNullOrWhiteSpace(Category)) throw new EntityValidationException("Permission category is required");
    }
}
