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
using Elmah;
using Ionic.Zip;

namespace Core.Business.Widgets
{
    public class WidgetInstallerService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;
        private readonly WidgetService _widgetService;
        private readonly WidgetConfigService _widgetConfigService;

        public WidgetInstallerService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
            _widgetService = new WidgetService();
            _widgetConfigService = new WidgetConfigService();
        }

        /// <summary>
        /// Install Widget
        /// </summary>
        /// <param name="file"></param>
        public ResultModel Install(Stream file)
        {
            string tempAreaName = Path.GetRandomFileName();

            string tempPathArea = GetTempWidgetPath(tempAreaName);

            try
            {
                TempUnpack(file, tempPathArea);

                WidgetSettingsService widgetSettingsService;

                var settingsResult = TryLoadSettings(tempAreaName, out widgetSettingsService);

                if (!settingsResult.IsSuccess) { return settingsResult;}

                string widgetFolderName = GetWidgetFolderName(widgetSettingsService.WidgetName,
                    widgetSettingsService.Author);

                bool isUpdate = IsUpdate(widgetSettingsService.WidgetName,
                    widgetSettingsService.Author);

                var scriptsResult = TryInstallSqlScripts(widgetSettingsService.GetSqlInstallScripts(), widgetSettingsService.WidgetName, widgetSettingsService.Author,
                    widgetSettingsService.Version, tempPathArea, isUpdate);

                if (!scriptsResult.IsSuccess) { return new ResultModel(scriptsResult.ErrorMessage); }

                int widgetId = UpdateOrInstall(widgetSettingsService, widgetFolderName);

                AddNavigationLinks(widgetId, widgetSettingsService.GetNavigation());

                AddBreadcrumbs(widgetId, widgetSettingsService.GetBreadcrumbs());

                AddDllsToDatabase(widgetId, tempPathArea);

                MoveTempFilesWidgets(tempPathArea, widgetFolderName);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(null).Log(new Error(ex));
                return new ResultModel(Localization.Languages.Business.Widgets.WidgetInstallerService.UnknownError);
            }
            finally
            {
                RemoveTempFiles(tempPathArea);
            }

            return new ResultModel();
        }

        private ResultModel TryLoadSettings(string tempAreaName, out WidgetSettingsService widgetSettingsResult)
        {
            widgetSettingsResult = null;

            try
            {
                widgetSettingsResult = new WidgetSettingsService(_widgetConfigService.GetTempSettingsPath(tempAreaName));
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message);
            }

            return new ResultModel();
        }

        private void AddDllsToDatabase(int widgetId, string tempWidgetPath)
        {
            var dllService = new DllService();

            var widgetDllFilePaths = _widgetConfigService.GetDlls(tempWidgetPath);

            foreach (var widgetDllFilePath in widgetDllFilePaths)
            {
                string dllName = Path.GetFileName(widgetDllFilePath);

                dllService.AddWidgetDll(dllName, widgetId);
            }
        }

        private string GetTempWidgetPath(string tempWidgetName)
        {
            return _widgetConfigService.GetTempFolderPath(tempWidgetName);
        }

        /// <summary>
        /// Add or update the widget record
        /// </summary>
        /// <param name="widgetSettings"></param>
        /// <param name="widgetFolderName"></param>
        /// <returns>The widget id</returns>
        private int UpdateOrInstall(WidgetSettingsService widgetSettings,
            string widgetFolderName)
        {
            bool isUpdate = IsUpdate(widgetSettings.WidgetName, widgetSettings.Author);

            if (isUpdate)
            {
                return UpdateWidgetDbRecord(widgetSettings);
            }

            return AddWidgetToDatabase(widgetSettings, widgetFolderName);
        }

        /// <summary>
        /// Updates the record in the database
        /// </summary>
        /// <param name="widgetSettings"></param>
        /// <returns>The widget id</returns>
        private int UpdateWidgetDbRecord(WidgetSettingsService widgetSettings)
        {
            var widget = _unitOfWorkRepository.WidgetsRepository.GetByNameAndAuthor(widgetSettings.WidgetName,
                widgetSettings.Author);

            UpdateWidgetObject(widgetSettings, widget);

            _widgetService.SaveWidget(widget);

            return widget.Id;
        }

        private void UpdateWidgetObject(WidgetSettingsService widgetSettings, Database.Entities.Widgets widget)
        {
            widget.Version = widgetSettings.Version;
            widget.AuthorUrl = widgetSettings.AuthorUrl;
            widget.Controller = widgetSettings.StartController;
            widget.StartIndex = widgetSettings.StartIndex;
            widget.Image = widgetSettings.Image;
            widget.Namespace = widgetSettings.Namespace;
            widget.Description = widgetSettings.Description;
            widget.UpdateUrl = widgetSettings.UpdateUrl;
        }

        private void AddNavigationLinks(int widgetId, IEnumerable<AdminNavigationSettingsModel> widgetNavigation)
        {
            var adminNavigationService = new AdminNavigationService();

            adminNavigationService.ClearWidgetNavigation(widgetId);

            foreach (var navigation in widgetNavigation)
            {
               adminNavigationService.AddWidgetNavigation(widgetId, navigation.Controller, navigation.Action, navigation.Icon, navigation.LocalizedBase, navigation.LocalizedName);
            }
        }

        private void AddBreadcrumbs(int widgetId, IEnumerable<AdminBreadcrumbSettingsModel> widgetBreadcrumbs)
        {
            var adminBreadCrumbsService = new AdminBreadCrumbsService();

            adminBreadCrumbsService.ClearWidgetBreadCrumbs(widgetId);

            foreach (var breadcrumb in widgetBreadcrumbs)
            {
                adminBreadCrumbsService.AddWidgetBreadCrumbs(widgetId, breadcrumb.Controller, breadcrumb.Action, breadcrumb.Icon, breadcrumb.LocalizedBase, breadcrumb.LocalizedTitle,
             breadcrumb.LocalizedDescription, breadcrumb.LocalizedControllerFriendlyName, breadcrumb.LocalizedActionFriendlyName);
            }

            _unitOfWorkRepository.SaveChanges();
        }

        private string GetWidgetFolderName(string widgetName, string author)
        {
            string newWidgetName = FileService.CleanFileName(widgetName);
            string newAuthor = FileService.CleanFileName(author);

            if (newWidgetName.Length > 100)
            {
                newWidgetName = widgetName.Substring(0, 100);
            }

            if (newAuthor.Length > 100)
            {
                newAuthor = author.Substring(0, 100);
            }

            return string.Format("{0}-{1}", newWidgetName, newAuthor);
        }

        /// <summary>
        /// Returns widget id
        /// </summary>
        /// <param name="widgetSettings"></param>
        /// <param name="widgetFolderName"></param>
        /// <returns></returns>
        private int AddWidgetToDatabase(WidgetSettingsService widgetSettings, string widgetFolderName)
        {
            return _widgetService.AddToDatabase(widgetSettings.WidgetName, widgetSettings.Description, widgetFolderName, widgetSettings.Version, widgetSettings.Author, widgetSettings.AuthorUrl, widgetSettings.Image, widgetSettings.StartController, widgetSettings.StartIndex, widgetSettings.Namespace, widgetSettings.UpdateUrl);
        }

        private bool IsUpdate(string widgetName, string author)
        {
            return _unitOfWorkRepository.WidgetsRepository.GetByNameAndAuthor(widgetName, author) != null;
        }

        /// <summary>
        /// Unpacks the widget in a temporary directory
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

        private void RemoveTempFiles(string tempWidgetPath)
        {
            // We don't want the last directory, the widget folder itself. Delete the parent
            string pathToRemove = tempWidgetPath.Substring(0, tempWidgetPath.LastIndexOf(@"\") + 1);
            FileService.RemoveDirectory(pathToRemove, true);
        }

        private ResultModel TryInstallSqlScripts(IEnumerable<SqlScript> sqlScripts, string name, string author, string versionUpdateOrInstall, string tempPath, bool isUpdate)
        {
            bool isSuccess = true;
            var databaseService = new DatabaseService();
            var versionOrderingService = new VersionOrderingService();

            var startVersion = GetVersion(name, author, versionUpdateOrInstall, isUpdate);

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

                transaction.Commit();
            }

            // Quit
            return new ResultModel
            {
                ErrorMessage =
                    !isSuccess ? Localization.Languages.Business.Widgets.WidgetInstallerService.ErrorInSqlScript : string.Empty
            };
        }

        private string GetVersion(string widgetName, string author, string versionUpdateOrInstall, bool isUpdate)
        {
            if (isUpdate)
            {
                return _unitOfWorkRepository.WidgetsRepository.GetByNameAndAuthor(
                    widgetName, author).Version;
            }

            return versionUpdateOrInstall;
        }

        private bool IsWidgetFileInBinFolder(string tempWidgetPath, string widgetTempFilePath)
        {
            string skipPath = Path.Combine(tempWidgetPath, "bin");

            return new DirectoryInfo(widgetTempFilePath).Parent.FullName == skipPath;
        }

        private void MoveTempFilesWidgets(string tempWidgetPath, string destinationWidgetFolderName)
        {
            string tempDirectoryWidgetName = new DirectoryInfo(tempWidgetPath).Name;

            foreach (var widgetTempFilePath in Directory.GetFiles(tempWidgetPath, "*.*", SearchOption.AllDirectories))
            {
                if (IsWidgetFileInBinFolder(tempWidgetPath, widgetTempFilePath))
                {
                    if (IsNewWidgetDllFile(widgetTempFilePath))
                    {
                        FileService.CopyFile(widgetTempFilePath, Path.Combine(FileService.GetBinPath(), Path.GetFileName(widgetTempFilePath)));
                    }
                }

                string destinationFilePath = GetTargetFileWidgetLocation(tempDirectoryWidgetName,
                    widgetTempFilePath, destinationWidgetFolderName);

                FileService.CreateDirectory(new DirectoryInfo(destinationFilePath).Parent.FullName);

                FileService.MoveFile(widgetTempFilePath, destinationFilePath);
            }
        }

        private bool IsNewWidgetDllFile(string widgetTempFilePath)
        {
            var versionComparer = new VersionOrderingService();
            string dllFileName = Path.GetFileName(widgetTempFilePath);
            string dllInBinPath = Path.Combine(FileService.GetBinPath(), dllFileName);

            if (!FileService.FileExists(dllInBinPath)) { return true; }

            var dllInBinFileInfo = FileVersionInfo.GetVersionInfo(dllInBinPath);
            var dllFromWidgetFileInfo = FileVersionInfo.GetVersionInfo(widgetTempFilePath);

            if (versionComparer.IsHigher(dllFromWidgetFileInfo.FileVersion, dllInBinFileInfo.FileVersion))
            {
                return true;
            }

            return false;
        }

        private string GetTargetFileWidgetLocation(string sourceWidgetFolderName, string widgetTempFilePath, string destinationWidgetFolderName)
        {
            string pathWithoutApplicationPath = widgetTempFilePath.Substring(FileService.GetBaseDirectory().Length + _widgetConfigService.GetTempFolderName().Length + sourceWidgetFolderName.Length + 2); // exclude "//" symbols

            return Path.Combine(_widgetConfigService.GetAreaPath(destinationWidgetFolderName), pathWithoutApplicationPath);
        }
    }
}
