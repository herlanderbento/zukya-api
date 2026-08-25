namespace Zukya.Application.Common.Exceptions;

public class ConflictException(string? message) : ApplicationException(message)
{
    public static void ThrowIfNotNull(object? @object, string exceptionMessage)
    {
        if (@object != null)
            throw new ConflictException(exceptionMessage);
    }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
            throw new ConflictException(exceptionMessage);
    }
}