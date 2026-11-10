using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Account.ViewModel;

namespace LevelUp.BLL.Services.Account.Interfaces;

public interface IAccountService
{
    Task<Result> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
}
