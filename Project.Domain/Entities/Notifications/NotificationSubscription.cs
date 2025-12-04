using System;
using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Notifications
{
    public class NotificationSubscription : FullAuditedEntity
    {
        public Guid UserId { get; set; }
        public string NotificationName { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityId { get; set; }
    }
}
