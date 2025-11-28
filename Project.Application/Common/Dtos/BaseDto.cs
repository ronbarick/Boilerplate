using System;

namespace Project.Application.Common.Dtos;

public abstract class EntityDto<TKey>
{
    public TKey Id { get; set; } = default!;
}

public abstract class CreationAuditedEntityDto<TKey> : EntityDto<TKey>
{
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
}

public abstract class AuditedEntityDto<TKey> : CreationAuditedEntityDto<TKey>
{
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
}

public abstract class FullAuditedEntityDto<TKey> : AuditedEntityDto<TKey>
{
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }
}
