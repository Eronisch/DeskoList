using System.ComponentModel.DataAnnotations;

namespace Topsite.Models
{
    public class EditWebsiteModel : BasicWebsiteModel
    {
        public int Id { get; set; }
        public bool IsMonitoringEnabled { get; set; }
        [Display(ResourceType = typeof(Localization.Languages.Models.EditWebsiteModel), Name = "IsSponsored")]
        public bool IsSponsored { get; set; }
    }
}