using Project.Core.Interfaces.Common;
using Project.Core.Interfaces.DependencyInjection;

namespace Project.Infrastructure.Services.Common;

public class Clock : IClock, ITransientDependency
{
    public DateTime Now => DateTime.UtcNow;
}
