namespace Zukya.Application.Common.Exceptions;

public class UnauthorizedException(string? message) : ApplicationException(message)
{
    public static void Throw(string exceptionMessage)
    {
        throw new UnauthorizedException(exceptionMessage);
    }

    public static void ThrowIfFalse(bool condition, string exceptionMessage)
    {
        if (!condition)
            throw new UnauthorizedException(exceptionMessage);
    }
}