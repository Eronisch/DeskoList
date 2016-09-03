using System.Web.Mvc;
using Core.Business.Languages;
using Core.Business.Pages;
using Core.Business.Settings;
using Core.Business.Url;
using Topsite.Models;
using Web.Language;
using Web.Messages;
using Web.Search;

namespace Topsite.Controllers
{
    public class SharedController : Controller
    {
        private readonly SettingsService _settingsService;

        public SharedController()
        {
            _settingsService = new SettingsService();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 900, VaryByParam = "none")]
        public ActionResult GetMenu()
        {
            var navigationService = new NavigationService();

            return PartialView("/Themes/" + _settingsService.GetActiveThemeMap() + "/Views/Partials/_Navigation.cshtml", navigationService.GetNavigation());
        }

        [ChildActionOnly]
        public ActionResult GetLanguages()
        {
            var languageService = new LanguageService();

            return PartialView("/Themes/" + _settingsService.GetActiveThemeMap() + "/Views/Partials/_Languages.cshtml",languageService.GetInstalledLanguages());
        }
        
        public ActionResult SetLanguage(string culture)
        {
            var languageService = new LanguageService();

            if (languageService.IsLanguageSupported(culture))
            {
                var languageCookieService = new LanguageCookieService();

                languageCookieService.SetCookie(culture);
            }
            else
            {
                this.SetError(Localization.Languages.Controllers.Shared.LanguageNotSupported);
            }
            
            if (Request.UrlReferrer != null)
            {
                if (Url.IsLocalUrl(Request.UrlReferrer.PathAndQuery))
                {
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public ActionResult GetSearch()
        {
            return PartialView("~/Themes/LightBlue/Views/Partials/_Search.cshtml", new SearchModel(SearchHelper.GetSearchText(TempData), UrlHelpers.GetCurrentBaseUrl()));
        }
    }
}