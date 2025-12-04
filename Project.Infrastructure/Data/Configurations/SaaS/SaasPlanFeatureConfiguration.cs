using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasPlanFeatureConfiguration : IEntityTypeConfiguration<SaasPlanFeature>
{
    public void Configure(EntityTypeBuilder<SaasPlanFeature> builder)
    {
        builder.ToTable("SaasPlanFeatures");
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Plan)
               .WithMany(x => x.Features)
               .HasForeignKey(x => x.PlanId);
    }
}
