using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSInvoiceConfiguration : IEntityTypeConfiguration<SaaSInvoice>
{
    public void Configure(EntityTypeBuilder<SaaSInvoice> builder)
    {
        builder.ToTable("SaaSInvoices");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Tax).HasColumnType("decimal(18,2)");
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
    }
}
