using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Core.Business.Plugin
{
    public static class PluginHtmlSubscribeService
    {
        static PluginHtmlSubscribeService()
        {
            SubscribedPlugins = new Collection<PluginHtml>();    
        }

        private static readonly Collection<PluginHtml> SubscribedPlugins;

        public static void Subscribe(string controller, string action, string viewName)
        {
            string pluginNamespace = Assembly.GetCallingAssembly().GetName().Name;

            SubscribedPlugins.Add(new PluginHtml(controller, action, GetPluginAreaFromNamespace(pluginNamespace), viewName));
        }

        public static IEnumerable<PluginHtml> Get(string controller, string action)
        {
            return
                SubscribedPlugins.Where(
                    p =>
                        p.Controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase) &&
                        p.Action.Equals(action, StringComparison.CurrentCultureIgnoreCase));
        }

        private static string GetPluginAreaFromNamespace(string @namespace)
        {
            var pluginService = new PluginService();

            return pluginService.GetByNameSpace(@namespace).Area;
        }
    }
}
