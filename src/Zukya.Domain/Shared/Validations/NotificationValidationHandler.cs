namespace Zukya.Domain.Shared.Validations;

public class NotificationValidationHandler : ValidationHandler
{
    private readonly List<ValidationError> _errors = new();

    public IReadOnlyCollection<ValidationError> Errors => _errors.AsReadOnly();

    public bool HasErrors() => _errors.Count > 0;

    public override void HandleError(ValidationError error) => _errors.Add(error);
}
