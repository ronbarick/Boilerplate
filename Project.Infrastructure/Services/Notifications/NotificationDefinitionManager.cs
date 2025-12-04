using System.Collections.Generic;
using System.Linq;
using Project.Domain.Interfaces.Notifications;
using Project.Domain.Notifications;

namespace Project.Infrastructure.Services.Notifications
{
    public class NotificationDefinitionManager : INotificationDefinitionManager
    {
        private readonly Dictionary<string, NotificationDefinition> _definitions;

        public NotificationDefinitionManager(IEnumerable<NotificationDefinitionProvider> providers)
        {
            _definitions = new Dictionary<string, NotificationDefinition>();
            var definitions = new List<NotificationDefinition>();
            
            foreach (var provider in providers)
            {
                provider.Define(definitions);
            }

            foreach (var definition in definitions)
            {
                _definitions[definition.Name] = definition;
            }
        }

        public void Add(NotificationDefinition definition)
        {
            _definitions[definition.Name] = definition;
        }

        public NotificationDefinition Get(string name)
        {
            return _definitions.TryGetValue(name, out var definition) ? definition : null;
        }

        public IReadOnlyList<NotificationDefinition> GetAll()
        {
            return _definitions.Values.ToList();
        }
    }
}
