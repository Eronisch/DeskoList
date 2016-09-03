using System.Collections.Generic;
using System.Linq;
using Database.Entities;

namespace Core.Models.Poll
{
    public class PollResultModel
    {
        public PollResultModel(int id, string question, IList<PollAnswers> pollAnswers)
        {
            Id = id;
            Question = question;
            AmountVotes = GetAmountVotes(pollAnswers);
            Answers = GetAnswers(pollAnswers, AmountVotes);
        }

        public int Id { get; private set; }
        public string Question { get; private set; }
        public IEnumerable<PollAnswerModel> Answers { get; private set; }
        public int AmountVotes { get; }
        

        private IEnumerable<PollAnswerModel> GetAnswers(IEnumerable<PollAnswers> pollAnswers, int amountVotes)
        {
            return pollAnswers.Select(a => new PollAnswerModel(a.Id, a.Answer, a.PollVotes.Count, amountVotes));
        }

        private int GetAmountVotes(IEnumerable<PollAnswers> pollAnswers)
        {
            return pollAnswers.SelectMany(x => x.PollVotes).Count();
        }
    }
}
