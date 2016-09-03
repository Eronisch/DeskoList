using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
            
        }

        public LoginModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Languages.Models.LoginModel), Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType= typeof(Shared), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Languages.Models.LoginModel), Name = "Password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}