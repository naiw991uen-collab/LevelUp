namespace LevelUp.BLL.Services.Devices.ViewModel;

public class DeviceDrinkViewModel
{
    public int DrinkId { get; set; }
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal Total => Quantity * UnitPrice;
}
