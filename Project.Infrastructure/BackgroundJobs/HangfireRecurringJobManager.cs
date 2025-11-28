using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using CoreJobs = Project.Core.BackgroundJobs;

namespace Project.Infrastructure.BackgroundJobs;

public class HangfireRecurringJobManager : CoreJobs.IRecurringJobManager
{
    public void AddOrUpdate<TJob>(string jobId, Expression<Action<TJob>> methodCall, string cronExpression)
    {
        RecurringJob.AddOrUpdate(jobId, methodCall, cronExpression);
    }

    public void AddOrUpdate<TJob>(string jobId, Expression<Func<TJob, Task>> methodCall, string cronExpression)
    {
        RecurringJob.AddOrUpdate(jobId, methodCall, cronExpression);
    }

    public void Trigger(string jobId)
    {
        RecurringJob.Trigger(jobId);
    }

    public void RemoveIfExists(string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
    }
}
