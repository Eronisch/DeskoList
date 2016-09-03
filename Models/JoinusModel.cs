using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models;
using Localization.Languages.Models.Shared;
using Topsite.Annotations;

namespace Topsite.Models
{
    public class JoinusModel
    {
        public JoinusModel()
        {
            Account = new BasicAccountModel();
            Website = new BasicWebsiteModel();
        }

        public BasicAccountModel Account { get; set; }
        public BasicWebsiteModel Website { get; set; }

        [Required(ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "Required")]
        [Username(ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "Username")]
        [MaxLength(15, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(3, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MinimumLength")]
        [Display(ResourceType = typeof (JoinUsModel), Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(5, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MinimumLength")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (JoinUsModel), Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MaximumLength")]
        [MinLength(5, ErrorMessageResourceType = typeof (Shared), ErrorMessageResourceName = "MinimumLength")]
        [Compare("Password", ErrorMessageResourceType = typeof (JoinUsModel),
            ErrorMessageResourceName = "Passwords")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (JoinUsModel), Name = "PasswordRepeat")]
        public string PasswordRepeat { get; set; }

        public string CaptchaKey { get; set; }
    }
}