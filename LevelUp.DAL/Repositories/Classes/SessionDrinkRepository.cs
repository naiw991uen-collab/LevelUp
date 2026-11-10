using LevelUp.DAL.Contexts;
using LevelUp.DAL.Models;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.DAL.Repositories.Classes;

public class SessionDrinkRepository : Repository<SessionDrink>, ISessionDrinkRepository
{
    public SessionDrinkRepository(LevelUpDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<SessionDrink>> GetWithoutSessionByRoomIdAsync(int roomId, bool tracking = false, CancellationToken ct = default)
    {
        var query = WithoutSession(tracking).Where(x => x.RoomId == roomId);
        return await query.ToListAsync(ct);
    }

    public async Task<IEnumerable<SessionDrink>> GetAllWithoutSessionAsync(CancellationToken ct = default)
    {
        return await WithoutSession(tracking: false).ToListAsync(ct);
    }

    private IQueryable<SessionDrink> WithoutSession(bool tracking)
    {
        IQueryable<SessionDrink> query = _dbContext.SessionDrinks
            .Include(x => x.Drink)
            .Where(x => x.GameSessionId == null)
            .OrderBy(x => x.CreatedAt);

        return tracking ? query : query.AsNoTracking();
    }
}
