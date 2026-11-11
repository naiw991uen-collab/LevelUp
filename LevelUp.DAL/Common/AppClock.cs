namespace LevelUp.DAL.Common;

public static class AppClock
{
    private static readonly TimeZoneInfo Egypt = FindEgypt();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Egypt);

    private static TimeZoneInfo FindEgypt()
    {
        foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone))
                return zone;
        }

        return TimeZoneInfo.Local;
    }
}
