using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Pages
{
    public class BasicPageViewModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BasicPageViewModel), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(125, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BasicPageViewModel), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BasicPageViewModel), Name = "Keywords")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Keywords { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BasicPageViewModel), Name = "Content")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(5000, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [AllowHtml]
        public string Content { get; set; }
    }
}