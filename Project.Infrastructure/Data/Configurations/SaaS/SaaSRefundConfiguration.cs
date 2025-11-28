using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSRefundConfiguration : IEntityTypeConfiguration<SaaSRefund>
{
    public void Configure(EntityTypeBuilder<SaaSRefund> builder)
    {
        builder.ToTable("SaaSRefunds");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.RefundAmount).HasColumnType("decimal(18,2)");
        
        builder.HasOne(x => x.Payment)
               .WithMany()
               .HasForeignKey(x => x.PaymentId);
    }
}
