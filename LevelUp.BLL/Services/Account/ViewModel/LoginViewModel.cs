namespace LevelUp.BLL.Services.Account.ViewModel;

public class LoginViewModel
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; } = true;
}
