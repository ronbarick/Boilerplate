using System.Collections.Generic;
using Project.Domain.Notifications;

namespace Project.Domain.Interfaces.Notifications
{
    public interface INotificationDefinitionManager
    {
        void Add(NotificationDefinition definition);
        NotificationDefinition Get(string name);
        IReadOnlyList<NotificationDefinition> GetAll();
    }
}
