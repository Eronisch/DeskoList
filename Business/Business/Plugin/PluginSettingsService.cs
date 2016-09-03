using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Core.Business.Software;
using Core.Models.ThirdParty;

namespace Core.Business.Plugin
{
    public class PluginSettingsService
    {
        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        /// <param name="settingsPath"></param>
        /// <exception cref="FileNotFoundException">Settings file missing</exception>
        /// <exception cref="XmlException">Invalid xml</exception>
        public PluginSettingsService(string settingsPath)
        {
            _settings = LoadSettings(settingsPath);
        }

        private readonly XDocument _settings;

        #region About

        public string Name
        {
            get { return _settings.Root.Element("About").Element("Name").Value; }
        }
        public string Description
        {
            get { return _settings.Root.Element("About").Element("Description").Value; }
        }

        public string Version
        {
            get { return _settings.Root.Element("About").Element("Version").Value; }
        }

        public string Author
        {
            get { return _settings.Root.Element("About").Element("Author").Value; }
        }

        public string AuthorUrl
        {
            get { return _settings.Root.Element("About").Element("AuthorUrl").Value; }
        }

        #endregion

        #region Configuration

        public string UpdateUrl
        {
            get
            {
                var imageElement = _settings.Root.Element("Configuration").Element("UpdateUrl");

                return imageElement != null ? imageElement.Value : null;
            }
        }

        public string Namespace
        {
            get
            {
                var namespaceElement = _settings.Root.Element("Configuration").Element("Namespace");

                return namespaceElement != null ? namespaceElement.Value : null;
            }
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

        #region Load

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
                    throw new FileNotFoundException(Localization.Languages.Business.Plugin.PluginSettingsService.SettingsFileMissing);
                }

                throw new XmlException(Localization.Languages.Business.Plugin.PluginSettingsService.InvalidSettingsFile);
            }
        }

        #endregion

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

            if (sqlElement == null || sqlElement.IsEmpty) { return new SqlScript[0];}

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
    }
}
