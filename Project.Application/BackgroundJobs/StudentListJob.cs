using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Project.Application.Common.Dtos;
using Project.Application.Students;
using Project.Application.Students.Dtos;

namespace Project.Application.BackgroundJobs;

/// <summary>
/// Recurring job that fetches student list daily at 10 PM
/// </summary>
public class StudentListJob
{
    private readonly IStudentAppService _studentAppService;
    private readonly ILogger<StudentListJob> _logger;

    public StudentListJob(
        IStudentAppService studentAppService,
        ILogger<StudentListJob> logger)
    {
        _studentAppService = studentAppService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Starting StudentListJob at {Time}", DateTime.UtcNow);

            var input = new GetStudentsInput
            {
                MaxResultCount = 1000, // Adjust as needed
                SkipCount = 0
            };

            var result = await _studentAppService.GetListAsync(input);

            _logger.LogInformation(
                "StudentListJob completed successfully. Total students: {Count}",
                result.TotalCount);

            // You can add additional logic here:
            // - Send email report
            // - Generate statistics
            // - Export to file
            // - Sync with external system
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing StudentListJob");
            throw; // Re-throw to let Hangfire handle retry logic
        }
    }
}
