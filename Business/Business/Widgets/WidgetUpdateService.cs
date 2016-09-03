using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using Core.Business.File;
using Core.Business.Settings;
using Core.Business.Software;
using Core.Business.Web;
using Core.Models.ThirdParty;
using Core.Models.Update;
using Database;
using Database.Entities;
using Elmah;
using Ionic.Zip;

namespace Core.Business.Widgets
{
    public class WidgetUpdateService
    {
        private const string WidgetsDownloadFolderName = "WidgetUpdates";
        private const string WidgetsTempUnpackFolderName = "TempWidgetsUpdates";

        #region Public Methods

        public bool IsDownloadingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings().IsDownloading;
            }
        }

        public bool IsCheckingForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings().IsChecking;
            }
        }

        public bool IsInstallingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings().IsInstalling;
            }
        }

        public DateTime GetLastCheckedForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings().LastCheckedDate;
            }
        }

        public int GetAmountUpdatesAvailable()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetOpenUpdatesRepository.GetAll().Count();
            }

        }

        public IEnumerable<WidgetOpenUpdates> GetAvailableUpdatesFromDatabase()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetOpenUpdatesRepository.GetAll().ToList();
            }
        }

        public IEnumerable<Database.Entities.Widgets> GetWidgetsWithUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetsRepository.GetWidgetsWithUpdatesAvailable().ToList();
            }
        }

        public void SearchAndDownloadUpdates()
        {
            SetIsCheckingForUpdates(true);
            GetAndAddOnlineUpdates();
            SetIsCheckingForUpdates(false);
            SetIsDownloading(true);
            DownloadOpenUpdates();
            SetIsDownloading(false);
        }

        public void InstallDownloadedUpdates()
        {
            if (OpenUpdatesAvailable())
            {
                var settingsService = new SettingsService();

                SetIsInstalling(true);

                settingsService.UpdateMaintenance(true);

                InstallUpdates();

                settingsService.UpdateMaintenance(false);

                SetIsInstalling(false);
            }
        }

        public void UpdateInBackground()
        {
            SetIsCheckingForUpdates(true);

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                GetAndAddOnlineUpdates();

                SetIsCheckingForUpdates(false);

                SetIsDownloading(true);

                DownloadOpenUpdates();

                SetIsDownloading(false);

                SetIsInstalling(true);

                if (OpenUpdatesAvailable())
                {
                    var settingsService = new SettingsService();

                    settingsService.UpdateMaintenance(true);

                    InstallUpdates();

                    settingsService.UpdateMaintenance(false);
                }

                SetIsInstalling(false);
            });
        }

        public UpdatingStatus GetUpdatingStatus()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings();

                if (settings.IsChecking)
                {
                    return UpdatingStatus.Checking;
                }

                if (settings.IsDownloading)
                {
                    return UpdatingStatus.Downloading;
                }

                if (settings.IsInstalling)
                {
                    return UpdatingStatus.Installing;
                }

                return UpdatingStatus.None;
            }
        }

        public bool IsUpdatedSuccessfully()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings().IsUpdatingSuccess;
            }
        }

        public bool IsUpdating()
        {
            return IsCheckingForUpdates() || IsDownloadingUpdates() || IsInstallingUpdates();
        }

        #endregion

        #region Private Methods

        private void DownloadOpenUpdates()
        {
            var asyncTasks = new Collection<Task>();

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                foreach (var update in unitOfWorkRepository.WidgetOpenUpdatesRepository.GetNotDownloadedUpdates())
                {
                    var localUpdate = update;

                    asyncTasks.Add(Task.Run(() => AsyncDownloadUpdate(localUpdate)));
                }
            }
            Task.WaitAll(asyncTasks.ToArray());
        }

        private bool OpenUpdatesAvailable()
        {
            return GetAmountUpdatesAvailable() > 0;
        }

        private void GetAndAddOnlineUpdates()
        {
            foreach (var widget in GetUpdateAbleWidgets())
            {
                var allUpdates = GetOnlineUpdates(widget.UpdateUrl);

                var newUpdates = GetNotAddedUpdates(allUpdates, widget.Version);

                foreach (var newUpdate in newUpdates)
                {
                    AddUpdateToWidget(widget.Id, newUpdate);
                }
            }

            SetUpdateChecked(DateTime.Now);
        }

        private void SetUpdateChecked(DateTime date)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings();

                settings.LastCheckedDate = date;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsCheckingForUpdates(bool isChecking)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings();

                settings.IsChecking = isChecking;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsInstalling(bool isInstalling)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings();

                settings.IsInstalling = isInstalling;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsDownloading(bool isDownloading)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings();

                settings.IsDownloading = isDownloading;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private IEnumerable<Database.Entities.Widgets> GetUpdateAbleWidgets()
        {
            var widgetService = new WidgetService();

            return widgetService.GetAllWidgets().Where(w => !string.IsNullOrEmpty(w.UpdateUrl));
        }

        private WidgetSettingsService LoadSettings(string widgetPath)
        {
            return new WidgetSettingsService(Path.Combine(widgetPath, "Settings.xml"));
        }

        private void UpdateWidgetVersion(int widgetId, string version)
        {
            var widgetService = new WidgetService();

            var widget = widgetService.GetWidget(widgetId);

            widget.Version = version;

            widgetService.SaveWidget(widget);
        }

        private void RemoveOpenUpdate(int widgetId, string version)
        {
            // Create a new uof because of async method
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.WidgetOpenUpdatesRepository.GetByWidgetIdAndVersion(widgetId,
                    version);

                if (openUpdate != null)
                {
                    unitOfWorkRepository.WidgetOpenUpdatesRepository.Remove(openUpdate);

                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private string GetTempPath(int widgetId, string version)
        {
            return Path.Combine(FileService.GetBaseDirectory(), WidgetsTempUnpackFolderName, widgetId.ToString(), version);
        }

        private void MoveFiles(string tempDownloadPath, string widgetAreaName)
        {
            var widgetConfigService = new WidgetConfigService();
            string sourcePath = Path.Combine(tempDownloadPath, "Content");

            if (FileService.DoesDirectoryExist(sourcePath))
            {
                FileService.MoveDirectory(sourcePath, widgetConfigService.GetAreaPath(widgetAreaName));
            }
        }

        private void UnpackTempFiles(string pathDownloadedUpdate, string tempPath)
        {
            FileService.CreateDirectory(tempPath);

            using (ZipFile zipFile = ZipFile.Read(pathDownloadedUpdate))
            {
                foreach (ZipEntry e in zipFile)
                {
                    e.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void InstallSqlScripts(IEnumerable<SqlScript> sqlFilePaths)
        {
            using (var databaseService = new DatabaseService())
            {
                if (sqlFilePaths.Any())
                {
                    using (var transaction = databaseService.BeginTransaction())
                    {
                        foreach (var scriptPath in sqlFilePaths)
                        {
                            databaseService.ExecuteQuery(FileService.GetFileContent(scriptPath.Location), transaction);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        private void InstallUpdate(int widgetId, string zipDownloadPath, string version)
        {
            // Todo: Create rollback functionality
            var widgetService = new WidgetService();

            var widget = widgetService.GetWidget(widgetId);

            string widgetTempPath = GetTempPath(widget.Id, version);

            UnpackTempFiles(zipDownloadPath, widgetTempPath);

            var settingsUpdate = LoadSettings(widgetTempPath);

            InstallSqlScripts(settingsUpdate.GetSqlInstallScripts());

            CopyDllsToBin(widgetTempPath);

            MoveFiles(widgetTempPath, widget.AreaName);

            RemoveOpenUpdate(widget.Id, version);

            ConfigureNavigationLinks(widget.Id, settingsUpdate.GetNavigation());

            ConfigureBreadcrumbs(widget.Id, settingsUpdate.GetBreadcrumbs());

            UpdateWidgetInfo(widget.Id, settingsUpdate.WidgetName, settingsUpdate.Author,
                settingsUpdate.AuthorUrl, settingsUpdate.StartController,
                settingsUpdate.StartIndex, settingsUpdate.Image,
                settingsUpdate.Description, settingsUpdate.UpdateUrl,
                settingsUpdate.Namespace);

            UpdateWidgetVersion(widget.Id, version);

            FileService.DeleteFile(zipDownloadPath);
        }

        private void CopyDllsToBin(string widgetTempPath)
        {
            string binWidgetPath = Path.Combine(widgetTempPath, "Content", "bin");

            if (Directory.Exists(binWidgetPath))
            {
                var dllsInBinFolder = Directory.GetFiles(binWidgetPath, "*.dll");

                foreach (var dll in dllsInBinFolder)
                {
                    FileService.CopyFile(dll, Path.Combine(FileService.GetBinPath(), Path.GetFileName(dll)));
                }
            }
        }

        private void ConfigureBreadcrumbs(int widgetId, IEnumerable<AdminBreadcrumbSettingsModel> widgetBreadcrumbs)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                foreach (var breadcrumb in widgetBreadcrumbs)
                {
                    var dbBreadcrumb =
                        unitOfWorkRepository.AdminBreadCrumbsRepository.GetByControllerAndAction(breadcrumb.Controller,
                            breadcrumb.Action);

                    if (dbBreadcrumb == null)
                    {
                        unitOfWorkRepository.AdminBreadCrumbsRepository.Add(new AdminBreadcrumbs
                        {
                            Action = breadcrumb.Action,
                            Controller = breadcrumb.Controller,
                            LocalizationBase = breadcrumb.LocalizedBase,
                            LocalizedTitle = breadcrumb.LocalizedTitle,
                            LocalizedActionFriendlyName = breadcrumb.LocalizedActionFriendlyName,
                            LocalizedControllerFriendlyName = breadcrumb.LocalizedControllerFriendlyName,
                            LocalizedDescription = breadcrumb.LocalizedDescription,
                            WidgetId = widgetId,
                            Icon = breadcrumb.Icon
                        });
                    }
                    else
                    {
                        dbBreadcrumb.LocalizationBase = breadcrumb.LocalizedBase;
                        dbBreadcrumb.LocalizedTitle = breadcrumb.LocalizedTitle;
                        dbBreadcrumb.LocalizedActionFriendlyName = breadcrumb.LocalizedActionFriendlyName;
                        dbBreadcrumb.LocalizedControllerFriendlyName = breadcrumb.LocalizedControllerFriendlyName;
                        dbBreadcrumb.LocalizedDescription = breadcrumb.LocalizedDescription;
                        dbBreadcrumb.Icon = breadcrumb.Icon;
                    }
                }

                unitOfWorkRepository.AdminBreadCrumbsRepository.GetBreadcrumbsByWidgetId(widgetId)
                    .ToList()
                    .ForEach(dbBreadCrumb =>
                    {
                        if (!widgetBreadcrumbs.Any(
                            b =>
                                b.Action.Equals(dbBreadCrumb.Action, StringComparison.CurrentCultureIgnoreCase) &&
                                b.Controller.Equals(dbBreadCrumb.Controller, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            unitOfWorkRepository.AdminBreadCrumbsRepository.Remove(dbBreadCrumb);
                        }
                    });

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void ConfigureNavigationLinks(int widgetId, IEnumerable<AdminNavigationSettingsModel> widgetNavigation)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                foreach (var navigation in widgetNavigation)
                {
                    var dbNavigation =
                        unitOfWorkRepository.AdminNavigationRepository.GetByControllerAndAction(
                            navigation.Controller,
                            navigation.Action);

                    if (dbNavigation == null)
                    {
                        unitOfWorkRepository.AdminNavigationRepository.Add(new AdminNavigation
                        {
                            Action = navigation.Action,
                            Controller = navigation.Controller,
                            Icon = navigation.Icon,
                            LocalizedBase = navigation.LocalizedBase,
                            LocalizedName = navigation.LocalizedName,
                            WidgetId = widgetId
                        });
                    }
                    else
                    {
                        dbNavigation.Icon = navigation.Icon;
                        dbNavigation.LocalizedBase = navigation.LocalizedBase;
                        dbNavigation.LocalizedName = navigation.LocalizedName;
                    }
                }

                // Remove unused navigation links
                unitOfWorkRepository.AdminNavigationRepository.GetAllByWidgetId(widgetId).ToList().ForEach(w =>
                {
                    if (
                        !widgetNavigation.Any(
                            wn =>
                                wn.Action.Equals(w.Action, StringComparison.CurrentCultureIgnoreCase) &&
                                wn.Controller.Equals(w.Controller, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        unitOfWorkRepository.AdminNavigationRepository.RemoveNavigation(w);
                    }
                });

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void UpdateWidgetInfo(int widgetId, string name, string author, string authorUrl, string startController, string startIndex, string image, string description, string updateUrl, string @namespace)
        {
            var widgetService = new WidgetService();

            var widget = widgetService.GetWidget(widgetId);

            widget.Name = name;
            widget.Author = author;
            widget.AuthorUrl = authorUrl;
            widget.Controller = startController;
            widget.StartIndex = startIndex;
            widget.Image = image;
            widget.Description = description;
            widget.UpdateUrl = updateUrl;
            widget.Namespace = @namespace;

            widgetService.SaveWidget(widget);
        }

        private void RemoveUnusedWidgetDownloadsFolder()
        {
            string downloadsPath = Path.Combine(FileService.GetBaseDirectory(), WidgetsDownloadFolderName);

            // Remove sub folders
            if (Directory.Exists(downloadsPath))
            {
                foreach (var directory in Directory.GetDirectories(downloadsPath, "*", SearchOption.AllDirectories))
                {
                    if (Directory.Exists(directory))
                    {
                        if (!Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Any())
                        {
                            Directory.Delete(directory, true);
                        }
                    }
                }
            }

            // Remove downloads folder
            if (!Directory.GetFiles(downloadsPath, "*.*", SearchOption.AllDirectories).Any())
            {
                Directory.Delete(downloadsPath);
            }
        }

        private void RemoveTempWidgetsUpdatesFolder()
        {
            FileService.RemoveDirectory(Path.Combine(FileService.GetBaseDirectory(), WidgetsTempUnpackFolderName), recursive: true);
        }

        private void InstallUpdates()
        {
            var versionOrderingService = new VersionOrderingService();
            var settingsService = new SettingsService();
            var crashedWidgets = new Collection<int>();
            bool updateWhenIncorrectVersion = settingsService.UpdateWhenIncorrectVersion();
            string deskoVersion = settingsService.GetVersion();

            IEnumerable<WidgetOpenUpdates> widgetOpenUpdates;

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                widgetOpenUpdates =
                    unitOfWorkRepository.WidgetOpenUpdatesRepository.GetDownloadedUpdates()
                        .ToList()
                        .OrderBy(x => x.Version, versionOrderingService);
            }

            foreach (var update in widgetOpenUpdates)
            {
                if ((crashedWidgets.All(cw => cw != update.WidgetId))
                    && (updateWhenIncorrectVersion || versionOrderingService.Equals(update.DeskoVersion, deskoVersion)))
                {
                    try
                    {
                        InstallUpdate(update.WidgetId, update.DownloadPath, update.Version);
                    }
                    catch (Exception ex)
                    {
                        crashedWidgets.Add(update.WidgetId);
                        ErrorLog.GetDefault(null).Log(new Error(ex));
                    }
                }
            }

            RemoveUnusedWidgetDownloadsFolder();

            RemoveTempWidgetsUpdatesFolder();

            SetUpdatingSuccess(!crashedWidgets.Any());
        }

        private void SetUpdatingSuccess(bool isUpdatingSuccess)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.WidgetUpdateSettingsRepository.GetSettings();
                settings.IsUpdatingSuccess = isUpdatingSuccess;
                unitOfWorkRepository.SaveChanges();
            }
        }

        private void AddToDownloadingList(WidgetOpenUpdates updateModel, string downloadPath, int taskId)
        {
            WidgetDownloadManager.Manager.AddDownload(updateModel.Id, updateModel.WidgetId, new DownloadUpdate(updateModel.Name, updateModel.Description, updateModel.Version, updateModel.DownloadUrl, updateModel.DeskoVersion, taskId, downloadPath, 0));
        }

        private async Task AsyncDownloadUpdate(WidgetOpenUpdates update)
        {
            var webOnlineService = new WebOnlineService();

            int taskId = Task.CurrentId.Value;

            string downloadPath = Path.Combine(FileService.GetBaseDirectory(), WidgetsDownloadFolderName, update.WidgetId.ToString(), update.Version);

            FileService.CreateDirectory(downloadPath);

            string fileLocation = Path.Combine(downloadPath, string.Format("{0}.zip", update.Version));

            AddToDownloadingList(update, fileLocation, taskId);

            await webOnlineService.DownloadAsyncFile(update.DownloadUrl, fileLocation, HandlerProgressChanged, taskId, HandlerProgressComplete, taskId);
        }

        private void HandlerProgressChanged(object o, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, object taskId)
        {
            WidgetDownloadManager.Manager.UpdateProgress((int)taskId, downloadProgressChangedEventArgs.ProgressPercentage);
        }

        private void HandlerProgressComplete(object o, AsyncCompletedEventArgs asyncCompletedEventHandler, object taskId)
        {
            int parsedTaskId = (int)taskId;

            var download = WidgetDownloadManager.Manager.GetDownload(parsedTaskId);

            if (download != null)
            {
                SetOpenUpdateToDownloaded(download.ExtraId, download.Version, download.DownloadPath);

                WidgetDownloadManager.Manager.RemoveDownload(parsedTaskId);
            }
        }

        private void SetOpenUpdateToDownloaded(int widgetId, string version, string downloadPath)
        {
            // Create a new uof because of async method
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.WidgetOpenUpdatesRepository.GetByWidgetIdAndVersion(widgetId, version);

                if (openUpdate != null)
                {
                    openUpdate.IsDownloaded = true;
                    openUpdate.DownloadPath = downloadPath;
                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private void AddUpdateToWidget(int widgetId, OpenUpdateModel newUpdate)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                unitOfWorkRepository.WidgetOpenUpdatesRepository.Add(new WidgetOpenUpdates
                {
                    WidgetId = widgetId,
                    Version = newUpdate.Version,
                    Description = newUpdate.Description,
                    Name = newUpdate.Name,
                    IsDownloaded = false,
                    DownloadUrl = newUpdate.DownloadUrl,
                    DeskoVersion = newUpdate.DeskoVersion
                });

                unitOfWorkRepository.SaveChanges();
            }
        }

        private IEnumerable<OpenUpdateModel> GetNotAddedUpdates(IEnumerable<OpenUpdateModel> availableUpdates, string currentVersion)
        {
            var versionOrderingService = new VersionOrderingService();

            return availableUpdates.Where(
                  update =>
                      versionOrderingService.Compare(update.Version, currentVersion) > 0 &&
                      !DoesVersionExistInOpenUpdates(update.Version)).ToList();
        }

        private bool DoesVersionExistInOpenUpdates(string version)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.WidgetOpenUpdatesRepository.IsVersionAdded(version);
            }
        }

        private IEnumerable<OpenUpdateModel> GetOnlineUpdates(string downloadUrl)
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            var webOnlineService = new WebOnlineService();

            return
                javaScriptSerializer.Deserialize<IEnumerable<OpenUpdateModel>>(
                    webOnlineService.DownloadPage(downloadUrl)).ToList();
        }

        #endregion
    }
}
