using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Localization.Languages.Admin.Models;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Widgets
{
    public class WidgetSettingsModel
    {
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int Id { get; set; }

        [Display(ResourceType = typeof(WidgetSettings), Name = "Order")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int? Order { get; set; }

        [Display(ResourceType = typeof(WidgetSettings), Name = "Section")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int ThemeSectionId { get; set; }
        public IEnumerable<SelectListItem> ThemeSections { get; set; }

        public WidgetSettingsModel()
        {
            ThemeSections = new Collection<SelectListItem>();
        }
    }
}