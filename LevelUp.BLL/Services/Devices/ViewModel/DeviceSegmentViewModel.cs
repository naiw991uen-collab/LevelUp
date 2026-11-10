using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Devices.ViewModel;

public class DeviceSegmentViewModel
{
    public GameMode Mode { get; set; }
    public decimal HourlyRate { get; set; }
    public int Seconds { get; set; }
    public decimal Cost { get; set; }
    public bool IsCurrent { get; set; }
}
