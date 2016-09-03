using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Core.Business.Software;
using Core.Models.Themes;
using Core.Models.ThirdParty;

namespace Core.Business.Themes
{
    public class ThemeSettingsService
    {
        private readonly XDocument _settings;

        public ThemeSettingsService(string settingsPath)
        {
            _settings = LoadSettings(settingsPath);
        }

        public string Name
        {
            get { return _settings.Root.Element("Name").Value; }
        }

        public string Description
        {
            get { return _settings.Root.Element("Description").Value; }
        }

        public string Image
        {
            get { return _settings.Root.Element("Image").Value; }
        }

        public string Version
        {
            get { return _settings.Root.Element("Version").Value; }
        }

        public string Author
        {
            get { return _settings.Root.Element("Author").Value; }
        }

        public string AuthorUrl
        {
            get { return _settings.Root.Element("AuthorUrl").Value; }
        }

        public string UpdateUrl
        {
            get { return _settings.Root.Element("UpdateUrl").Value; }
        }

        public IEnumerable<ThemeSectionModel> ThemeSections
        {
            get
            {
                return GetThemeSections();
            }
        }

        #region Sql

        public IEnumerable<SqlScript> GetSqlInstallScripts()
        {
            return GetSqlScripts(true);
        }

        public IEnumerable<SqlScript> GetSqlUninstallScripts()
        {
            return GetSqlScripts(false);
        }

        private IEnumerable<SqlScript> GetSqlScripts(bool isInstall)
        {
            var versionComparer = new VersionOrderingService();

            var sqlElement = _settings.Root.Element("Sql");

            if (sqlElement == null || sqlElement.IsEmpty) { return new SqlScript[0]; }

            return
                _settings.Root.Element("Sql")
                    .Element(isInstall ? "Install" : "Uninstall")
                    .Elements("Script")
                    .Select(script => new SqlScript
                    {
                        Location = script.Attribute("Location").Value,
                        Version = script.Attribute("Version").Value
                    }).OrderBy(settings => settings.Version, versionComparer);
        }

        #endregion

        #region Navigation

        public IEnumerable<AdminNavigationSettingsModel> GetNavigation()
        {
            var navigationElement = _settings.Root.Element("Navigation");

            if (navigationElement == null || navigationElement.IsEmpty) { return new AdminNavigationSettingsModel[0]; }

            return navigationElement
                .Elements("Link")
                .Select(link => new AdminNavigationSettingsModel
                {
                    Controller = link.Attribute("Controller").Value,
                    Action = link.Attribute("Action").Value,
                    LocalizedBase = link.Attribute("LocalizedBase").Value,
                    LocalizedName = link.Attribute("LocalizedName").Value,
                    Icon = link.Attribute("Icon").Value
                });
        }

        #endregion

        #region Breadcrumbs

        public IEnumerable<AdminBreadcrumbSettingsModel> GetBreadcrumbs()
        {
            var breadCrumbsElement = _settings.Root.Element("Breadcrumbs");

            if (breadCrumbsElement == null || breadCrumbsElement.IsEmpty) { return new AdminBreadcrumbSettingsModel[0]; }

            return from breadcrumb in breadCrumbsElement.Elements("Crumb")
                   select new AdminBreadcrumbSettingsModel
                   {
                       Action = breadcrumb.Attribute("Action").Value,
                       Controller = breadcrumb.Attribute("Controller").Value,
                       LocalizedBase = breadcrumb.Attribute("LocalizedBase").Value,
                       LocalizedControllerFriendlyName = breadcrumb.Attribute("LocalizedControllerFriendlyName").Value,
                       LocalizedActionFriendlyName = breadcrumb.Attribute("LocalizedActionFriendlyName").Value,
                       LocalizedTitle = breadcrumb.Attribute("LocalizedTitle").Value,
                       LocalizedDescription = breadcrumb.Attribute("LocalizedDescription").Value,
                       Icon = breadcrumb.Attribute("Icon").Value
                   };
        }

        #endregion

        /// <summary>
        /// Loads the settings
        /// </summary>
        /// <param name="settingsPath"></param>
        /// <exception cref="FileNotFoundException">Settings file missing</exception>
        /// <exception cref="XmlException">Invalid xml</exception>
        private XDocument LoadSettings(string settingsPath)
        {
            try
            {
                return XDocument.Load(settingsPath);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new FileNotFoundException(Localization.Languages.Business.Themes.ThemeSettingsService.SettingsNotFound);
                }

                throw new XmlException(Localization.Languages.Business.Themes.ThemeSettingsService.LoadingSettingsError);
            }
        }


        private IEnumerable<ThemeSectionModel> GetThemeSections()
        {
            return from themeSection in _settings.Root.Element("Sections").Elements("Section")
                   select new ThemeSectionModel
                   {
                       CodeName = themeSection.Element("CodeName").Value,
                       FriendlyName = themeSection.Element("FriendlyName").Value
                   };
        }
    }
}
