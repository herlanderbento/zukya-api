namespace Zukya.Application.Common.Exceptions;

public class WrongCredentialsException(string? message) : ApplicationException(message)
{
    public static void ThrowIfNull(object? @object, string exceptionMessage)
    {
        if (@object is null)
            throw new WrongCredentialsException(exceptionMessage);
    }

    public static void ThrowIfFalse(bool condition, string exceptionMessage)
    {
        if (!condition)
            throw new WrongCredentialsException(exceptionMessage);
    }
}
