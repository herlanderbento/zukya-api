namespace Zukya.Domain.Shared.Validations;

public abstract class Validator(ValidationHandler handler)
{
    protected readonly ValidationHandler Handler = handler;

    public abstract void Validate();
}
