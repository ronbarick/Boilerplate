using System;
using System.Threading.Tasks;
using Project.Application.Common.Dtos;
using Project.Application.Students.Dtos;
using Project.Domain.Dtos.Reporting;
using Project.Domain.Interfaces;
using Project.Domain.Shared.Enums;

namespace Project.Application.Students;

public interface IStudentAppService : IApplicationService
{
    Task<PagedResultDto<StudentDto>> GetListAsync(GetStudentsInput input);
    Task<StudentDto> CreateAsync(CreateStudentDto input);
    Task<StudentDto> GetAsync(Guid id);
    Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto input);
    Task DeleteAsync(Guid id);
    Task<string> ScheduleStudentListJobAsync(int delayInMinutes = 0);
    Task TestNotificationAsync(string message, NotificationSeverity severity = NotificationSeverity.Info);
    Task<ReportFileDto> GenerateStudentReportAsync(ReportRequestDto input);
}
