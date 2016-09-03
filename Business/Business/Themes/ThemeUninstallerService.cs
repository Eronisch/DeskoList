using System;
using System.Collections.Generic;
using System.IO;
using Core.Business.Dll;
using Core.Business.File;
using Core.Business.Settings;
using Core.Models;
using Core.Models.ThirdParty;
using Database;

namespace Core.Business.Themes
{
    public class ThemeUninstallerService
    {
        private readonly ThemeService _themeService;
        private readonly SettingsService _settingsService;
        private readonly ThemeConfigService _themeConfigService;

        public ThemeUninstallerService()
        {
            _themeService = new ThemeService();
            _settingsService = new SettingsService();
            _themeConfigService = new ThemeConfigService();
        }

        public ResultModel Uninstall(int themeId)
        {
            var theme = GetTheme(themeId);

            if (theme == null) { return new ResultModel(Localization.Languages.Business.Themes.ThemeUninstallerService.ThemeNotFound); }

            if (_settingsService.GetActiveThemeId() == themeId) { return new ResultModel(Localization.Languages.Business.Themes.ThemeUninstallerService.ThemeIsActive); }

            string pathTheme = _themeConfigService.GetAreaPath(theme.FolderName);

            ThemeSettingsService themeSettingsService;

            var settingsResult = TryLoadSettings(theme.FolderName, out themeSettingsService);

            if (!settingsResult.IsSuccess) { return new ResultModel(settingsResult.ErrorMessage); }

            var scriptResult = UnInstallSqlScripts(themeSettingsService.GetSqlUninstallScripts(), pathTheme);

            if (!scriptResult.IsSuccess) { return new ResultModel(scriptResult.ErrorMessage); }

            RemoveThemeInDatabase(themeId);

            RemoveThemeFiles(theme.FolderName);

            RemoveNotUsedDlls(theme.Id, theme.FolderName);

            return new ResultModel();
        }

        private ResultModel TryLoadSettings(string tempAreaName, out ThemeSettingsService themeSettingsService)
        {
            themeSettingsService = null;

            try
            {
                themeSettingsService = new ThemeSettingsService(_themeConfigService.GetSettingsPath(tempAreaName));
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message);
            }

            return new ResultModel();
        }

        private void RemoveNotUsedDlls(int themeId, string folderName)
        {
            var dllService = new DllService();

            foreach (var file in GetFilesInBin(folderName))
            {
                if (!dllService.IsDllBeingUsedByThemeId(themeId, Path.GetFileName(file)))
                {
                    FileService.RemoveFileInBin(Path.GetFileName(file));
                }
            }
        }

        private IEnumerable<string> GetFilesInBin(string folderName)
        {
            return _themeConfigService.GetDlls(folderName);
        }

        private void RemoveThemeInDatabase(int themeId)
        {
            // There's a cascade on the ThemeSectionWidgets & Theme widgets
            _themeService.RemoveTheme(themeId);
        }

        private void RemoveThemeFiles(string folderName)
        {
            FileService.RemoveDirectory(_themeConfigService.GetAreaPath(folderName), true);
        }

        private Database.Entities.Themes GetTheme(int themeId)
        {
            return _themeService.GetThemeById(themeId);
        }

        private ResultModel UnInstallSqlScripts(IEnumerable<SqlScript> sqlScripts, string pathTheme)
        {
            var databaseService = new DatabaseService();

            bool isSuccess = true;

            foreach (var script in sqlScripts)
            {
                try
                {
                    databaseService.ExecuteQuery(FileService.GetFileContent(Path.Combine(pathTheme, script.Location)));
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return new ResultModel
            {
                ErrorMessage = !isSuccess ? Localization.Languages.Business.Themes.ThemeUninstallerService.ErrorInSqlScript : string.Empty
            };
        }
    }
}
