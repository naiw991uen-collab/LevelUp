using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Dashboard.ViewModel;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetAsync(GameMode mode);
    Task<Result<string>> UpdateRateAsync(UpdateRateViewModel model);
}
