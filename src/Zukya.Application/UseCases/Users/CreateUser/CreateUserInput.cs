using FluentValidation;
using MediatR;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.Users.Enums;

namespace Zukya.Application.UseCases.Users.CreateUser;

public class CreateUserInput(
    string? email,
    string? phone,
    string? taxId)
    : IRequest<UserTokenOutput>
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; } = email;
    public string? Phone { get; set; } = phone;
    public UserRole Role { get; set; }
    public string? TaxId { get; set; } = taxId;
    public string Password { get; set; } = string.Empty;
}

public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(255)
            .WithMessage("Name must not exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(20)
            .WithMessage("Password must not exceed 20 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Either Email or Phone must be provided.")
            .OverridePropertyName("Email");

        RuleFor(x => x.TaxId)
            .MaximumLength(255)
            .WithMessage("TaxId must not exceed 255 characters.")
            .When(x => !string.IsNullOrEmpty(x.TaxId));

        When(
            x => x.Role == UserRole.Seller,
            () =>
            {
                RuleFor(x => x.TaxId)
                    .NotEmpty()
                    .WithMessage("TaxId is required for Seller.");

                RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required for Seller.");
            }
        );
    }
}
