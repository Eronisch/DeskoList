using System;
using System.Collections.ObjectModel;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Manager for storing plugin filters
    /// </summary>
    public static class PluginFilterStorageService
    {
        private static readonly PluginStorageService<PluginFilters> PluginStorageService;

        static PluginFilterStorageService()
        {
            PluginStorageService = new PluginStorageService<PluginFilters>();
        }

        /// <summary>
        /// Add a new filter
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="type"></param>
        /// <param name="methodName">A public method</param>
        public static void AddFilter(PluginFilters filterKey, Type type, string methodName)
        {
            PluginStorageService.AddHook(filterKey, type, methodName);
        }

        /// <summary>
        /// Remove a specific filter
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="type"></param>
        /// <param name="methodName">A public method</param>
        public static void RemoveFilter(PluginFilters filterKey, Type type, string methodName)
        {
           PluginStorageService.RemoveHook(filterKey, type, methodName);
        }

        /// <summary>
        /// Check if the filter is added
        /// </summary>
        /// <param name="filterKey"></param>
        /// <param name="type"></param>
        /// <param name="methodName">A public method</param>
        /// <returns></returns>
        public static bool IsFilterAdded(PluginFilters filterKey, Type type, string methodName)
        {
            return PluginStorageService.IsHookAdded(filterKey, type, methodName);
        }

        /// <summary>
        /// Get all the filter hooks
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static Collection<Plugin> GetHooks(PluginFilters filterKey)
        {
            return PluginStorageService.GetHooks(filterKey);
        }

        /// <summary>
        /// Get the storage
        /// </summary>
        public static PluginStorageService<PluginFilters> FilterStorage
        {
            get { return PluginStorageService.Storage; }
        }
    }
}
