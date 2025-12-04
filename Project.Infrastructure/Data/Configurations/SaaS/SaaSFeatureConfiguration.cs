using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSFeatureConfiguration : IEntityTypeConfiguration<SaaSFeature>
{
    public void Configure(EntityTypeBuilder<SaaSFeature> builder)
    {
        builder.ToTable("SaaSFeatures");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
    }
}
