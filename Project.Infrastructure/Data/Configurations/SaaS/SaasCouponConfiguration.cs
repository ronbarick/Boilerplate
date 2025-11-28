using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaasCouponConfiguration : IEntityTypeConfiguration<SaasCoupon>
{
    public void Configure(EntityTypeBuilder<SaasCoupon> builder)
    {
        builder.ToTable("SaasCoupons");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
        
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
