using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.DAL.Repositories.Interfaces;

public interface IGameSessionRepository : IRepository<GameSession>
{
    Task<GameSession?> GetOpenByRoomIdAsync(int roomId, bool tracking = false, CancellationToken ct = default);

    Task<IEnumerable<GameSession>> GetAllOpenAsync(CancellationToken ct = default);

    Task<IEnumerable<GameSession>> GetClosedAsync(DateOnly? fromDay = null,
                                                  DateOnly? toDay = null,
                                                  GameMode? startMode = null,
                                                  int? roomId = null,
                                                  CancellationToken ct = default);
}
