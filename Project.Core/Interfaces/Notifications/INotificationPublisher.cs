using System;
using System.Threading.Tasks;
using Project.Core.Entities.Notifications;

namespace Project.Core.Interfaces.Notifications
{
    public interface INotificationPublisher
    {
        Task PublishAsync(string notificationName, object data = null, Guid[] userIds = null, NotificationSeverity severity = NotificationSeverity.Info, Guid? tenantId = null);
        Task PublishToAllAsync(string notificationName, object data = null, NotificationSeverity severity = NotificationSeverity.Info);
        Task PublishToTenantAsync(Guid tenantId, string notificationName, object data = null, NotificationSeverity severity = NotificationSeverity.Info);
    }
}
