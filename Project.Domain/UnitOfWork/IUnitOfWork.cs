namespace Project.Domain.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Guid Id { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
