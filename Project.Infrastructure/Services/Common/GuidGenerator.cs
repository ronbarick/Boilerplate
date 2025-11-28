using Project.Core.Interfaces.Common;
using Project.Core.Interfaces.DependencyInjection;

namespace Project.Infrastructure.Services.Common;

public class GuidGenerator : IGuidGenerator, ITransientDependency
{
    public Guid Create()
    {
        return Guid.NewGuid();
    }
}
