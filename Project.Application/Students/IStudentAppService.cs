using System;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.Students.Dtos;
using Project.Application.Services;
using Project.Core.Interfaces;

namespace Project.Application.Students;

public interface IStudentAppService : IApplicationService
{
    Task<PagedResultDto<StudentDto>> GetListAsync(GetStudentsInput input);
    Task<StudentDto> GetAsync(Guid id);
    Task<StudentDto> CreateAsync(CreateStudentDto input);
    Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto input);
    Task DeleteAsync(Guid id);
    Task<string> ScheduleStudentListJobAsync(int delayInMinutes = 0);
    Task TestNotificationAsync(string message, Project.Core.Entities.Notifications.NotificationSeverity severity = Project.Core.Entities.Notifications.NotificationSeverity.Info);
    Task<Project.Core.Dtos.Reporting.ReportFileDto> GenerateStudentReportAsync(Project.Core.Dtos.Reporting.ReportRequestDto input);
}
