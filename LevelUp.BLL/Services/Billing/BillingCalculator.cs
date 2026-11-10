namespace LevelUp.BLL.Services.Billing;

public static class BillingCalculator
{
    public const int BusinessDayStartHour = 6;

    public static int BilledMinutes(TimeSpan duration) =>
        duration <= TimeSpan.Zero ? 0 : (int)Math.Ceiling(duration.TotalSeconds / 60);

    public static decimal SegmentCost(TimeSpan duration, decimal hourlyRate) =>
        Math.Round(BilledMinutes(duration) * hourlyRate / 60, MidpointRounding.AwayFromZero);

    public static DateOnly BusinessDay(DateTime time) =>
        DateOnly.FromDateTime(time.AddHours(-BusinessDayStartHour));
}
