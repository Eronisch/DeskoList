using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class ResetModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Models.ResetModel), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }
    }

    public class ResetStepTwoModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Models.ResetModel), Name = "Answer")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string Answer { get; set; }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int QuestionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        public string Question { get; set; }
    }
}