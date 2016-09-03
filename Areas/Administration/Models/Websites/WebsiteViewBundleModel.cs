using Core.Models.Statistics;
using Core.Models.Websites;

namespace Topsite.Areas.Administration.Models.Websites
{
    public class WebsiteViewBundleModel
    {
        public WebsiteModel Website { get; set; }
        public BundleStatisticsModel Statistics { get; set; }
        public bool IsPartialView { get; set; }
    }
}