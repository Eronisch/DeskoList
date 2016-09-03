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
using Core.Models.Themes;
using Core.Models.Update;
using Database;
using Database.Entities;
using Elmah;
using Ionic.Zip;

namespace Core.Business.Themes
{
    public class ThemeUpdateService
    {
        private const string ThemesDownloadFolderName = "ThemeUpdates";
        private const string ThemesTempUnpackFolderName = "TempThemeUpdates";

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
                    InstallUpdates();
                }
            });
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
                SetIsInstalling(true);

                InstallUpdates();

                SetIsInstalling(false);
            }
        }

        public IEnumerable<Database.Entities.Themes> GetWidgetsWithUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.ThemesRepository.GetThemesWithUpdatesAvailable().ToList();
            }
        }

        public DateTime GetLastCheckedForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

                return settings.LastCheckedDate;
            }
        }

        public bool IsDownloadingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

                return settings.IsDownloading;
            }
        }

        public bool IsCheckingForUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();
                return settings.IsChecking;
            }
        }

        public bool IsInstallingUpdates()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();
                return settings.IsInstalling;
            }
        }

        public int GetAmountUpdatesAvailable()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.ThemeOpenUpdatesRepository.GetAll().Count();
            }
        }

        public UpdatingStatus GetUpdatingStatus()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

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

        public bool IsUpdating()
        {
            return IsCheckingForUpdates() || IsDownloadingUpdates() || IsInstallingUpdates();
        }

        public bool IsUpdatedSuccessfully()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();
                return settings.IsUpdatingSuccess;
            }
        }

        #region Private Methods

        private void GetAndAddOnlineUpdates()
        {
            foreach (var theme in GetUpdateAbleThemes())
            {
                var allUpdates = GetOnlineUpdates(theme.UpdateUrl);

                var newUpdates = GetNotAddedUpdates(allUpdates, theme.Version);

                foreach (var newUpdate in newUpdates)
                {
                    AddUpdateToTheme(theme.Id, newUpdate);
                }
            }

            SetUpdateChecked(DateTime.Now);
        }

        private void DownloadOpenUpdates()
        {
            var asyncTasks = new Collection<Task>();

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                foreach (var update in unitOfWorkRepository.ThemeOpenUpdatesRepository.GetNotDownloadedUpdates())
                {
                    var localUpdate = update;

                    asyncTasks.Add(Task.Run(() => AsyncDownloadUpdate(localUpdate)));
                }
            }

            Task.WaitAll(asyncTasks.ToArray());
        }

        private void InstallUpdates()
        {
            SetIsInstalling(true);

            var versionOrderingService = new VersionOrderingService();
            var settingsService = new SettingsService();
            bool updateWhenIncorrectVersion = settingsService.UpdateWhenIncorrectVersion();
            string deskoVersion = settingsService.GetVersion();
            var crashedThemes = new Collection<int>();

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdates = unitOfWorkRepository.ThemeOpenUpdatesRepository.GetDownloadedUpdates().ToList().OrderBy(x => x.Version, versionOrderingService);

                foreach (var update in openUpdates)
                {
                    if ((crashedThemes.All(cw => cw != update.ThemeId))
                        && (updateWhenIncorrectVersion || versionOrderingService.Equals(update.DeskoVersion, deskoVersion)))
                    {
                        try
                        {
                            InstallUpdate(update.ThemeId, update.DownloadPath, update.Version);
                        }
                        catch (Exception ex)
                        {
                            crashedThemes.Add(update.ThemeId);
                            ErrorLog.GetDefault(null).Log(new Error(ex));
                        }
                    }
                }
            }

            RemoveUnusedThemesDownloadsFolder();

            RemoveTempThemesUpdatesFolder();

            SetUpdatingSuccess(!crashedThemes.Any());

            SetIsInstalling(false);
        }

        private void SetUpdatingSuccess(bool isUpdatingSuccess)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();
                settings.IsUpdatingSuccess = isUpdatingSuccess;
                unitOfWorkRepository.SaveChanges();
            }
        }

        private void InstallUpdate(int themeId, string zipDownloadPath, string version)
        {
            var themeService = new ThemeService();

            // Todo: Create rollback functionality
            var theme = themeService.GetThemeById(themeId);

            string themeTempPath = GetTempPath(theme.Id, version);

            UnpackTempFiles(zipDownloadPath, themeTempPath);

            var settingsUpdate = LoadSettings(themeTempPath);

            ConfigureThemeSections(theme.Id, settingsUpdate.ThemeSections);

            MoveFiles(themeTempPath, theme.FolderName);

            RemoveOpenUpdate(theme.Id, version);

            UpdateThemeInfo(theme.Id, settingsUpdate.Name, settingsUpdate.Author,
                settingsUpdate.AuthorUrl, settingsUpdate.Image,
                settingsUpdate.Description, settingsUpdate.UpdateUrl);

            UpdateThemeVersion(theme.Id, version);

            FileService.DeleteFile(zipDownloadPath);
        }

        private void ConfigureThemeSections(int themeId, IEnumerable<ThemeSectionModel> settingsThemeSections)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var theme = unitOfWorkRepository.ThemesRepository.GetById(themeId);

                var dbThemeSections = theme.WidgetsThemeSection.ToList();

                foreach (var section in settingsThemeSections)
                {
                    var dbThemeSection =
                        dbThemeSections.FirstOrDefault(
                            t => t.CodeName.Equals(section.CodeName, StringComparison.CurrentCultureIgnoreCase));

                    if (dbThemeSection == null)
                    {
                        // Add new theme section
                        unitOfWorkRepository.ThemeSectionRepository.AddThemeSection(new WidgetsThemeSection
                        {
                            ThemeId = themeId,
                            CodeName = section.CodeName,
                            FriendlyName = section.FriendlyName
                        });
                    }
                    else
                    {
                        // Update name
                        dbThemeSection.FriendlyName = section.FriendlyName;
                    }
                }

                dbThemeSections.ToList().ForEach(section =>
                {
                    // Remove unused theme section
                    if (settingsThemeSections.All(s => s.CodeName != section.CodeName))
                    {
                        unitOfWorkRepository.ThemeSectionRepository.RemoveThemeSection(section);
                    }
                });

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void UpdateThemeVersion(int themeId, string version)
        {
            var themeService = new ThemeService();

            var theme = themeService.GetThemeById(themeId);

            theme.Version = version;

            themeService.UpdateTheme(theme);
        }

        private void UpdateThemeInfo(int themeId, string name, string author, string authorUrl, string image, string description, string updateUrl)
        {
            var themeService = new ThemeService();

            var theme = themeService.GetThemeById(themeId);

            theme.ThemeName = name;
            theme.AuthorName = author;
            theme.AuthorUrl = authorUrl;
            theme.Image = image;
            theme.Description = description;
            theme.UpdateUrl = updateUrl;

            themeService.UpdateTheme(theme);
        }

        private void RemoveOpenUpdate(int themeId, string version)
        {
            // Create a new uof because of async method
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.ThemeOpenUpdatesRepository.GetByThemeIdAndVersion(themeId,
                    version);

                if (openUpdate != null)
                {
                    unitOfWorkRepository.ThemeOpenUpdatesRepository.Remove(openUpdate);

                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private ThemeSettingsService LoadSettings(string tempThemePath)
        {
            return new ThemeSettingsService(Path.Combine(tempThemePath, "Settings.xml"));
        }

        private void RemoveTempThemesUpdatesFolder()
        {
            FileService.RemoveDirectory(Path.Combine(FileService.GetBaseDirectory(), ThemesTempUnpackFolderName), recursive: true);
        }

        private void RemoveUnusedThemesDownloadsFolder()
        {
            string downloadsPath = Path.Combine(FileService.GetBaseDirectory(), ThemesDownloadFolderName);

            if (Directory.Exists(downloadsPath))
            {
                foreach (var directory in Directory.GetDirectories(downloadsPath, "*", SearchOption.AllDirectories))
                {
                    if (Directory.Exists(directory))
                    {
                        if (!Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Any())
                        {
                            Directory.Delete(directory);
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

        private string GetTempPath(int themeId, string version)
        {
            return Path.Combine(FileService.GetBaseDirectory(), ThemesTempUnpackFolderName, themeId.ToString(), version);
        }

        private void MoveFiles(string tempDownloadPath, string themeFolderName)
        {
            string sourcePath = Path.Combine(tempDownloadPath, "Content");

            if (FileService.DoesDirectoryExist(sourcePath))
            {
                FileService.MoveDirectory(sourcePath, Path.Combine(FileService.GetBaseDirectory(), "Themes", themeFolderName));
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

        private void SetIsInstalling(bool isInstalling)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

                settings.IsInstalling = isInstalling;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private bool OpenUpdatesAvailable()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.ThemeOpenUpdatesRepository.GetAll().Any();
            }
        }

        private void AddToDownloadingList(ThemeOpenUpdates updateModel, string downloadPath, int taskId)
        {
            ThemeDownloadManager.Manager.AddDownload(updateModel.Id, updateModel.ThemeId, new DownloadUpdate(updateModel.Name, updateModel.Description, updateModel.Version, updateModel.DownloadUrl, updateModel.DeskoVersion, taskId, downloadPath, 0));
        }

        private void HandlerProgressChanged(object o, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, object taskId)
        {
            ThemeDownloadManager.Manager.UpdateProgress((int)taskId, downloadProgressChangedEventArgs.ProgressPercentage);
        }

        private void HandlerProgressComplete(object o, AsyncCompletedEventArgs asyncCompletedEventHandler, object taskId)
        {
            int parsedTaskId = (int)taskId;

            var download = ThemeDownloadManager.Manager.GetDownload(parsedTaskId);

            if (download != null)
            {
                SetOpenUpdateToDownloaded(download.ExtraId, download.Version, download.DownloadPath);

                ThemeDownloadManager.Manager.RemoveDownload(parsedTaskId);
            }
        }

        private void SetOpenUpdateToDownloaded(int themeId, string version, string downloadPath)
        {
            // Create a new uof because of async method
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var openUpdate = unitOfWorkRepository.ThemeOpenUpdatesRepository.GetByThemeIdAndVersion(themeId, version);

                if (openUpdate != null)
                {
                    openUpdate.IsDownloaded = true;
                    openUpdate.DownloadPath = downloadPath;
                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private async Task AsyncDownloadUpdate(ThemeOpenUpdates update)
        {
            var webOnlineService = new WebOnlineService();

            int taskId = Task.CurrentId.Value;

            string downloadPath = Path.Combine(FileService.GetBaseDirectory(), ThemesDownloadFolderName, update.ThemeId.ToString(), update.Version);

            FileService.CreateDirectory(downloadPath);

            string fileLocation = Path.Combine(downloadPath, string.Format("{0}.zip", update.Version));

            AddToDownloadingList(update, fileLocation, taskId);

            await webOnlineService.DownloadAsyncFile(update.DownloadUrl, fileLocation, HandlerProgressChanged, taskId, HandlerProgressComplete, taskId);
        }

        private void SetIsDownloading(bool isDownloading)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

                settings.IsDownloading = isDownloading;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetIsCheckingForUpdates(bool isChecking)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

                settings.IsChecking = isChecking;

                unitOfWorkRepository.SaveChanges();
            }
        }

        private void SetUpdateChecked(DateTime date)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var settings = unitOfWorkRepository.ThemeUpdateSettingsRepository.GetSettings();

                settings.LastCheckedDate = date;
            }
        }

        private IEnumerable<Database.Entities.Themes> GetUpdateAbleThemes()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.ThemesRepository.GetAll().Where(w => !string.IsNullOrEmpty(w.UpdateUrl)).ToList();
            }
        }

        private IEnumerable<OpenUpdateModel> GetOnlineUpdates(string downloadUrl)
        {
            var webOnlineService = new WebOnlineService();
            var javaScriptSerializer = new JavaScriptSerializer();

            return
                javaScriptSerializer.Deserialize<IEnumerable<OpenUpdateModel>>(
                    webOnlineService.DownloadPage(downloadUrl)).ToList();
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
                return unitOfWorkRepository.ThemeOpenUpdatesRepository.IsVersionAdded(version);
            }
        }

        private void AddUpdateToTheme(int themeId, OpenUpdateModel newUpdate)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                unitOfWorkRepository.ThemeOpenUpdatesRepository.Add(new ThemeOpenUpdates
            {
                ThemeId = themeId,
                Version = newUpdate.Version,
                Description = newUpdate.Description,
                Name = newUpdate.Name,
                IsDownloaded = false,
                DeskoVersion = newUpdate.DeskoVersion,
                DownloadUrl = newUpdate.DownloadUrl
            });
                unitOfWorkRepository.SaveChanges();
            }

        }

        #endregion
    }
}
