using Core.Business.ThirdParty;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Plugin config that is used for retrieving file (locations)
    /// </summary>
    public class PluginConfigService : AbstractConfigService
    {
        public PluginConfigService() : base("Plugins", "TempPlugins") { }
    }
}
