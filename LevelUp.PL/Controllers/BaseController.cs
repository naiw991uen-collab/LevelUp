using LevelUp.BLL.Common;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.PL.Controllers;

public abstract class BaseController : Controller
{
    protected void SetMessage(Result<string> result)
    {
        if (result.Success)
            TempData["SuccessMessage"] = result.Value;
        else
            TempData["ErrorMessage"] = result.Error;
    }

    protected string FirstError() =>
        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault()
        ?? "البيانات مش مظبوطة";
}
