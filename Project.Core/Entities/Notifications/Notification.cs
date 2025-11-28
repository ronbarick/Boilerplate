using System;
using Project.Core.Entities.Base;

namespace Project.Core.Entities.Notifications
{
    public class Notification : FullAuditedEntity
    {
        public string NotificationName { get; set; }
        public string Data { get; set; } // JSON
        public NotificationSeverity Severity { get; set; }
    }

    public enum NotificationSeverity
    {
        Info,
        Success,
        Warn,
        Error,
        Fatal
    }
}
