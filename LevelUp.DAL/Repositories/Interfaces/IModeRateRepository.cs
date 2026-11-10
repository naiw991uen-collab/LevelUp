using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.DAL.Repositories.Interfaces;

public interface IModeRateRepository : IRepository<ModeRate>
{
    Task<ModeRate?> GetCurrentAsync(GameMode mode, CancellationToken ct = default);
}
