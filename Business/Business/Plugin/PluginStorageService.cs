using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Business.Plugin
{
    public  class PluginStorageService<TEnum>
    {
        private readonly PluginStorage<TEnum> _storage;

        public PluginStorageService()
        {
            _storage = GetAvailableHooks();
        }

        public void AddHook(TEnum hookKey, Type type, string methodName)
        {
            if (!IsHookAdded(hookKey, type, methodName))
            {
                GetHooks(hookKey).Add(GetPlugin(type, methodName));
            }
        }

        public void RemoveHook(TEnum hookKey, Type type, string methodName)
        {
            if (IsHookAdded(hookKey, type, methodName))
            {
                GetHooks(hookKey).Remove(GetPlugin(type, methodName));
            }
        }

        public bool IsHookAdded(TEnum hookKey, Type type, string methodName)
        {
            return
                GetHooks(hookKey)
                    .Any(
                        h => 
                            h.Method.Equals(methodName, StringComparison.CurrentCultureIgnoreCase)
                            && h.Type == type);
        }

        public Collection<Plugin> GetHooks(TEnum hookKey)
        {
            return _storage.Storage[hookKey];
        }

        public PluginStorageService<TEnum> Storage
        {
            get { return this; }
        } 

        #region Private Methods

        private PluginStorage<TEnum> GetAvailableHooks()
        {
            var pluginHooks = (TEnum[])Enum.GetValues(typeof(TEnum));

            return new PluginStorage<TEnum>(pluginHooks);
        }

        private Plugin GetPlugin(Type type, string methodName)
        {
            return new Plugin
            {
                Method = methodName,
                Type = type
            };
        }

        #endregion
    }
}
