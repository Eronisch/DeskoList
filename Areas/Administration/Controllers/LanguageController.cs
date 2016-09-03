using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Admin;
using Core.Business.Languages;
using Core.Business.Settings;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Language;
using Web.Cookies;
using Web.Messages;

namespace Topsite.Areas.Administration.Controllers
{
    public class LanguageController : AdminController
    {
        private const string CookieKey = "languageInstall";
        private const string CookieKeyNamePath = "path";
        private const string CookieKeyNameAbbreviation = "abbreviation";

        public ActionResult Index(bool compare = false)
        {
            var languageService = new LanguageService();
            var settingsService = new SettingsService(); ;

            return View(new InstallLanguageModel(compare, languageService.GetInstalledLanguages(), settingsService.GetActiveCultureName()));
        }

        [HttpPost]
        public ActionResult Index(InstallLanguageModel installLanguageModel)
        {
            if (ModelState.IsValid)
            {
                var languageInstaller = new LanguageInstallerService(installLanguageModel.LanguageDll.InputStream, installLanguageModel.LanguageDll.FileName);

                languageInstaller.Install();

                CookieService.SetCookie(CookieKey, new NameValueCollection
                {
                    {CookieKeyNamePath, languageInstaller.GetRelativeDllFilePath()},
                    {CookieKeyNameAbbreviation, languageInstaller.GetAbbreviation()}
                });

                this.SetSuccess(Language.LanguageSuccessfullyAdded);
            }
            else
            {
                this.SetError(ModelState.Values.First(x => x.Errors.Any()).Errors.First().ErrorMessage);
            }

            return RedirectToAction("Index", "Language", new { compare = ModelState.IsValid });
        }

        public ActionResult Delete(int id)
        {
            var languageDeleteService = new LanguageDeleteService();

            languageDeleteService.Delete(id);

            this.SetSuccess(Language.LanguageSuccessfullyDeleted);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> CompareLanguage()
        {
            var cookie = CookieService.GetCookieValues(CookieKey);

            CookieService.RemoveCookie(CookieKey);

            return await Task.Run(() =>
            {
                if (cookie.Count > 0)
                {
                    var compareLanguageService = new CompareLanguageService(cookie.Get(CookieKeyNamePath),
                        cookie.Get(CookieKeyNameAbbreviation));

                    return PartialView("~/Areas/Administration/Views/Shared/_CompareLanguage.cshtml", compareLanguageService.GetDifferences());
                }

                return null;
            });
        }
    }
}
