using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Plugin;
using Core.Business.Settings;
using Core.Business.Websites;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Websites;
using Topsite.Models;
using Web.Account;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class WebsitesController : AdminController
    {
        private readonly WebsiteService _websiteService;
        private readonly WebsiteStatisticsService _websiteStatisticsService;
        private readonly SettingsService _settingsService;
        private readonly Web.Category.CategoryService _categoryService;

        public WebsitesController()
        {
            _websiteService = new WebsiteService();
            _websiteStatisticsService = new WebsiteStatisticsService();
            _settingsService = new SettingsService();
            _categoryService = new Web.Category.CategoryService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetWebsites()
        {
            var websites = _websiteService.GetAllWebsites(true);

            var json = from website in websites
                       select new
                       {
                           Id = website.Id,
                           Title = website.Title,
                           Url = website.Url,
                           Date = website.DateAdded.ToString(CultureInfo.InvariantCulture),
                           View = BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.View, ControllerContext.RequestContext.GetActionRoute("View", "Websites", new { id = website.Id }),
                                    BootstrapButtonType.Info,
                                    BootstrapSize.ExtraSmall),
                           Edit =
                               BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "Websites", new { id = website.Id }),
                                   BootstrapButtonType.Warning,
                                   BootstrapSize.ExtraSmall),
                           Delete =
                               BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Websites", new { id = website.Id }),
                                   BootstrapButtonType.Error, BootstrapSize.ExtraSmall, new Dictionary<string, string>
                                   {
                                       {"id", "deleteLink"}
                                   })
                       };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(int id, bool isPartialView = false)
        {
            var website = _websiteService.GetWebsiteModel(id);

            if (website == null) { this.SetError(Websites.WebsiteNotFound); return RedirectToAction("Index"); }

            var statistics = _websiteStatisticsService.GetStatisticsFromWebsite(website.Id, isMonths: true);

            return View(new WebsiteViewBundleModel
            {
                Website = website,
                Statistics = statistics,
                IsPartialView = isPartialView
            });
        }

        public ActionResult Edit(int id)
        {
            var website = _websiteService.GetWebsiteModel(id);

            if (website == null) { this.SetError(Websites.WebsiteNotFound); return RedirectToAction("Index"); }

            return View(new EditWebsiteModel
           {
               BannerUrl = website.BannerUrl,
               Categories = _categoryService.GetListItemCategories(website.CategoryId),
               Category = website.CategoryId,
               Description = website.Description,
               Id = website.Id,
               Keywords = website.Keywords,
               Title = website.Title,
               Url = website.Url,
               UseBannerUrl = !website.HasFileBanner,
               IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled(),
               ServerIp = website.ServerIp,
               ServerPort = website.ServerPort,
               IsSponsored = website.IsSponsored
           });
        }

        [HttpPost]
        public ActionResult Edit(EditWebsiteModel editWebsiteModel)
        {
            if (ModelState.IsValid)
            {
                var websiteValidationResult = _websiteService.ValidateWebsite(editWebsiteModel.BannerFile?.InputStream, editWebsiteModel.BannerFile?.FileName,
                    editWebsiteModel.BannerUrl, editWebsiteModel.Url, editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl, false);

                if (!websiteValidationResult.IsSuccess) { GoToEditView(editWebsiteModel, websiteValidationResult.ErrorMessage); }

                var pluginResult = PluginHookActivateService.FireHook(PluginHooks.EditWebsite, LoginHelper.GetUserId(), editWebsiteModel.Title,
                    editWebsiteModel.Description, editWebsiteModel.Url, editWebsiteModel.BannerUrl,
                    editWebsiteModel.Keywords, editWebsiteModel.Category, editWebsiteModel.BannerFile,
                    editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, !editWebsiteModel.UseBannerUrl, editWebsiteModel.IsSponsored);

                if (!pluginResult.IsSuccess) { GoToEditView(editWebsiteModel, pluginResult.ErrorMessage); }

                this.SetSuccess(Websites.WebsiteSuccessfullyEdited);

                _websiteService.UpdateWebsite(editWebsiteModel.Id, editWebsiteModel.Title, editWebsiteModel.Description, editWebsiteModel.Keywords, editWebsiteModel.Category, editWebsiteModel.Url, editWebsiteModel.BannerUrl, editWebsiteModel.ServerIp, editWebsiteModel.ServerPort, editWebsiteModel.BannerFile?.InputStream, editWebsiteModel.BannerFile?.FileName, !editWebsiteModel.UseBannerUrl, editWebsiteModel.IsSponsored);

                return RedirectToAction("Index");
            }

            editWebsiteModel.Categories = _categoryService.GetListItemCategories(editWebsiteModel.Category);
            editWebsiteModel.IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled();

            return View(editWebsiteModel);
        }

        public ActionResult Delete(int id)
        {
            var website = _websiteService.GetWebsiteModel(id);

            if (website == null) { this.SetError(Websites.WebsiteNotFound); return RedirectToAction("Index"); }

            _websiteService.DeleteWebsite(id);

            this.SetSuccess(Websites.WebsiteSuccessfullyDeleted);

            return RedirectToAction("Index");
        }

        public string GetWebsiteTitle(int id)
        {
            var website = _websiteService.GetWebsiteModel(id);

            if (website == null) { return string.Empty; }

            return website.Title;
        }

        #region Private Methods

        public ActionResult GoToEditView(EditWebsiteModel editWebsiteModel, string errorMessage)
        {
            editWebsiteModel.Categories = _categoryService.GetListItemCategories(editWebsiteModel.Category);
            editWebsiteModel.IsMonitoringEnabled = _settingsService.IsUserServerMonitoringEnabled();

            this.SetError(errorMessage);

            return View(editWebsiteModel);
        }

        #endregion
    }
}

