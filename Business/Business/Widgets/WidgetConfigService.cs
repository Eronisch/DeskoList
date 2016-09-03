using Core.Business.ThirdParty;

namespace Core.Business.Widgets
{
    /// <summary>
    /// Widget config that is used for retrieving file (locations)
    /// </summary>
    public class WidgetConfigService : AbstractConfigService
    {
        public WidgetConfigService() : base("Widgets", "TempWidgets") { }
    }
}
