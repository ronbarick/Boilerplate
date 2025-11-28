using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Core.Entities.Notifications;

namespace Project.Core.Interfaces.Notifications
{
    public interface INotificationStore
    {
        Task InsertNotificationAsync(Notification notification);
        Task InsertUserNotificationAsync(UserNotification userNotification);
        Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int skipCount, int maxResultCount, NotificationState? state = null);
        Task UpdateUserNotificationStateAsync(Guid userId, Guid userNotificationId, NotificationState state);
        Task DeleteNotificationAsync(Guid notificationId);
        Task<int> GetUnreadNotificationCountAsync(Guid userId);
        Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, string entityTypeName = null, string entityId = null);
        Task InsertSubscriptionAsync(NotificationSubscription subscription);
        Task DeleteSubscriptionAsync(Guid userId, string notificationName, string entityTypeName = null, string entityId = null);
    }
}
