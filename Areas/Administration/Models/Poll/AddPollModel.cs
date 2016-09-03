using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Topsite.Areas.Administration.Models.Poll
{
    public class AddPollModel : PollModel
    {
        public IList<PollAnswerModel> Answers { get; set; }
    }
}