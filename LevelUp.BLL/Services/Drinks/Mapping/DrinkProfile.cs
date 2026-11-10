using AutoMapper;
using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Drinks.ViewModel;
using LevelUp.DAL.Models;

namespace LevelUp.BLL.Services.Drinks.Mapping;

public class DrinkProfile : Profile
{
    public DrinkProfile()
    {
        // GetIndexAsync
        CreateMap<Drink, DrinkViewModel>();

        // AddAsync
        CreateMap<CreateDrinkViewModel, Drink>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => TextHelpers.NormalizeName(src.Name)))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price!.Value))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.SessionDrinks, opt => opt.Ignore());
    }
}
