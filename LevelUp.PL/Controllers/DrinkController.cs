using LevelUp.BLL.Services.Drinks.Interfaces;
using LevelUp.BLL.Services.Drinks.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

[Authorize]
public class DrinkController : BaseController
{
    private readonly IDrinkService _drinkService;

    public DrinkController(IDrinkService drinkService)
    {
        _drinkService = drinkService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? editId)
    {
        var model = await _drinkService.GetIndexAsync(editId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CreateDrinkViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var page = await _drinkService.GetIndexAsync();
            page.NewDrink = model;
            return View(nameof(Index), page);
        }

        SetMessage(await _drinkService.AddAsync(model));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrice(UpdateDrinkPriceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = FirstError();
            return RedirectToAction(nameof(Index), new { editId = model.Id });
        }

        var result = await _drinkService.UpdatePriceAsync(model);
        SetMessage(result);

        return result.Success
            ? RedirectToAction(nameof(Index))
            : RedirectToAction(nameof(Index), new { editId = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAvailable(int id)
    {
        SetMessage(await _drinkService.ToggleAvailableAsync(id));
        return RedirectToAction(nameof(Index));
    }
}
