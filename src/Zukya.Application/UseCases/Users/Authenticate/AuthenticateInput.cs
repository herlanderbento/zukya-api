using FluentValidation;
using MediatR;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Application.UseCases.Users.Authenticate;

public class AuthenticateInput(string login, string password) : IRequest<UserTokenOutput>
{
    public string Login { get; set; } = login;
    public string Password { get; set; } = password;
}

public class AuthenticateInputValidator : AbstractValidator<AuthenticateInput>
{
    public AuthenticateInputValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Login is required")
            .MinimumLength(3)
            .WithMessage("Login must be at least 3 characters long.")
            .MaximumLength(255)
            .WithMessage("Login must not exceed 255 characters.");


        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 3 characters long.")
            .MaximumLength(20)
            .WithMessage("Password must not exceed 255 characters.");
    }
}
