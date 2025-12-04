using Project.Domain.Interfaces.Common;
using Project.Domain.Interfaces.DependencyInjection;

namespace Project.Infrastructure.Services.Common;

public class Clock : IClock, ITransientDependency
{
    public DateTime Now => DateTime.UtcNow;
}
