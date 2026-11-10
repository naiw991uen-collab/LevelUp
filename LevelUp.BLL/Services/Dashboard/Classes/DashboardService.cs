using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Billing;
using LevelUp.BLL.Services.Dashboard.Interfaces;
using LevelUp.BLL.Services.Dashboard.ViewModel;
using LevelUp.BLL.Services.Devices.ViewModel;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LevelUp.BLL.Services.Dashboard.Classes;

public class DashboardService : IDashboardService
{
    private const string GeneralError = "حصلت مشكلة، جرّب تاني";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardViewModel> GetAsync(GameMode mode)
    {
        var now = DateTime.Now;
        var day = BillingCalculator.BusinessDay(now);

        var rooms = (await _unitOfWork.GetRepository<Room>().GetAllAsync()).OrderBy(r => r.Id).ToList();
        var openSessions = (await _unitOfWork.GameSessionRepository.GetAllOpenAsync()).ToList();
        var closedToday = await _unitOfWork.GameSessionRepository.GetClosedAsync(fromDay: day, toDay: day);

        var todaySessions = closedToday
            .Concat(openSessions.Where(s => s.BusinessDay == day))
            .ToList();

        var withMode = todaySessions.Where(s => s.Segments.Any(seg => seg.Mode == mode)).ToList();
        var openByRoom = openSessions.ToDictionary(s => s.RoomId);

        var rows = rooms.Select(room =>
        {
            var sessions = todaySessions.Where(s => s.RoomId == room.Id).ToList();
            var segments = sessions.SelectMany(s => s.Segments.Where(seg => seg.Mode == mode)).ToList();
            var open = openByRoom.GetValueOrDefault(room.Id);

            return new DashboardRoomViewModel
            {
                RoomName = room.Name,
                IsActive = room.IsActive,
                Status = open?.Status,
                CurrentMode = open is null ? null : CurrentMode(open),
                Seconds = segments.Sum(seg => Math.Max(0, (int)Duration(seg, now).TotalSeconds)),
                PlayCost = segments.Sum(seg => BillingCalculator.SegmentCost(Duration(seg, now), seg.HourlyRate)),
                Drinks = sessions
                    .Where(s => s.StartMode == mode)
                    .SelectMany(s => s.SessionDrinks)
                    .GroupBy(d => new { d.DrinkId, d.UnitPrice })
                    .Select(g => new DeviceDrinkViewModel
                    {
                        DrinkId = g.Key.DrinkId,
                        Name = g.First().Drink.Name,
                        Quantity = g.Sum(d => d.Quantity),
                        UnitPrice = g.Key.UnitPrice
                    })
                    .ToList()
            };
        }).ToList();

        return new DashboardViewModel
        {
            Mode = mode,
            Day = day,
            CurrentRate = (await _unitOfWork.ModeRateRepository.GetCurrentAsync(mode))?.HourlyRate ?? 0,
            Rows = rows,
            ClosedCount = withMode.Count(s => s.Status == SessionStatus.Closed),
            RunningCount = withMode.Count(s => s.Status == SessionStatus.Running),
            StoppedCount = withMode.Count(s => s.Status == SessionStatus.Stopped),
            RunningNowCount = openSessions.Count(s => s.Status == SessionStatus.Running && CurrentMode(s) == mode),
            ActiveRoomCount = rooms.Count(r => r.IsActive)
        };
    }

    public async Task<Result<string>> UpdateRateAsync(UpdateRateViewModel model)
    {
        try
        {
            var newRate = model.HourlyRate!.Value;
            var modeName = $"ال{TextHelpers.ModeText(model.Mode)}";
            var current = await _unitOfWork.ModeRateRepository.GetCurrentAsync(model.Mode);

            if (current?.HourlyRate == newRate)
                return Result<string>.OK($"سعر {modeName} زي ما هو");

            await _unitOfWork.ModeRateRepository.AddAsync(new ModeRate
            {
                Mode = model.Mode,
                HourlyRate = newRate,
                EffectiveFrom = DateTime.Now
            });
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Mode} hourly rate changed from {OldRate} to {NewRate}", model.Mode, current?.HourlyRate, newRate);
            return Result<string>.OK($"اتحفظ: {TextHelpers.MoneyText(newRate)} ج.م للساعة · بيتطبق على الجلسات الجديدة");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update {Mode} hourly rate failed", model.Mode);
            return Result<string>.Fail(GeneralError);
        }
    }

    private static TimeSpan Duration(SessionSegment segment, DateTime now) =>
        (segment.EndedAt ?? now) - segment.StartedAt;

    private static GameMode CurrentMode(GameSession session) =>
        session.Segments.OrderByDescending(s => s.StartedAt).Select(s => (GameMode?)s.Mode).FirstOrDefault()
        ?? session.StartMode;
}
