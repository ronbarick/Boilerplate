using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Project.Application.BackgroundJobs;

namespace Project.WebApi.Endpoints;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly Hangfire.IRecurringJobManager _recurringJobManager;

    public JobsController(
        IBackgroundJobClient backgroundJobClient,
        Hangfire.IRecurringJobManager recurringJobManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    [HttpPost("enqueue")]
    public IActionResult EnqueueJob([FromBody] string message)
    {
        var jobId = _backgroundJobClient.Enqueue<TestJob>(job => job.Execute(message));
        return Ok(new { JobId = jobId, Message = "Job enqueued successfully" });
    }

    [HttpPost("schedule")]
    public IActionResult ScheduleJob([FromBody] ScheduleJobRequest request)
    {
        var jobId = _backgroundJobClient.Schedule<TestJob>(
            job => job.Execute(request.Message),
            TimeSpan.FromSeconds(request.DelayInSeconds));
        
        return Ok(new { JobId = jobId, Message = $"Job scheduled to run in {request.DelayInSeconds} seconds" });
    }

    [HttpPost("recurring")]
    public IActionResult AddRecurringJob([FromBody] RecurringJobRequest request)
    {
        _recurringJobManager.AddOrUpdate(
            request.JobName,
            () => Console.WriteLine($"[RecurringJob] {request.JobName}: {request.Message}"),
            request.CronExpression);
        
        return Ok(new { Message = $"Recurring job '{request.JobName}' added successfully" });
    }

    [HttpDelete("recurring/{jobName}")]
    public IActionResult RemoveRecurringJob(string jobName)
    {
        _recurringJobManager.RemoveIfExists(jobName);
        return Ok(new { Message = $"Recurring job '{jobName}' removed successfully" });
    }
}

public record ScheduleJobRequest(string Message, int DelayInSeconds);
public record RecurringJobRequest(string JobName, string Message, string CronExpression);
