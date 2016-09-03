using System;
using Core.Business.Schedule;
using Core.Business.Software;
using Core.Infrastructure.Tasks;
using Core.Infrastructure.Tasks.Email;
using Core.Infrastructure.Tasks.Reset;
using Core.Infrastructure.Tasks.Thumbnail;
using Core.Infrastructure.Tasks.Update;
using Core.Models.Settings;
using Database;
using DatabaseXML;

namespace Core.Business.Settings
{
    public class SettingsService : IDisposable
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        public Database.Entities.Settings GetSettings()
        {
            return _unitOfWorkRepository.SettingsRepository.GetById(id: 1);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_unitOfWorkRepository != null) _unitOfWorkRepository.Dispose();
            }
        }

        public void UpdateMaintenance(bool isMaintenance)
        {
            var settings = GetSettings();

            settings.Maintenance = isMaintenance;

            _unitOfWorkRepository.SaveChanges();
        }

        public bool IsInstalled(string version)
        {
            var versionOrderingService = new VersionOrderingService();

            return versionOrderingService.Compare(version, GetVersion()) >= 0;
        }

        public int GetActiveThemeId()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveThemeId();
        }

        public bool UpdateWhenIncorrectVersion()
        {
            return _unitOfWorkRepository.SettingsRepository.UpdateWhenIncorrectVersion();
        }

        public string GetActiveCulture()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveLanguageCulture();
        }

        public string GetActiveCultureName()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveLanguageName();
        }

        public string GetActiveTheme()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveTheme();
        }

        public string GetActiveThemeMap()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveThemeMap();
        }

        public string GetTitle()
        {
            return _unitOfWorkRepository.SettingsRepository.GetTitle();
        }

        public string GetShortTitle()
        {
            return _unitOfWorkRepository.SettingsRepository.GetShortTitle();
        }

        public string GetSlogan()
        {
            return _unitOfWorkRepository.SettingsRepository.GetSlogan();
        }

        public int GetAmountWebsitesToShow()
        {
            return _unitOfWorkRepository.SettingsRepository.ShowAmountWebsites();
        }

        public bool IsEmailVerificationRequired()
        {
            return _unitOfWorkRepository.SettingsRepository.IsEmailVerificationRequired();
        }

        public bool IsAutoUpdateEnabled()
        {
            return _unitOfWorkRepository.SettingsRepository.IsAutoUpdateEnabled();
        }

        public string GetVersion()
        {
            return _unitOfWorkRepository.SettingsRepository.GetVersion();
        }

        public string GetCronjobUserStatisticsEmail()
        {
            return _unitOfWorkRepository.SettingsRepository.GetCronjobUserStatisticsEmail();
        }

        public string GetCronjobUpdate()
        {
            return _unitOfWorkRepository.SettingsRepository.GetCronjobUpdate();
        }

        public string GetCronjobInAndOut()
        {
            return _unitOfWorkRepository.SettingsRepository.GetCronjobInAndOut();
        }

        public bool IsMaintenance()
        {
            return _unitOfWorkRepository.SettingsRepository.IsMaintenance();
        }

        public bool IsUserServerMonitoringEnabled()
        {
            return _unitOfWorkRepository.SettingsRepository.IsUserServerMonitoringEnabled();
        }

        public bool IsAdminVerificationRequired()
        {
            return _unitOfWorkRepository.SettingsRepository.IsAdminVerificationRequired();
        }

        public BasicSettingsModel BasicSettingsModel()
        {
            var basicInfo = _unitOfWorkRepository.SettingsRepository.GetBasicInfo();

            return new BasicSettingsModel
            {
                Title = basicInfo.Title,
                Description = basicInfo.Description,
                Keywords = basicInfo.Keywords,
                ShortTitle = basicInfo.SiteTitle,
                Slogan = basicInfo.SiteSlogan
            };
        }

        public string GetDescription()
        {
            return _unitOfWorkRepository.SettingsRepository.GetDescription();
        }

        public string GetKeywords()
        {
            return _unitOfWorkRepository.SettingsRepository.GetKeywords();
        }

        public bool IsCreateThumbnailsEnabled()
        {
            return _unitOfWorkRepository.SettingsRepository.IsCreateThumbnailsEnabled();
        }

        public string GetCronjobWebsiteThumbnail()
        {
            return _unitOfWorkRepository.SettingsRepository.GetCronjobWebsiteThumbnail();
        }

        public void UpdateSettings(string title, string shortTitle, string description, string keywords, int amountWebsites, string url, string statisticsUserEmailCronjob, string inAndOutResetCronjob, string updateCronjob, string createUserWebsiteThumbnailsCronjob, string monitorUserServerCronjob, int languageId, string timezone, string slogan, int themeId, string captchaSecretKey, string captchaSiteKey, bool isUserServerMonitoringEnabled, bool isUserEmailVerificationRequired, bool isAutoUpdatingEnabled, bool isEmailingUserStatisticsEnabled, bool isCreatingWebsiteThumbnailsEnabled, bool isResetInAndOutsEnabled, bool updateWhenIncorrectVersion, bool isAdminVerificationRequired)
        {
            var settings = new Database.Entities.Settings
            {
                ID = 1,
                Version = LocalDatabaseSettingsService.Manager.Version,
                Title = title,
                SiteTitle = shortTitle,
                Description = description,
                Keywords = keywords,
                ShowAmountWebsites = amountWebsites,
                CronjobUserStatisticsEmail = statisticsUserEmailCronjob,
                CronjobInAndOut = inAndOutResetCronjob,
                CronUpdate = updateCronjob,
                CronPingServer = monitorUserServerCronjob,
                SiteSlogan = slogan,
                Url = url,
                IsEmailVerificationRequired = isUserEmailVerificationRequired,
                IsPingEnabled = isUserServerMonitoringEnabled,
                Maintenance = false,
                ThemeId = themeId,
                RecaptchaSecretKey = captchaSecretKey,
                RecaptchaSiteKey = captchaSiteKey,
                AutoUpdate = isAutoUpdatingEnabled,
                IsEmailingUserStatisticsEnabled = isEmailingUserStatisticsEnabled,
                IsCreateThumbnailsEnabled = isCreatingWebsiteThumbnailsEnabled,
                IsResetInAndOutsEnabled = isResetInAndOutsEnabled,
                CronWebsiteThumbnail = createUserWebsiteThumbnailsCronjob,
                LanguageId = languageId,
                UpdateWhenIncorrectVersion = updateWhenIncorrectVersion,
                Timezone = timezone,
                IsAdminVerificationRequired = isAdminVerificationRequired
            };

            _unitOfWorkRepository.SettingsRepository.UpdateSettings(settings);

            _unitOfWorkRepository.SaveChanges();

            UpdateUserServerMonitoring(isUserServerMonitoringEnabled, monitorUserServerCronjob);

            UpdateResetInAndOuts(isResetInAndOutsEnabled, inAndOutResetCronjob);

            UpdateEmailUserStatisticsTask(isEmailingUserStatisticsEnabled, statisticsUserEmailCronjob);

            UpdateUpdaterTask(isAutoUpdatingEnabled, updateCronjob);

            UpdateUserWebsiteThumbnailsTask(isCreatingWebsiteThumbnailsEnabled, createUserWebsiteThumbnailsCronjob);
        }

        public void SetVersion(string version)
        {
            var settings = GetSettings();

            settings.Version = version;

            _unitOfWorkRepository.SaveChanges();
        }

        public void SetActiveTheme(int themeId)
        {
            var settings = GetSettings();

            settings.ThemeId = themeId;

            _unitOfWorkRepository.SettingsRepository.UpdateSettings(settings);

            _unitOfWorkRepository.SaveChanges();
        }

        public string GetCronjobPing()
        {
            return _unitOfWorkRepository.SettingsRepository.GetCronjobPingServer();
        }

        public CaptchaModel GetCaptcha()
        {
            var captcha = _unitOfWorkRepository.SettingsRepository.GetCaptcha();

            return new CaptchaModel
            {
                RepcaptchaSecretKey = captcha.RecaptchaSecretKey,
                RepcaptchaSiteKey = captcha.RecaptchaSiteKey
            };
        }

        public bool IsEmailingUserStatisticsEnabled()
        {
            return _unitOfWorkRepository.SettingsRepository.IsEmailingUserStatisticsEnabled();
        }

        public bool IsResetInAndOutsEnabled()
        {
            return _unitOfWorkRepository.SettingsRepository.IsResetInAndOutsEnabled();
        }

        public string GetActiveLanguageCulture()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveLanguageCulture();
        }


        public string GetActiveTimeZone()
        {
            return _unitOfWorkRepository.SettingsRepository.GetTimeZone();
        }

        public string GetActiveLanguageAbbreviation()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveLanguageAbbreviation();
        }

        public int GetActiveLanguageId()
        {
            return _unitOfWorkRepository.SettingsRepository.GetActiveLanguageId();
        }

        #region Private Methods

        private void UpdateEmailUserStatisticsTask(bool isEnabled, string cronjob)
        {
            if (!isEnabled)
            {
                ScheduleService.UnSchedule(ScheduleConfig.GetEmailUserWebsiteStatisticsKey());
            }
            else if (isEnabled && !ScheduleConfig.IsEmailingUserStatisticsEnabled())
            {
                ScheduleService.Schedule<EmailTask>(ScheduleConfig.GetEmailUserWebsiteStatisticsKey(), cronjob);
            }
            else if (cronjob != ScheduleConfig.GetCronjobUserStatisticsEmail())
            {
                ScheduleService.UpdateSchedule(ScheduleConfig.GetEmailUserWebsiteStatisticsKey(), cronjob);
            }
        }

        private void UpdateResetInAndOuts(bool isEnabled, string cronjob)
        {
            if (!isEnabled)
            {
                ScheduleService.UnSchedule(ScheduleConfig.GetResetInAndOutsKey());
            }
            else if (isEnabled && !ScheduleConfig.IsEmailingUserStatisticsEnabled())
            {
                ScheduleService.Schedule<ResetTask>(ScheduleConfig.GetResetInAndOutsKey(), cronjob);
            }
            else if (cronjob != ScheduleConfig.GetCronjobUserStatisticsEmail())
            {
                ScheduleService.UpdateSchedule(ScheduleConfig.GetResetInAndOutsKey(), cronjob);
            }
        }

        private void UpdateUpdaterTask(bool isEnabled, string cronjob)
        {
            if (!isEnabled)
            {
                ScheduleService.UnSchedule(ScheduleConfig.GetUpdateKey());
            }
            else if (isEnabled && !ScheduleConfig.IsEmailingUserStatisticsEnabled())
            {
                ScheduleService.Schedule<UpdateTask>(ScheduleConfig.GetUpdateKey(), cronjob);
            }
            else if (cronjob != ScheduleConfig.GetCronjobUpdate())
            {
                ScheduleService.UpdateSchedule(ScheduleConfig.GetUpdateKey(), cronjob);
            } 
        }

        private void UpdateUserWebsiteThumbnailsTask(bool isEnabled, string cronjob)
        {
            if (!isEnabled)
            {
                ScheduleService.UnSchedule(ScheduleConfig.GetCreateUserWebsitesThumbnailKey());
            }
            else if (isEnabled && !ScheduleConfig.IsCreateThumbnailsEnabled())
            {
                ScheduleService.Schedule<ThumbnailTask>(ScheduleConfig.GetCreateUserWebsitesThumbnailKey(), cronjob);
            }
            else if (cronjob != ScheduleConfig.GetCronjobUpdate())
            {
                ScheduleService.UpdateSchedule(ScheduleConfig.GetCreateUserWebsitesThumbnailKey(), cronjob);
            }
        }

        private void UpdateUserServerMonitoring(bool isEnabled, string cronjob)
        {
            if (!isEnabled)
            {
                ScheduleService.UnSchedule(ScheduleConfig.GetUserServerMonitoringKey());
            }
            else if (isEnabled && !ScheduleConfig.IsUserServerMonitoringEnabled())
            {
                ScheduleService.Schedule<ThumbnailTask>(ScheduleConfig.GetUserServerMonitoringKey(), cronjob);
            }
            else if (cronjob != ScheduleConfig.GetCronjobUpdate())
            {
                ScheduleService.UpdateSchedule(ScheduleConfig.GetCronjobUserServerMonitoring(), cronjob);
            }
        }

        #endregion
    }
}
