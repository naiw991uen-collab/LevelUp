using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Rooms.ViewModel;

namespace LevelUp.BLL.Services.Rooms.Interfaces;

public interface IRoomService
{
    Task<RoomIndexViewModel> GetIndexAsync(int? editId = null);

    Task<Result<string>> AddAsync(CreateRoomViewModel model);
    Task<Result<string>> UpdateNameAsync(UpdateRoomNameViewModel model);
    Task<Result<string>> UpdateDefaultModeAsync(UpdateRoomDefaultModeViewModel model);
    Task<Result<string>> ToggleActiveAsync(int id);
}
