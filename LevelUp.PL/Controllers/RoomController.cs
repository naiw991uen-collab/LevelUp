using LevelUp.BLL.Services.Rooms.Interfaces;
using LevelUp.BLL.Services.Rooms.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

[Authorize]
public class RoomController : BaseController
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? editId)
    {
        var model = await _roomService.GetIndexAsync(editId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CreateRoomViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var page = await _roomService.GetIndexAsync();
            page.NewRoom = model;
            return View(nameof(Index), page);
        }

        SetMessage(await _roomService.AddAsync(model));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateName(UpdateRoomNameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = FirstError();
            return RedirectToAction(nameof(Index), new { editId = model.Id });
        }

        var result = await _roomService.UpdateNameAsync(model);
        SetMessage(result);

        return result.Success
            ? RedirectToAction(nameof(Index))
            : RedirectToAction(nameof(Index), new { editId = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDefaultMode(UpdateRoomDefaultModeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = FirstError();
            return RedirectToAction(nameof(Index));
        }

        SetMessage(await _roomService.UpdateDefaultModeAsync(model));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        SetMessage(await _roomService.ToggleActiveAsync(id));
        return RedirectToAction(nameof(Index));
    }
}
