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

namespace Core.Business.Plugin
{
    public class PluginUpdateService
    {
        private const string PluginDownloadFolderName = "PluginUpdates";
        private const string PluginTempUnpackFolderName = "TempPluginUpdates";

        #region Public Methods

        public bool IsDownloadingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings().IsDownloading;
            }
        }

        public bool IsCheckingForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings().IsChecking;
            }
        }

        public bool IsInstallingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings().IsInstalling;
            }
        }

        public DateTime GetLastCheckedForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings().LastCheckedDate;
            }
        }

        public int GetAmountUpdatesAvailable()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginOpenUpdatesRepository.GetAll().Count();
            }

        }

        public IEnumerable<PluginOpenUpdates> GetAvailableUpdatesFromDatabase()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginOpenUpdatesRepository.GetAll().ToList();
            }
        }

        public IEnumerable<Plugins> GetPluginsWithUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.PluginRepository.GetPluginsWithUpdatesAvailable().ToList();
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
                var settings = unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings();

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
                return unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings().IsUpdatingSuccess;
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
                foreach (var update in unitOfWorkRepository.PluginOpenUpdatesRepository.GetNotDownloadedUpdates())
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
            foreach (var plugin in GetUpdateAblePlugins())
            {
                var allUpdates = GetOnlineUpdates(plugin.UpdateUrl);

                var newUpdates = GetNotAddedUpdates(allUpdates, plugin.Version);

                foreach (var newUpdate in newUpdates)
                {
                    AddUpdateToPlugin(plugin.Id, newUpdate);
                }
            }

            SetUpdateChecked(DateTime.Now);
        }

        private void SetUpdateChecked(DateTime date)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings();

                settings.LastCheckedDate = date;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsCheckingForUpdates(bool isChecking)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings();

                settings.IsChecking = isChecking;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsInstalling(bool isInstalling)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings();

                settings.IsInstalling = isInstalling;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsDownloading(bool isDownloading)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings();

                settings.IsDownloading = isDownloading;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private IEnumerable<Plugins> GetUpdateAblePlugins()
        {
            var pluginService = new PluginService();

            return pluginService.GetAll().Where(w => !string.IsNullOrEmpty(w.UpdateUrl));
        }

        private PluginSettingsService LoadSettings(string pluginPath)
        {
            return new PluginSettingsService(Path.Combine(pluginPath, "Settings.xml"));
        }

        private void UpdatePluginVersion(int pluginId, string version)
        {
            var pluginService = new PluginService();

            var plugin = pluginService.Get(pluginId);

            plugin.Version = version;

            pluginService.SavePlugin(plugin);
        }

        private void RemoveOpenUpdate(int pluginId, string version)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.PluginOpenUpdatesRepository.GetByPluginIdAndVersion(pluginId,
                    version);

                if (openUpdate != null)
                {
                    unitOfWorkRepository.PluginOpenUpdatesRepository.Remove(openUpdate);

                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private string GetTempPath(int pluginId, string version)
        {
            return Path.Combine(FileService.GetBaseDirectory(), PluginTempUnpackFolderName, pluginId.ToString(), version);
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

        private void InstallUpdate(int pluginId, string zipDownloadPath, string version)
        {
            // Todo: Create rollback functionality
            var pluginService = new PluginService();

            var plugin = pluginService.Get(pluginId);

            string pluginTempPath = GetTempPath(plugin.Id, version);

            UnpackTempFiles(zipDownloadPath, pluginTempPath);

            var settingsUpdate = LoadSettings(pluginTempPath);

            InstallSqlScripts(settingsUpdate.GetSqlInstallScripts());

            CopyDllsToBin(pluginTempPath);

            RemoveOpenUpdate(plugin.Id, version);

            UpdatePluginInfo(plugin.Id, settingsUpdate.Name, settingsUpdate.Author,
                settingsUpdate.AuthorUrl,
                settingsUpdate.Description, settingsUpdate.UpdateUrl);

            UpdatePluginVersion(plugin.Id, version);

            FileService.DeleteFile(zipDownloadPath);
        }

        private void CopyDllsToBin(string pluginTempPath)
        {
            string binPluginPath = Path.Combine(pluginTempPath, "Content", "bin");

            if (Directory.Exists(binPluginPath))
            {
                var dllsInBinFolder = Directory.GetFiles(binPluginPath, "*.dll");

                foreach (var dll in dllsInBinFolder)
                {
                    FileService.CopyFile(dll, Path.Combine(FileService.GetBinPath(), Path.GetFileName(dll)));
                }
            }
        }


        private void UpdatePluginInfo(int pluginId, string name, string author, string authorUrl, string description, string updateUrl)
        {
            var pluginService = new PluginService();

            var plugin = pluginService.Get(pluginId);

            plugin.Name = name;
            plugin.Author = author;
            plugin.AuthorUrl = authorUrl;
            plugin.Description = description;
            plugin.UpdateUrl = updateUrl;

            pluginService.SavePlugin(plugin);
        }

        private void RemoveUnusedPluginDownloadsFolder()
        {
            string downloadsPath = Path.Combine(FileService.GetBaseDirectory(), PluginDownloadFolderName);

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

        private void RemoveTempPluginUpdatesFolder()
        {
            FileService.RemoveDirectory(Path.Combine(FileService.GetBaseDirectory(), PluginTempUnpackFolderName), recursive: true);
        }

        private void InstallUpdates()
        {
            var versionOrderingService = new VersionOrderingService();
            var settingsService = new SettingsService();
            var crashedPlugins = new Collection<int>();
            bool updateWhenIncorrectVersion = settingsService.UpdateWhenIncorrectVersion();
            string deskoVersion = settingsService.GetVersion();

            IEnumerable<PluginOpenUpdates> pluginOpenUpdates;

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                pluginOpenUpdates =
                    unitOfWorkRepository.PluginOpenUpdatesRepository.GetDownloadedUpdates()
                        .ToList()
                        .OrderBy(x => x.Version, versionOrderingService);
            }

            foreach (var update in pluginOpenUpdates)
            {
                if ((crashedPlugins.All(cw => cw != update.PluginId))
                    && (updateWhenIncorrectVersion || versionOrderingService.Equals(update.DeskoVersion, deskoVersion)))
                {
                    try
                    {
                        InstallUpdate(update.PluginId, update.DownloadPath, update.Version);
                    }
                    catch (Exception ex)
                    {
                        crashedPlugins.Add(update.PluginId);
                        ErrorLog.GetDefault(null).Log(new Error(ex));
                    }
                }
            }

            RemoveUnusedPluginDownloadsFolder();

            RemoveTempPluginUpdatesFolder();

            SetUpdatingSuccess(!crashedPlugins.Any());
        }

        private void SetUpdatingSuccess(bool isUpdatingSuccess)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.PluginUpdateSettingsRepository.GetSettings();
                settings.IsUpdatingSuccess = isUpdatingSuccess;
                unitOfWorkRepository.SaveChanges();
            }
        }

        private void AddToDownloadingList(PluginOpenUpdates updateModel, string downloadPath, int taskId)
        {
            PluginDownloadManager.Manager.AddDownload(updateModel.Id, updateModel.PluginId, new DownloadUpdate(updateModel.Name, updateModel.Description, updateModel.Version, updateModel.DownloadUrl, updateModel.DeskoVersion, taskId, downloadPath, 0));
        }

        private async Task AsyncDownloadUpdate(PluginOpenUpdates update)
        {
            var webOnlineService = new WebOnlineService();

            int taskId = Task.CurrentId.Value;

            string downloadPath = Path.Combine(FileService.GetBaseDirectory(), PluginDownloadFolderName, update.PluginId.ToString(), update.Version);

            FileService.CreateDirectory(downloadPath);

            string fileLocation = Path.Combine(downloadPath, string.Format("{0}.zip", update.Version));

            AddToDownloadingList(update, fileLocation, taskId);

            await webOnlineService.DownloadAsyncFile(update.DownloadUrl, fileLocation, HandlerProgressChanged, taskId, HandlerProgressComplete, taskId);
        }

        private void HandlerProgressChanged(object o, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, object taskId)
        {
            PluginDownloadManager.Manager.UpdateProgress((int)taskId, downloadProgressChangedEventArgs.ProgressPercentage);
        }

        private void HandlerProgressComplete(object o, AsyncCompletedEventArgs asyncCompletedEventHandler, object taskId)
        {
            int parsedTaskId = (int)taskId;

            var download = PluginDownloadManager.Manager.GetDownload(parsedTaskId);

            if (download != null)
            {
                SetOpenUpdateToDownloaded(download.ExtraId, download.Version, download.DownloadPath);

                PluginDownloadManager.Manager.RemoveDownload(parsedTaskId);
            }
        }

        private void SetOpenUpdateToDownloaded(int pluginId, string version, string downloadPath)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.PluginOpenUpdatesRepository.GetByPluginIdAndVersion(pluginId, version);

                if (openUpdate != null)
                {
                    openUpdate.IsDownloaded = true;
                    openUpdate.DownloadPath = downloadPath;
                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private void AddUpdateToPlugin(int pluginId, OpenUpdateModel newUpdate)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                unitOfWorkRepository.PluginOpenUpdatesRepository.Add(new PluginOpenUpdates
                {
                    PluginId = pluginId,
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
                return unitOfWorkRepository.PluginOpenUpdatesRepository.IsVersionAdded(version);
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
