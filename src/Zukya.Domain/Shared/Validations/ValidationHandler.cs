namespace Zukya.Domain.Shared.Validations;

public abstract class ValidationHandler
{
    public abstract void HandleError(ValidationError error);

    public void HandleError(string message) => HandleError(new ValidationError(message));
}
