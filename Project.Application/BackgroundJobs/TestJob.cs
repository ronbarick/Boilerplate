using Microsoft.Extensions.Logging;

namespace Project.Application.BackgroundJobs;

public class TestJob
{
    private readonly ILogger<TestJob> _logger;

    public TestJob(ILogger<TestJob> logger)
    {
        _logger = logger;
    }

    public void Execute(string message)
    {
        _logger.LogInformation("TestJob executed with message: {Message}", message);
        Console.WriteLine($"[TestJob] {DateTime.UtcNow}: {message}");
    }
}
