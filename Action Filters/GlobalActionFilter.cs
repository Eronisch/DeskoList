using System;
using System.Web.Mvc;
using Core.Business.Pages;
using Core.Business.Settings;
using DatabaseXML;
using Localization.Services;
using Web.Route;
using Web.Seo;

namespace Topsite.Action_Filters
{
    public class GlobalActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RedirectIfNotInstalled(filterContext);

            if (LocalDatabaseSettingsService.Manager.IsInstalled)
            {
                if (IsPageRequest(filterContext))
                {
                    RedirectIfMaintenance(filterContext);

                    SetSeo(filterContext);
                }
            }
        }

        /// <summary>
        /// Checks if the request is a non ajax and isn't a child action request
        /// </summary>
        /// <returns></returns>
        private bool IsPageRequest(ActionExecutingContext filterContext)
        {
            return !filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.IsChildAction == false;
        }

        private void SetSeo(ActionExecutingContext filterContext)
        {
            var seoPagesService = new SeoPageService();

            var seo = seoPagesService.GetSeo(RouteProvider.GetController(), RouteProvider.GetAction());

            if (seo != null)
            {
                filterContext.Controller.SetSeo(LocalizationService.GetValue(seo.ResourceBaseName, seo.ResourceTitleName), LocalizationService.GetValue(seo.ResourceBaseName, seo.ResourceDescriptionName));
            }
        }

        private void RedirectIfMaintenance(ActionExecutingContext filterContext)
        {
            if (RouteProvider.GetController().Equals("maintenance", StringComparison.CurrentCultureIgnoreCase))
            {
                var settingsService = new SettingsService();

                if (settingsService.IsMaintenance())
                {
                    filterContext.Result = new RedirectResult("/Maintenance", false);
                }
            }
        }

        private void RedirectIfNotInstalled(ActionExecutingContext filterContext)
        {
            if (!LocalDatabaseSettingsService.Manager.IsInstalled && !RouteProvider.GetController().Equals("autoinstaller", StringComparison.CurrentCultureIgnoreCase))
            {
                filterContext.Result = new RedirectResult("/AutoInstaller", false);
            }
        }
    }
}