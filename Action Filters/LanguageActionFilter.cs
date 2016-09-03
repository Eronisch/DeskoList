using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using Core.Business.Settings;
using DatabaseXML;
using Web.Language;

namespace Topsite.Action_Filters
{
    public class LanguageActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!LocalDatabaseSettingsService.Manager.IsInstalled) { return; }

            if (!filterContext.IsChildAction)
            {
                var languageCookieService = new LanguageCookieService();

                string culture = languageCookieService.GetCookie();

                if (string.IsNullOrEmpty(culture))
                {
                    SetDefaultLanguage();
                }
                else
                {
                    SetCurrentThreadCulture(culture);
                }
            }
        }

        private void SetDefaultLanguage()
        {
            string activeCulture;

            using (var settingsService = new SettingsService())
            {
                activeCulture = settingsService.GetActiveCulture();
            }

            SetCurrentThreadCulture(activeCulture);
        }

        private void SetCurrentThreadCulture(string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
        }
    }
}