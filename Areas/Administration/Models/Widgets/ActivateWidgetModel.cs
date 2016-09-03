using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Widgets
{
    public class ActivateWidgetModel
    {
        public ActivateWidgetModel(int widgetId) : this()
        {
            WidgetId = widgetId;
        }

        public ActivateWidgetModel()
        {
            Order = 1;
        }

        public int WidgetId { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.ActivateWidgetModel), Name = "Order")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int? Order { get; set; }

        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.ActivateWidgetModel), Name = "Section")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int ThemeSectionId { get; set; }
    }
}