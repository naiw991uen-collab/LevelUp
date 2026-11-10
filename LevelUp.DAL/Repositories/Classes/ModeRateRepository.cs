using LevelUp.DAL.Contexts;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.DAL.Repositories.Classes;

public class ModeRateRepository : Repository<ModeRate>, IModeRateRepository
{
    public ModeRateRepository(LevelUpDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ModeRate?> GetCurrentAsync(GameMode mode, CancellationToken ct = default)
    {
        var now = DateTime.Now;

        return await _dbContext.ModeRates
            .AsNoTracking()
            .Where(x => x.Mode == mode && x.EffectiveFrom <= now)
            .OrderByDescending(x => x.EffectiveFrom)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(ct);
    }
}
