using System.Web.Mvc;
using Core.Business.Settings;
using Core.Business.Websites;
using Topsite.Models;
using Web.Search;

namespace Topsite.Controllers
{
    public class SearchController : Controller
    {
        private readonly RankingService _rankingService;
        private readonly SettingsService _settingsService;

        public SearchController()
        {
            _rankingService = new RankingService();
            _settingsService = new SettingsService();
        }

        public ActionResult Index(SearchModel searchModel)
        {
            SearchHelper.SetSearchedText(searchModel.Text, TempData);

            return View(new BundleWebsitesRanking
            {
                Websites = _rankingService.GetWebsites(1, searchModel.Text),
                IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled(),
                IsThumbnailEnabled = _settingsService.IsCreateThumbnailsEnabled()
            });
        }
    }
}