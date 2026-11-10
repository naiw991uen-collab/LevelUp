using LevelUp.DAL.Models;

namespace LevelUp.DAL.Repositories.Interfaces;

public interface IUnitOfWork
{
    IGameSessionRepository GameSessionRepository { get; }
    IModeRateRepository ModeRateRepository { get; }
    ISessionDrinkRepository SessionDrinkRepository { get; }

    IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
