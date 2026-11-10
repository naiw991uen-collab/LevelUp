using AutoMapper;
using LevelUp.BLL.Services.Rooms.ViewModel;
using LevelUp.DAL.Models;

namespace LevelUp.BLL.Services.Rooms.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        // GetIndexAsync
        CreateMap<Room, RoomViewModel>()
            .ForMember(dest => dest.SessionStatus, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentMode, opt => opt.Ignore());

        // AddAsync
        CreateMap<CreateRoomViewModel, Room>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.GameSessions, opt => opt.Ignore())
            .ForMember(dest => dest.SessionDrinks, opt => opt.Ignore());
    }
}
