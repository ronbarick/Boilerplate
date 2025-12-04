using System;

namespace Project.Domain.Entities.Base;

public abstract class FullAuditedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    [System.ComponentModel.DataAnnotations.ConcurrencyCheck]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
