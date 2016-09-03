using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class SettingsRepository : IRepository<Settings>
    {
        private readonly DbSet<Settings> _query;

        public SettingsRepository(DbSet<Settings> settings)
        {
            _query = settings;
        }

        public Settings GetById(int id)
        {
            return _query.Find(id);
        }

        public int GetActiveThemeId()
        {
            return _query.Select(x => x.ThemeId).First();
        }

        public int ShowAmountWebsites()
        {
            return _query.Select(x => x.ShowAmountWebsites).First();

        }

        public string GetTitle()
        {
            return _query.Select(x => x.Title).First();
        }

        public string GetShortTitle()
        {
            return _query.Select(x => x.SiteTitle).First();
        }

        public string GetSlogan()
        {
            return _query.Select(x => x.SiteSlogan).First();
        }

        public string GetDescription()
        {
            return _query.Select(x => x.Description).First();
        }

        public string GetActiveLanguageCulture()
        {
            return _query.Select(x => x.Languages.Culture).First();
        }

        public string GetActiveLanguageName()
        {
            return _query.Select(x => x.Languages.Name).First();
        }

        public string GetActiveLanguageAbbreviation()
        {
            return _query.Select(x => x.Languages.Abbreviation).First();
        }

        public int GetActiveLanguageId()
        {
            return _query.Select(x => x.Languages.Id).First();
        }

        public string GetTimeZone()
        {
            return _query.Select(x => x.Timezone).First();
        }

        public Settings GetCaptcha()
        {
            var settingsData = (from settings in _query
                select new
                {
                    settings.RecaptchaSecretKey,
                    settings.RecaptchaSiteKey
                }).First();

            return new Settings
            {
                RecaptchaSecretKey = settingsData.RecaptchaSecretKey,
                RecaptchaSiteKey = settingsData.RecaptchaSiteKey
            };
        }

        public string GetKeywords()
        {
            return _query.Select(x => x.Keywords).First();
        }

        public string GetActiveTheme()
        {
            return _query.Select(x => x.Themes.ThemeName).First();
        }

        public string GetActiveThemeMap()
        {
            return _query.Select(x => x.Themes.FolderName).First();
        }

        public bool IsEmailVerificationRequired()
        {
            return _query.Select(x => x.IsAdminVerificationRequired).First();
        }

        public string GetVersion()
        {
            return _query.Select(x => x.Version).First();
        }


        public string GetCronjobUserStatisticsEmail()
        {
            return _query.Select(x => x.CronjobUserStatisticsEmail).First();
        }

        public string GetCronjobUpdate()
        {
            return _query.Select(x => x.CronUpdate).First();
        }

        public string GetCronjobInAndOut()
        {
            return _query.Select(x => x.CronjobInAndOut).First();
        }

        public bool IsMaintenance()
        {
            return _query.Select(x => x.Maintenance).First();
        }

        public Settings GetBasicInfo()
        {
            return _query.First();
        }

        public IQueryable<Settings> GetAll()
        {
            return _query;
        }

        public bool IsUserServerMonitoringEnabled()
        {
            return _query.Select(x => x.IsPingEnabled).First();
        }

        public bool UpdateWhenIncorrectVersion()
        {
            return _query.Select(x => x.UpdateWhenIncorrectVersion).First();
        }

        public bool IsAutoUpdateEnabled()
        {
            return _query.Select(x => x.AutoUpdate).First();
        }

        public void UpdateSettings(Settings settings)
        {
            _query.AddOrUpdate(settings);
        }

        public bool IsCreateThumbnailsEnabled()
        {
            return _query.Select(x => x.IsCreateThumbnailsEnabled).First();
        }

        public string GetCronjobWebsiteThumbnail()
        {
            return _query.Select(x => x.CronWebsiteThumbnail).First();
        }

        public string GetCronjobPingServer()
        {
            return _query.Select(x => x.CronPingServer).First();
        }

        public bool IsEmailingUserStatisticsEnabled()
        {
            return _query.Select(x => x.IsEmailingUserStatisticsEnabled).First();
        }

        public bool IsResetInAndOutsEnabled()
        {
            return _query.Select(x => x.IsResetInAndOutsEnabled).First();
        }

        public bool IsAdminVerificationRequired()
        {
            return _query.Select(x => x.IsAdminVerificationRequired).First();
        }
    }
}
