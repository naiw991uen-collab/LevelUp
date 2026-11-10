using AutoMapper;
using LevelUp.BLL.Services.Devices.ViewModel;
using LevelUp.DAL.Models;

namespace LevelUp.BLL.Services.Devices.Mapping;

public class DeviceProfile : Profile
{
    public DeviceProfile()
    {
        // BuildCard
        CreateMap<SessionDrink, DeviceDrinkViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Drink.Name));
    }
}
