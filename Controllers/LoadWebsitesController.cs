using System.Web.Mvc;
using Core.Business.Settings;
using Core.Business.Websites;
using Topsite.Models;

namespace Topsite.Controllers
{
    public class LoadWebsitesController : Controller
    {
        private readonly RankingService _rankingService;
        private readonly SettingsService _settingsService;

        public LoadWebsitesController()
        {
            _rankingService = new RankingService();
            _settingsService = new SettingsService();
        }

        public ActionResult Index(int page, int startIndex, string search = null, int? categoryid = null)
        {
            var bundleWebsiteRanking = new BundleWebsitesRanking();

            int nextPage = page+1;

            if (search == null && categoryid == null)
            {
                bundleWebsiteRanking.Websites = _rankingService.GetWebsites(nextPage);
            }
            else if (categoryid != null && search == null)
            {
                bundleWebsiteRanking.Websites = _rankingService.GetWebsites(nextPage, (int)categoryid);
            }
            else
            {
                bundleWebsiteRanking.Websites = _rankingService.GetWebsites(nextPage, search);
            }

            bundleWebsiteRanking.StartIndex = startIndex;
            bundleWebsiteRanking.IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled();
            bundleWebsiteRanking.IsThumbnailEnabled = _settingsService.IsCreateThumbnailsEnabled();

            return View(bundleWebsiteRanking);
        }
    }
}