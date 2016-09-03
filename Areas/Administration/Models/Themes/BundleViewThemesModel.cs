using System.Collections.Generic;

namespace Topsite.Areas.Administration.Models.Themes
{
    public class BundleViewThemesModel
    {
        public IEnumerable<InstalledThemeModel> Themes { get; set; }
        public InstallThemeModel InstallThemeModel { get; set; }
    }
}