using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Core.Entities.Notifications;

namespace Project.Core.Interfaces.Notifications
{
    public interface IRealTimeNotifier
    {
        Task SendNotificationsAsync(UserNotification[] userNotifications);
    }
}
