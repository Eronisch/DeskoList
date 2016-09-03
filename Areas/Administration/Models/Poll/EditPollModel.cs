using System.Collections.Generic;

namespace Topsite.Areas.Administration.Models.Poll
{
    public class EditPollModel : PollModel
    {
        public int Id { get; set; }
        public IList<PollEditAnswerModel> Answers { get; set; }
    }
}