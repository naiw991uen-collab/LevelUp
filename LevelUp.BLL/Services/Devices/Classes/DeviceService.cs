using AutoMapper;
using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Billing;
using LevelUp.BLL.Services.Devices.Enum;
using LevelUp.BLL.Services.Devices.Interfaces;
using LevelUp.BLL.Services.Devices.ViewModel;
using LevelUp.BLL.Services.Drinks.ViewModel;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LevelUp.BLL.Services.Devices.Classes;

public class DeviceService : IDeviceService
{
    private const string RoomNotFoundError = "الغرفة مش موجودة";
    private const string DrinkNotFoundError = "المشروب مش موجود";
    private const string GeneralError = "حصلت مشكلة، جرّب تاني";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeviceService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    private IRepository<Room> Rooms => _unitOfWork.GetRepository<Room>();
    private IRepository<Drink> Drinks => _unitOfWork.GetRepository<Drink>();
    private IGameSessionRepository Sessions => _unitOfWork.GameSessionRepository;
    private ISessionDrinkRepository RoomDrinks => _unitOfWork.SessionDrinkRepository;

    public async Task<DeviceIndexViewModel> GetIndexAsync(DeviceFilter filter = DeviceFilter.All)
    {
        var now = DateTime.Now;
        var rooms = (await Rooms.GetAllAsync(r => r.IsActive)).OrderBy(r => r.Id).ToList();
        var openSessions = (await Sessions.GetAllOpenAsync()).ToDictionary(s => s.RoomId);
        var waitingDrinks = (await RoomDrinks.GetAllWithoutSessionAsync()).ToLookup(d => d.RoomId);
        var availableDrinks = (await Drinks.GetAllAsync(d => d.IsAvailable)).OrderBy(d => d.Id).ToList();

        return new DeviceIndexViewModel
        {
            AllCards = rooms
                .Select(r => BuildCard(r, openSessions.GetValueOrDefault(r.Id), waitingDrinks[r.Id], now))
                .ToList(),
            Filter = filter,
            SingleRate = (await _unitOfWork.ModeRateRepository.GetCurrentAsync(GameMode.Single))?.HourlyRate ?? 0,
            MultiRate = (await _unitOfWork.ModeRateRepository.GetCurrentAsync(GameMode.Multi))?.HourlyRate ?? 0,
            AvailableDrinks = _mapper.Map<List<DrinkViewModel>>(availableDrinks)
        };
    }

    public Task<int> GetRunningCountAsync() =>
        Sessions.CountAsync(s => s.Status != SessionStatus.Closed);

    public Task<Result<string>> StartAsync(StartSessionViewModel model) =>
        SafelyAsync("Start", model.RoomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(model.RoomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            if (!room.IsActive)
                return Reject(room.Id, "room is disabled", $"{room.Name} متعطلة");

            if (await Sessions.AnyAsync(s => s.RoomId == room.Id && s.Status != SessionStatus.Closed))
                return Reject(room.Id, "room already has an open session", $"{room.Name} فيها جلسة أصلاً");

            var rate = await _unitOfWork.ModeRateRepository.GetCurrentAsync(model.Mode);
            if (rate is null)
                return Reject(room.Id, $"no rate for {model.Mode}", $"مفيش سعر متسجل لـ{TextHelpers.ModeText(model.Mode)}");

            var now = DateTime.Now;
            var session = new GameSession
            {
                RoomId = room.Id,
                StartedAt = now,
                StartMode = model.Mode,
                Status = SessionStatus.Running,
                BusinessDay = BillingCalculator.BusinessDay(now)
            };
            session.Segments.Add(new SessionSegment { Mode = model.Mode, HourlyRate = rate.HourlyRate, StartedAt = now });

            foreach (var drink in await RoomDrinks.GetWithoutSessionByRoomIdAsync(room.Id, tracking: true))
                session.SessionDrinks.Add(drink);

            await Sessions.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} started session {SessionId} ({Mode} at {Rate})",
                room.Id, session.Id, model.Mode, rate.HourlyRate);
            return Result<string>.OK($"بدأت {room.Name} {TextHelpers.ModeText(model.Mode)}");
        }, conflictError: "الغرفة فيها جلسة أصلاً");

    public Task<Result<string>> SwitchModeAsync(SwitchModeViewModel model) =>
        SafelyAsync("SwitchMode", model.RoomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(model.RoomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            var session = await Sessions.GetOpenByRoomIdAsync(room.Id, tracking: true);
            var current = session?.Segments.FirstOrDefault(s => s.EndedAt is null);
            if (session?.Status != SessionStatus.Running || current is null)
                return Reject(room.Id, "room is not running", $"{room.Name} مش شغالة");

            if (current.Mode == model.Mode)
                return Reject(room.Id, $"already {model.Mode}", $"{room.Name} شغالة {TextHelpers.ModeText(model.Mode)} أصلاً");

            var rate = await _unitOfWork.ModeRateRepository.GetCurrentAsync(model.Mode);
            if (rate is null)
                return Reject(room.Id, $"no rate for {model.Mode}", $"مفيش سعر متسجل لـ{TextHelpers.ModeText(model.Mode)}");

            var now = DateTime.Now;
            current.EndedAt = now;
            session.Segments.Add(new SessionSegment { Mode = model.Mode, HourlyRate = rate.HourlyRate, StartedAt = now });
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} session {SessionId} switched from {OldMode} to {NewMode} at {Rate}",
                room.Id, session.Id, current.Mode, model.Mode, rate.HourlyRate);
            return Result<string>.OK($"{room.Name} اتحولت لـ{TextHelpers.ModeText(model.Mode)}");
        });

    public Task<Result<string>> AddDrinkAsync(RoomDrinkViewModel model) =>
        SafelyAsync("AddDrink", model.RoomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(model.RoomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            if (!room.IsActive)
                return Reject(room.Id, "room is disabled", $"{room.Name} متعطلة");

            var drink = await Drinks.GetByIdAsync(model.DrinkId);
            if (drink is null)
                return Result<string>.NotFound(DrinkNotFoundError);

            if (!drink.IsAvailable)
                return Reject(room.Id, $"drink {drink.Id} is unavailable", $"{drink.Name} مش موجود حالياً");

            var session = await Sessions.GetOpenByRoomIdAsync(room.Id, tracking: true);
            if (session?.Status == SessionStatus.Stopped)
                return Reject(room.Id, "room is waiting for payment", $"{room.Name} مستنية الحساب");

            var rows = session is not null
                ? session.SessionDrinks
                : await RoomDrinks.GetWithoutSessionByRoomIdAsync(room.Id, tracking: true);

            var row = rows.FirstOrDefault(d => d.DrinkId == drink.Id && d.UnitPrice == drink.Price);
            if (row is not null)
                row.Quantity++;
            else
                await RoomDrinks.AddAsync(new SessionDrink
                {
                    GameSessionId = session?.Id,
                    RoomId = room.Id,
                    DrinkId = drink.Id,
                    Quantity = 1,
                    UnitPrice = drink.Price
                });

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Drink {DrinkId} added to room {RoomId} (session {SessionId})",
                drink.Id, room.Id, session?.Id);
            return Result<string>.OK($"اتضاف {drink.Name} لـ{room.Name}");
        });

    public Task<Result<string>> RemoveDrinkAsync(RoomDrinkViewModel model) =>
        SafelyAsync("RemoveDrink", model.RoomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(model.RoomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            var session = await Sessions.GetOpenByRoomIdAsync(room.Id, tracking: true);
            if (session?.Status == SessionStatus.Stopped)
                return Reject(room.Id, "room is waiting for payment", $"{room.Name} مستنية الحساب");

            var rows = session is not null
                ? session.SessionDrinks
                : await RoomDrinks.GetWithoutSessionByRoomIdAsync(room.Id, tracking: true);

            var row = rows
                .Where(d => d.DrinkId == model.DrinkId)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefault();

            if (row is null)
            {
                var drink = await Drinks.GetByIdAsync(model.DrinkId);
                return Reject(room.Id, $"drink {model.DrinkId} is not on the room",
                    drink is null ? DrinkNotFoundError : $"{drink.Name} مش على {room.Name}");
            }

            if (row.Quantity > 1)
                row.Quantity--;
            else
                await RoomDrinks.DeleteAsync(row);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Drink {DrinkId} removed from room {RoomId} (session {SessionId})",
                model.DrinkId, room.Id, session?.Id);
            return Result<string>.OK($"اتشال {row.Drink.Name} من {room.Name}");
        });

    public Task<Result<string>> StopAsync(int roomId) =>
        SafelyAsync("Stop", roomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(roomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            var session = await Sessions.GetOpenByRoomIdAsync(room.Id, tracking: true);
            var current = session?.Segments.FirstOrDefault(s => s.EndedAt is null);
            if (session?.Status != SessionStatus.Running || current is null)
                return Reject(room.Id, "room is not running", $"{room.Name} مش شغالة");

            var now = DateTime.Now;
            current.EndedAt = now;
            session.Status = SessionStatus.Stopped;
            await _unitOfWork.SaveChangesAsync();

            var total = BuildCard(room, session, [], now).Total;
            _logger.LogInformation("Room {RoomId} session {SessionId} stopped, total {Total}", room.Id, session.Id, total);
            return Result<string>.OK($"{room.Name} وقفت · الحساب {TextHelpers.MoneyText(total)} ج.م");
        });

    public Task<Result<string>> ResumeAsync(int roomId) =>
        SafelyAsync("Resume", roomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(roomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            var session = await Sessions.GetOpenByRoomIdAsync(room.Id, tracking: true);
            if (session?.Status != SessionStatus.Stopped)
                return Reject(room.Id, "room is not stopped", $"{room.Name} مش واقفة");

            var mode = session.Segments.OrderByDescending(s => s.StartedAt).Select(s => (GameMode?)s.Mode).FirstOrDefault()
                       ?? session.StartMode;

            var rate = await _unitOfWork.ModeRateRepository.GetCurrentAsync(mode);
            if (rate is null)
                return Reject(room.Id, $"no rate for {mode}", $"مفيش سعر متسجل لـ{TextHelpers.ModeText(mode)}");

            session.Segments.Add(new SessionSegment { Mode = mode, HourlyRate = rate.HourlyRate, StartedAt = DateTime.Now });
            session.Status = SessionStatus.Running;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} session {SessionId} resumed ({Mode} at {Rate})",
                room.Id, session.Id, mode, rate.HourlyRate);
            return Result<string>.OK($"{room.Name} رجعت تلعب {TextHelpers.ModeText(mode)}");
        });

    public Task<Result<string>> CloseAsync(int roomId) =>
        SafelyAsync("Close", roomId, async () =>
        {
            var room = await Rooms.GetByIdAsync(roomId);
            if (room is null)
                return Result<string>.NotFound(RoomNotFoundError);

            var session = await Sessions.GetOpenByRoomIdAsync(room.Id, tracking: true);
            if (session is null)
                return Reject(room.Id, "room has no open session", $"{room.Name} مفيهاش جلسة");

            if (session.Status != SessionStatus.Stopped)
                return Reject(room.Id, "session is still running", $"{room.Name} لازم توقف الأول");

            var now = DateTime.Now;
            var total = BuildCard(room, session, [], now).Total;

            session.Status = SessionStatus.Closed;
            session.EndedAt = session.Segments.Max(s => s.EndedAt) ?? now;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} session {SessionId} closed, paid {Total}", room.Id, session.Id, total);
            return Result<string>.OK($"اتقفلت جلسة {room.Name} · اتحصّل {TextHelpers.MoneyText(total)} ج.م");
        });

    private DeviceCardViewModel BuildCard(Room room, GameSession? session, IEnumerable<SessionDrink> waitingDrinks, DateTime now)
    {
        var card = new DeviceCardViewModel
        {
            RoomId = room.Id,
            RoomName = room.Name,
            DefaultMode = room.DefaultMode
        };

        if (session is null)
        {
            card.Drinks = _mapper.Map<List<DeviceDrinkViewModel>>(waitingDrinks.OrderBy(d => d.CreatedAt));
            return card;
        }

        var segments = session.Segments.OrderBy(s => s.StartedAt).ToList();
        var last = segments.LastOrDefault();

        card.Status = session.Status;
        card.CurrentMode = last?.Mode ?? session.StartMode;
        card.CurrentRate = last?.HourlyRate ?? 0;
        card.StartedAt = session.StartedAt;
        card.StoppedAt = session.Status == SessionStatus.Stopped ? last?.EndedAt : null;

        card.Segments = segments.Select(s =>
        {
            var duration = (s.EndedAt ?? now) - s.StartedAt;
            return new DeviceSegmentViewModel
            {
                Mode = s.Mode,
                HourlyRate = s.HourlyRate,
                Seconds = Math.Max(0, (int)duration.TotalSeconds),
                Cost = BillingCalculator.SegmentCost(duration, s.HourlyRate),
                IsCurrent = s.EndedAt is null
            };
        }).ToList();

        card.Drinks = _mapper.Map<List<DeviceDrinkViewModel>>(session.SessionDrinks.OrderBy(d => d.CreatedAt));
        return card;
    }

    private Result<string> Reject(int roomId, string reason, string error)
    {
        _logger.LogWarning("Device action on room {RoomId} rejected: {Reason}", roomId, reason);
        return Result<string>.Fail(error);
    }

    private async Task<Result<string>> SafelyAsync(string action, int roomId, Func<Task<Result<string>>> work,
                                                   string conflictError = GeneralError)
    {
        try
        {
            return await work();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "{Action} on room {RoomId} rejected by the database", action, roomId);
            return Result<string>.Fail(conflictError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Action} on room {RoomId} failed", action, roomId);
            return Result<string>.Fail(GeneralError);
        }
    }
}
