using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasTenantSubscriptionPaymentConfiguration : IEntityTypeConfiguration<SaasTenantSubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SaasTenantSubscriptionPayment> builder)
    {
        builder.ToTable("SaasTenantSubscriptionPayments");
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Subscription)
               .WithMany(x => x.Payments)
               .HasForeignKey(x => x.SubscriptionId);
               
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ProrationAmount).HasColumnType("decimal(18,2)");
    }
}
