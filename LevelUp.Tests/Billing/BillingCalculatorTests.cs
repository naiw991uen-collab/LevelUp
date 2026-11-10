using LevelUp.BLL.Services.Billing;

namespace LevelUp.Tests.Billing;

public class BillingCalculatorTests
{
    [Theory]
    [InlineData(-30, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(59, 1)]
    [InlineData(60, 1)]
    [InlineData(61, 2)]
    [InlineData(3600, 60)]
    [InlineData(3601, 61)]
    public void BilledMinutes_CountsEveryStartedMinute(int seconds, int expected)
    {
        Assert.Equal(expected, BillingCalculator.BilledMinutes(TimeSpan.FromSeconds(seconds)));
    }

    [Fact]
    public void BilledMinutes_FractionOfSecondStartsAMinute()
    {
        Assert.Equal(2, BillingCalculator.BilledMinutes(TimeSpan.FromSeconds(60.4)));
    }

    [Theory]
    [InlineData(0, 100, 0)]
    [InlineData(-10, 100, 0)]
    [InlineData(61, 100, 3)]
    [InlineData(3600, 150, 150)]
    [InlineData(540, 100, 15)]
    [InlineData(180, 50, 3)]
    [InlineData(60, 100, 2)]
    [InlineData(5400, 100, 150)]
    public void SegmentCost_RoundsPerStartedMinute(int seconds, int hourlyRate, int expected)
    {
        Assert.Equal(expected, BillingCalculator.SegmentCost(TimeSpan.FromSeconds(seconds), hourlyRate));
    }

    [Theory]
    [InlineData("2026-11-10 02:00", "2026-11-09")]
    [InlineData("2026-11-10 05:59", "2026-11-09")]
    [InlineData("2026-11-10 06:00", "2026-11-10")]
    [InlineData("2026-11-10 23:59", "2026-11-10")]
    [InlineData("2026-11-10 00:00", "2026-11-09")]
    [InlineData("2026-01-01 03:00", "2025-12-31")]
    public void BusinessDay_StartsAtSixInTheMorning(string time, string expected)
    {
        Assert.Equal(DateOnly.Parse(expected), BillingCalculator.BusinessDay(DateTime.Parse(time)));
    }
}
