namespace Zukya.Application.Common.Exceptions;

public class UnauthorizedException(string? message) : ApplicationException(message)
{
    public static void ThrowIfNull(object? @object, string exceptionMessage)
    {
        if (@object is null)
            throw new UnauthorizedException(exceptionMessage);
    }

    public static void ThrowIfFalse(bool condition, string exceptionMessage)
    {
        if (!condition)
            throw new UnauthorizedException(exceptionMessage);
    }
}
