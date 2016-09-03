using System;
using System.Reflection;
using System.Web.Mvc;
using Core.Business.Plugin;
using Database.Entities;

namespace Plugin
{
    public class PluginController : Controller
    {
        private readonly string _callingAssembly;
        private readonly Lazy<Plugins> _plugin;
        private readonly PluginConfigService _pluginConfigService;

        public PluginController()
        {
            PluginService pluginService = new PluginService();
            this._callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
            this._plugin = new Lazy<Plugins>(() => pluginService.GetByNameSpace(this._callingAssembly));
            this._pluginConfigService = new PluginConfigService();
        }

        public string GetPluginPath(string pathFile) =>
            $"/{this._pluginConfigService.GetRelativeAreaPath(this.PluginAreaName)}/{pathFile}";

        public string PluginAreaName =>
            this._plugin.Value.Area;

        public int PluginId =>
            this._plugin.Value.Id;
    }
}
