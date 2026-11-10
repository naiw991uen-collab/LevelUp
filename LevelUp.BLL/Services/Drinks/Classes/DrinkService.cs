using AutoMapper;
using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Drinks.Interfaces;
using LevelUp.BLL.Services.Drinks.ViewModel;
using LevelUp.DAL.Models;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LevelUp.BLL.Services.Drinks.Classes;

public class DrinkService : IDrinkService
{
    private const string DuplicateNameError = "المشروب ده موجود قبل كده";
    private const string NotFoundError = "المشروب مش موجود";
    private const string GeneralError = "حصلت مشكلة، جرّب تاني";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DrinkService> _logger;

    public DrinkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DrinkService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    private IRepository<Drink> Drinks => _unitOfWork.GetRepository<Drink>();

    public async Task<DrinkIndexViewModel> GetIndexAsync(int? editId = null)
    {
        var drinks = (await Drinks.GetAllAsync()).OrderBy(d => d.Id).ToList();

        return new DrinkIndexViewModel
        {
            Drinks = _mapper.Map<List<DrinkViewModel>>(drinks),
            EditId = drinks.Any(d => d.Id == editId) ? editId : null
        };
    }

    public async Task<Result<string>> AddAsync(CreateDrinkViewModel model)
    {
        try
        {
            var drink = _mapper.Map<Drink>(model);

            if (await Drinks.AnyAsync(d => d.Name == drink.Name))
            {
                _logger.LogWarning("Add drink rejected: name {Name} already exists", drink.Name);
                return Result<string>.Fail(DuplicateNameError);
            }

            await Drinks.AddAsync(drink);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Drink {DrinkId} {Name} added at {Price}", drink.Id, drink.Name, drink.Price);
            return Result<string>.OK($"اتضاف {drink.Name} بسعر {TextHelpers.MoneyText(drink.Price)} ج.م");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Add drink rejected by the database: {Name}", model.Name);
            return Result<string>.Fail(DuplicateNameError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Add drink failed: {Name}", model.Name);
            return Result<string>.Fail(GeneralError);
        }
    }

    public async Task<Result<string>> UpdatePriceAsync(UpdateDrinkPriceViewModel model)
    {
        try
        {
            var drink = await Drinks.GetByIdAsync(model.Id);
            if (drink is null)
                return Result<string>.NotFound(NotFoundError);

            var oldPrice = drink.Price;
            var newPrice = model.Price!.Value;

            if (newPrice == oldPrice)
                return Result<string>.OK($"سعر {drink.Name} زي ما هو");

            drink.Price = newPrice;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Drink {DrinkId} price changed from {OldPrice} to {NewPrice}", drink.Id, oldPrice, newPrice);
            return Result<string>.OK($"اتعدل سعر {drink.Name} لـ {TextHelpers.MoneyText(newPrice)} ج.م (الجلسات القديمة مش بتتأثر)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update price of drink {DrinkId} failed", model.Id);
            return Result<string>.Fail(GeneralError);
        }
    }

    public async Task<Result<string>> ToggleAvailableAsync(int id)
    {
        try
        {
            var drink = await Drinks.GetByIdAsync(id);
            if (drink is null)
                return Result<string>.NotFound(NotFoundError);

            drink.IsAvailable = !drink.IsAvailable;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Drink {DrinkId} {State}", id, drink.IsAvailable ? "available" : "unavailable");
            return Result<string>.OK(drink.IsAvailable
                ? $"{drink.Name} رجع متوفر"
                : $"{drink.Name} بقى مش موجود حالياً");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Toggle drink {DrinkId} failed", id);
            return Result<string>.Fail(GeneralError);
        }
    }
}
