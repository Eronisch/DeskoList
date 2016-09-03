using System.Web.Mvc;
using Core.Business.Settings;
using Core.Business.Websites;
using Topsite.Models;

namespace Topsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var rankingService = new RankingService();
            var settingsService = new SettingsService();

            return View(new BundleWebsitesRanking
            {
                StartIndex = 1,
                Websites = rankingService.GetWebsites(),
                IsMonitoringEnabled = settingsService.IsUserServerMonitoringEnabled(),
                IsThumbnailEnabled = settingsService.IsCreateThumbnailsEnabled()
            });
        }
    }
}