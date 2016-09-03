using Core.Business.Settings;

namespace Core.Infrastructure.Tasks
{
    internal class ScheduleConfig
    {
        private const string ResetInAndOutsKey = "ResetInOuts";
        private const string ResetDailyInAndOutsKey = "ResetDaily";
        private const string EmailWebsiteStatisticsKey = "EmailUserWebsiteStatistics";
        private const string PingUserServersKey = "PingUserServers";
        private const string CreateUserWebsitesThumbnailsKey = "UserWebsiteThumbnail";
        private const string UpdateKey = "Updater";
        private const string ResetDailyInAndOutsCronjob = "0 0 0 1 1/1 ? *";

        public static string GetResetInAndOutsKey()
        {
            return ResetInAndOutsKey;
        }

        public static string GetResetDailyInAndOutsKey()
        {
            return ResetDailyInAndOutsKey;
        }

        public static string GetEmailUserWebsiteStatisticsKey()
        {
            return EmailWebsiteStatisticsKey;
        }

        public static string GetUserServerMonitoringKey()
        {
            return PingUserServersKey;
        }

        public static string GetCreateUserWebsitesThumbnailKey()
        {
            return CreateUserWebsitesThumbnailsKey;
        }

        public static string GetUpdateKey()
        {
            return UpdateKey;
        }

        public static string GetResetDailyInAndOutsCronjob()
        {
            return ResetDailyInAndOutsCronjob;
        }

        public static string GetCronjobResetInAndOuts()
        {
            var settingsService = new SettingsService();

            return settingsService.GetCronjobInAndOut();
        }

        public static string GetCronjobUserStatisticsEmail()
        {
            var settingsService = new SettingsService();

            return settingsService.GetCronjobUserStatisticsEmail();
        }

        public static string GetCronjobUserServerMonitoring()
        {
            var settingsService = new SettingsService();

            return settingsService.GetCronjobPing();
        }

        public static string GetCronjobCreateUserWebsiteThumbnails()
        {
            var settingsService = new SettingsService();

            return settingsService.GetCronjobWebsiteThumbnail();
        }

        public static string GetCronjobUpdate()
        {
            var settingsService = new SettingsService();

            return settingsService.GetCronjobUpdate();
        }

        public static bool IsAutoUpdateEnabled()
        {
            var settingsService = new SettingsService();

            return settingsService.IsAutoUpdateEnabled();
        }

        public static bool IsUserServerMonitoringEnabled()
        {
            var settingsService = new SettingsService();

            return settingsService.IsUserServerMonitoringEnabled();
        }

        public static bool IsEmailingUserStatisticsEnabled()
        {
            var settingsService = new SettingsService();

            return settingsService.IsEmailingUserStatisticsEnabled();
        }

        public static bool IsCreateThumbnailsEnabled()
        {
             var settingsService = new SettingsService();

            return settingsService.IsCreateThumbnailsEnabled();
        }

        public static bool IsResetInAndOutsEnabled()
        {
            var settingsService = new SettingsService();

            return settingsService.IsResetInAndOutsEnabled();
        }
    }
}
