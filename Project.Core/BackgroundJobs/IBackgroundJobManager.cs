using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.Core.BackgroundJobs;

/// <summary>
/// Abstraction for background job management
/// </summary>
public interface IBackgroundJobManager
{
    /// <summary>
    /// Enqueue a job to run immediately
    /// </summary>
    string Enqueue<TJob>(Expression<Action<TJob>> methodCall);

    /// <summary>
    /// Enqueue a job to run immediately (async)
    /// </summary>
    string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall);

    /// <summary>
    /// Schedule a job to run after a delay
    /// </summary>
    string Schedule<TJob>(Expression<Action<TJob>> methodCall, TimeSpan delay);

    /// <summary>
    /// Schedule a job to run after a delay (async)
    /// </summary>
    string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay);

    /// <summary>
    /// Schedule a job to run at a specific time
    /// </summary>
    string Schedule<TJob>(Expression<Action<TJob>> methodCall, DateTimeOffset enqueueAt);

    /// <summary>
    /// Schedule a job to run at a specific time (async)
    /// </summary>
    string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, DateTimeOffset enqueueAt);

    /// <summary>
    /// Delete a job by ID
    /// </summary>
    bool Delete(string jobId);
}
