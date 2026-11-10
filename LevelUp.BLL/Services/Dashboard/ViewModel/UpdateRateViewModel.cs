using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Dashboard.ViewModel;

public class UpdateRateViewModel
{
    public GameMode Mode { get; set; }
    public decimal? HourlyRate { get; set; }
}
