using System.Web.Mvc;
using Core.Models.BreadCrumbs;
using Database.Entities;

namespace Web.Breadcrumbs
{
    /// <summary>
    /// Set and retrieve breadcrumbs information from admin pages
    /// Uses tempdata
    /// </summary>
    public static class AdminPageBreadCrumbsHelper
    {
        private const string KeyBreadCrumbs = "adminBreadCrumbs";

        /// <summary>
        /// Adds the breadrumb to the tempdata from the controller
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="breadcrumbs"></param>
        public static void SetBreadCrumbs(ControllerBase controller, AdminBreadcrumbs breadcrumbs)
        {
            if (breadcrumbs != null)
            {
                controller.TempData[KeyBreadCrumbs] = new BreadCrumbsModel
                {
                    Action = breadcrumbs.Action,
                    Controller = breadcrumbs.Controller,
                    Description = Localization.Services.LocalizationService.GetValue(breadcrumbs.LocalizationBase, breadcrumbs.LocalizedDescription),
                    FriendlyAction = Localization.Services.LocalizationService.GetValue(breadcrumbs.LocalizationBase, breadcrumbs.LocalizedActionFriendlyName),
                    FriendlyController = Localization.Services.LocalizationService.GetValue(breadcrumbs.LocalizationBase, breadcrumbs.LocalizedControllerFriendlyName),
                    Title = Localization.Services.LocalizationService.GetValue(breadcrumbs.LocalizationBase, breadcrumbs.LocalizedTitle),
                    Icon = breadcrumbs.Icon
                };
            }
        }

        /// <summary>
        /// Gets the breadcrumb from the tempdata
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static BreadCrumbsModel GetBreadCrumbs(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.Controller.TempData[KeyBreadCrumbs] as BreadCrumbsModel;
        }

        /// <summary>
        /// Get the title from the tempdata breadcrumb
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static string GetBreadCrumbsTitle(this HtmlHelper htmlHelper)
        {
            var breadcrumbs = htmlHelper.ViewContext.Controller.TempData[KeyBreadCrumbs];

            return breadcrumbs != null ? ((BreadCrumbsModel) breadcrumbs).Title : "??";
        }
    }
}
