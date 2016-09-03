using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.News
{
    public class BaseViewNewsModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BaseViewNewsModel), Name = "Subject")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Subject { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BaseViewNewsModel), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BaseViewNewsModel), Name = "Information")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(5000, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [AllowHtml]
        public string Information { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BaseViewNewsModel), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Description { get; set; }
    }
}