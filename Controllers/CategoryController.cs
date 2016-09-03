using System.Web.Mvc;
using Core.Business.Categories;
using Core.Business.Settings;
using Core.Business.Websites;
using Localization.Languages.Controllers;
using Topsite.Models;
using Web.Messages;
using Web.Seo;

namespace Topsite.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        [HttpGet]
        public ActionResult Index(int categoryId)
        {
            var rankingService = new RankingService();
            var categoryService = new CategoryService();
            var settingsService = new SettingsService();

            var bundleModels = new BundleCategoryModel
            {
                WebsitesRanking = new BundleWebsitesRanking
                {
                    IsMonitoringEnabled = settingsService.IsUserServerMonitoringEnabled(),
                    IsThumbnailEnabled = settingsService.IsCreateThumbnailsEnabled()
                }
            };

            var categorySeo = categoryService.GetSeoByCategoryId(categoryId);

            if (categorySeo != null)
            {
                bundleModels.CategoryId = categorySeo.CategoryId;
                this.UpdateSeo(categorySeo.Name, categorySeo.Name);

                bundleModels.WebsitesRanking.Websites = rankingService.GetWebsites(1, categorySeo.CategoryId);
            }
            else
            {
                this.SetError(Category.NoCategoryFound);

                return RedirectToAction("Index", "Home");
            }

            return View(bundleModels);
        }
    }
}