using LevelUp.BLL.Services.Rooms.ViewModel;

namespace LevelUp.BLL.Services.Sessions.ViewModel;

public class SessionIndexViewModel
{
    public SessionFilterViewModel Filter { get; set; } = new();
    public DateOnly Day { get; set; }
    public IReadOnlyList<FinishedSessionViewModel> Rows { get; set; } = [];
    public IReadOnlyList<RoomViewModel> Rooms { get; set; } = [];

    public int Count => Rows.Count;
    public int TotalSeconds => Rows.Sum(r => r.PlayedSeconds);
    public decimal PlayTotal => Rows.Sum(r => r.PlayCost);
    public decimal DrinksTotal => Rows.Sum(r => r.DrinksTotal);
    public decimal Total => PlayTotal + DrinksTotal;
}
