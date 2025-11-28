using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.SaaS;

namespace Project.Infrastructure.Data.Configurations.SaaS;

public class SaaSWebhookLogConfiguration : IEntityTypeConfiguration<SaaSWebhookLog>
{
    public void Configure(EntityTypeBuilder<SaaSWebhookLog> builder)
    {
        builder.ToTable("SaaSWebhookLogs");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Gateway).IsRequired().HasMaxLength(64);
        builder.Property(x => x.EventId).HasMaxLength(128);
        builder.Property(x => x.EventType).HasMaxLength(128);
        
        builder.HasIndex(x => x.EventId);
    }
}
