using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;

namespace Topsite.Areas.Administration.Models.Widgets
{
    public class BundleInstalledWidgets
    {
        public IEnumerable<SelectListItem> Themes { get; set; }
        public IList<InstalledWidget> InstalledWidgets { get; set; }
        public WidgetInstallModel WidgetInstallModel { get; set; }

        public BundleInstalledWidgets()
        {
            Themes = new Collection<SelectListItem>();
            InstalledWidgets = new List<InstalledWidget>();
            WidgetInstallModel = new WidgetInstallModel();
        }
    }
}