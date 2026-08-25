namespace Zukya.Application.Common.Exceptions;

public class BadRequestException(string? message) : ApplicationException(message)
{
    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
            throw new BadRequestException(exceptionMessage);
    }
}