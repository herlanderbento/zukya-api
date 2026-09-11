using Zukya.Domain.Users.Entities;

namespace Zukya.Application.UseCases.Users.Common;

public record UserOutput(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? Avatar,
    string? Status,
    string? TaxId,
    decimal? ReputationScore,
    string? WarehouseCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? DeletedAt)
{
    public Guid Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public string? Email { get; set; } = Email;
    public string? Phone { get; set; } = Phone;
    public string? Avatar { get; set; } = Avatar;
    public string? Status { get; set; } = Status;
    public string? TaxId { get; set; } = TaxId;
    public decimal? ReputationScore { get; set; } = ReputationScore;
    public string? WarehouseCode { get; set; } = WarehouseCode;
    public DateTimeOffset CreatedAt { get; set; } = CreatedAt;
    public DateTimeOffset UpdatedAt { get; set; } = UpdatedAt;
    public DateTimeOffset? DeletedAt { get; set; } = DeletedAt;

    public static UserOutput ToOutput(User user) =>
        new(
            user.Id,
            user.Name,
            user.Email,
            user.Phone,
            user.Avatar?.Path,
            user.Status.ToString(),
            user.TaxId,
            user.ReputationScore,
            user.WarehouseCode,
            user.CreatedAt,
            user.UpdatedAt,
            user.DeletedAt
        );
}
