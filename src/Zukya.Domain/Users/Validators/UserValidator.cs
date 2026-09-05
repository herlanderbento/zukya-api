using Zukya.Domain.Shared.Validations;

namespace Zukya.Domain.Users.Validators;

public static class UserValidator
{
    public static void Validate(string name,
        string? email,
        string? phone,
        string? password)
    {
        DomainValidation.NotNullOrEmpty(name, nameof(name));
        DomainValidation.MinLength(name, 6, nameof(name));
        DomainValidation.MaxLength(name, 255, nameof(name));

        if (!string.IsNullOrEmpty(email))
        {
            DomainValidation.MaxLength(name, 255, nameof(name));
            DomainValidation.Email(email, nameof(email));
        }

        if (!string.IsNullOrEmpty(phone)) DomainValidation.MaxLength(phone, 55, nameof(phone));

        if (!string.IsNullOrEmpty(password))
        {
            DomainValidation.NotNullOrEmpty(password, nameof(password));
            DomainValidation.MinLength(password, 6, nameof(password));
            DomainValidation.MaxLength(password, 255, nameof(password));
        }
    }
}
