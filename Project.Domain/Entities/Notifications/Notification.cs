using System;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Notifications
{
    public class Notification : FullAuditedEntity
    {
        public string NotificationName { get; set; }
        public string Data { get; set; } // JSON
        public NotificationSeverity Severity { get; set; }
    }


}
