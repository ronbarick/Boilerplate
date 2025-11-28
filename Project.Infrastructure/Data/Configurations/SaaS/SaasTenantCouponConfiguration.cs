using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasTenantCouponConfiguration : IEntityTypeConfiguration<SaasTenantCoupon>
{
    public void Configure(EntityTypeBuilder<SaasTenantCoupon> builder)
    {
        builder.ToTable("SaasTenantCoupons");
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Coupon)
               .WithMany()
               .HasForeignKey(x => x.CouponId);
               
        builder.HasIndex(x => new { x.TenantId, x.CouponId });
    }
}
