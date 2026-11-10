namespace LevelUp.BLL.Services.Drinks.ViewModel;

public class DrinkViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}
