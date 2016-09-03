using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Blacklist
{
    public class BlacklistModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.BlacklistModel), Name = "Domain")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        [RegularExpression(@"^(([a-zA-Z]{1})|([a-zA-Z]{1}[a-zA-Z]{1})|([a-zA-Z]{1}[0-9]{1})|([0-9]{1}[a-zA-Z]{1})|([a-zA-Z0-9][a-zA-Z0-9-_]{1,61}[a-zA-Z0-9]))\.([a-zA-Z]{2,6}|[a-zA-Z0-9-]{2,30}\.[a-zA-Z]{2,3})$", ErrorMessageResourceType = typeof(Localization.Languages.Admin.Models.BlacklistModel), ErrorMessageResourceName = "InvalidDomain")]
        public string Domain { get; set; }
    }
}