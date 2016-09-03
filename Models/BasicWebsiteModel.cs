using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class BasicWebsiteModel
    {
        private string PrivateUrl { get; set; }
        private string PrivateBannerUrl { get; set; }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [Annotations.Url(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Url")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "Url")]
        public string Url
        {
            get { return PrivateUrl; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!value.Contains("http://"))
                        PrivateUrl = "http://" + value;
                }
                PrivateUrl = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(75, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "Title")]
        public string Title { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(15, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "Description")]
        public string Description { get; set; }

        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(150, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MaximumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "BannerUrl")]
        public string BannerUrl
        {
            get { return PrivateBannerUrl; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!value.StartsWith("http"))
                        PrivateBannerUrl = "http://" + value;
                }
                PrivateBannerUrl = value;
            }
        }

        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "LocalRemoteBanner")]
        public bool UseBannerUrl { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "BannerFile")]
        public HttpPostedFileBase BannerFile { get; set; }

        [MaxLength(150, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "Keywords")]
        public string Keywords { get; set; }

        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "Category")]
        public int Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        /* Server */
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "ServerPort")]
        public int? ServerPort { get; set; }
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicWebsiteModel), Name = "ServerIp")]
        public string ServerIp { get; set; }
    }
}