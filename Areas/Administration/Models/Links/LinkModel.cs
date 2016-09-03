using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Links
{
    public class LinkModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.LinkModel), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.LinkModel), Name = "Url")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [Annotations.Url(ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.LinkModel), ErrorMessageResourceName = "InvalidUrl")]
        public string Url { get; set; }
    }
}