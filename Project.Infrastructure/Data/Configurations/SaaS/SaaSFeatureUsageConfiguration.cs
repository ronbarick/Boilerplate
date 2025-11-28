using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSFeatureUsageConfiguration : IEntityTypeConfiguration<SaaSFeatureUsage>
{
    public void Configure(EntityTypeBuilder<SaaSFeatureUsage> builder)
    {
        builder.ToTable("SaaSFeatureUsages");
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => new { x.TenantId, x.FeatureName });
    }
}
