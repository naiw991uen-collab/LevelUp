using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Devices.Enum;
using LevelUp.BLL.Services.Devices.ViewModel;

namespace LevelUp.BLL.Services.Devices.Interfaces;

public interface IDeviceService
{
    Task<DeviceIndexViewModel> GetIndexAsync(DeviceFilter filter = DeviceFilter.All);
    Task<int> GetRunningCountAsync();

    Task<Result<string>> StartAsync(StartSessionViewModel model);
    Task<Result<string>> SwitchModeAsync(SwitchModeViewModel model);
    Task<Result<string>> AddDrinkAsync(RoomDrinkViewModel model);
    Task<Result<string>> RemoveDrinkAsync(RoomDrinkViewModel model);
    Task<Result<string>> StopAsync(int roomId);
    Task<Result<string>> ResumeAsync(int roomId);
    Task<Result<string>> CloseAsync(int roomId);
}
