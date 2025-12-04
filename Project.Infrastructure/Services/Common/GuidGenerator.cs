using Project.Domain.Interfaces.Common;
using Project.Domain.Interfaces.DependencyInjection;

namespace Project.Infrastructure.Services.Common;

public class GuidGenerator : IGuidGenerator, ITransientDependency
{
    public Guid Create()
    {
        return Guid.NewGuid();
    }
}
