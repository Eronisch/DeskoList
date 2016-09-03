using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Business.Dll;
using Core.Business.File;
using Core.Business.Settings;
using Core.Business.Software;
using Database.Entities;

namespace Core.Business.Plugin
{
    public class PluginEnableService
    {
        private readonly PluginService _pluginService;
        private readonly SettingsService _settingsService;
        private readonly PluginConfigService _pluginConfigService;

        public PluginEnableService()
        {
            _pluginService = new PluginService();
            _settingsService = new SettingsService();
            _pluginConfigService = new PluginConfigService();
        }

        /// <summary>
        /// Returns false if the plugin was not found
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public bool UpdateStatus(int pluginId, bool enable)
        {
            var plugin = _pluginService.Get(pluginId);

            if (plugin == null) { return false; }

            SetStatus(enable, plugin);

            UpdateDlls(plugin, enable);

            return true;
        }

        private void SetStatus(bool enable, Plugins plugin)
        {
            plugin.Enabled = enable;

            _pluginService.SavePlugin(plugin);
        }

        private void UpdateDlls(Plugins plugin, bool enable)
        {
            if (enable)
            {
                InstallDllsToAppBin(plugin.Area);
            }
            else
            {
                DeleteDllsFromAppBinWhenNotUsed(plugin);
            }
        }

        private void InstallDllsToAppBin(string areaName)
        {
            var dllPaths = _pluginConfigService.GetDlls(areaName);

            var appDllsPaths = FileService.GetFiles(FileService.GetBinPath(), "*.dll").ToList();

            var versionComparer = new VersionOrderingService();

            foreach (var dll in dllPaths)
            {
                string dllName = Path.GetFileName(dll);

                bool isDllInstalled = appDllsPaths.Any(adp => Path.GetFileName(adp) == dllName);
                string dllAppBinFilePath = Path.Combine(FileService.GetBinPath(), dllName);

                if (!isDllInstalled
                    || versionComparer.IsHigher(FileVersionInfo.GetVersionInfo(dllAppBinFilePath).FileVersion, FileVersionInfo.GetVersionInfo(dll).FileVersion))
                {
                    FileService.CopyFile(dll, GetNewPath(FileService.GetBinPath(), dllName));   
                }
            }
        }

        private string GetNewPath(string binPath, string fileName)
        {
            return Path.Combine(binPath, fileName);
        }

        private void DeleteDllsFromAppBinWhenNotUsed(Plugins plugin)
        {
            var dllService = new DllService();

            var dllFilePaths = _pluginConfigService.GetDlls(plugin.Area);

            foreach (var filePath in dllFilePaths)
            {
                string dllName = Path.GetFileName(filePath);

                if (!dllService.IsDllBeingUsedByPluginId(plugin.Id, dllName))
                {
                    FileService.DeleteFile(Path.Combine(FileService.GetBinPath(), dllName));
                }
            }
        }
    }
}
