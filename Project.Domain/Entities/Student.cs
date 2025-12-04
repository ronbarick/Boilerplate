using System;
using Project.Domain.Entities.Base;
using Project.Domain.Interfaces;

namespace Project.Domain.Entities;

/// <summary>
/// Student entity that must belong to a tenant.
/// TenantId is automatically assigned and strict tenant filtering is applied.
/// </summary>
public class Student : FullAuditedEntity, IMustHaveTenant
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public int Grade { get; set; }
}
