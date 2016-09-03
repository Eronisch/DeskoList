using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Date;
using Core.Business.Languages;
using Core.Business.Settings;
using Core.Business.Software;
using Core.Business.Themes;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Settings;
using Topsite.Models;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class SettingsController : AdminController
    {
        private readonly SettingsService _settingsService;
        private readonly ThemeService _themeService;
        private readonly SoftwareUpdateService _softwareUpdateService;
        private readonly LanguageService _languageService;

        public SettingsController()
        {
            _settingsService = new SettingsService();
            _themeService = new ThemeService();
            _softwareUpdateService = new SoftwareUpdateService();
            _languageService = new LanguageService();
        }

        public ActionResult Index()
        {
            var settings = _settingsService.GetSettings();

            return View(new SettingsModel
            {
                CronMonitorServer = settings.CronPingServer,
                CronWebsiteThumbnail = settings.CronWebsiteThumbnail,
                CronjobInAndOut = settings.CronjobInAndOut,
                CronjobUserStatisticsEmail = settings.CronjobUserStatisticsEmail,
                Description = settings.Description,
                EmailVerification = settings.IsEmailVerificationRequired,
                IsCreatingWebsiteThumbnailsEnabled = settings.IsCreateThumbnailsEnabled,
                IsMaintenance = settings.Maintenance,
                IsMonitorEnabled = settings.IsPingEnabled,
                Keywords = settings.Keywords,
                RecaptchaSecretKey = settings.RecaptchaSecretKey,
                RecaptchaSiteKey = settings.RecaptchaSiteKey,
                ShowAmountWebsites = settings.ShowAmountWebsites,
                SiteSlogan = settings.SiteSlogan,
                SiteTitle = settings.SiteTitle,
                ThemeId = settings.ThemeId,
                Title = settings.Title,
                Url = settings.Url,
                Themes = GetThemes(settings.ThemeId),
                AutoUpdate = settings.AutoUpdate,
                CronUpdate = settings.CronUpdate,
                IsEmailingUserStatisticsEnabled = settings.IsEmailingUserStatisticsEnabled,
                IsResetInAndOutsEnabled = settings.IsResetInAndOutsEnabled,
                Timezone = settings.Timezone,
                Timezones = GetTimeZones(_settingsService.GetActiveTimeZone()),
                IsUpdateWhenIncorrectVersionEnabled = settings.UpdateWhenIncorrectVersion,
                LanguageId = settings.LanguageId,
                Languages = GetLanguages(_settingsService.GetActiveLanguageId()),
                IsAdminVerificationRequired = settings.IsAdminVerificationRequired
            });
        }

        [HttpPost]
        public ActionResult Index(SettingsModel settingsModel)
        {
            if (ModelState.IsValid)
            {
                _settingsService.UpdateSettings(settingsModel.SiteTitle, settingsModel.Title, settingsModel.Description, settingsModel.Keywords,
                    (byte)settingsModel.ShowAmountWebsites, settingsModel.Url, settingsModel.CronjobUserStatisticsEmail, settingsModel.CronjobInAndOut, settingsModel.CronUpdate,
                    settingsModel.CronWebsiteThumbnail, settingsModel.CronMonitorServer, settingsModel.LanguageId, settingsModel.Timezone, settingsModel.SiteSlogan, settingsModel.ThemeId, settingsModel.RecaptchaSecretKey,
                    settingsModel.RecaptchaSiteKey, settingsModel.IsMonitorEnabled, settingsModel.EmailVerification, settingsModel.AutoUpdate, settingsModel.IsEmailingUserStatisticsEnabled, settingsModel.IsCreatingWebsiteThumbnailsEnabled, settingsModel.IsResetInAndOutsEnabled, settingsModel.IsUpdateWhenIncorrectVersionEnabled, settingsModel.IsAdminVerificationRequired);

                this.SetSuccess(Settings.SettingsSuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            settingsModel.Themes = GetThemes(settingsModel.ThemeId);
            settingsModel.Timezones = GetTimeZones(settingsModel.Timezone);
            settingsModel.Languages = GetLanguages(settingsModel.LanguageId);

            return View(settingsModel);
        }


        public ActionResult Updates()
        {
            return View(new SettingsUpdateModel
            {
                LastCheckedForUpdates = _softwareUpdateService.GetLastCheckedForUpdates(),
                AmountUpdatesAvailable = _softwareUpdateService.GetAvailableUpdatesFromDatabase().Count(),
                StatusUpdates = (UpdateAvailableStatus)GetUpdateAvailableStatus(),
                IsChecking = _softwareUpdateService.IsCheckingForUpdates(),
                IsInstalling = _softwareUpdateService.IsInstallingUpdates(),
                IsDownloading = _softwareUpdateService.IsDownloadingUpdates()
            });
        }

        public ActionResult GetAvailableSystemUpdates()
        {
            var versionComparer = new VersionOrderingService();

            return View("_SoftwareAvailableUpdates", (from update in _softwareUpdateService.GetAvailableUpdatesFromDatabase()
                                                      select new SoftwareUpdateTableModel
                                                      {
                                                          Name = update.Name,
                                                          Description = update.Description,
                                                          IsDownloaded = update.IsDownloaded,
                                                          Version = update.Version,
                                                          IsDownloading = SoftwareDownloadManager.Manager.IsDownloading(update.Version),
                                                          Progress = update.IsDownloaded ? 100 : SoftwareDownloadManager.Manager.GetProgress(update.Version)
                                                      }).OrderByDescending(update => update.Version, versionComparer));
        }

        public ActionResult GetDateChecked()
        {
            return View("_DateLastChecked", _softwareUpdateService.GetLastCheckedForUpdates());
        }

        public JsonResult GetDownloadProgress()
        {
            return Json(new
            {
                isDownloading = _softwareUpdateService.IsDownloadingUpdates(),
                downloads = from download in SoftwareDownloadManager.Manager.GetDownloads()
                            select new
                            {
                                version = download.Version,
                                progress = download.ProgressPercentage,
                                id = download.Version,
                            }
            }, JsonRequestBehavior.AllowGet);
        }

        public int GetUpdatingStatus()
        {
            return (int)_softwareUpdateService.GetUpdatingStatus();
        }

        public void InstallUpdates()
        {
            _softwareUpdateService.UpdateInBackground();
        }

        public JsonResult WasUpdatingSuccess()
        {
            return Json(_softwareUpdateService.IsUpdatedSuccessfully(), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GlobalMessageUpdateAvailable()
        {
            return PartialView("_GlobalMessageUpdateAvailable", new GlobalMessageUpdateModel
            {
                IsSettingsUpdatePage = RouteProvider.GetController().Equals("Settings", StringComparison.CurrentCultureIgnoreCase) && RouteProvider.GetAction().Equals("Updates", StringComparison.CurrentCultureIgnoreCase),
                UpdateStatus = (UpdateAvailableStatus)GetUpdateAvailableStatus()
            });
        }

        public bool IsCheckingSoftware()
        {
            return _softwareUpdateService.IsCheckingForUpdates();
        }

        public bool IsInstallingSoftware()
        {
            return _softwareUpdateService.IsInstallingUpdates();
        }

        public bool IsDownloadingSoftware()
        {
            return _softwareUpdateService.IsDownloadingUpdates();
        }

        /// <summary>
        /// Check in the database if there are updates available
        /// </summary>
        /// <returns>0: There are no updates available, 1: There is one update available, 2: There are multiple updates available</returns>
        public int GetUpdateAvailableStatus()
        {
            int amountUpdatesAvailable = _softwareUpdateService.GetAvailableUpdatesFromDatabase().Count();

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

        private IEnumerable<SelectListItem> GetThemes(int selectedThemeId)
        {
            return _themeService.GetThemes().Select(theme => new SelectListItem
            {
                Selected = theme.Id == selectedThemeId,
                Text = theme.ThemeName,
                Value = theme.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        private IEnumerable<SelectListItem> GetLanguages(int languageId)
        {
            int activeLanguageId = _settingsService.GetActiveLanguageId();

            return _languageService.GetInstalledLanguages().Select(l => new SelectListItem
             {
                 Selected = activeLanguageId == languageId,
                 Text = l.Name,
                 Value = l.Id.ToString()
             });
        }

        private IEnumerable<SelectListItem> GetTimeZones(string timezoneId)
        {
            return DateHelper.GetTimeZones().Select(tz => new SelectListItem
            {
                Text = tz.DisplayName,
                Value = tz.Id,
                Selected = timezoneId == tz.Id
            });
        }
    }
}
