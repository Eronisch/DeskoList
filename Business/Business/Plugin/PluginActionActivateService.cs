namespace Core.Business.Plugin
{
    /// <summary>
    /// Manager for activating plugins on specific action filters
    /// </summary>
    public static class PluginActionActivateService
    {
        private static readonly PluginActivateService<PluginActions> PluginActivateService;

        static PluginActionActivateService()
        {
            PluginActivateService = new PluginActivateService<PluginActions>(PluginActionStorageService.ActionStorage);
        }

        public static void FireAction(PluginActions pluginAction, params object[] values)
        {
            PluginActivateService.FireAction(pluginAction, values);
        }
    }
}
