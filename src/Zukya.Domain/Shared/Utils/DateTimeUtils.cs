namespace Zukya.Domain.Shared.Utils;

public static class DateTimeUtils
{
    public static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;

        return dateTime.Kind == DateTimeKind.Local ? dateTime.ToUniversalTime() : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}