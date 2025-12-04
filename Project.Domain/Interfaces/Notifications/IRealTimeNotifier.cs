using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Domain.Entities.Notifications;

namespace Project.Domain.Interfaces.Notifications
{
    public interface IRealTimeNotifier
    {
        Task SendNotificationsAsync(UserNotification[] userNotifications);
    }
}
