using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Dashboard.ViewModel;

public class DashboardViewModel
{
    public GameMode Mode { get; set; }
    public DateOnly Day { get; set; }
    public decimal CurrentRate { get; set; }
    public UpdateRateViewModel NewRate { get; set; } = new();

    public IReadOnlyList<DashboardRoomViewModel> Rows { get; set; } = [];

    public int ClosedCount { get; set; }
    public int RunningCount { get; set; }
    public int StoppedCount { get; set; }
    public int RunningNowCount { get; set; }
    public int ActiveRoomCount { get; set; }

    public int SessionCount => ClosedCount + RunningCount + StoppedCount;
    public int Seconds => Rows.Sum(r => r.Seconds);
    public decimal PlayTotal => Rows.Sum(r => r.PlayCost);
    public decimal DrinksTotal => Rows.Sum(r => r.DrinksTotal);
    public decimal Total => PlayTotal + DrinksTotal;
}
