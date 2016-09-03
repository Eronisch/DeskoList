using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Localization.Languages.Models.Shared;

namespace Topsite.Areas.Administration.Models.Poll
{
    public class PollModel
    {
        [Display(ResourceType = typeof(Localization.Languages.Admin.Models.PollModel), Name = "Question")]
        [Required(ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "Required")]
        [MaxLength(75, ErrorMessageResourceType = typeof(Shared), ErrorMessageResourceName = "MaximumLength")]
        public string Question { get; set; }
    }
}