using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Core.Business.Breadcrumbs;
using Core.Business.Dll;
using Core.Business.File;
using Core.Business.Navigation;
using Core.Business.Software;
using Core.Models;
using Core.Models.ThirdParty;
using Database;
using Database.Entities;
using Elmah;
using Ionic.Zip;

namespace Core.Business.Plugin
{
    public class PluginInstallerService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;
        private readonly PluginConfigService _pluginConfig;
        private readonly PluginService _pluginService;

        public PluginInstallerService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
            _pluginService = new PluginService();
            _pluginConfig = new PluginConfigService();
        }

        /// <summary>
        /// Tries to install the plugin
        /// Catches all errors
        /// Will restart the website
        /// </summary>
        /// <param name="file"></param>
        public ResultModel Install(Stream file)
        {
            string tempAreaName = Path.GetRandomFileName();

            string tempPathArea = GetTempPluginPath(tempAreaName);

            try
            {
                TempUnpack(file, tempPathArea);

                PluginSettingsService pluginSettingsService;

                var settingsResult = TryLoadSettings(tempAreaName, out pluginSettingsService);

                if (!settingsResult.IsSuccess) { return settingsResult;}

                string pluginFolderName = GetPluginFolderName(pluginSettingsService.Name,
                    pluginSettingsService.Author);

                bool isUpdate = IsUpdate(pluginSettingsService.Name,
                    pluginSettingsService.Author);

                var scriptsResult = TryInstallSqlScripts(pluginSettingsService.GetSqlInstallScripts(), pluginSettingsService.Name, pluginSettingsService.Author,
                    pluginSettingsService.Version, tempPathArea, isUpdate);

                int pluginId = UpdateOrInstall(pluginSettingsService, pluginFolderName);

                if (!scriptsResult.IsSuccess) { return new ResultModel(scriptsResult.ErrorMessage); }

                AddNavigationLinks(pluginId, pluginSettingsService.GetNavigation());

                AddBreadcrumbs(pluginId, pluginSettingsService.GetBreadcrumbs());

                AddDllsToDatabase(pluginId, tempPathArea);

                MoveTempFilesPlugin(tempPathArea, pluginFolderName);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(null).Log(new Error(ex));
                return new ResultModel(Localization.Languages.Business.Plugin.PluginInstallerService.UnknownError);
            }
            finally
            {
                RemoveTempFiles(tempPathArea);
            }

            return new ResultModel();
        }

        private ResultModel TryLoadSettings(string tempAreaName, out PluginSettingsService pluginSettingsService)
        {
            pluginSettingsService = null;

            try
            {
                pluginSettingsService = new PluginSettingsService(_pluginConfig.GetTempSettingsPath(tempAreaName));
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message);
            }

            return new ResultModel();
        }

        private void AddDllsToDatabase(int pluginId, string tempPluginPath)
        {
            var dllService = new DllService();

            var pluginDllFilePaths = _pluginConfig.GetDlls(tempPluginPath);

            foreach (var pluginDllFilePath in pluginDllFilePaths)
            {
                string dllName = Path.GetFileName(pluginDllFilePath);

                dllService.AddPluginDll(dllName, pluginId);
            }
        }

        private string GetTempPluginPath(string tempPluginName)
        {
            return _pluginConfig.GetTempFolderPath(tempPluginName);
        }

        /// <summary>
        /// Add or update the plugin record
        /// </summary>
        /// <param name="pluginSettings"></param>
        /// <param name="pluginFolderName"></param>
        /// <returns>The plugin id</returns>
        private int UpdateOrInstall(PluginSettingsService pluginSettings,
            string pluginFolderName)
        {
            bool isUpdate = IsUpdate(pluginSettings.Name, pluginSettings.Author);

            if (isUpdate)
            {
                return UpdatePluginDbRecord(pluginSettings);
            }

            return AddPluginToDatabase(pluginSettings, pluginFolderName);
        }

        /// <summary>
        /// Updates the record in the database
        /// </summary>
        /// <param name="pluginSettings"></param>
        /// <returns>The plugin id</returns>
        private int UpdatePluginDbRecord(PluginSettingsService pluginSettings)
        {
            var plugin = _unitOfWorkRepository.PluginRepository.GetByNameAndAuthor(pluginSettings.Name,
                pluginSettings.Author);

            UpdatePluginObject(pluginSettings, plugin);

            _pluginService.SavePlugin(plugin);

            return plugin.Id;
        }

        private void UpdatePluginObject(PluginSettingsService pluginSettings, Plugins plugin)
        {
            plugin.Version = pluginSettings.Version;
            plugin.AuthorUrl = pluginSettings.AuthorUrl;
            plugin.Description = pluginSettings.Description;
            plugin.UpdateUrl = pluginSettings.UpdateUrl;
        }

        private void AddNavigationLinks(int pluginId, IEnumerable<AdminNavigationSettingsModel> pluginNavigation)
        {
            var adminNavigationService = new AdminNavigationService();

            adminNavigationService.ClearPluginNavigation(pluginId);

            foreach (var navigation in pluginNavigation)
            {
                adminNavigationService.AddPluginNavigation(pluginId, navigation.Controller, navigation.Action, navigation.Icon, navigation.LocalizedBase, navigation.LocalizedName);
            }
        }

        private void AddBreadcrumbs(int pluginId, IEnumerable<AdminBreadcrumbSettingsModel> breadCrumbs)
        {
            var adminBreadCrumbsService = new AdminBreadCrumbsService();

            adminBreadCrumbsService.ClearPluginBreadCrumbs(pluginId);

            foreach (var breadcrumb in breadCrumbs)
            {
               adminBreadCrumbsService.AddPluginBreadCrumbs(pluginId, breadcrumb.Controller, breadcrumb.Action, breadcrumb.Icon, breadcrumb.LocalizedBase, breadcrumb.LocalizedTitle,
                breadcrumb.LocalizedDescription, breadcrumb.LocalizedControllerFriendlyName, breadcrumb.LocalizedActionFriendlyName);
            }
        }

        private string GetPluginFolderName(string pluginName, string author)
        {
            string cleanPluginName = FileService.CleanFileName(pluginName);
            string cleanAuthorName = FileService.CleanFileName(author);

            if (cleanPluginName.Length > 100)
            {
                cleanPluginName = pluginName.Substring(0, 100);
            }

            if (cleanAuthorName.Length > 100)
            {
                cleanAuthorName = author.Substring(0, 100);
            }

            return string.Format("{0}-{1}", cleanPluginName, cleanAuthorName);
        }

        /// <summary>
        /// Returns plugin id
        /// </summary>
        /// <param name="pluginSettings"></param>
        /// <param name="pluginFolderName"></param>
        /// <returns></returns>
        private int AddPluginToDatabase(PluginSettingsService pluginSettings, string pluginFolderName)
        {
            return _pluginService.AddToDatabase(pluginSettings.Name, pluginSettings.Description, pluginFolderName, pluginSettings.Version, pluginSettings.Author, pluginSettings.AuthorUrl, pluginSettings.Namespace, pluginSettings.UpdateUrl);
        }

        private bool IsUpdate(string pluginName, string author)
        {
            return _unitOfWorkRepository.PluginRepository.GetByNameAndAuthor(pluginName, author) != null;
        }

        /// <summary>
        /// Unpacks the plugin in a temporary directory
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tempUnPackPath"></param>
        private void TempUnpack(Stream file, string tempUnPackPath)
        {
            FileService.CreateDirectory(tempUnPackPath);

            using (ZipFile zipFile = ZipFile.Read(file))
            {
                foreach (ZipEntry e in zipFile)
                {
                    e.Extract(tempUnPackPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void RemoveTempFiles(string tempPluginPath)
        {
            // We don't want the last directory, the plugin folder itself. Delete the parent
            string pathToRemove = tempPluginPath.Substring(0, tempPluginPath.LastIndexOf(@"\") + 1);
            FileService.RemoveDirectory(pathToRemove, true);
        }

        private ResultModel TryInstallSqlScripts(IEnumerable<SqlScript> sqlScripts, string pluginName, string author, string versionUpdateOrInstall, string tempPath, bool isUpdate)
        {
            bool isSuccess = true;
            var databaseService = new DatabaseService();
            var versionOrderingService = new VersionOrderingService();

            var startVersion = GetVersion(pluginName, author, versionUpdateOrInstall, isUpdate);

            using (var transaction = databaseService.BeginTransaction())
            {
                foreach (var script in sqlScripts)
                {
                    // Is installation, start from the current version
                    if (!isUpdate && versionOrderingService.Compare(script.Version, startVersion) >= 0
                        // Is update, start from anything that is higher than the current version
                        || isUpdate && versionOrderingService.Compare(script.Version, startVersion) > 0)
                    {
                        try
                        {
                            databaseService.ExecuteQuery(FileService.GetFileContent(Path.Combine(tempPath, script.Location)), transaction);
                        }
                        catch
                        {
                            isSuccess = false;
                        }
                    }
                }

                if (isSuccess) transaction.Commit();
            }

            // Quit
            return new ResultModel
            {
                ErrorMessage =
                    !isSuccess ? Localization.Languages.Business.Plugin.PluginInstallerService.ErrorInSqlScript : string.Empty
            };
        }

        private string GetVersion(string pluginName, string author, string versionUpdateOrInstall, bool isUpdate)
        {
            if (isUpdate)
            {
                return _unitOfWorkRepository.PluginRepository.GetByNameAndAuthor(
                    pluginName, author).Version;
            }

            return versionUpdateOrInstall;
        }

        private bool IsPluginFileInBinFolder(string tempPluginPath, string pluginTempFilePath)
        {
            string skipPath = Path.Combine(tempPluginPath, "bin");

            return new DirectoryInfo(pluginTempFilePath).Parent.FullName == skipPath;
        }

        private void MoveTempFilesPlugin(string tempPluginPath, string destinationPluginFolderName)
        {
            string tempDirectoryPluginName = new DirectoryInfo(tempPluginPath).Name;

            foreach (var pluginTempFilePath in Directory.GetFiles(tempPluginPath, "*.*", SearchOption.AllDirectories))
            {
                if (IsPluginFileInBinFolder(tempPluginPath, pluginTempFilePath))
                {
                    if (IsNewPluginDllFile(pluginTempFilePath))
                    {
                        FileService.CopyFile(pluginTempFilePath, Path.Combine(FileService.GetBinPath(), Path.GetFileName(pluginTempFilePath)));
                    }
                }

                string destinationFilePath = GetTargetFilePluginLocation(tempDirectoryPluginName,
                    pluginTempFilePath, destinationPluginFolderName);

                FileService.CreateDirectory(new DirectoryInfo(destinationFilePath).Parent.FullName);

                FileService.MoveFile(pluginTempFilePath, destinationFilePath);
            }
        }

        private static bool IsNewPluginDllFile(string pluginTempFilePath)
        {
            var versionComparer = new VersionOrderingService();
            string dllFileName = Path.GetFileName(pluginTempFilePath);
            string dllInBinPath = Path.Combine(FileService.GetBinPath(), dllFileName);

            if (!FileService.FileExists(dllInBinPath)) { return true; }

            var dllInBinFileInfo = FileVersionInfo.GetVersionInfo(dllInBinPath);
            var dllFromPluginFileInfo = FileVersionInfo.GetVersionInfo(pluginTempFilePath);

            if (versionComparer.IsHigher(dllFromPluginFileInfo.FileVersion, dllInBinFileInfo.FileVersion))
            {
                return true;
            }

            return false;
        }

        private string GetTargetFilePluginLocation(string sourcePluginFolderName, string pluginTempFilePath, string destinationPluginFolderName)
        {
            string pathWithoutApplicationPath = pluginTempFilePath.Substring(FileService.GetBaseDirectory().Length + _pluginConfig.GetTempFolderName().Length + sourcePluginFolderName.Length + 2); // exclude "//" symbols

            return Path.Combine(_pluginConfig.GetAreaPath(destinationPluginFolderName), pathWithoutApplicationPath);
        }
    }
}
