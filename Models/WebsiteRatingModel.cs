using System.ComponentModel.DataAnnotations;

namespace Topsite.Models
{
    public class WebsiteRatingModel
    {
        [Required(ErrorMessageResourceType = typeof(Localization.Languages.Models.Shared.Shared), ErrorMessageResourceName = "Required")]
        [Range(1, 5, ErrorMessageResourceType = typeof(Localization.Languages.Models.Shared.Shared), ErrorMessageResourceName = "Range")]
        [Display(ResourceType = typeof(Localization.Languages.Models.WebsiteRatingModel), Name = "Rating")]
        public int Rating { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Languages.Models.Shared.Shared), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Languages.Models.WebsiteRatingModel), Name = "WebsiteId")]
        public int WebsiteId { get; set; }

        public WebsiteRatingModel()
        {
            Rating = 5;
        }

        public WebsiteRatingModel(int websiteId) : this()
        {
            WebsiteId = websiteId;
        }
    }
}