using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Web.Account
{
    public class AccountService
    {
        private readonly Core.Business.Account.AccountService _accountService;

        public AccountService()
        {
            _accountService = new Core.Business.Account.AccountService();
        }

        public IEnumerable<SelectListItem> GetSecurityQuestions(int? selectedQuestionId = null)
        {
            return _accountService.GetSecurityQuestions().Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Question,
                Selected = selectedQuestionId != null && s.Id == selectedQuestionId.Value
            });
        }
    }
}
