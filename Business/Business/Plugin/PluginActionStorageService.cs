using System;
using System.Collections.ObjectModel;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Manager for storing plugin actions
    /// </summary>
    public static class PluginActionStorageService
    {
        private static readonly PluginStorageService<PluginActions> PluginStorageService;

        static PluginActionStorageService()
        {
            PluginStorageService = new PluginStorageService<PluginActions>();
        }

        /// <summary>
        /// Add a new action
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="type"></param>
        /// <param name="methodName">A public method</param>
        public static void AddAction(PluginActions actionKey, Type type, string methodName)
        {
          PluginStorageService.AddHook(actionKey, type, methodName);
        }

        /// <summary>
        /// Remove a specific action
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        public static void RemoveAction(PluginActions actionKey, Type type, string methodName)
        {
           PluginStorageService.RemoveHook(actionKey, type, methodName);
        }

        /// <summary>
        /// Check if the action is added
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="type"></param>
        /// <param name="methodName">A public method</param>
        /// <returns></returns>
        public static bool IsActionAdded(PluginActions actionKey, Type type, string methodName)
        {
            return PluginStorageService.IsHookAdded(actionKey, type, methodName);
        }

        /// <summary>
        /// Get all the actions
        /// </summary>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        public static Collection<Plugin> GetActions(PluginActions actionKey)
        {
            return PluginStorageService.GetHooks(actionKey);
        }

        /// <summary>
        /// Get the action storage
        /// </summary>
        public static PluginStorageService<PluginActions> ActionStorage
        {
            get { return PluginStorageService.Storage; }
        }
    }
}
