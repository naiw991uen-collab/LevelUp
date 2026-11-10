using LevelUp.BLL.Services.Devices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LevelUp.PL.Filters;

public class RunningCountFilter : IAsyncResultFilter
{
    private readonly IDeviceService _deviceService;

    public RunningCountFilter(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ViewResult view && context.HttpContext.User.Identity?.IsAuthenticated == true)
            view.ViewData["RunningCount"] = await _deviceService.GetRunningCountAsync();

        await next();
    }
}
