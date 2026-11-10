namespace LevelUp.BLL.Services.Drinks.ViewModel;

public class DrinkIndexViewModel
{
    public IReadOnlyList<DrinkViewModel> Drinks { get; set; } = [];
    public int? EditId { get; set; }
    public CreateDrinkViewModel NewDrink { get; set; } = new();

    public int TotalCount => Drinks.Count;
    public int AvailableCount => Drinks.Count(d => d.IsAvailable);
}
