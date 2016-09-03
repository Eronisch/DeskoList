using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Settings;
using Core.Business.Software;
using Core.Business.Themes;
using Core.Business.Widgets;
using Database.Entities;
using Topsite.Areas.Administration.Models.Settings;
using Topsite.Areas.Administration.Models.Updates;
using Topsite.Areas.Administration.Models.Widgets;
using Web.Messages;
using Widgets = Localization.Languages.Admin.Controllers.Widgets;

namespace Topsite.Areas.Administration.Controllers
{
    public class WidgetsController : AdminController
    {
        private readonly WidgetService _widgetService;
        private readonly ThemeService _themeService;
        private readonly SettingsService _settingsService;
        private readonly WidgetUpdateService _widgetUpdateService;

        public WidgetsController()
        {
            _widgetService = new WidgetService();
            _themeService = new ThemeService();
            _settingsService = new SettingsService();
            _widgetUpdateService = new WidgetUpdateService();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            DisplayCookieMessage();
        }

        public ActionResult Active()
        {
            return View(GetActiveThemeWidgets());
        }

        public ActionResult Index()
        {
            int activeTheme = _settingsService.GetActiveThemeId();

            var themeSections = _themeService.GetThemeSections(activeTheme);

            var returnModel = new BundleInstalledWidgets
            {
                InstalledWidgets = _widgetService.GetAllWidgets().Select(widget => new InstalledWidget(widget.Id)
                {
                    Name = widget.Name,
                    Description = widget.Description,
                    Image = widget.Image,
                    AreaName = widget.AreaName,
                    IsUsedInAnyOfTheThemes = widget.WidgetsTheme.Any(x => x.IsEnabled),
                    IsUsedInAllThemes = widget.WidgetsTheme.All(x => x.IsEnabled),
                    IsEnabledInActiveTheme = themeSections.Any(s => s.WidgetsTheme.Any(wt => wt.IsEnabled && wt.WidgetId == widget.Id))
                }).ToList(),
                Themes = GetThemeSections(themeSections)
            };

            return View(returnModel);
        }

        [HttpPost]
        public ActionResult Activate(InstalledWidget widget)
        {
            var widgetCookieService = new WidgetCookieService();

            if (ModelState.IsValid)
            {
                var widgetUpdateService = new WidgetActivateService();

                if (widgetUpdateService.SetWidgetToActive(widget.ActivateWidget.WidgetId,
                    widget.ActivateWidget.ThemeSectionId, widget.ActivateWidget.Order.Value))
                {
                    widgetCookieService.Set(Widgets.WidgetAdded, true);

                }
                else
                {
                    widgetCookieService.Set(Widgets.NoWidgetFound, false);
                }
            }
            else
            {
                widgetCookieService.Set(ModelState.Values.First(x => x.Errors.Any()).Errors.First().ErrorMessage, false);
            }

            return RedirectToAction("Active");
        }

        public ActionResult UpdateGlobalStatus(int widgetId, bool status)
        {
            var widgetCookieService = new WidgetCookieService();
            var widgetUpdateService = new WidgetActivateService();

            if (widgetUpdateService.UpdateStatus(widgetId, status, isGlobal: true))
            {
                widgetCookieService.Set(Widgets.GlobalWidgetUpdate, true);
            }
            else
            {
                widgetCookieService.Set(Widgets.NoWidgetFound, false);
            }

            return RedirectToAction("Index");
        }

        public ActionResult UpdateStatus(int widgetId, bool status)
        {
            var widgetCookieService = new WidgetCookieService();
            var widgetActivateService = new WidgetActivateService();

            bool isSuccess = widgetActivateService.UpdateStatus(widgetId, status, isGlobal: false);

            if (isSuccess && status)
            {
                widgetCookieService.Set(Widgets.WidgetSuccessfullyEnabled, true);
            }
            else if (isSuccess && !status)
            {
                widgetCookieService.Set(Widgets.WidgetSuccessfullyDisabled, true);
            }
            else
            {
                widgetCookieService.Set(Widgets.NoWidgetFound, false);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int widgetId)
        {
            var widgetCookieService = new WidgetCookieService();
            var widgetUninstallerService = new WidgetUninstallService();

            var resultUninstall = widgetUninstallerService.Uninstall(widgetId);

            if (resultUninstall.IsSuccess)
            {
                widgetCookieService.Set(Widgets.WidgetSuccessfullyDeleted, true);
            }
            else
            {
                widgetCookieService.Set(resultUninstall.ErrorMessage, false);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Install(WidgetInstallModel widgetInstallModel)
        {
            var widgetCookieService = new WidgetCookieService();

            if (ModelState.IsValid)
            {
                var installService = new WidgetInstallerService();
                var installResult = installService.Install(widgetInstallModel.File.InputStream);

                if (installResult.IsSuccess)
                {
                    widgetCookieService.Set(Widgets.WidgetInstalledSuccessfully, true);
                }
                else
                {
                    widgetCookieService.Set(installResult.ErrorMessage, false);
                }
            }
            else
            {
                this.SetError(ModelState.Values.First(x => x.Errors.Any()).Errors.First().ErrorMessage);
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Settings(IEnumerable<ViewWidgetsModel> viewWidgetsModel)
        {
            if (ModelState.IsValid)
            {
                var settingsModel = viewWidgetsModel.Single().Settings;

                _widgetService.SaveSettings(settingsModel.Id, settingsModel.ThemeSectionId, settingsModel.Order.Value);

                this.SetSuccess(Widgets.SettingsUpdated);
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
                LastCheckedForUpdates = _widgetUpdateService.GetLastCheckedForUpdates(),
                AmountUpdatesAvailable = _widgetUpdateService.GetAmountUpdatesAvailable(),
                StatusUpdates = (UpdateAvailableStatus)GetUpdateAvailableStatus(),
                IsChecking = _widgetUpdateService.IsCheckingForUpdates(),
                IsInstalling = _widgetUpdateService.IsInstallingUpdates(),
                IsDownloading = _widgetUpdateService.IsDownloadingUpdates()
            });
        }

        public int GetUpdatingStatus()
        {
            return (int)_widgetUpdateService.GetUpdatingStatus();
        }

        public JsonResult WasUpdatingSuccess()
        {
            return Json(_widgetUpdateService.IsUpdatedSuccessfully(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAvailableSystemUpdates()
        {
            var versionComparer = new VersionOrderingService();

            string deskoVersion = _settingsService.GetVersion();

            return View("_TabbedUpdates", _widgetUpdateService.GetWidgetsWithUpdates().Select(widget => new TabbedUpdateTableModel
            {
                Id = widget.Id,
                Name = widget.Name,
                Updates = widget.WidgetOpenUpdates.Select(update => new UpdateTableModel
                {
                    Name = update.Name,
                    Description = update.Description,
                    IsDownloaded = update.IsDownloaded,
                    Version = update.Version,
                    DeskoVersion = update.DeskoVersion,
                    IsVersionIncorrect = !versionComparer.Equals(update.DeskoVersion, deskoVersion),
                    IsDownloading = WidgetDownloadManager.Manager.IsDownloading(update.Version),
                    Progress = update.IsDownloaded ? 100 : WidgetDownloadManager.Manager.GetProgress(update.Version)
                }).OrderBy(w => w.Version, versionComparer)
            }).ToList());
        }

        public ActionResult GetDateChecked()
        {
            return View("_DateLastChecked", _widgetUpdateService.GetLastCheckedForUpdates());
        }

        public void InstallUpdates()
        {
            _widgetUpdateService.UpdateInBackground();
        }

        public bool IsCheckingSoftware()
        {
            return _widgetUpdateService.IsCheckingForUpdates();
        }

        public bool IsInstallingSoftware()
        {
            return _widgetUpdateService.IsInstallingUpdates();
        }

        public bool IsDownloadingSoftware()
        {
            return _widgetUpdateService.IsDownloadingUpdates();
        }

        public JsonResult GetDownloadProgress()
        {
            return Json(new
                {
                    isDownloading = _widgetUpdateService.IsDownloadingUpdates(),
                    downloads = from download in WidgetDownloadManager.Manager.GetDownloads()
                                select new
                                {
                                    version = download.Version,
                                    progress = download.ProgressPercentage,
                                    id = download.ExtraId,
                                }
                }, JsonRequestBehavior.AllowGet);
        }


        public int GetUpdateAvailableStatus()
        {
            int amountUpdatesAvailable = _widgetUpdateService.GetAmountUpdatesAvailable();

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

        #region Private Methods

        private void DisplayCookieMessage()
        {
            var widgetCookieService = new WidgetCookieService();

            var cookie = widgetCookieService.Get();

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

                widgetCookieService.Remove();
            }
        }

        private IEnumerable<SelectListItem> GetThemeSections(IEnumerable<WidgetsThemeSection> themeSections,
            int? selectedThemeId = null)
        {
            return themeSections.Select(ts => new SelectListItem
            {
                Selected = selectedThemeId != null && selectedThemeId == ts.Id,
                Text = ts.FriendlyName,
                Value = ts.Id.ToString(CultureInfo.InvariantCulture)
            }).OrderBy(x => x.Text);
        }

        private IList<ViewWidgetsModel> GetActiveThemeWidgets()
        {
            var activeTheme = _settingsService.GetActiveThemeId();
            var themeSections = _themeService.GetThemeSections(activeTheme).ToList();

            var allWidgets = _widgetService.GetActiveWidgets();

            return (from widget in allWidgets
                    let widgetTheme = themeSections.SelectMany(ts => ts.WidgetsTheme).First(tm => tm.WidgetId == widget.Id)
                    select new ViewWidgetsModel
                    {
                        Widget = new ActiveWidgetModel
                        {
                            WidgetId = widget.Id,
                            Name = widget.Name,
                            AreaName = widget.AreaName,
                            Description = widget.Description,
                            Image = widget.Image,
                            ThemeSection = widgetTheme.WidgetsThemeSection.FriendlyName
                        },
                        Settings = GetWidgetSettings(widget, themeSections)
                    }).OrderBy(x => x.Widget.Name).ToList();
        }

        private WidgetSettingsModel GetWidgetSettings(Database.Entities.Widgets widget, IEnumerable<WidgetsThemeSection> themeSections)
        {
            var widgetSection =
                  widget.WidgetsTheme.FirstOrDefault(tm => themeSections.Any(x => x.Id == tm.ThemeSectionId));

            var widgetSettings = new WidgetSettingsModel();

            if (widgetSection != null)
            {
                widgetSettings.Id = widgetSection.Id;
                widgetSettings.Order = widgetSection.Order;
                widgetSettings.ThemeSectionId = widgetSection.ThemeSectionId;
                widgetSettings.ThemeSections = GetThemeSections(themeSections, widgetSection.Id);
            }
            else
            {
                widgetSettings.ThemeSections = GetThemeSections(themeSections);
            }

            return widgetSettings;
        }

        #endregion
    }
}
