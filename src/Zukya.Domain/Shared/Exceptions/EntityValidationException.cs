using Zukya.Domain.Shared.Validations;

namespace Zukya.Domain.Shared.Exceptions;

public class EntityValidationException(
    string? message,
    IReadOnlyCollection<ValidationError>? errors = null)
    : Exception(message)
{
    public IReadOnlyCollection<ValidationError>? Errors { get; } = errors;
}