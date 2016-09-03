using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;
using Topsite.Annotations;

namespace Topsite.Models
{
    public class InstallModel
    {
        /* Database */
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "DatabaseHost")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string DatabaseHost { get; set; }
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "DatabaseName")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string DatabaseName { get; set; }
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "DatabaseUsername")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string DatabaseUsername { get; set; }
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "DatabasePassword")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string DatabasePassword { get; set; }

        /* User */
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [Username(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Username")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "ConfirmPassword")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        public string ConfirmPassword { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Email", ErrorMessage = null)]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Security_Question")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int SecurityQuestion { get; set; }

        public IEnumerable<SelectListItem> SecurityQuestions { get; set; }
            
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Security_Answer")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SecurityAnswer { get; set; }

        /* Email Settings - No Reply */
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailHost")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string NoReplyHost { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailUsername")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string NoReplyUsername { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailPassword")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string NoReplyPassword { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailPort")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int? NoReplyPort { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailSecure")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool NoReplySecureConnection { get; set; }

        /* Email Settings - Reply */
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailHost")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string ReplyHost { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailUsername")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string ReplyUsername { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailPassword")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string ReplyPassword { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailPort")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int? ReplyPort { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "EmailSecure")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool ReplySecureConnection { get; set; }

        /* Settings */
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsLongTitle")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsLongTitle { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsShortTitle")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsShortTitle { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsDescription")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsDescription { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsKeywords")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsKeywords { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsAmountWebsites")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int? SettingsAmountWebsites { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsSlogan")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsSlogan { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsUrl")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsUrl { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingEmailVerificationRequired")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool SettingsIsEmailVerificationRequired { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "IsAdminVerificationRequired")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool SettingsIsAdminVerificationRequired { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsUserServerMonitoringEnabled")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool SettingsIsUserServerMonitoringEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsAutoUpdate")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool SettingsIsAutoUpdateEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "SettingsIsResetInAndOutsEnabled")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public bool SettingsIsResetInAndOutsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsEmailUserStatisticsCronjob { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsInAndOutCronJob { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsUpdateCronjob { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsMonitorUserWebsitesCronjob { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SettingsCreateUserWebsiteThumbnailsCronjob { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "IsEmailingUserStatisticsEnabled")]
        public bool SettingsIsEmailingUserStatisticsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "IsCreateUserWebsiteThumbnailsEnabled")]
        public bool SettingsIsCreateUserWebsiteThumbnailsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "IsUpdateWhenIncorrectVersionEnabled")]
        public bool IsUpdateWhenIncorrectVersionEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Language")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int LanguageId { get; set; }

        public IEnumerable<SelectListItem> Languages { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "Timezone")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string TimezoneId { get; set; }

        public IEnumerable<SelectListItem> Timezones { get; set; }

        /* Captcha */
        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "RecaptchaSiteKey")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CaptchaSiteKey { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.InstallModel), Name = "RecaptchaSecretKey")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CaptchaSecretKey { get; set; }
    }
}