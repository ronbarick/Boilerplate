namespace Project.Domain.Interfaces;

/// <summary>
/// Generic repository interface for entities with Guid primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, Guid>
    where TEntity : class
{
}

/// <summary>
/// Generic repository interface with custom primary key type.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : class
{
    /// <summary>
    /// Gets an entity by its primary key.
    /// </summary>
    Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of all entities.
    /// </summary>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queryable for advanced queries.
    /// </summary>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its primary key.
    /// </summary>
    Task DeleteAsync(TKey id, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    Task DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets count of all entities.
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of entities with optional sorting.
    /// </summary>
    Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string? sorting = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets count of entities matching a predicate.
    /// </summary>
    Task<long> GetLongCountAsync(CancellationToken cancellationToken = default);
}
