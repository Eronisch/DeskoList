using System;
using System.Collections.Generic;
using System.IO;
using Core.Business.Dll;
using Core.Business.File;
using Core.Models;
using Core.Models.ThirdParty;
using Database;
using Elmah;
using Localization.Languages.Business.Plugin;

namespace Core.Business.Plugin
{
    public class PluginUninstallService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;
        private readonly DatabaseService _databaseService;
        private readonly PluginConfigService _pluginConfigService;

        public PluginUninstallService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
            _databaseService = new DatabaseService();
            _pluginConfigService = new PluginConfigService();
        }

        /// <summary>
        /// Uninstall the plugin
        /// Catches all exceptions
        /// Restarts the website
        /// </summary>
        /// <param name="pluginId"></param>
        public ResultModel Uninstall(int pluginId)
        {
            try
            {
                var plugin = GetPlugin(pluginId);

                if (plugin == null) { return new ResultModel(PluginUninstallerService.NotFound); }

                string pathPlugin = _pluginConfigService.GetAreaPath(plugin.Area);

                PluginSettingsService pluginSettingsService;

                var settingsResult = TryLoadSettings(plugin.Area, out pluginSettingsService);

                if (!settingsResult.IsSuccess) { return new ResultModel(settingsResult.ErrorMessage); }

                var scriptResult = UnInstallPluginSqlScripts(pluginSettingsService.GetSqlUninstallScripts(), pathPlugin);

                if (!scriptResult.IsSuccess) { return new ResultModel(scriptResult.ErrorMessage); }

                RemoveUnusedDllsFromDatabase(plugin.Id, plugin.Area);

                RemoveNotUsedDlls(plugin.Id, plugin.Area);

                DeleteDbRecords(plugin.Id);

                RemovePluginFolder(pathPlugin);

                return new ResultModel();
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(null).Log(new Error(ex));
                return new ResultModel(PluginUninstallerService.UnknownError);
            }
        }

        private ResultModel TryLoadSettings(string tempAreaName, out PluginSettingsService pluginSettingsResult)
        {
            pluginSettingsResult = null;

            try
            {
                pluginSettingsResult = new PluginSettingsService(_pluginConfigService.GetSettingsPath(tempAreaName));
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message);
            }

            return new ResultModel();
        }

        private void RemoveUnusedDllsFromDatabase(int pluginId, string areaName)
        {
            var dllService = new DllService();

            var dllFilePaths = _pluginConfigService.GetDlls(areaName);

            foreach (var dllFilePath in dllFilePaths)
            {
                string dllName = Path.GetFileName(dllFilePath);

                if (!dllService.IsDllBeingUsedByPluginId(pluginId, dllName))
                {
                    dllService.RemoveDll(dllName);
                }
            }
        }

        private Database.Entities.Plugins GetPlugin(int pluginId)
        {
            var pluginService = new PluginService();

            return pluginService.Get(pluginId);
        }

        private void DeleteDbRecords(int pluginId)
        {
            _unitOfWorkRepository.PluginRepository.Remove(pluginId);

            _unitOfWorkRepository.SaveChanges();
        }

        private void RemovePluginFolder(string pathPlugin)
        {
            FileService.RemoveDirectory(pathPlugin, true);
        }

        private IEnumerable<string> GetFilesInBin(string areaName)
        {
            return _pluginConfigService.GetDlls(areaName);
        }

        private void RemoveNotUsedDlls(int pluginId, string areaName)
        {
            var dllService = new DllService();

            foreach (var file in GetFilesInBin(areaName))
            {
                if (!dllService.IsDllBeingUsedByPluginId(pluginId, Path.GetFileName(file)))
                {
                    FileService.RemoveFileInBin(Path.GetFileName(file));
                }
            }
        }

        private ResultModel UnInstallPluginSqlScripts(IEnumerable<SqlScript> sqlScripts, string pathPlugin)
        {
            bool isSuccess = true;

            foreach (var script in sqlScripts)
            {
                try
                {
                    _databaseService.ExecuteQuery(FileService.GetFileContent(Path.Combine(pathPlugin, script.Location)));
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return new ResultModel
            {
                ErrorMessage = !isSuccess ? PluginUninstallerService.ErrorInSqlScript : string.Empty
            };
        }
    }
}
