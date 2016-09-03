namespace Core.Models.Account
{
    public class AccountSecurityQuestionModel
    {
        public AccountSecurityQuestionModel(int id, string question)
        {
            Id = id;
            Question = question;
        }

        public readonly int Id;
        public readonly string Question;
    }
}
