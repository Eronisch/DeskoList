using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Settings;
using Core.Business.Software;
using Core.Business.Themes;
using Topsite.Areas.Administration.Models.Settings;
using Topsite.Areas.Administration.Models.Themes;
using Topsite.Areas.Administration.Models.Updates;
using Web.Messages;

namespace Topsite.Areas.Administration.Controllers
{
    public class ThemesController : AdminController
    {
        private readonly ThemeService _themeService;
        private readonly SettingsService _settingsService;
        private readonly ThemeUpdateService _themeUpdateService;

        public ThemesController()
        {
            _themeService = new ThemeService();
            _settingsService = new SettingsService();
            _themeUpdateService = new ThemeUpdateService();
        }

        public ActionResult Index()
        {
            int activeThemeId = _settingsService.GetActiveThemeId();

            return View(new BundleViewThemesModel
            {
                Themes = _themeService.GetThemes().Select(theme => new InstalledThemeModel
                {
                    Id = theme.Id,
                    Author = theme.AuthorName,
                    AuthorUrl = theme.AuthorUrl,
                    Description = theme.Description,
                    ImagePath = string.Format("/Themes/{0}/{1}", theme.FolderName, theme.Image),
                    IsActive = activeThemeId == theme.Id,
                    ThemeName = theme.ThemeName,
                    Version = theme.Version
                }).OrderByDescending(t => t.IsActive).ThenBy(t => t.ThemeName)
            });
        }

        public ActionResult Activate(int id)
        {
            if (_themeService.GetThemeById(id) != null)
            {
                _settingsService.SetActiveTheme(themeId: id);

                this.SetSuccess(Localization.Languages.Admin.Controllers.Themes.ThemeActivatedSuccessfully);
            }
            else
            {
                this.SetError(Localization.Languages.Admin.Controllers.Themes.ThemeNotFound);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Uninstall(int id)
        {
            var themeUninstallerService = new ThemeUninstallerService();

            var uninstallResult = themeUninstallerService.Uninstall(id);

            if (!uninstallResult.IsSuccess)
            {
                this.SetError(uninstallResult.ErrorMessage);
            }
            else
            {
                this.SetSuccess(Localization.Languages.Admin.Controllers.Themes.ThemeUninstalledSuccessfully);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Install(InstallThemeModel installThemeModel)
        {
            if (ModelState.IsValid)
            {
                var themeInstallerService = new ThemeInstallerService();

                var themeInstallerResult = themeInstallerService.Install(installThemeModel.Theme.InputStream);

                if (!themeInstallerResult.IsSuccess)
                {
                    this.SetError(themeInstallerResult.ErrorMessage);
                }
                else
                {
                    this.SetSuccess(Localization.Languages.Admin.Controllers.Themes.ThemeInstalledSuccessfully);
                }
            }
            else
            {
                this.SetError(ModelState.Values.First(x => x.Errors.Any()).Errors.First().ErrorMessage);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Updates()
        {
            return View(new SettingsUpdateModel
            {
                LastCheckedForUpdates = _themeUpdateService.GetLastCheckedForUpdates(),
                AmountUpdatesAvailable = _themeUpdateService.GetAmountUpdatesAvailable(),
                StatusUpdates = (UpdateAvailableStatus)GetUpdateAvailableStatus(),
                IsChecking = _themeUpdateService.IsCheckingForUpdates(),
                IsInstalling = _themeUpdateService.IsInstallingUpdates(),
                IsDownloading = _themeUpdateService.IsDownloadingUpdates()
            });
        }

        public ActionResult GetAvailableSystemUpdates()
        {
            var versionComparer = new VersionOrderingService();

            string deskoVersion = _settingsService.GetVersion();

            return View("_TabbedUpdates",
                _themeUpdateService.GetWidgetsWithUpdates().Select(theme => new TabbedUpdateTableModel
            {
                Id = theme.Id,
                Name = theme.ThemeName,
                Updates = theme.ThemeOpenUpdates.Select(update => new UpdateTableModel
                {
                    Name = update.Name,
                    Description = update.Description,
                    IsDownloaded = update.IsDownloaded,
                    Version = update.Version,
                    DeskoVersion = update.DeskoVersion,
                    IsVersionIncorrect = !versionComparer.Equals(update.DeskoVersion, deskoVersion),
                    IsDownloading = ThemeDownloadManager.Manager.IsDownloading(update.Version),
                    Progress = update.IsDownloaded ? 100 : ThemeDownloadManager.Manager.GetProgress(update.Version)
                }).OrderBy(w => w.Version, versionComparer)
            }).ToList());
        }

        #region Ajax Update Calls

        public int GetUpdatingStatus()
        {
            return (int)_themeUpdateService.GetUpdatingStatus();
        }

        public JsonResult WasUpdatingSuccess()
        {
            return Json(_themeUpdateService.IsUpdatedSuccessfully(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDateChecked()
        {
            return View("_DateLastChecked", _themeUpdateService.GetLastCheckedForUpdates());
        }

        public void InstallUpdates()
        {
            _themeUpdateService.UpdateInBackground();
        }

        public bool IsCheckingSoftware()
        {
            return _themeUpdateService.IsCheckingForUpdates();
        }

        public bool IsInstallingSoftware()
        {
            return _themeUpdateService.IsInstallingUpdates();
        }

        public bool IsDownloadingSoftware()
        {
            return _themeUpdateService.IsDownloadingUpdates();
        }

        /// <summary>
        /// Check in the database if there are updates available
        /// </summary>
        /// <returns>0: There are no updates available, 1: There is one update available, 2: There are multiple updates available</returns>
        public int GetUpdateAvailableStatus()
        {
            int amountUpdatesAvailable = _themeUpdateService.GetAmountUpdatesAvailable();

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

        public JsonResult GetDownloadProgress()
        {
            return Json(new
            {
                isDownloading = _themeUpdateService.IsDownloadingUpdates(),
                downloads = from download in ThemeDownloadManager.Manager.GetDownloads()
                            select new
                            {
                                version = download.Version,
                                progress = download.ProgressPercentage,
                                id = download.ExtraId,
                            }
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
