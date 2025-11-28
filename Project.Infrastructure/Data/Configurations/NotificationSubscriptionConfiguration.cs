using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Entities.Notifications;

namespace Project.Infrastructure.Data.Configurations
{
    public class NotificationSubscriptionConfiguration : IEntityTypeConfiguration<NotificationSubscription>
    {
        public void Configure(EntityTypeBuilder<NotificationSubscription> builder)
        {
            builder.ToTable("NotificationSubscriptions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.NotificationName).IsRequired().HasMaxLength(128);
            builder.Property(x => x.EntityTypeName).HasMaxLength(256);
            builder.Property(x => x.EntityId).HasMaxLength(128);

            builder.HasIndex(x => new { x.UserId, x.NotificationName, x.EntityTypeName, x.EntityId });
            builder.HasIndex(x => x.NotificationName);
        }
    }
}
