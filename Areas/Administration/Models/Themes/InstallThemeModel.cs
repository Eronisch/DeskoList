using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Topsite.Areas.Administration.Models.Themes
{
    public class InstallThemeModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.InstallThemeModel), Name = "Theme")]
        [Required(ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.InstallThemeModel), ErrorMessageResourceName = "NoFileSelected")]
        [FileExtensions(Extensions = ".zip", ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.InstallThemeModel), ErrorMessageResourceName = "InvalidExtension", ErrorMessage = null)]
        public HttpPostedFileBase Theme { get; set; }
    }
}