using System;

namespace Core.Models.Poll
{
    public class PollAnswerModel
    {
        public PollAnswerModel(int id, string answer, int amountVotes, int totalAmountVotes)
        {
            Id = id;
            Answer = answer;
            AmountVotes = amountVotes;
            Percentage = GetPercentage(totalAmountVotes, amountVotes);
        }

        public int Id { get; private set; }
        public string Answer { get; private set; }
        public int AmountVotes { get; private set; }
        public int Percentage { get; private set; }

        private int GetPercentage(int totalAmountVotes, int amountVotes)
        {
            if (totalAmountVotes == 0 || amountVotes == 0)
            {
                return 0;
            }

            return (int) Math.Round((double) (100 / totalAmountVotes * amountVotes), 0);
        }
    }
}
