using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Topsite.Areas.Administration.Models.Widgets
{
    public class WidgetInstallModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.WidgetInstallModel), Name = "Widget")]
        [Required(ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.WidgetInstallModel), ErrorMessageResourceName = "NoFileSelected")]
        [FileExtensions(Extensions = ".zip", ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.WidgetInstallModel), ErrorMessageResourceName = "InvalidExtension", ErrorMessage = null)]
        public HttpPostedFileBase File { get; set; }
    }
}