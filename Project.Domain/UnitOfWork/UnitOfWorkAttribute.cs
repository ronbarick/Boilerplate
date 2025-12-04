namespace Project.Domain.UnitOfWork;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
public class UnitOfWorkAttribute : Attribute
{
    public bool IsTransactional { get; set; }
    public bool IsDisabled { get; set; }

    public UnitOfWorkAttribute(bool isTransactional = false)
    {
        IsTransactional = isTransactional;
    }
}
