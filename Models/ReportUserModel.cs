using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class ReportUserModel
    {
        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.ReportUserModel), Name = "Reason")]
        public string Reason { get; set; }

        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        public int WebsiteId { get; set; }

        public string WebsiteTitle { get; set; }
    }
}