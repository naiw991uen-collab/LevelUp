using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Drinks.ViewModel;

namespace LevelUp.BLL.Services.Drinks.Interfaces;

public interface IDrinkService
{
    Task<DrinkIndexViewModel> GetIndexAsync(int? editId = null);

    Task<Result<string>> AddAsync(CreateDrinkViewModel model);
    Task<Result<string>> UpdatePriceAsync(UpdateDrinkPriceViewModel model);
    Task<Result<string>> ToggleAvailableAsync(int id);
}
