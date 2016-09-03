using System.Collections.Generic;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Manager for firing filter actions
    /// </summary>
    public static class PluginFilterActivateService
    {
        private static readonly PluginActivateService<PluginFilters> PluginActivateService;

        static PluginFilterActivateService()
        {
            PluginActivateService = new PluginActivateService<PluginFilters>(PluginFilterStorageService.FilterStorage);
        }

        public static IEnumerable<string> FireHook(PluginFilters pluginFilter, params object[] values)
        {
            return PluginActivateService.FireFilter(pluginFilter, values);
        }
    }
}
