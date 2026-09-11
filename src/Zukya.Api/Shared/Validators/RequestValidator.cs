using FluentValidation;
using FluentValidation.Results;
using Zukya.Domain.Shared.Exceptions;

namespace Zukya.Api.Shared.Validators;

public class RequestValidator(IServiceProvider serviceProvider)
{
    public void Validate<T>(T request, CancellationToken cancellationToken)
    {
        IValidator<T> validator = serviceProvider.GetRequiredService<IValidator<T>>();
        ValidationResult? validationResult = validator.ValidateAsync(request, cancellationToken).Result;

        if (validationResult.IsValid) return;
        var errorMessages = validationResult.Errors
            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            .ToList();

        throw new EntityValidationException(string.Join(" | ", errorMessages));
    }
}
