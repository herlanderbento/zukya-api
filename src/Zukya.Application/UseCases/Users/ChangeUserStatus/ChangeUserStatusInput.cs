using FluentValidation;
using MediatR;
using Zukya.Domain.Users.Enums;

namespace Zukya.Application.UseCases.Users.ChangeUserStatus;

public class ChangeUserStatusInput : IRequest
{
    public Guid Id { get; set; }
    public UserStatus Status { get; set; }
}

public class ChangeUserStatusInputValidator : AbstractValidator<ChangeUserStatusInput>
{
    public ChangeUserStatusInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status value provided.");
    }
}
