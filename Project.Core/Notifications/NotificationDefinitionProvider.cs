using System.Collections.Generic;

namespace Project.Core.Notifications
{
    public abstract class NotificationDefinitionProvider
    {
        public abstract void Define(List<NotificationDefinition> definitions);
    }
}
