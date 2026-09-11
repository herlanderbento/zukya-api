using Zukya.Domain.Roles.Entities;
using Zukya.Domain.Shared.Entities;
using Zukya.Domain.Shared.Exceptions;
using Zukya.Domain.Shared.ValueObjects;
using Zukya.Domain.Users.Enums;
using Zukya.Domain.Users.Validators;

namespace Zukya.Domain.Users.Entities;

public class User : AggregateRoot
{
    public string Name { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Password { get; private set; }
    public int? AccessLevel { get; }
    public Image? Avatar { get; private set; }
    public UserStatus? Status { get; private set; }
    public string? TaxId { get; private set; }
    public decimal? ReputationScore { get; private set; }
    public string? WarehouseCode { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }


    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public User(
        string name,
        string? email,
        string? phone,
        string? password,
        int? accessLevel,
        string? taxId)
    {
        Name = name;
        Email = email ?? null;
        Phone = phone ?? null;
        Password = password ?? null;
        AccessLevel = accessLevel ?? 0;
        Status = UserStatus.Pending;
        TaxId = taxId ?? null;
        ReputationScore = 0;
        WarehouseCode = null;

        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        DeletedAt = null;

        Validate();
    }

    public void Update(
        string? name,
        string? email,
        string? phone,
        string? taxId)
    {
        Name = name ?? Name;
        Email = email ?? Email;
        Phone = phone ?? Phone;
        TaxId = taxId ?? TaxId;

        Touch();
        Validate();
    }

    public void ChangePassword(string password)
    {
        Password = password;

        Touch();
        Validate();
    }

    public void ChangeStatus(UserStatus status)
    {
        Status = status;
        Touch();
    }

    public void ChangeAvatar(string path)
    {
        Avatar = new Image(path);
        Touch();
    }

    public void AssignRole(Role role)
    {
        if (!IsAdmin()) throw new EntityValidationException("Only administrative users can be assigned roles.");

        if (_roles.Contains(role)) return;
        _roles.Add(role);
        Touch();
    }

    public void RemoveRole(Role role)
    {
        if (!IsAdmin()) throw new EntityValidationException("Only administrative users can manage roles.");

        if (!_roles.Contains(role)) return;

        _roles.Remove(role);
        Touch();
    }

    private bool IsAdmin() => AccessLevel == 5;

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    private void Validate() => UserValidator.Validate(Name, Email, Phone, Password);
}
