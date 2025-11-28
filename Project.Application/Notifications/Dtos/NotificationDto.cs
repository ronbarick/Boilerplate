using System;
using Project.Core.Entities.Notifications;

namespace Project.Application.Notifications.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string NotificationName { get; set; }
        public string Data { get; set; }
        public NotificationSeverity Severity { get; set; }
        public NotificationState State { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ReadTime { get; set; }
    }

    public class GetNotificationsInput
    {
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; } = 10;
        public NotificationState? State { get; set; }
    }

    public class SubscribeToNotificationInput
    {
        public string NotificationName { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityId { get; set; }
    }
}
