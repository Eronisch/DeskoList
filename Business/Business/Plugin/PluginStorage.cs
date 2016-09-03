using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Business.Plugin
{
    public class Plugin
    {
        public string Method { get; set; }
        public Type Type{ get; set; }
    }

    public class PluginStorage<TEnum>
    {
        private readonly Dictionary<TEnum, Collection<Plugin>> _storage;

        public PluginStorage(TEnum[] availablePlugins)
        {
            ThrowExceptionIfNotEnum();

            _storage = new Dictionary<TEnum, Collection<Plugin>>();

            Add(availablePlugins);
        }

        private static void ThrowExceptionIfNotEnum()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new Exception("The available plugins must be an enum!");
            }
        }

        private void Add(IEnumerable<TEnum> plugins)
        {
            foreach (var plugin in plugins)
            {
                _storage.Add(plugin, new Collection<Plugin>());    
            }
        }

        public Dictionary<TEnum, Collection<Plugin>> Storage
        {
            get
            {
                return _storage;
            }
        }
    }
}
