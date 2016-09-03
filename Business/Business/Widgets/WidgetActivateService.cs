using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Business.Dll;
using Core.Business.File;
using Core.Business.Settings;
using Core.Business.Software;

namespace Core.Business.Widgets
{
    public class WidgetActivateService
    {
        private readonly WidgetService _widgetService;
        private readonly SettingsService _settingsService;
        private readonly WidgetConfigService _widgetConfigService;

        public WidgetActivateService()
        {
            _widgetService = new WidgetService();
            _settingsService = new SettingsService();
            _widgetConfigService = new WidgetConfigService();
        }

        /// <summary>
        /// Returns false if the widget was not found
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="enableWidget"></param>
        /// <param name="isGlobal"></param>
        /// <returns></returns>
        public bool UpdateStatus(int widgetId, bool enableWidget, bool isGlobal)
        {
            var widget = _widgetService.GetWidget(widgetId);

            if (widget == null) { return false; }

            UpdateStatus(widget, enableWidget, isGlobal);

            UpdateWidgetDlls(widget, enableWidget);

            _widgetService.SaveWidget(widget);

            return true;
        }

        /// <summary>
        /// Set widget to active in the current theme
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="themeSectionId"></param>
        /// <param name="order"></param>
        /// <returns>Returns true if the widget exists</returns>
        public bool SetWidgetToActive(int widgetId, int themeSectionId, int order)
        {
            var widget = _widgetService.GetWidget(widgetId);

            if (widget == null) { return false; }

            _widgetService.ConfigureWidgetTheme(widgetId, themeSectionId, order);

            UpdateWidgetDlls(widget, enableWidget: true);

            return true;
        }

        private void UpdateStatus(Database.Entities.Widgets widget, bool enableWidget, bool isGlobal)
        {
            if (isGlobal)
            {
                UpdateAllActiveThemesStatus(widget, enableWidget);
            }
            else
            {
                UpdateActiveThemeStatus(widget, enableWidget);
            }
        }

        private void UpdateAllActiveThemesStatus(Database.Entities.Widgets widget, bool enableWidget)
        {
            foreach (var widgetTheme in widget.WidgetsTheme)
            {
                widgetTheme.IsEnabled = enableWidget;
            }
        }

        private void UpdateActiveThemeStatus(Database.Entities.Widgets widget, bool enableWidget)
        {
            widget.WidgetsTheme.First(tm => tm.WidgetsThemeSection.ThemeId == _settingsService.GetActiveThemeId())
                .IsEnabled = enableWidget;
        }

        private void UpdateWidgetDlls(Database.Entities.Widgets widget, bool enableWidget)
        {
            if (enableWidget)
            {
                InstallDllsToAppBin(widget.AreaName);
            }
            else
            {
                DeleteDllsFromAppBinWhenNotUsed(widget.Id, widget.AreaName);
            }
        }

        private void InstallDllsToAppBin(string widgetAreaName)
        {
            var widgetDllPaths = _widgetConfigService.GetDlls(widgetAreaName);

            var appDllsPaths = FileService.GetFiles(FileService.GetBinPath(), "*.dll").ToList();

            var versionComparer = new VersionOrderingService();

            foreach (var widgetDllPath in widgetDllPaths)
            {
                string dllName = Path.GetFileName(widgetDllPath);

                bool isDllInstalled = appDllsPaths.Any(adp => Path.GetFileName(adp) == dllName);
                string dllAppBinFilePath = Path.Combine(FileService.GetBinPath(), dllName);

                if (!isDllInstalled
                    || versionComparer.IsHigher(FileVersionInfo.GetVersionInfo(dllAppBinFilePath).FileVersion, FileVersionInfo.GetVersionInfo(widgetDllPath).FileVersion))
                {
                    FileService.CopyFile(widgetDllPath, GetNewPath(FileService.GetBinPath(), dllName));   
                }
            }
        }

        private string GetNewPath(string binPath, string fileName)
        {
            return Path.Combine(binPath, fileName);
        }

        private void DeleteDllsFromAppBinWhenNotUsed(int widgetId, string widgetAreaName)
        {
            var dllService = new DllService();

            var widgetDllFilePaths = _widgetConfigService.GetDlls(widgetAreaName);

            foreach (var widgetDllFilePath in widgetDllFilePaths)
            {
                string dllName = Path.GetFileName(widgetDllFilePath);

                if (!dllService.IsDllBeingUsedByWidgetId(widgetId, dllName))
                {
                    FileService.DeleteFile(Path.Combine(FileService.GetBinPath(), dllName));
                }
            }
        }
    }
}
