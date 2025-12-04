using System;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Notifications
{
    public class UserNotification : FullAuditedEntity
    {
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
        public NotificationState State { get; set; }
        public DateTime? ReadTime { get; set; }

        public virtual Notification Notification { get; set; }
    }

    public enum NotificationState
    {
        Unread,
        Read
    }
}
