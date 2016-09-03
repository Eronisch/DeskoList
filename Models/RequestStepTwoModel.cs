using System.ComponentModel.DataAnnotations;
using Localization.Languages.Models.Shared;

namespace Topsite.Models
{
    public class RequestStepTwoModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Models.RequestStepTwoModel), Name = "Answer")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string Answer { get; set; }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public int QuestionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        public string Email { get; set; }

        public string Question { get; set; }
    }
}