using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSAddonConfiguration : IEntityTypeConfiguration<SaaSAddon>
{
    public void Configure(EntityTypeBuilder<SaaSAddon> builder)
    {
        builder.ToTable("SaaSAddons");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}
