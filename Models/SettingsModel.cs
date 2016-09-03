using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class SettingsModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Keywords")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "ShowAmountWebsites")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int? ShowAmountWebsites { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Maintenance")]
        public bool IsMaintenance { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "SiteTitle")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SiteTitle { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "SiteSlogan")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string SiteSlogan { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "EmailVerification")]
        public bool EmailVerification{ get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Url")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [Url(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Url", ErrorMessage = null)]
        public string Url { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CronjobUserStatisticsEmail { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "MonitorEnabled")]
        public bool IsMonitorEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "MonitorEnabled")]
        public bool IsEmailingUserStatisticsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Theme")]
        public int ThemeId { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CronjobInAndOut { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "RecaptchaSecretKey")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string RecaptchaSecretKey { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "RecaptchaSiteKey")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string RecaptchaSiteKey { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CronWebsiteThumbnail { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Language")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int LanguageId { get; set; }

        public IEnumerable<SelectListItem> Languages { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Timezone")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Timezone { get; set; }

        public IEnumerable<SelectListItem> Timezones { get; set; }
            
        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "CreateWebsiteThumbnails")]
        public bool IsCreatingWebsiteThumbnailsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "IsResetInAndOutsEnabled")]
        public bool IsResetInAndOutsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "IsUpdateWhenIncorrectVersionEnabled")]
        public bool IsUpdateWhenIncorrectVersionEnabled { get; set; }


        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "IsAdminVerificationRequired")]
        public bool IsAdminVerificationRequired { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "AutoUpdate")]
        public bool AutoUpdate { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CronMonitorServer { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.SettingsModel), Name = "Cronjob")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string CronUpdate { get; set; }


        public IEnumerable<SelectListItem> Themes { get; set; }

        public SettingsModel()
        {
            Themes = new Collection<SelectListItem>();
            ShowAmountWebsites = 10;
        }
    }
}
