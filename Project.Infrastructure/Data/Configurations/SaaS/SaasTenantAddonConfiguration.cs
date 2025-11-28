using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasTenantAddonConfiguration : IEntityTypeConfiguration<SaasTenantAddon>
{
    public void Configure(EntityTypeBuilder<SaasTenantAddon> builder)
    {
        builder.ToTable("SaasTenantAddons");
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Addon)
               .WithMany()
               .HasForeignKey(x => x.AddonId);
               
        builder.HasIndex(x => x.TenantId);
    }
}
