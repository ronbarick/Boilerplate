using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasTenantPaymentMethodConfiguration : IEntityTypeConfiguration<SaasTenantPaymentMethod>
{
    public void Configure(EntityTypeBuilder<SaasTenantPaymentMethod> builder)
    {
        builder.ToTable("SaasTenantPaymentMethods");
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => new { x.TenantId, x.IsDefault });
    }
}
