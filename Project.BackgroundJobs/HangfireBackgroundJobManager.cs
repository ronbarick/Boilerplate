using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;

namespace Project.BackgroundJobs;

/// <summary>
/// Hangfire implementation of IBackgroundJobManager
/// </summary>
public class HangfireBackgroundJobManager : IBackgroundJobManager
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireBackgroundJobManager(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public string Enqueue<TJob>(Expression<Action<TJob>> methodCall)
    {
        return _backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue(methodCall);
    }

    public string Schedule<TJob>(Expression<Action<TJob>> methodCall, TimeSpan delay)
    {
        return _backgroundJobClient.Schedule(methodCall, delay);
    }

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, TimeSpan delay)
    {
        return _backgroundJobClient.Schedule(methodCall, delay);
    }

    public string Schedule<TJob>(Expression<Action<TJob>> methodCall, DateTimeOffset enqueueAt)
    {
        return _backgroundJobClient.Schedule(methodCall, enqueueAt);
    }

    public string Schedule<TJob>(Expression<Func<TJob, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return _backgroundJobClient.Schedule(methodCall, enqueueAt);
    }

    public bool Delete(string jobId)
    {
        return _backgroundJobClient.Delete(jobId);
    }
}
