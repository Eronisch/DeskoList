using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Plugin;
using Core.Business.Settings;
using Core.Business.Software;
using Topsite.Areas.Administration.Models.Plugin;
using Topsite.Areas.Administration.Models.Settings;
using Topsite.Areas.Administration.Models.Updates;
using Web.Messages;

namespace Topsite.Areas.Administration.Controllers
{
    public class PluginsController : AdminController
    {
        private readonly PluginService _pluginService;
        private readonly PluginUpdateService _pluginUpdateService;
        private readonly SettingsService _settingsService;

        public PluginsController()
        {
            _pluginService = new PluginService();
            _pluginUpdateService = new PluginUpdateService();
            _settingsService = new SettingsService();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            DisplayCookieMessage();
        }

        public ActionResult Index()
        {
            return View(new PluginModel(_pluginService.GetPlugins()));
        }

        public ActionResult Active()
        {
            return View("Index", new PluginModel(_pluginService.GetActivePlugins()));
        }

        public ActionResult InActive()
        {
            return View("Index", new PluginModel(_pluginService.GetInActivePlugins()));
        }

        [HttpPost]
        public ActionResult Install(PluginModel pluginModel)
        {
            var pluginCookieService = new PluginCookieService();

            if (ModelState.IsValid)
            {
                var pluginInstallerService = new PluginInstallerService();

                var pluginResult = pluginInstallerService.Install(pluginModel.PluginFile.InputStream);

                if (pluginResult.IsSuccess)
                {
                    pluginCookieService.Set(Localization.Languages.Admin.Controllers.Plugin.SuccessfullyInstalled, true);
                    
                }
                else
                {
                    pluginCookieService.Set(pluginResult.ErrorMessage, false);
                }
            }
            else
            {
                this.SetError(ModelState.Values.First(x => x.Errors.Any()).Errors.First().ErrorMessage);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var pluginCookieService = new PluginCookieService();
            var pluginUninstallService = new PluginUninstallService();

            var resultUninstall = pluginUninstallService.Uninstall(id);

            if (resultUninstall.IsSuccess)
            {
                pluginCookieService.Set(Localization.Languages.Admin.Controllers.Plugin.SuccessfullyDeleted, true);
            }
            else
            {
                pluginCookieService.Set(resultUninstall.ErrorMessage, false);
            }

            return RedirectToAction("Index");
        }

        public ActionResult UpdateStatus(int id, bool status)
        {
            var pluginCookieService = new PluginCookieService();
            var pluginEnableService = new PluginEnableService();

            bool isSuccess = pluginEnableService.UpdateStatus(id, status);

            if (isSuccess && status)
            {
                pluginCookieService.Set(Localization.Languages.Admin.Controllers.Plugin.SuccessfullyEnabled, true);
            }
            else if (isSuccess && !status)
            {
                pluginCookieService.Set(Localization.Languages.Admin.Controllers.Plugin.SuccessfullyDisabled, true);
            }
            else
            {
                pluginCookieService.Set(Localization.Languages.Admin.Controllers.Plugin.NotFound, false);
            }

            return RedirectToAction("Index");
        }

        #region Update Actions

        public int GetUpdatingStatus()
        {
            return (int)_pluginUpdateService.GetUpdatingStatus();
        }

        public JsonResult WasUpdatingSuccess()
        {
            return Json(_pluginUpdateService.IsUpdatedSuccessfully(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAvailableSystemUpdates()
        {
            var versionComparer = new VersionOrderingService();

            string deskoVersion = _settingsService.GetVersion();

            return View("_TabbedUpdates", _pluginUpdateService.GetPluginsWithUpdates().Select(plugin => new TabbedUpdateTableModel
            {
                Id = plugin.Id,
                Name = plugin.Name,
                Updates = plugin.PluginOpenUpdates.Select(update => new UpdateTableModel
                {
                    Name = update.Name,
                    Description = update.Description,
                    IsDownloaded = update.IsDownloaded,
                    Version = update.Version,
                    DeskoVersion = update.DeskoVersion,
                    IsVersionIncorrect = !versionComparer.Equals(update.DeskoVersion, deskoVersion),
                    IsDownloading = PluginDownloadManager.Manager.IsDownloading(update.Version),
                    Progress = update.IsDownloaded ? 100 : PluginDownloadManager.Manager.GetProgress(update.Version)
                }).OrderBy(w => w.Version, versionComparer)
            }).ToList());
        }

        public ActionResult GetDateChecked()
        {
            return View("_DateLastChecked", _pluginUpdateService.GetLastCheckedForUpdates());
        }

        public void InstallUpdates()
        {
            _pluginUpdateService.UpdateInBackground();
        }

        public bool IsCheckingSoftware()
        {
            return _pluginUpdateService.IsCheckingForUpdates();
        }

        public bool IsInstallingSoftware()
        {
            return _pluginUpdateService.IsInstallingUpdates();
        }

        public bool IsDownloadingSoftware()
        {
            return _pluginUpdateService.IsDownloadingUpdates();
        }

        public JsonResult GetDownloadProgress()
        {
            return Json(new
            {
                isDownloading = _pluginUpdateService.IsDownloadingUpdates(),
                downloads = from download in PluginDownloadManager.Manager.GetDownloads()
                            select new
                            {
                                version = download.Version,
                                progress = download.ProgressPercentage,
                                id = download.ExtraId
                            }
            }, JsonRequestBehavior.AllowGet);
        }


        public int GetUpdateAvailableStatus()
        {
            int amountUpdatesAvailable = _pluginUpdateService.GetAmountUpdatesAvailable();

            if (amountUpdatesAvailable == 0)
            {
                return (int)UpdateAvailableStatus.NoUpdateAvailable;
            }

            if (amountUpdatesAvailable == 1)
            {
                return (int)UpdateAvailableStatus.SingleUpdateAvailable;
            }

            return (int)UpdateAvailableStatus.MultipleUpdatesAvailable;
        }

        #endregion

        #region Private Methods

        private void DisplayCookieMessage()
        {
            var pluginCookieService = new PluginCookieService();

            var cookie = pluginCookieService.Get();

            if (cookie != null)
            {
                if (cookie.IsSuccess)
                {
                    this.SetSuccess(cookie.Message);
                }
                else
                {
                    this.SetError(cookie.Message);
                }

                pluginCookieService.Remove();
            }
        }

        #endregion

    }
}
