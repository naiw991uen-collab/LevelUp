using LevelUp.BLL.Services.Devices.Enum;
using LevelUp.BLL.Services.Drinks.ViewModel;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Devices.ViewModel;

public class DeviceIndexViewModel
{
    public IReadOnlyList<DeviceCardViewModel> AllCards { get; set; } = [];
    public DeviceFilter Filter { get; set; }
    public decimal SingleRate { get; set; }
    public decimal MultiRate { get; set; }
    public IReadOnlyList<DrinkViewModel> AvailableDrinks { get; set; } = [];

    public IEnumerable<DeviceCardViewModel> Cards => AllCards.Where(c => Matches(c, Filter));

    public int Count(DeviceFilter filter) => AllCards.Count(c => Matches(c, filter));
    public int StoppedCount => AllCards.Count(c => c.IsStopped);

    public decimal RateOf(GameMode mode) => mode == GameMode.Multi ? MultiRate : SingleRate;

    private static bool Matches(DeviceCardViewModel card, DeviceFilter filter) => filter switch
    {
        DeviceFilter.Running => !card.IsIdle,
        DeviceFilter.Idle => card.IsIdle,
        DeviceFilter.Single => card.CurrentMode == GameMode.Single,
        DeviceFilter.Multi => card.CurrentMode == GameMode.Multi,
        _ => true
    };
}
