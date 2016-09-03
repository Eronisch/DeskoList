using System;
using System.IO;
using System.Web.Mvc;
using Core.Business.Account;
using Core.Business.Breadcrumbs;
using Core.Business.Email;
using Core.Business.File;
using Core.Business.Languages;
using Core.Business.Pages;
using Core.Business.Schedule;
using Core.Business.Settings;
using Core.Business.Themes;
using Core.Business.Widgets;
using Core.Models.Install;
using Database;
using Database.Entities;
using DatabaseXML;
using Web.Infrastructure.Factory;
using Web.Infrastructure.ViewEngines;

namespace Web.AutoInstaller
{
    /// <summary>
    /// Autoinstaller, installs the settings, themes, admin account etc..
    /// This should only be used once for installing the software
    /// </summary>
    public class InstallerService
    {
        public InstallType Install(string databaseHost, string databaseName, string databaseUsername, string databasePassword,
            string emailNoReplyHost, int emailNoReplyPort, bool emailNoReplySecureConnection, string emailNoReplyEmail, string emailNoReplyPassword,
            string emailReplyHost, int emailReplyPort, bool emailReplySecureConnection, string emailReplyEmail, string emailReplyPassword,
            string settingsLongTitle, string settingsShortTitle, string settingsDescription, string settingsKeywords, int settingsAmountWebsites, string settingsUrl,
            string settingsSlogan, int settingsLanguageId, string settingsTimezoneId, string settingsCaptchaKey, string settingsCaptchaSecret, bool settingsIsUserMonitoringEnabled, bool settingsIsEmailVerificationEnabled,
            bool settingsIsAutoUpdateEnabled, bool settingsIsEmailingUserStatisticsEnabled, bool settingsIsCreateUserWebsiteThumbnailsEnabled, bool settingsIsResetInAndOutsEnabled, bool settingsIsUpdateWhenIncorrectVersionEnabled,
            bool settingsIsAdminVerificationRequired, string cronjobEmailUserStatistics, string cronjobInAndOut, string cronjobUpdate, string cronjobCreateThumbnails, string cronjobMonitorWebsites,
            string accountUsername, string accountPassword, string accountEmail, int accountSecurityQuestionId, string accountSecurityAnswer
            )
        {
            if (!UpdateXmlDatabaseSettings(databaseHost, databaseName, databaseUsername, databasePassword)) { return InstallType.UpdateXmlDatabaseFailed; }

            if (!InstallDatabaseScript()) { return InstallType.ExecuteDatabaseScriptFailed; }

            if (!AddThemes()) { return InstallType.AddingThemesFailed; }

            if (!InstallEmailTemplates()) { return InstallType.AddingEmailTemplatesFailed; }

            if (!InstallEmailAccounts(emailNoReplyHost, emailNoReplyEmail, emailNoReplyPassword, emailNoReplyPort, emailNoReplySecureConnection, emailReplyHost, emailReplyEmail, emailReplyPassword, emailReplyPort, emailReplySecureConnection)) { return InstallType.AddingEmailAccountsFailed; }

            if (!InstallWidgets()) { return InstallType.AddingWidgetsFailed; }

            if (!InstallPages()) { return InstallType.AddingPagesFailed; }

            if (!InstallLanguages()) { return InstallType.AddingLanguagesFailed; }

            if (!InstallSettings(settingsLongTitle, settingsShortTitle, settingsDescription, settingsKeywords, settingsAmountWebsites, settingsUrl, cronjobEmailUserStatistics, cronjobInAndOut, cronjobUpdate, cronjobCreateThumbnails, cronjobMonitorWebsites, settingsLanguageId, settingsTimezoneId, settingsSlogan, 1, settingsCaptchaSecret, settingsCaptchaKey, settingsIsUserMonitoringEnabled, settingsIsEmailVerificationEnabled, settingsIsAutoUpdateEnabled, settingsIsEmailingUserStatisticsEnabled, settingsIsCreateUserWebsiteThumbnailsEnabled, settingsIsResetInAndOutsEnabled, settingsIsUpdateWhenIncorrectVersionEnabled, settingsIsAdminVerificationRequired)) { return InstallType.AddingSettingsFailed; }

            if (!InstallAdministratorAccount(accountUsername, accountPassword, accountEmail, accountSecurityQuestionId, accountSecurityAnswer)) { return InstallType.AddingAdministratorFailed; }

            Initialize();

            return InstallType.Success;
        }

        private void Initialize()
        {
            LocalDatabaseSettingsService.Manager.SetToInstalled();

            new BuiltInTasksScheduler().Schedule();

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new CustomViewEngine());

            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());
        }

        #region Database

        private bool InstallDatabaseScript()
        {
            try
            {
                var databaseService = new DatabaseService();

                if (!databaseService.CreateDatabase())
                {
                    ExecuteSqlCommands(Path.Combine(FileService.GetBaseDirectory(), "Install/Content/Database/Script.sql"));
                }

                ExecuteSqlCommands(Path.Combine(FileService.GetBaseDirectory(), "Install/Content/Database/Elmah.sql"));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ExecuteSqlCommands(string filepath)
        {
            var databaseService = new DatabaseService();

            string fileContent = FileService.GetFileContent(filepath);

            string[] sql = fileContent.Split(new[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sqlCommand in sql)
            {
                if (!string.IsNullOrWhiteSpace(sqlCommand))
                    databaseService.ExecuteQuery(sqlCommand);
            }
        }

        private bool UpdateXmlDatabaseSettings(string host, string name, string username, string password)
        {
            try
            {
                LocalDatabaseSettingsService.Manager.UpdateDatabase(host, name, username, password);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Themes

        private bool AddThemes()
        {
            var themeService = new ThemeService();

            try
            {
                themeService.AddTheme("Light blue", "An elegant lightblue theme. This is the default theme. There are 9 modules included by default.", "Jamie", "http://deskolist.com", "LightBlue", "Info/theme.jpg", "1.0");
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Widgets

        private bool InstallWidgets()
        {
            try
            {
                AddWidgets();

                AddThemeSections();

                SetActiveWidgets();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddWidgets()
        {
            const string authorName = "Jamie";
            const string authorUrl = "http://deskolist.com";

            var widgetService = new WidgetService();

            widgetService.AddToDatabase("Rating Widget", "The latest rated websites are displayed here including the name and amount stars.", "RatingWidget", "1.0", authorName, authorUrl,
                "rating.png", "Rating", "Index", "Topsite_Rating");
            widgetService.AddToDatabase("Statistics Widget", "A Widget to display the amount of users registerd, active users, amount websites and current software version.", "StatisticsWidget", "1.0", authorName,
                authorUrl, "statistics.png", "Statistics", "Index", "Topsite_Statistics");
            widgetService.AddToDatabase("Reset Widget", "Display when the in & out statistics are being reset.", "ResetWidget", "1.0", authorName, authorUrl,
                "reset.png", "Reset", "Index", "Topsite_Reset");
            widgetService.AddToDatabase("Poll Widget", "Simple poll Widget to users vote on their favorite answer.", "PollWidget", "1.0", authorName, authorUrl,
                "poll.png", "Poll", "Index", "Topsite_Poll");
            widgetService.AddToDatabase("Share Widget", "Share the current page you are on easily.", "ShareWidget", "1.0", authorName, authorUrl,
                "social.png", "Share", "Index", "Topsite_Share");
            widgetService.AddToDatabase("Login Widget", "Let users login with their credentials including two links to request their username and reset their password.", "LoginWidget", "1.0", authorName, authorUrl,
                "login.png", "Login", "Index", "Topsite_Login");
            widgetService.AddToDatabase("Category Widget", "Show all the categories users can add their websites to.", "CategoryWidget", "1.0", authorName,
                authorUrl, "categories.png", "Categories", "Index", "Topsite_Categories");
            widgetService.AddToDatabase("News Widget", "Display your latest news articles.", "NewsWidget", "1.0", authorName, authorUrl, "news.png", "News",
               "Index", "Topsite_News");
            widgetService.AddToDatabase("Links Widget", "Show your partners or other links users might be interested in. ", "LinksWidget", "1.0", authorName, authorUrl,
                "links.png", "Links", "Index", "Topsite_Links");
        }

        private void AddThemeSections()
        {
            var themeService = new ThemeService();
            themeService.AddThemeSection("Right Side", "RightContent", 1);
            themeService.AddThemeSection("Left Side", "LeftContent", 1);
        }

        private void SetActiveWidgets()
        {
            var widgetService = new WidgetService();

            // Rate
            widgetService.ConfigureWidgetTheme(1, 1, 4);

            // Statistics
            widgetService.ConfigureWidgetTheme(2, 1, 5);

            // Reset
            widgetService.ConfigureWidgetTheme(3, 1, 6);

            // Poll
            widgetService.ConfigureWidgetTheme(4, 1, 3);

            // Share
            widgetService.ConfigureWidgetTheme(5, 1, 1);

            // Login
            widgetService.ConfigureWidgetTheme(6, 1, 2);

            // Categories
            widgetService.ConfigureWidgetTheme(7, 2, 1);

            // News
            widgetService.ConfigureWidgetTheme(8, 2, 2);

            // Links
            widgetService.ConfigureWidgetTheme(9, 2, 3);
        }

        #endregion

        #region Settings

        /// <summary>
        /// Returns true if the settings were updated successfully
        /// </summary>
        /// <param name="title"></param>
        /// <param name="shortTitle"></param>
        /// <param name="description"></param>
        /// <param name="keywords"></param>
        /// <param name="amountWebsites"></param>
        /// <param name="url"></param>
        /// <param name="statisticsUserEmailCronjob"></param>
        /// <param name="inAndOutsResetCronjob"></param>
        /// <param name="updateCronjob"></param>
        /// <param name="createUserWebsiteThumbnailsCronjob"></param>
        /// <param name="monitorUserWebsitesCronjob"></param>
        /// <param name="languageId"></param>
        /// <param name="timezone"></param>
        /// <param name="slogan"></param>
        /// <param name="themeId"></param>
        /// <param name="captchaSecretKey"></param>
        /// <param name="captchaSiteKey"></param>
        /// <param name="isUserServerMonitoringEnabled"></param>
        /// <param name="isUserEmailVerificationRequired"></param>
        /// <param name="isAutoUpdateEnabled"></param>
        /// <param name="isEmailingUserStatisticsEnabled"></param>
        /// <param name="isCreatingWebsiteThumbnailsEnabled"></param>
        /// <param name="isResetInAndOutsEnabled"></param>
        /// <param name="updateWhenIncorrectVersion"></param>
        /// <param name="isAdminVerificationRequired"></param>
        /// <returns></returns>
        private bool InstallSettings(string title, string shortTitle, string description, string keywords, int amountWebsites, string url, string statisticsUserEmailCronjob, string inAndOutsResetCronjob, string updateCronjob, string createUserWebsiteThumbnailsCronjob, string monitorUserWebsitesCronjob, int languageId, string timezone, string slogan, int themeId, string captchaSecretKey, string captchaSiteKey, bool isUserServerMonitoringEnabled, bool isUserEmailVerificationRequired, bool isAutoUpdateEnabled, bool isEmailingUserStatisticsEnabled, bool isCreatingWebsiteThumbnailsEnabled, bool isResetInAndOutsEnabled, bool updateWhenIncorrectVersion, bool isAdminVerificationRequired)
        {
            try
            {
                var settingsService = new SettingsService();
                settingsService.UpdateSettings(title, shortTitle, description, keywords, amountWebsites, url,
                    statisticsUserEmailCronjob, inAndOutsResetCronjob, updateCronjob, createUserWebsiteThumbnailsCronjob, monitorUserWebsitesCronjob, languageId, timezone, slogan, themeId, captchaSecretKey, captchaSiteKey, isUserServerMonitoringEnabled,
                    isUserEmailVerificationRequired, isAutoUpdateEnabled, isEmailingUserStatisticsEnabled, isCreatingWebsiteThumbnailsEnabled, isResetInAndOutsEnabled, updateWhenIncorrectVersion, isAdminVerificationRequired);

                InsertUpdateSettings();

                return true;
            }
            catch
            {
                return false;
            }

        }

        private DateTime GetDeskoPublishedDate()
        {
            return new DateTime(2016, 9, 3, 18, 0, 0);
        }

        private void InsertUpdateSettings()
        {
            InsertSoftwareUpdateSettings();
            InsertThemeUpdateSettings();
            InsertWidgetUpdateSettings();
            InsertPluginUpdateSettings();
        }

        private void InsertSoftwareUpdateSettings()
        {
            var unitOfWorkRepository = new UnitOfWorkRepository();
            unitOfWorkRepository.SoftwareUpdateSettingsRepository.InsertSettings(new SoftwareUpdateSettings
            {
                IsChecking = false,
                IsDownloading = false,
                IsInstalling = false,
                IsUpdatingSuccess = false,
                LastCheckedDate = GetDeskoPublishedDate()
            });
            unitOfWorkRepository.SaveChanges();
        }

        private void InsertThemeUpdateSettings()
        {
            var unitOfWorkRepository = new UnitOfWorkRepository();
            unitOfWorkRepository.ThemeUpdateSettingsRepository.Insert(new ThemeUpdateSettings
            {
                IsChecking = false,
                IsDownloading = false,
                IsInstalling = false,
                IsUpdatingSuccess = false,
                LastCheckedDate = GetDeskoPublishedDate()
            });
            unitOfWorkRepository.SaveChanges();
        }

        private void InsertWidgetUpdateSettings()
        {
            var unitOfWorkRepository = new UnitOfWorkRepository();
            unitOfWorkRepository.WidgetUpdateSettingsRepository.Insert(new WidgetUpdateSettings
            {
                IsChecking = false,
                IsDownloading = false,
                IsInstalling = false,
                IsUpdatingSuccess = false,
                LastCheckedDate = GetDeskoPublishedDate()
            });
            unitOfWorkRepository.SaveChanges();
        }

        private void InsertPluginUpdateSettings()
        {
            var unitOfWorkRepository = new UnitOfWorkRepository();
            unitOfWorkRepository.PluginUpdateSettingsRepository.Insert(new PluginUpdateSettings
            {
                IsChecking = false,
                IsDownloading = false,
                IsInstalling = false,
                IsUpdatingSuccess = false,
                LastCheckedDate = GetDeskoPublishedDate()
            });
            unitOfWorkRepository.SaveChanges();
        }

        #endregion

        #region Users

        private bool InstallAdministratorAccount(string username, string password, string email, int securityQuestion,
            string securityAnswer)
        {
            try
            {
                var accountService = new AccountService();
                accountService.AddAdministrator(username, password, email, securityQuestion, securityAnswer);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Pages

        private bool InstallPages()
        {
            try
            {
                AddSeoPages();
                AddNavigationPages();
                AddBreadCrumbs();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddSeoPages()
        {
            var seoPageService = new SeoPageService();
            seoPageService.AddSeo("Join", "Index", "Localization.Languages.Database.SeoPages", "JoinIndexTitle", "JoinIndexDescription");
            seoPageService.AddSeo("Contact", "Index", "Localization.Languages.Database.SeoPages", "ContactIndexTitle", "ContactIndexDescription");
            seoPageService.AddSeo("Cp", "Index", "Localization.Languages.Database.SeoPages", "CpIndexTitle", "CpIndexDescription");
            seoPageService.AddSeo("Account", "Edit", "Localization.Languages.Database.SeoPages", "AccountEditTitle", "AccountEditDescription");
            seoPageService.AddSeo("Account", "ResetStep1", "Localization.Languages.Database.SeoPages", "AccountResetTitle", "AccountResetDescription");
            seoPageService.AddSeo("Account", "ResetStep2", "Localization.Languages.Database.SeoPages", "AccountResetTitle", "AccountResetDescription");
            seoPageService.AddSeo("Search", "Index", "Localization.Languages.Database.SeoPages", "SearchIndexTitle", "SearchIndexDescription");
            seoPageService.AddSeo("Website", "Statistics", "Localization.Languages.Database.SeoPages", "WebsiteStatisticsTitle", "WebsiteStatisticsDescription");
            seoPageService.AddSeo("Website", "Add", "Localization.Languages.Database.SeoPages", "WebsiteAddTitle", "WebsiteAddDescription");
            seoPageService.AddSeo("Website", "Edit", "Localization.Languages.Database.SeoPages", "WebsiteEditTitle", "WebsiteAddDescription");
            seoPageService.AddSeo("Website", "Code", "Localization.Languages.Database.SeoPages", "WebsiteGetCodeTitle", "WebsiteGetCodeDescription");
            seoPageService.AddSeo("Website", "Codes", "Localization.Languages.Database.SeoPages", "WebsiteCodesTitle", "WebsiteCodesDescription");
            seoPageService.AddSeo("Website", "Api", "Localization.Languages.Database.SeoPages", "WebsiteApiTitle", "WebsiteApiDescription");
            seoPageService.AddSeo("Account", "RequestStep1", "Localization.Languages.Database.SeoPages", "AccountRequestTitle", "AccountRequestDescription");
            seoPageService.AddSeo("Account", "RequestStep2", "Localization.Languages.Database.SeoPages", "AccountRequestTitle", "AccountRequestDescription");
            seoPageService.AddSeo("Account", "Activate", "Localization.Languages.Database.SeoPages", "AccountActivateTitle", "AccountActivateDescription");
            seoPageService.AddSeo("Account", "Login", "Localization.Languages.Database.SeoPages", "AccountLoginTitle", "AccountLoginDescription");
            seoPageService.AddSeo("Category", "Index", "Localization.Languages.Database.SeoPages", "CategoryIndexTitle", "CategoryIndexDescription");
            seoPageService.AddSeo("Home", "Index", "Localization.Languages.Database.SeoPages", "HomeIndexTitle", string.Empty);
            seoPageService.AddSeo("Website", "ViewAll", "Localization.Languages.Database.SeoPages", "WebsiteViewAllTitle", "WebsiteViewAllDescription");
            seoPageService.AddSeo("Website", "Vote", "Localization.Languages.Database.SeoPages", "WebsiteVoteTitle", "WebsiteVoteDescription");
        }

        private void AddNavigationPages()
        {
            var navigationService = new NavigationService();
            navigationService.AddNavigation("Register", "/Join", 1, null);
            navigationService.AddNavigation("Contact", "/Contact", 2, null);
        }

        private void AddBreadCrumbs()
        {
            var adminBreadCrumbsService = new AdminBreadCrumbsService(); ;
            adminBreadCrumbsService.AddBreadCrumbs("Dashboard", "Index", "fa fa-home", "Localization.Languages.Database.AdminBreadcrumbs", "HomeDashboardTitle", "DashBoardIndexDescription", "HomeDashboardController", "HomeDashboardAction");
            adminBreadCrumbsService.AddBreadCrumbs("Websites", "Index", "fa fa-bars", "Localization.Languages.Database.AdminBreadcrumbs", "WebsitesIndexTitle", "WebsitesIndexDescription", "WebsitesIndexController", "WebsitesIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Websites", "View", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "WebsitesViewTitle", "WebsitesViewDescription", "WebsitesViewController", "WebsitesViewAction");
            adminBreadCrumbsService.AddBreadCrumbs("Websites", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "WebsitesEditTitle", "WebsitesEditDescription", "WebsitesEditController", "WebsitesEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Accounts", "Index", "fa fa-home", "Localization.Languages.Database.AdminBreadcrumbs", "AccountsHomeTitle", "AccountsIndexDescription", "AccountsHomeController", "AccountsIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Accounts", "View", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "AccountsViewTitle", "AccountsViewDescription", "AccountsViewController", "AccountsViewAction");
            adminBreadCrumbsService.AddBreadCrumbs("Accounts", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "AccountsEditTitle", "AccountsEditDescription", "AccountsEditController", "AccountsEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Settings", "Index", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "SettingsIndexTitle", "SettingsIndexDescription", "SettingsIndexController", "SettingsIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Themes", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "ThemesIndexTitle", "ThemesIndexDescription", "ThemesIndexController", "ThemesIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Reports", "Index", "fa fa-bars", "Localization.Languages.Database.AdminBreadcrumbs", "ReportsIndexTitle", "ReportsIndexDescription", "ReportsIndexController", "ReportsIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Reports", "View", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "ReportsViewTitle", "ReportsViewDescription", "ReportsViewController", "ReportsViewAction");
            adminBreadCrumbsService.AddBreadCrumbs("News", "Index", "fa fa-bars", "Localization.Languages.Database.AdminBreadcrumbs", "NewsIndexTitle", "NewsIndexDescription", "NewsIndexController", "NewsIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("News", "Add", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "NewsAddTitle", "NewsAddDescription", "NewsAddController", "NewsAddAction");
            adminBreadCrumbsService.AddBreadCrumbs("News", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "NewsEditTitle", "NewsEditDescription", "NewsEditController", "NewsEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Pages", "Index", "fa fa-bars", "Localization.Languages.Database.AdminBreadcrumbs", "PagesIndexTitle", "PagesIndexDescription", "PagesIndexController", "PagesIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Pages", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "PagesEditTitle", "PagesEditDescription", "PagesEditController", "PagesEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Pages", "Add", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "PagesAddTitle", "PagesAddDescription", "PagesAddController", "PagesAddAction");
            adminBreadCrumbsService.AddBreadCrumbs("Categories", "Index", "fa fa-bars", "Localization.Languages.Database.AdminBreadcrumbs", "CategoriesIndexTitle", "CategoriesIndexDescription", "CategoriesIndexController", "CategoriesIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Categories", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "CategoriesEditTitle", "CategoriesEditDescription", "CategoriesEditController", "CategoriesEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Categories", "Add", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "CategoriesAddTitle", "CategoriesAddDescription", "CategoriesAddController", "CategoriesAddAction");
            adminBreadCrumbsService.AddBreadCrumbs("Filemanager", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "FileManagerIndexTitle", "FileManagerDescription", "FilemanagerIndexController", "FileManagerIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Settings", "Updates", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "SettingsUpdatesTitle", "SettingsUpdatesDescription", "SettingsUpdatesController", "SettingsUpdatesAction");
            adminBreadCrumbsService.AddBreadCrumbs("Elmah", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "ElmahIndexTitle", "ElmahIndexDescription", "ElmahIndexController", "ElmahIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Widgets", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "WidgetsIndexTitle", "WidgetsIndexDescription", "WidgetsIndexController", "WidgetsIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Widgets", "Active", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "WidgetsActiveTitle", "WidgetsActiveDescription", "WidgetsActiveController", "WidgetsActiveAction");
            adminBreadCrumbsService.AddBreadCrumbs("Plugins", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "PluginsIndexTitle", "PluginsIndexDescription", "PluginsIndexController", "PluginsIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Plugins", "Active", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "PluginsActiveTitle", "PluginsActiveDescription", "PluginsActiveController", "PluginsActiveAction");
            adminBreadCrumbsService.AddBreadCrumbs("Plugins", "Inactive", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "PluginsInActiveTitle", "PluginsInActiveDescription", "PluginsInActiveController", "PluginsInActiveAction");
            adminBreadCrumbsService.AddBreadCrumbs("Widgets", "Updates", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "WidgetsUpdatesTitle", "WidgetsUpdatesDescription", "WidgetsUpdatesController", "WidgetsUpdatesAction");
            adminBreadCrumbsService.AddBreadCrumbs("Plugins", "Updates", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "PluginUpdatesTitle", "PluginsUpdatesDescription", "PluginUpdatesController", "PluginUpdatesAction");
            adminBreadCrumbsService.AddBreadCrumbs("Themes", "Updates", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "ThemeUpdatesTitle", "ThemeUpdatesDescription", "ThemeUpdatesController", "ThemeUpdatesAction");
            adminBreadCrumbsService.AddBreadCrumbs("Blacklist", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "BlacklistIndexTitle", "BlacklistIndexDescription", "BlacklistIndexController", "BlacklistIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Blacklist", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "BlacklistEditTitle", "BlacklistEditDescription", "BlacklistEditController", "BlacklistEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Blacklist", "Add", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "BlacklistAddTitle", "BlacklistAddDescription", "BlacklistAddController", "BlacklistAddAction");
            adminBreadCrumbsService.AddBreadCrumbs("Links", "Index", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "LinksIndexTitle", "LinksIndexDescription", "LinksIndexController", "LinksIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Links", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "LinksEditTitle", "LinksEditDescription", "LinksEditController", "LinksEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Links", "Add", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "LinksAddTitle", "LinksAddDescription", "LinksAddController", "LinksAddAction");
            adminBreadCrumbsService.AddBreadCrumbs("Poll", "Index", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "PollIndexTitle", "PollIndexDescription", "PollIndexController", "PollIndexAction");
            adminBreadCrumbsService.AddBreadCrumbs("Poll", "Edit", "fa fa-edit", "Localization.Languages.Database.AdminBreadcrumbs", "PollEditTitle", "PollEditDescription", "PollEditController", "PollEditAction");
            adminBreadCrumbsService.AddBreadCrumbs("Poll", "Add", "fa fa-plus-circle", "Localization.Languages.Database.AdminBreadcrumbs", "PollAddTitle", "PollAddDescription", "PollAddController", "PollAddAction");
            adminBreadCrumbsService.AddBreadCrumbs("Poll", "View", "fa fa-eye", "Localization.Languages.Database.AdminBreadcrumbs", "PollViewTitle", "PollViewDescription", "PollViewController", "PollViewAction");
        }

        #endregion

        #region Email

        private bool InstallEmailAccounts(string noReplyHost, string noReplyUsername, string noReplyPassword,
            int noReplyPort, bool noReplySecure, string replyHost, string replyUsername, string replyPassword,
            int replyPort, bool replySecure)
        {
            try
            {
                var emailAccountService = new EmailAccountService();
                emailAccountService.AddEmailAccount(1, replyHost, replyUsername, replyPassword, replyPort, replySecure);
                emailAccountService.AddEmailAccount(2, noReplyHost, noReplyUsername, noReplyPassword, noReplyPort,
                    noReplySecure);

                return true;
            }
            catch
            {
                return false;
            }

        }

        private bool InstallEmailTemplates()
        {
            try
            {
                var emailTemplateService = new EmailTemplateService();

                emailTemplateService.AddEmailTemplate(1,
                    "<p> Dear {username},<br /> <br /> Welcome to {title}!<br /> <br /> Thank you for registering your account, we wish you all the best with ranking your website to the top.<br /> <br /> Sincerely,<br /> <br /> {title}</p>",
                    "Welcome {username} to {title}!", "Welcome email, no verification required");
                emailTemplateService.AddEmailTemplate(2,
                    "<p> Dear {username},<br /> <br /> Welcome to {title}!<br /> <br /> Thank you for registering your account, to complete your registration you will have to verify your account by following this link: {link}<br /> <br /> Sincerely,<br /> <br /> {title}</p>",
                    "Welcome {username} to {title}!", "Welcome email, verification required");
                emailTemplateService.AddEmailTemplate(3,
                    "Dear {username}, <br /><br /> Youve requested a password reset for your account. Your new password is: {password}. <br /><br /> Please do not forgot this password, we recommend you to write it down somewhere. <br /><br /> Sincerely, {title}",
                    "Your new password for {title}", "Password reset email");
                emailTemplateService.AddEmailTemplate(4,
                    "Dear user, <br /><br /> Your account name is: {username} <br /><br /> Please dont forget your account name, we recommend you to write it down somewhere. <br /><br /> Sincerely, {title}",
                    "Your account name at {title}", "Username request email");
                emailTemplateService.AddEmailTemplate(5,
                    "Dear administrator, <br /><br /> Youve received a contact form submission from {name}. <br /><br /> Details: <br /><br /> Email: {email} <br /> Name: {name} <br/> Subject: {subject} <br/> Send on: {sendDate} <br /> IP: {ip} <br/> Message: {message}",
                    "You’ve received an email from {name} regarding {subject}", "Contact form email");
                emailTemplateService.AddEmailTemplate(6,
                    "Dear {username}, <br /><br /> Here are your website results from the past 7 days, these statistics include the amounts of visitors you had from us including votes. <br /><br /> {table} <br /> Sincerely, <br /><br /> {title}",
                    "Your weekly statistics for {website}", "Weekly statistics email");
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Language

        private bool InstallLanguages()
        {
            try
            {
                var languageService = new LanguageService();
                languageService.AddLanguage("en", "en-US", "English");
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
