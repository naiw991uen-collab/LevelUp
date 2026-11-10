using LevelUp.DAL.Contexts;
using LevelUp.DAL.Models;
using LevelUp.DAL.Repositories.Interfaces;

namespace LevelUp.DAL.Repositories.Classes;

public class UnitOfWork : IUnitOfWork
{
    private readonly LevelUpDbContext _dbContext;
    private readonly Dictionary<string, object> _repos = [];

    public IGameSessionRepository GameSessionRepository { get; }
    public IModeRateRepository ModeRateRepository { get; }
    public ISessionDrinkRepository SessionDrinkRepository { get; }

    public UnitOfWork(LevelUpDbContext dbContext,
                      IGameSessionRepository gameSessionRepository,
                      IModeRateRepository modeRateRepository,
                      ISessionDrinkRepository sessionDrinkRepository)
    {
        _dbContext = dbContext;
        GameSessionRepository = gameSessionRepository;
        ModeRateRepository = modeRateRepository;
        SessionDrinkRepository = sessionDrinkRepository;
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
    {
        var type = typeof(TEntity).Name;

        if (_repos.TryGetValue(type, out var repo))
            return (IRepository<TEntity>)repo;

        var newRepo = new Repository<TEntity>(_dbContext);
        _repos[type] = newRepo;

        return newRepo;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _dbContext.SaveChangesAsync(ct);
}
