using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasPlanConfiguration : IEntityTypeConfiguration<SaasPlan>
{
    public void Configure(EntityTypeBuilder<SaasPlan> builder)
    {
        builder.ToTable("SaasPlans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Currency).HasMaxLength(3);
        
        builder.HasMany(x => x.Features)
               .WithOne(x => x.Plan)
               .HasForeignKey(x => x.PlanId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
