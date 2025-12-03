using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Application.Notifications.Dtos;
using Project.Application.Services;
using Project.Domain.Interfaces;

namespace Project.Application.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(GetNotificationsInput input);
        Task MarkAsReadAsync(Guid userNotificationId);
        Task MarkAllAsReadAsync();
        Task<int> GetUnreadCountAsync();
        Task SubscribeAsync(SubscribeToNotificationInput input);
        Task UnsubscribeAsync(string notificationName);
    }
}
