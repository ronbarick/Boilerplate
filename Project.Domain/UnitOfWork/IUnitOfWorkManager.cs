namespace Project.Domain.UnitOfWork;

public interface IUnitOfWorkManager
{
    IUnitOfWork? Current { get; }
    IUnitOfWork Begin(bool requiresNew = false, bool isTransactional = false);
}
