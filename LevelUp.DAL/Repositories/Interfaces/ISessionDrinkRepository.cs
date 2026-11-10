using LevelUp.DAL.Models;

namespace LevelUp.DAL.Repositories.Interfaces;

public interface ISessionDrinkRepository : IRepository<SessionDrink>
{
    Task<IEnumerable<SessionDrink>> GetWithoutSessionByRoomIdAsync(int roomId, bool tracking = false, CancellationToken ct = default);

    Task<IEnumerable<SessionDrink>> GetAllWithoutSessionAsync(CancellationToken ct = default);
}
