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
using Core.Business.Web;
using Core.Models.Update;
using Database;
using Database.Entities;
using Elmah;
using Ionic.Zip;

namespace Core.Business.Software
{
    public class SoftwareUpdateService
    {
        /* Update Package explanation
         *      Version.Zip
         *      - Content
         *      - Sql
         *      - Settings.xml
         */

        private const string SystemUpdatesFolderName = "SystemUpdates";
        private const string SystemTempUpdatesFolderName = "TempSystemUpdates";

        public bool IsUpdating()
        {
            return IsCheckingForUpdates() || IsDownloadingUpdates() || IsInstallingUpdates();
        }

        public bool IsDownloadingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings().IsDownloading;
            }
        }

        public bool IsCheckingForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings().IsChecking;
            }
        }

        public bool IsInstallingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings().IsInstalling;
            }
        }

        public bool IsUpdatedSuccessfully()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings().IsUpdatingSuccess;
            }
        }

        public void SetUpdateChecked(DateTime date)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings();

                settings.LastCheckedDate = date;

                unitOfWorkRepository.SaveChanges();
            }
        }

        public void SetIsCheckingForUpdates(bool isChecking)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings();

                settings.IsChecking = isChecking;

                unitOfWorkRepository.SaveChanges();
            }
        }

        public DateTime GetLastCheckedForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings().LastCheckedDate;
            }
        }

        public void SetIsInstalling(bool isInstalling)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings();

                settings.IsInstalling = isInstalling;

                unitOfWorkRepository.SaveChanges();
            }
        }

        public void SetIsDownloading(bool isDownloading)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings();

                settings.IsDownloading = isDownloading;

                unitOfWorkRepository.SaveChanges();
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

                if (OpenUpdatesAvailable())
                {
                    SetIsInstalling(true);

                    var settingsService = new SettingsService();

                    settingsService.UpdateMaintenance(true);

                    InstallUpdates();

                    SetIsInstalling(false);

                    settingsService.UpdateMaintenance(false);
                }
            });
        }

        public void InstallDownloadedUpdates()
        {
            if (OpenUpdatesAvailable())
            {
                SetIsInstalling(true);

                var settingsService = new SettingsService();

                settingsService.UpdateMaintenance(true);

                InstallUpdates();

                settingsService.UpdateMaintenance(false);

                SetIsInstalling(false);
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

        public IEnumerable<OpenUpdates> GetAvailableUpdatesFromDatabase()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.OpenUpdatesRepository.GetAll().ToList();
            }
        }

        public UpdatingStatus GetUpdatingStatus()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings();

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

        #region Private Methods

        private bool OpenUpdatesAvailable()
        {
            return GetAvailableUpdatesFromDatabase().Any();
        }

        private void SetUpdateSuccess(bool isSuccess)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.SoftwareUpdateSettingsRepository.GetSettings();

                settings.IsUpdatingSuccess = isSuccess;

                unitOfWorkRepository.SaveChanges();
            }
        }


        private void GetAndAddOnlineUpdates()
        {
            var settingsService = new SettingsService();

            var currentVersion = settingsService.GetVersion();

            var versionOrderingService = new VersionOrderingService();

            var allUpdates = GetOnlineUpdates();

            var openUpdates =
                allUpdates.Where(
                    update =>
                        versionOrderingService.Compare(update.Version, currentVersion) > 0 &&
                        !DoesVersionExistInOpenUpdates(update.Version));

            foreach (var openUpdate in openUpdates)
            {
                AddUpdateToDatabase(openUpdate);
            }

            SetUpdateChecked(DateTime.Now);
        }

        private bool DoesVersionExistInOpenUpdates(string version)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.OpenUpdatesRepository.IsVersionAdded(version);
            }
        }

        private void DownloadOpenUpdates()
        {
            var asyncTasks = new Collection<Task>();

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                foreach (var update in unitOfWorkRepository.OpenUpdatesRepository.GetNotDownloadedUpdates())
                {
                    var localUpdate = update;

                    asyncTasks.Add(Task.Run(() => AsyncDownloadUpdate(localUpdate)));
                }
            }

            Task.WaitAll(asyncTasks.ToArray());
        }

        private void InstallUpdates()
        {
            var versionOrderingService = new VersionOrderingService();

            IEnumerable<OpenUpdates> openUpdates;

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                openUpdates =
                   unitOfWorkRepository.OpenUpdatesRepository.GetDownloadedUpdates()
                       .ToList()
                       .OrderBy(x => x.Version, versionOrderingService);

            }

            bool isUpdatingSuccess = true;

            CreateSystemUpdatesFolder();

            foreach (var update in openUpdates)
            {
                try
                {
                    InstallUpdate(update.DownloadPath, update.Version);
                }
                catch (Exception ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                    isUpdatingSuccess = false;
                    break;
                }
            }

            RemoveUnusedSystemDownloadsFolder();

            RemoveTempUpdatesFolder();

            SetUpdateSuccess(isUpdatingSuccess);
        }

        private void RemoveTempUpdatesFolder()
        {
            FileService.RemoveDirectory(Path.Combine(FileService.GetBaseDirectory(), SystemTempUpdatesFolderName), true);
        }

        private void InstallUpdate(string downloadPath, string version)
        {
            string tempPath = GetTempPath(version);

            UnpackTempFiles(downloadPath, tempPath);

            var settingsUpdate = GetSettings(tempPath);

            InstallSqlScripts(GetSqlScripts(tempPath));

            MoveFiles(tempPath);

            DeleteRelativePathFiles(settingsUpdate.GetRelativePathsToDelete());

            DeleteInstallFile(downloadPath);

            RemoveOpenUpdate(version);

            UpdateSoftwareVersion(version);
        }

        private void UpdateSoftwareVersion(string version)
        {
            var settingsService = new SettingsService();
            settingsService.SetVersion(version);
        }

        private void DeleteInstallFile(string downloadPath)
        {
            FileService.DeleteFile(downloadPath);
        }

        private void RemoveOpenUpdate(string version)
        {
            // Create a new uof because of async method
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.OpenUpdatesRepository.GetByVersion(version);

                if (openUpdate != null)
                {
                    unitOfWorkRepository.OpenUpdatesRepository.Remove(openUpdate);

                    unitOfWorkRepository.SaveChanges();
                }
            };
        }

        private string GetTempPath(string version)
        {
            return Path.Combine(FileService.GetBaseDirectory(), SystemTempUpdatesFolderName, version);
        }

        private IEnumerable<string> GetSqlScripts(string tempDownloadPath)
        {
            return FileService.GetFiles(Path.Combine(tempDownloadPath, "Sql"), "*.sql");
        }

        private void MoveFiles(string tempDownloadPath)
        {
            string contentPath = Path.Combine(tempDownloadPath, "Content");

            if (FileService.DoesDirectoryExist(contentPath))
            {
                FileService.MoveDirectory(contentPath, FileService.GetBaseDirectory());
            }
        }

        private SoftwareSettings GetSettings(string tempDownloadPath)
        {
            return new SoftwareSettings(Path.Combine(tempDownloadPath, "Settings.xml"));
        }

        private void DeleteRelativePathFiles(IEnumerable<string> relativePathFiles)
        {
            foreach (var filePath in relativePathFiles)
            {
                FileService.DeleteFile(Path.Combine(FileService.GetBaseDirectory(), filePath));
            }
        }

        private void RemoveUnusedSystemDownloadsFolder()
        {
            string downloadsPath = Path.Combine(FileService.GetBaseDirectory(), SystemUpdatesFolderName);

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

            // Remove download folder
            if (!Directory.GetFiles(downloadsPath, "*.*", SearchOption.AllDirectories).Any())
            {
                Directory.Delete(downloadsPath);
            }
        }

        private void CreateSystemUpdatesFolder()
        {
            FileService.CreateDirectory(Path.Combine(FileService.GetBaseDirectory(), SystemUpdatesFolderName));
        }

        private void InstallSqlScripts(IEnumerable<string> sqlFilePaths)
        {
            using (var databaseService = new DatabaseService())
            {
                if (sqlFilePaths.Any())
                {
                    using (var transaction = databaseService.BeginTransaction())
                    {
                        foreach (var scriptPath in sqlFilePaths)
                        {
                            databaseService.ExecuteQuery(FileService.GetFileContent(scriptPath), transaction);
                        }

                        transaction.Commit();
                    }
                }
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

        private void AddUpdateToDatabase(OpenUpdateModel downloadUpdate)
        {
            // Create a new uof because of async method
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                unitOfWorkRepository.OpenUpdatesRepository.Add(new OpenUpdates
                {
                    Version = downloadUpdate.Version,
                    Description = downloadUpdate.Description,
                    Name = downloadUpdate.Name,
                    IsDownloaded = false,
                    DownloadUrl = downloadUpdate.DownloadUrl
                });

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetOpenUpdateToDownloaded(string version, string downloadPath)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.OpenUpdatesRepository.GetByVersion(version);

                if (openUpdate != null)
                {
                    openUpdate.IsDownloaded = true;
                    openUpdate.DownloadPath = downloadPath;
                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private IEnumerable<OpenUpdateModel> GetOnlineUpdates()
        {
            var webOnlineService = new WebOnlineService();
            const string checkUpdatesUrl = "http://deskolist.com/software-updates";
            return new JavaScriptSerializer().Deserialize<IEnumerable<OpenUpdateModel>>(webOnlineService.DownloadPage(checkUpdatesUrl));
        }

        private async Task AsyncDownloadUpdate(OpenUpdates update)
        {
            var webOnlineService = new WebOnlineService();

            int taskId = Task.CurrentId.Value;

            string downloadsPath = Path.Combine(FileService.GetBaseDirectory(), SystemUpdatesFolderName);

            FileService.CreateDirectory(downloadsPath);

            string downloadPath = Path.Combine(downloadsPath, string.Format("{0}.zip", update.Version));

            AddToDownloadingList(update, downloadPath, taskId);

            await webOnlineService.DownloadAsyncFile(update.DownloadUrl, downloadPath, HandlerProgressChanged, taskId, HandlerProgressComplete, taskId);
        }

        private void HandlerProgressChanged(object o, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, object taskId)
        {
            SoftwareDownloadManager.Manager.UpdateProgress((int)taskId, downloadProgressChangedEventArgs.ProgressPercentage);
        }

        private void HandlerProgressComplete(object o, AsyncCompletedEventArgs asyncCompletedEventHandler, object taskId)
        {
            int parsedTaskId = (int)taskId;

            var download = SoftwareDownloadManager.Manager.GetDownload(parsedTaskId);

            if (download != null)
            {
                SetOpenUpdateToDownloaded(download.Version, download.DownloadPath);

                SoftwareDownloadManager.Manager.RemoveDownload(parsedTaskId);
            }
        }

        private void AddToDownloadingList(OpenUpdates updateModel, string downloadPath, int taskId)
        {
            SoftwareDownloadManager.Manager.AddDownload(taskId, new DownloadUpdate(updateModel.Name, updateModel.Description, updateModel.Version, updateModel.DownloadUrl, null, taskId, downloadPath, 0));
        }

        #endregion
    }
}
