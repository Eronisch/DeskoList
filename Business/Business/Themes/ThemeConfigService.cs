using Core.Business.ThirdParty;

namespace Core.Business.Themes
{
    class ThemeConfigService : AbstractConfigService
    {
        public ThemeConfigService() : base("Themes", "TempThemes") { }
    }
}
