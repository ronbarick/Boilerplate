using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;

namespace Project.BackgroundJobs;

/// <summary>
/// Hangfire implementation of IRecurringJobManager
/// </summary>
public class HangfireRecurringJobManager : IRecurringJobManager
{
    private readonly Hangfire.IRecurringJobManager _recurringJobManager;

    public HangfireRecurringJobManager(Hangfire.IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public void AddOrUpdate<TJob>(string jobId, Expression<Action<TJob>> methodCall, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
    }

    public void AddOrUpdate<TJob>(string jobId, Expression<Func<TJob, Task>> methodCall, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
    }

    public void Trigger(string jobId)
    {
        _recurringJobManager.Trigger(jobId);
    }

    public void RemoveIfExists(string jobId)
    {
        _recurringJobManager.RemoveIfExists(jobId);
    }
}
