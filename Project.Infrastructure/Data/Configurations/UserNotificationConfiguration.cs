using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.Notifications;

namespace Project.Infrastructure.Data.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("UserNotifications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.NotificationId).IsRequired();
            builder.Property(x => x.State).IsRequired();

            builder.HasOne(x => x.Notification)
                   .WithMany()
                   .HasForeignKey(x => x.NotificationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.State });
            builder.HasIndex(x => x.CreatedOn);
        }
    }
}
