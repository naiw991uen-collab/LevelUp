using LevelUp.BLL.Services.Sessions.Enum;

namespace LevelUp.BLL.Services.Sessions.ViewModel;

public class SessionFilterViewModel
{
    public DateOnly? Date { get; set; }
    public SessionModeFilter Mode { get; set; }
    public int? RoomId { get; set; }
}
