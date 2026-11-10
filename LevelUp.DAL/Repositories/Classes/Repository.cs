using System.Linq.Expressions;
using LevelUp.DAL.Contexts;
using LevelUp.DAL.Models;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.DAL.Repositories.Classes;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new()
{
    protected readonly LevelUpDbContext _dbContext;
    private readonly DbSet<TEntity> _set;

    public Repository(LevelUpDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = _dbContext.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                                        bool tracking = false,
                                                        CancellationToken ct = default)
    {
        IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
        if (predicate is not null) query = query.Where(predicate);
        return await query.ToListAsync(ct);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync([id], ct);

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
                                                    bool tracking = false,
                                                    CancellationToken ct = default)
    {
        IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
        return await query.FirstOrDefaultAsync(predicate, ct);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => _set.AsNoTracking().AnyAsync(predicate, ct);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null
            ? await _set.AsNoTracking().CountAsync(ct)
            : await _set.AsNoTracking().CountAsync(predicate, ct);

    public async Task AddAsync(TEntity entity)
        => await _set.AddAsync(entity);

    public Task UpdateAsync(TEntity entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }
}
