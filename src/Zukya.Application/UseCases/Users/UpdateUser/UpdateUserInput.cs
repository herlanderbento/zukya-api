using FluentValidation;
using MediatR;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Application.UseCases.Users.UpdateUser;

public class UpdateUserInput : IRequest<UserOutput>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? TaxId { get; set; }

    public UpdateUserInput(Guid id,
        string? name,
        string? phone,
        string? taxId)
    {
        Id = id;
        Name = name;
        Phone = phone;
        TaxId = taxId;
    }
}

public class UpdateUserInputValidator : AbstractValidator<UpdateUserInput>
{
    public UpdateUserInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID is required.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}
