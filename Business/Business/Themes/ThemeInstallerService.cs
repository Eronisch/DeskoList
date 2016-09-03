using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Business.Breadcrumbs;
using Core.Business.Dll;
using Core.Business.File;
using Core.Business.Navigation;
using Core.Business.Software;
using Core.Models;
using Core.Models.Themes;
using Core.Models.ThirdParty;
using Database;
using Elmah;
using Ionic.Zip;

namespace Core.Business.Themes
{
    public class ThemeInstallerService
    {
        private readonly ThemeService _themeService;
        private readonly ThemeConfigService _themeConfigService;

        public ThemeInstallerService()
        {
            _themeService = new ThemeService();
            _themeConfigService = new ThemeConfigService();
        }

        public ResultModel Install(Stream theme)
        {
            try
            {
                string tempThemeFilePath = GetTempPath(Path.GetRandomFileName());

                UnpackThemeToTemp(theme, tempThemeFilePath);

                ThemeSettingsService themeSettingsService;

                var settingsResult = TryLoadSettings(tempThemeFilePath, out themeSettingsService);

                if (!settingsResult.IsSuccess) { return new ResultModel(settingsResult.ErrorMessage); }

                string themeFolderName = GetThemeFolderName(themeSettingsService.Name, themeSettingsService.Author);

                bool isUpdate = IsUpdate(themeSettingsService.Name,
                    themeSettingsService.Author);

                var scriptsResult = TryInstallSqlScripts(themeSettingsService.GetSqlInstallScripts(), themeSettingsService.Name, themeSettingsService.Author,
                themeSettingsService.Version, tempThemeFilePath, isUpdate);

                if (!scriptsResult.IsSuccess) { return new ResultModel(scriptsResult.ErrorMessage); }

                int themeId = UpdateOrInstallInDatabase(themeSettingsService, themeFolderName);

                MoveFiles(tempThemeFilePath, themeFolderName);

                AddNavigationLinks(themeId, themeSettingsService.GetNavigation());

                AddBreadcrumbs(themeId, themeSettingsService.GetBreadcrumbs());

                AddDllsToDatabase(themeId, tempThemeFilePath);

                RemoveTempFiles(tempThemeFilePath);

                return new ResultModel();
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(null).Log(new Error(ex));
                return new ResultModel(Localization.Languages.Business.Themes.ThemeInstallerService.UnknownError);
            }
        }

        private bool IsUpdate(string name, string author)
        {
            return _themeService.GetThemeByNameAndAuthor(name, author) != null; 
        }

        private string GetVersion(string themeName, string author, string versionUpdateOrInstall, bool isUpdate)
        {
            if (isUpdate) { return _themeService.GetThemeByNameAndAuthor(themeName, author).Version; }

            return versionUpdateOrInstall;
        }

        private ResultModel TryInstallSqlScripts(IEnumerable<SqlScript> sqlScripts, string name, string author,
            string versionUpdateOrInstall, string tempPath, bool isUpdate)
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
                            databaseService.ExecuteQuery(
                                FileService.GetFileContent(Path.Combine(tempPath, script.Location)), transaction);
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
                    !isSuccess ? Localization.Languages.Business.Themes.ThemeInstallerService.ErrorInSqlScript : string.Empty
            };
        }

        private void AddNavigationLinks(int themeId, IEnumerable<AdminNavigationSettingsModel> navigationItems)
        {
            var adminNavigationService = new AdminNavigationService();

            adminNavigationService.ClearThemeNavigation(themeId);

            foreach (var navigation in navigationItems)
            {
                adminNavigationService.AddThemeNavigation(themeId, navigation.Controller, navigation.Action, navigation.Icon, navigation.LocalizedBase, navigation.LocalizedName);
            }
        }

        private void AddBreadcrumbs(int themeId, IEnumerable<AdminBreadcrumbSettingsModel> breadcrumbs)
        {
            var adminBreadCrumbsService = new AdminBreadCrumbsService();

            adminBreadCrumbsService.ClearThemeBreadCrumbs(themeId);

            foreach (var breadcrumb in breadcrumbs)
            {
                adminBreadCrumbsService.AddThemeBreadCrumbs(themeId, breadcrumb.Controller, breadcrumb.Action, breadcrumb.Icon, breadcrumb.LocalizedBase, breadcrumb.LocalizedTitle,
             breadcrumb.LocalizedDescription, breadcrumb.LocalizedControllerFriendlyName, breadcrumb.LocalizedActionFriendlyName);
            }
        }

        private string GetTempPath(string folderName)
        {
            return _themeConfigService.GetTempFolderPath(folderName);
        }

        private void AddDllsToDatabase(int themeId, string tempThemePath)
        {
            var dllService = new DllService();

            var dllFilePaths = _themeConfigService.GetDlls(tempThemePath);

            foreach (var dllFilePath in dllFilePaths)
            {
                string dllName = Path.GetFileName(dllFilePath);

                dllService.AddThemeDll(dllName, themeId);
            }
        }

        private ResultModel TryLoadSettings(string themeFolderName, out ThemeSettingsService themeSettingsService)
        {
            themeSettingsService = null;

            try
            {
                themeSettingsService = new ThemeSettingsService(_themeConfigService.GetSettingsPath(themeFolderName));
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message);
            }

            return new ResultModel();
        }

        private void MoveFiles(string tempThemePath, string destinationThemeFolderName)
        {
            string tempDirectoryThemeName = new DirectoryInfo(tempThemePath).Name;

            foreach (var themeTempFilePath in Directory.GetFiles(tempThemePath, "*.*", SearchOption.AllDirectories))
            {
                if (IsDllFileInBinFolder(tempThemePath, themeTempFilePath))
                {
                    if (IsNewThemeDllFile(themeTempFilePath))
                    {
                        FileService.CopyFile(themeTempFilePath, Path.Combine(FileService.GetBinPath(), Path.GetFileName(themeTempFilePath)));
                    }
                }

                string destinationFilePath = GetTargetFileThemeLocation(tempDirectoryThemeName,
                    themeTempFilePath, destinationThemeFolderName);

                FileService.CreateDirectory(new DirectoryInfo(destinationFilePath).Parent.FullName);

                FileService.MoveFile(themeTempFilePath, destinationFilePath);
            }
        }

        private string GetTargetFileThemeLocation(string sourceThemeFolderName, string themeTempFilePath, string destinationThemeFolderName)
        {
            string pathWithoutApplicationPath = themeTempFilePath.Substring(FileService.GetBaseDirectory().Length + _themeConfigService.GetTempFolderName().Length + sourceThemeFolderName.Length + 2); // exclude "//" symbols

            return Path.Combine(_themeConfigService.GetAreaPath(destinationThemeFolderName), pathWithoutApplicationPath);
        }

        private bool IsNewThemeDllFile(string ThemeTempFilePath)
        {
            var versionComparer = new VersionOrderingService();
            string dllFileName = Path.GetFileName(ThemeTempFilePath);
            string dllInBinPath = Path.Combine(FileService.GetBinPath(), dllFileName);

            if (!FileService.FileExists(dllInBinPath)) { return true; }

            var dllInBinFileInfo = FileVersionInfo.GetVersionInfo(dllInBinPath);
            var dllThemeInfo = FileVersionInfo.GetVersionInfo(ThemeTempFilePath);

            if (versionComparer.IsHigher(dllThemeInfo.FileVersion, dllInBinFileInfo.FileVersion))
            {
                return true;
            }

            return false;
        }

        private bool IsDllFileInBinFolder(string tempThemePath, string themeTempFilePath)
        {
            string skipPath = Path.Combine(tempThemePath, "bin");

            return new DirectoryInfo(themeTempFilePath).Parent.FullName == skipPath;
        }

        private int UpdateOrInstallInDatabase(ThemeSettingsService themeSettingsService, string themeFolderName)
        {
            var installedTheme = _themeService.GetThemeByNameAndAuthor(themeSettingsService.Name, themeSettingsService.Author);

            if (installedTheme != null)
            {
                installedTheme = _themeService.UpdateTheme(installedTheme.Id, themeSettingsService.Description, themeSettingsService.AuthorUrl, themeSettingsService.Version, themeSettingsService.Image, themeSettingsService.UpdateUrl);
            }
            else
            {
                installedTheme = _themeService.AddTheme(themeSettingsService.Name, themeSettingsService.Description, themeSettingsService.Author, themeSettingsService.AuthorUrl, themeFolderName, themeSettingsService.Version, themeSettingsService.Image, themeSettingsService.UpdateUrl);
            }

            SetThemeSections(installedTheme, themeSettingsService.ThemeSections);

            return installedTheme.Id;
        }

        private void SetThemeSections(Database.Entities.Themes theme, IEnumerable<ThemeSectionModel> themeSections)
        {
            var listThemeSections = themeSections.ToList();

            AddThemeSections(theme, listThemeSections);
            UpdateExistingThemeSections(theme, listThemeSections);
            RemoveThemeSections(theme, listThemeSections);
        }

        private void UpdateExistingThemeSections(Database.Entities.Themes theme, IEnumerable<ThemeSectionModel> themeSections)
        {
            foreach (var themeSection in themeSections
               .Where(ts => theme.WidgetsThemeSection.Any(t => t.CodeName.Equals(ts.CodeName, StringComparison.CurrentCultureIgnoreCase))))
            {
                var themeSectionInTheme = theme.WidgetsThemeSection.First(t => t.CodeName.Equals(themeSection.CodeName, StringComparison.CurrentCultureIgnoreCase));

                // Check if the friendly name has been changed
                if (!themeSection.FriendlyName.Equals(themeSectionInTheme.FriendlyName, StringComparison.CurrentCultureIgnoreCase))
                {
                    _themeService.UpdateThemeSection(theme.Id, themeSection.FriendlyName);
                }
            }
        }

        private void AddThemeSections(Database.Entities.Themes theme, IEnumerable<ThemeSectionModel> themeSections)
        {
            foreach (var themeSection in themeSections
               .Where(ts => !theme.WidgetsThemeSection.Any(t => t.CodeName.Equals(ts.CodeName, StringComparison.CurrentCultureIgnoreCase))))
            {
                _themeService.AddThemeSection(themeSection.FriendlyName, themeSection.CodeName, theme.Id);
            }
        }

        private void RemoveThemeSections(Database.Entities.Themes theme, IEnumerable<ThemeSectionModel> themeSections)
        {
            theme.WidgetsThemeSection.ToList().ForEach(themeSectionInTheme =>
            {
                {
                    // Check if the themesection still exists in the settings
                    if (!themeSections.Any(
                            ts =>
                                themeSectionInTheme.CodeName.Equals(ts.CodeName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        _themeService.RemoveThemeSection(themeSectionInTheme.Id);
                    }
                }
            });
        }

        private void UnpackThemeToTemp(Stream theme, string tempPath)
        {
            FileService.CreateDirectory(tempPath);

            using (ZipFile zipFile = ZipFile.Read(theme))
            {
                foreach (ZipEntry e in zipFile)
                {
                    e.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void RemoveTempFiles(string temPath)
        {
            // We don't want the last directory, the theme folder itself. Delete the parent
            string pathToRemove = temPath.Substring(0, temPath.LastIndexOf(@"\") + 1);
            FileService.RemoveDirectory(pathToRemove, true);
        }

        private string GetThemeFolderName(string themeName, string author)
        {
            string newName = FileService.CleanFileName(themeName);
            string newAuthor = FileService.CleanFileName(author);

            if (newName.Length > 100)
            {
                newName = themeName.Substring(0, 100);
            }

            if (newAuthor.Length > 100)
            {
                newAuthor = author.Substring(0, 100);
            }

            return string.Format("{0}-{1}", newName, newAuthor);
        }
    }
}
