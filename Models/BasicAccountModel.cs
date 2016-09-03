using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class BasicAccountModel
    {
        [EmailAddress(ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "Email", ErrorMessage = null)]
        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(50, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MaximumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicAccountModel), Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(255, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [Display(ResourceType = typeof(Localization.Languages.Models.BasicAccountModel), Name = "Answer")]
        public string Answer { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.BasicAccountModel), Name = "Question")]
        public int QuestionId { get; set; }

        public IEnumerable<SelectListItem> Questions { get; set; }
    }
}