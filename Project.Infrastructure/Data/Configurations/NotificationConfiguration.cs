using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.Notifications;

namespace Project.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NotificationName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Data).HasMaxLength(1048576); // 1MB limit for JSON data
            builder.Property(x => x.Severity).IsRequired();
            
            builder.HasIndex(x => x.CreatedOn);
        }
    }
}
