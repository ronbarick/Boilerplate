using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.Common.Dtos;
using Project.Core.Attributes;
using Project.Core.Constants;
using Project.Core.Entities;
using Project.Core.Interfaces;
using Project.Infrastructure.Extensions;
using Project.Application.Services;

using Project.Application.Students.Dtos;
using Project.Core.Emailing;
using Project.Core.Localization;
using Project.Core.Interfaces.Notifications;
using Project.Core.Entities.Notifications;
using Project.Core.Interfaces.Reporting;
using Project.Core.Dtos.Reporting;

namespace Project.Application.Students;

public class StudentAppService : AppServiceBase, IStudentAppService
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _templateProvider;
    private readonly Core.BackgroundJobs.IBackgroundJobManager _backgroundJobManager;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly IReportService _reportService;

    public StudentAppService(
        IRepository<Student> studentRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        IPermissionChecker permissionChecker,
        IEmailSender emailSender,
        IEmailTemplateProvider templateProvider,
        Core.BackgroundJobs.IBackgroundJobManager backgroundJobManager,
        INotificationPublisher notificationPublisher,
        ILocalizationManager localizationManager,
        IReportService reportService)
        : base(currentUser, currentTenant, permissionChecker, localizationManager)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
        _emailSender = emailSender;
        _templateProvider = templateProvider;
        _backgroundJobManager = backgroundJobManager;
        _notificationPublisher = notificationPublisher;
        _reportService = reportService;
    }

    [RequiresPermission(PermissionNames.Pages_Students)]
    public async Task<PagedResultDto<StudentDto>> GetListAsync(GetStudentsInput input)
    {
        input.Normalize();

        var query = _studentRepository.GetQueryable();

        // Apply filter/search using WhereIf
        var filter = input.Filter?.ToLower();
        query = query.WhereIf(
            !string.IsNullOrWhiteSpace(filter),
            s => s.FirstName.ToLower().Contains(filter!) ||
                 s.LastName.ToLower().Contains(filter!) ||
                 s.EmailAddress.ToLower().Contains(filter!));

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderByDynamic(input.Sorting ?? "FirstName ASC")
            .PageBy(input.SkipCount, input.MaxResultCount)
            .ToListAsync();

        return new PagedResultDto<StudentDto>
        {
            Items = _mapper.Map<List<StudentDto>>(students),
            TotalCount = totalCount
        };
    }

    [RequiresPermission(PermissionNames.Pages_Students, PermissionNames.Pages_Students_Create)]
    public async Task<StudentDto> CreateAsync(CreateStudentDto input)
    {


        var student = _mapper.Map<Student>(input);
        await _studentRepository.InsertAsync(student);

        // Send welcome email
        var emailBody = await _templateProvider.GetTemplateAsync("WelcomeEmail", new Dictionary<string, string>
        {
            { "UserName", $"{student.FirstName} {student.LastName}" },
            { "AppName", "Student Management System" },
            { "ActivationLink", "https://example.com/activate" }, // Placeholder link
            { "Year", DateTime.Now.Year.ToString() }
        });

        await _emailSender.SendAsync(student.EmailAddress, "Welcome to Student Management System", emailBody);

        return _mapper.Map<StudentDto>(student);
    }

    [RequiresPermission(PermissionNames.Pages_Students)]
    public async Task<StudentDto> GetAsync(Guid id)
    {
        var student = await _studentRepository.GetAsync(id);
        return _mapper.Map<StudentDto>(student);
    }

    [RequiresPermission(PermissionNames.Pages_Students, PermissionNames.Pages_Students_Edit)]
    public async Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto input)
    {
        var student = await _studentRepository.GetAsync(id);

        student.FirstName = input.FirstName;
        student.LastName = input.LastName;
        student.EmailAddress = input.EmailAddress;
        student.Grade = input.Grade;

        await _studentRepository.UpdateAsync(student);
        return _mapper.Map<StudentDto>(student);
    }

    [RequiresPermission(PermissionNames.Pages_Students, PermissionNames.Pages_Students_Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _studentRepository.DeleteAsync(id);
    }

    [RequiresPermission(PermissionNames.Pages_Students)]
    public Task<string> ScheduleStudentListJobAsync(int delayInMinutes = 0)
    {
        string jobId;

        if (delayInMinutes > 0)
        {
            // Schedule job to run after delay
            jobId = _backgroundJobManager.Schedule<BackgroundJobs.StudentListJob>(
                job => job.ExecuteAsync(),
                TimeSpan.FromMinutes(delayInMinutes));
        }
        else
        {
            // Run immediately
            jobId = _backgroundJobManager.Enqueue<BackgroundJobs.StudentListJob>(
                job => job.ExecuteAsync());
        }

        return Task.FromResult(jobId);
    }

    [RequiresPermission(PermissionNames.Pages_Students)]
    public async Task TestNotificationAsync(string message, NotificationSeverity severity = NotificationSeverity.Info)
    {
        await _notificationPublisher.PublishToAllAsync("TestNotification", new { message }, severity);
    }

    [RequiresPermission(PermissionNames.Pages_Students)]

    public async Task<ReportFileDto> GenerateStudentReportAsync(ReportRequestDto input)
    {
        return await _reportService.GenerateReportAsync(input, async () =>
        {
            // Fetch all students (consider adding filters from input if needed)
            var students = await _studentRepository.GetListAsync();

            // Map to DTOs for the report
            return _mapper.Map<List<StudentDto>>(students);
        }, reportName: "StudentReport"); // Server controls the report name
    }
}
