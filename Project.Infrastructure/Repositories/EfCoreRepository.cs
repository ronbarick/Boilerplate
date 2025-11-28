using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Base;
using Project.Core.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IRepository.
/// Provides automatic tenant filtering for entities implementing IMustHaveTenant.
/// </summary>
public class EfCoreRepository<TEntity> : EfCoreRepository<TEntity, Guid>, IRepository<TEntity>
    where TEntity : class
{
    public EfCoreRepository(AppDbContext context) : base(context)
    {
    }
}

/// <summary>
/// EF Core implementation of IRepository with custom primary key.
/// </summary>
public class EfCoreRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public EfCoreRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual IQueryable<TEntity> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var savedEntity = (await _dbSet.AddAsync(entity, cancellationToken)).Entity;

        if (autoSave)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return savedEntity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        _context.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        if (autoSave)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public virtual async Task DeleteAsync(TKey id, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, autoSave, cancellationToken);
        }
    }

    public virtual async Task DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);

        if (autoSave)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string? sorting = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Apply sorting if provided
        if (!string.IsNullOrWhiteSpace(sorting))
        {
            query = Project.Infrastructure.Extensions.QueryableExtensions.OrderByDynamic(query, sorting);
        }

        return await query
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetLongCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.LongCountAsync(cancellationToken);
    }
}
