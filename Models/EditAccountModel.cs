using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class EditAccountModel
    {
        public EditAccountModel() { }

        public EditAccountModel(bool isEmailVerificationRequired)
        {
            IsEmailVerificationRequired = isEmailVerificationRequired;
        }

        [Display(ResourceType = typeof(Localization.Languages.Models.EditAccountModel), Name = "Answer")]
        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Answer { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.EditAccountModel), Name = "Email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Email", ErrorMessage = null)]
        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [MinLength(5, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MinimumLength")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.EditAccountModel), Name = "OldPassword")]
        public string OldPassword { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Models.EditAccountModel), Name = "NewPassword")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        public int QuestionId { get; set; }

        public IEnumerable<SelectListItem> Questions{ get; set; }

        private bool IsEmailVerificationRequired { get; set; }

        public bool GetIsEmailVerificationRequired()
        {
            return IsEmailVerificationRequired;
        }
    }
}