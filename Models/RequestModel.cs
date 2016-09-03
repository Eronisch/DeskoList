using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class RequestModel
    {
        [EmailAddress(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Email", ErrorMessage = null)]
        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.RequestModel), Name = "Email")]
        public string Email { get; set; }
    }
}