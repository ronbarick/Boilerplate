using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Domain.Entities.Notifications;

namespace Project.Domain.Interfaces.Notifications
{
    public interface IUserNotificationManager
    {
        Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int skipCount, int maxResultCount, NotificationState? state = null);
        Task MarkAsReadAsync(Guid userId, Guid userNotificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task DeleteUserNotificationAsync(Guid userId, Guid userNotificationId);
        Task SubscribeAsync(Guid userId, string notificationName, string entityTypeName = null, string entityId = null);
        Task UnsubscribeAsync(Guid userId, string notificationName, string entityTypeName = null, string entityId = null);
    }
}
