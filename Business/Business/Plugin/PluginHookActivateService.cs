using Core.Models;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Manager for firing hooks
    /// </summary>
    public static class PluginHookActivateService
    {
        private static readonly PluginActivateService<PluginHooks> PluginActivateService;

        static PluginHookActivateService()
        {
            PluginActivateService = new PluginActivateService<PluginHooks>(PluginHookStorageService.HookStorage);
        }

        /// <summary>
        /// Fire a specific hook
        /// </summary>
        /// <param name="pluginHook"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static ResultModel FireHook(PluginHooks pluginHook, params object[] values)
        {
            return PluginActivateService.FireHook(pluginHook, values);
        }
    }
}
