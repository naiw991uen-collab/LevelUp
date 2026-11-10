using LevelUp.BLL.Services.Sessions.ViewModel;

namespace LevelUp.BLL.Services.Sessions.Interfaces;

public interface ISessionService
{
    Task<SessionIndexViewModel> GetIndexAsync(SessionFilterViewModel filter);
}
