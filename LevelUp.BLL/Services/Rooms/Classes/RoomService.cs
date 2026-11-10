using AutoMapper;
using LevelUp.BLL.Common;
using LevelUp.BLL.Services.Rooms.Interfaces;
using LevelUp.BLL.Services.Rooms.ViewModel;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LevelUp.BLL.Services.Rooms.Classes;

public class RoomService : IRoomService
{
    private const string DuplicateNameError = "فيه غرفة بنفس الاسم";
    private const string GeneralError = "حصلت مشكلة، جرّب تاني";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoomService> _logger;

    public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    private IRepository<Room> Rooms => _unitOfWork.GetRepository<Room>();

    public async Task<RoomIndexViewModel> GetIndexAsync(int? editId = null)
    {
        var rooms = (await Rooms.GetAllAsync()).OrderBy(r => r.Id).ToList();
        var openSessions = (await _unitOfWork.GameSessionRepository.GetAllOpenAsync())
            .ToDictionary(s => s.RoomId);

        var rows = rooms.Select(room =>
        {
            var row = _mapper.Map<RoomViewModel>(room);

            if (openSessions.TryGetValue(room.Id, out var session))
            {
                row.SessionStatus = session.Status;
                row.CurrentMode = session.Segments
                    .OrderByDescending(s => s.StartedAt)
                    .Select(s => (GameMode?)s.Mode)
                    .FirstOrDefault() ?? session.StartMode;
            }

            return row;
        }).ToList();

        return new RoomIndexViewModel
        {
            Rooms = rows,
            EditId = rooms.Any(r => r.Id == editId) ? editId : null
        };
    }

    public async Task<Result<string>> AddAsync(CreateRoomViewModel model)
    {
        try
        {
            var name = TextHelpers.NormalizeName(model.Name);

            if (await Rooms.AnyAsync(r => r.Name == name))
            {
                _logger.LogWarning("Add room rejected: name {Name} already exists", name);
                return Result<string>.Fail(DuplicateNameError);
            }

            var room = _mapper.Map<Room>(model);
            room.Name = name;
            await Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} {Name} added ({Mode})", room.Id, room.Name, room.DefaultMode);
            return Result<string>.OK($"اتضافت {room.Name} (افتراضي {TextHelpers.ModeText(room.DefaultMode)})");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Add room rejected by the database: {Name}", model.Name);
            return Result<string>.Fail(DuplicateNameError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Add room failed: {Name}", model.Name);
            return Result<string>.Fail(GeneralError);
        }
    }

    public async Task<Result<string>> UpdateNameAsync(UpdateRoomNameViewModel model)
    {
        try
        {
            var room = await Rooms.GetByIdAsync(model.Id);
            if (room is null)
                return Result<string>.NotFound("الغرفة مش موجودة");

            var oldName = room.Name;
            var newName = TextHelpers.NormalizeName(model.Name);

            if (newName == oldName)
                return Result<string>.OK($"اسم {oldName} زي ما هو");

            if (await Rooms.AnyAsync(r => r.Name == newName && r.Id != room.Id))
            {
                _logger.LogWarning("Rename room {RoomId} rejected: name {Name} already exists", room.Id, newName);
                return Result<string>.Fail(DuplicateNameError);
            }

            room.Name = newName;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} renamed from {OldName} to {NewName}", room.Id, oldName, newName);
            return Result<string>.OK($"اتغير اسم {oldName} لـ {newName}");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Rename room {RoomId} rejected by the database", model.Id);
            return Result<string>.Fail(DuplicateNameError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rename room {RoomId} failed", model.Id);
            return Result<string>.Fail(GeneralError);
        }
    }

    public async Task<Result<string>> UpdateDefaultModeAsync(UpdateRoomDefaultModeViewModel model)
    {
        try
        {
            var room = await Rooms.GetByIdAsync(model.Id);
            if (room is null)
                return Result<string>.NotFound("الغرفة مش موجودة");

            room.DefaultMode = model.DefaultMode;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} default mode set to {Mode}", room.Id, room.DefaultMode);
            return Result<string>.OK($"النمط الافتراضي لـ{room.Name} بقى {TextHelpers.ModeText(room.DefaultMode)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update default mode of room {RoomId} failed", model.Id);
            return Result<string>.Fail(GeneralError);
        }
    }

    public async Task<Result<string>> ToggleActiveAsync(int id)
    {
        try
        {
            var room = await Rooms.GetByIdAsync(id);
            if (room is null)
                return Result<string>.NotFound("الغرفة مش موجودة");

            if (room.IsActive)
            {
                var hasOpenSession = await _unitOfWork.GameSessionRepository
                    .AnyAsync(s => s.RoomId == id && s.Status != SessionStatus.Closed);

                if (hasOpenSession)
                {
                    _logger.LogWarning("Disable room {RoomId} rejected: it has an open session", id);
                    return Result<string>.Fail($"مينفعش تعطّل {room.Name}، فيها جلسة شغالة");
                }
            }

            room.IsActive = !room.IsActive;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Room {RoomId} {State}", id, room.IsActive ? "enabled" : "disabled");
            return Result<string>.OK(room.IsActive ? $"اتفعلت {room.Name}" : $"اتعطلت {room.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Toggle room {RoomId} failed", id);
            return Result<string>.Fail(GeneralError);
        }
    }
}
