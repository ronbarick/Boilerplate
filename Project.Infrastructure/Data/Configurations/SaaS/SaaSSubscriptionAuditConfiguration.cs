using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSSubscriptionAuditConfiguration : IEntityTypeConfiguration<SaaSSubscriptionAudit>
{
    public void Configure(EntityTypeBuilder<SaaSSubscriptionAudit> builder)
    {
        builder.ToTable("SaaSSubscriptionAudits");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Action).IsRequired().HasMaxLength(64);
        
        builder.HasIndex(x => x.SubscriptionId);
        builder.HasIndex(x => x.TenantId);
    }
}
