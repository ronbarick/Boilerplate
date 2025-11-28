using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.Core.BackgroundJobs;

/// <summary>
/// Abstraction for recurring job management
/// </summary>
public interface IRecurringJobManager
{
    /// <summary>
    /// Add or update a recurring job
    /// </summary>
    void AddOrUpdate<TJob>(string jobId, Expression<Action<TJob>> methodCall, string cronExpression);

    /// <summary>
    /// Add or update a recurring job (async)
    /// </summary>
    void AddOrUpdate<TJob>(string jobId, Expression<Func<TJob, Task>> methodCall, string cronExpression);

    /// <summary>
    /// Trigger a recurring job immediately
    /// </summary>
    void Trigger(string jobId);

    /// <summary>
    /// Remove a recurring job
    /// </summary>
    void RemoveIfExists(string jobId);
}
