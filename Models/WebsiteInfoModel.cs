using System.ComponentModel.DataAnnotations;
using Topsite.Annotations;

namespace Topsite.Models
{
    public class WebsiteInfoModel
    {
        public WebsiteInfoModel()
        {
            Id = -1;
        }

        [Required(ErrorMessage = "The field {0} is required")]
        [AlphaNumeric]
        public string User { get; set; }

        public int Id { get; set; }
    }
}