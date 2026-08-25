namespace Zukya.Application.Common.Exceptions;

public class ForbiddenException(string? message) : ApplicationException(message)
{
    public static void Throw(string exceptionMessage)
    {
        throw new ForbiddenException(exceptionMessage);
    }

    public static void ThrowIfFalse(bool condition, string exceptionMessage)
    {
        if (!condition)
            throw new ForbiddenException(exceptionMessage);
    }
}
