using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Devices.Enum;
using LevelUp.BLL.Services.Devices.Interfaces;
using LevelUp.BLL.Services.Devices.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

[Authorize]
public class DeviceController : BaseController
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DeviceFilter filter = DeviceFilter.All)
    {
        var model = await _deviceService.GetIndexAsync(filter);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Start(StartSessionViewModel model, DeviceFilter filter) =>
        RunAsync(() => _deviceService.StartAsync(model), filter);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> SwitchMode(SwitchModeViewModel model, DeviceFilter filter) =>
        RunAsync(() => _deviceService.SwitchModeAsync(model), filter);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> AddDrink(RoomDrinkViewModel model, DeviceFilter filter) =>
        RunAsync(() => _deviceService.AddDrinkAsync(model), filter);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> RemoveDrink(RoomDrinkViewModel model, DeviceFilter filter) =>
        RunAsync(() => _deviceService.RemoveDrinkAsync(model), filter);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Stop(int roomId, DeviceFilter filter) =>
        RunAsync(() => _deviceService.StopAsync(roomId), filter);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Resume(int roomId, DeviceFilter filter) =>
        RunAsync(() => _deviceService.ResumeAsync(roomId), filter);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Close(int roomId, DeviceFilter filter) =>
        RunAsync(() => _deviceService.CloseAsync(roomId), filter);

    private async Task<IActionResult> RunAsync(Func<Task<Result<string>>> action, DeviceFilter filter)
    {
        if (ModelState.IsValid)
            SetMessage(await action());
        else
            TempData["ErrorMessage"] = FirstError();

        return filter == DeviceFilter.All
            ? RedirectToAction(nameof(Index))
            : RedirectToAction(nameof(Index), new { filter });
    }
}
