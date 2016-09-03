using System;
using System.Collections.ObjectModel;

namespace Core.Business.Plugin
{
    public static class PluginHookStorageService
    {
        private static readonly PluginStorageService<PluginHooks> PluginStorageService;

        static PluginHookStorageService()
        {
            PluginStorageService = new PluginStorageService<PluginHooks>();
        }

        public static void AddHook(PluginHooks hookKey, Type type, string methodName)
        {
          PluginStorageService.AddHook(hookKey, type, methodName);
        }

        public static void RemoveHook(PluginHooks hookKey, Type type, string methodName)
        {
            PluginStorageService.RemoveHook(hookKey, type, methodName);
        }

        public static bool IsHookAdded(PluginHooks hookKey, Type type, string methodName)
        {
            return PluginStorageService.IsHookAdded(hookKey, type, methodName);
        }

        public static Collection<Plugin> GetHooks(PluginHooks hookKey)
        {
            return PluginStorageService.GetHooks(hookKey);
        }

        public static PluginStorageService<PluginHooks> HookStorage
        {
            get { return PluginStorageService.Storage; }
        }
    }
}
