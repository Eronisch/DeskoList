using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Poll
{
    public class PollAnswerModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.PollAnswerModel), Name = "Answer")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Answer { get; set; }
    }
}