using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasTenantSubscriptionConfiguration : IEntityTypeConfiguration<SaasTenantSubscription>
{
    public void Configure(EntityTypeBuilder<SaasTenantSubscription> builder)
    {
        builder.ToTable("SaasTenantSubscriptions");
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Plan)
               .WithMany() // Assuming Plan does not have explicit Subscriptions collection to avoid circular dependency issues if not needed
               .HasForeignKey(x => x.PlanId);

        builder.HasIndex(x => x.TenantId);
    }
}
