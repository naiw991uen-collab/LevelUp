using LevelUp.DAL.Contexts;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.DAL.Repositories.Classes;

public class GameSessionRepository : Repository<GameSession>, IGameSessionRepository
{
    public GameSessionRepository(LevelUpDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<GameSession?> GetOpenByRoomIdAsync(int roomId, bool tracking = false, CancellationToken ct = default)
    {
        var query = WithDetails(tracking);
        return await query.FirstOrDefaultAsync(x => x.RoomId == roomId
                                                 && x.Status != SessionStatus.Closed, ct);
    }

    public async Task<IEnumerable<GameSession>> GetAllOpenAsync(CancellationToken ct = default)
    {
        return await WithDetails(tracking: false)
            .Where(x => x.Status != SessionStatus.Closed)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<GameSession>> GetClosedAsync(DateOnly? fromDay = null,
                                                               DateOnly? toDay = null,
                                                               GameMode? startMode = null,
                                                               int? roomId = null,
                                                               CancellationToken ct = default)
    {
        var query = WithDetails(tracking: false)
            .Where(x => x.Status == SessionStatus.Closed);

        if (fromDay is not null) query = query.Where(x => x.BusinessDay >= fromDay);
        if (toDay is not null) query = query.Where(x => x.BusinessDay <= toDay);
        if (startMode is not null) query = query.Where(x => x.StartMode == startMode);
        if (roomId is not null) query = query.Where(x => x.RoomId == roomId);

        return await query
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);
    }

    private IQueryable<GameSession> WithDetails(bool tracking)
    {
        IQueryable<GameSession> query = _dbContext.GameSessions
            .Include(x => x.Room)
            .Include(x => x.Segments)
            .Include(x => x.SessionDrinks).ThenInclude(x => x.Drink)
            .AsSplitQuery();

        return tracking ? query : query.AsNoTracking();
    }
}
