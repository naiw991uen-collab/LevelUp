using AutoMapper;
using LevelUp.BLL.Services.Billing;
using LevelUp.BLL.Services.Devices.ViewModel;
using LevelUp.BLL.Services.Rooms.ViewModel;
using LevelUp.BLL.Services.Sessions.Enum;
using LevelUp.BLL.Services.Sessions.Interfaces;
using LevelUp.BLL.Services.Sessions.ViewModel;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using LevelUp.DAL.Repositories.Interfaces;

namespace LevelUp.BLL.Services.Sessions.Classes;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SessionIndexViewModel> GetIndexAsync(SessionFilterViewModel filter)
    {
        var day = filter.Date ?? BillingCalculator.BusinessDay(DateTime.Now);

        var sessions = await _unitOfWork.GameSessionRepository
            .GetClosedAsync(fromDay: day, toDay: day, roomId: filter.RoomId);

        var rows = sessions
            .Select(BuildRow)
            .Where(row => Matches(row, filter.Mode))
            .ToList();

        var rooms = (await _unitOfWork.GetRepository<Room>().GetAllAsync()).OrderBy(r => r.Id);

        return new SessionIndexViewModel
        {
            Filter = filter,
            Day = day,
            Rows = rows,
            Rooms = _mapper.Map<List<RoomViewModel>>(rooms)
        };
    }

    private FinishedSessionViewModel BuildRow(GameSession session)
    {
        var parts = new List<DeviceSegmentViewModel>();

        foreach (var segment in session.Segments.OrderBy(s => s.StartedAt))
        {
            var duration = (segment.EndedAt ?? session.EndedAt ?? segment.StartedAt) - segment.StartedAt;
            var seconds = Math.Max(0, (int)duration.TotalSeconds);
            var cost = BillingCalculator.SegmentCost(duration, segment.HourlyRate);

            var same = parts.FirstOrDefault(p => p.Mode == segment.Mode && p.HourlyRate == segment.HourlyRate);
            if (same is not null)
            {
                same.Seconds += seconds;
                same.Cost += cost;
            }
            else
            {
                parts.Add(new DeviceSegmentViewModel
                {
                    Mode = segment.Mode,
                    HourlyRate = segment.HourlyRate,
                    Seconds = seconds,
                    Cost = cost
                });
            }
        }

        return new FinishedSessionViewModel
        {
            RoomName = session.Room.Name,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            Parts = parts,
            Drinks = _mapper.Map<List<DeviceDrinkViewModel>>(session.SessionDrinks.OrderBy(d => d.CreatedAt))
        };
    }

    private static bool Matches(FinishedSessionViewModel row, SessionModeFilter mode) => mode switch
    {
        SessionModeFilter.Single => row.Parts.Any(p => p.Mode == GameMode.Single),
        SessionModeFilter.Multi => row.Parts.Any(p => p.Mode == GameMode.Multi),
        SessionModeFilter.Mixed => row.Parts.Select(p => p.Mode).Distinct().Count() > 1,
        _ => true
    };
}
