using System.Collections.Generic;
using System.Web.Mvc;
using Topsite.Models;

namespace Topsite.Areas.Administration.Models.Account
{
    public class EditAccountAdminModel : EditAccountModel
    {
        public EditAccountAdminModel() { }

        public EditAccountAdminModel(int accountId, string email, int securityQuestionId, string securityAnswer, IEnumerable<SelectListItem> securityQuestions, bool isBanned, bool isAdminVerified, bool isEmailVerified)
        {
            AccountBanModel = new AccountBanModel(accountId);
            Email = email;
            AccountId = accountId;
            QuestionId = securityQuestionId;
            Answer = securityAnswer;
            Questions = securityQuestions;
            _isBanned = isBanned;
            _isAdminVerified = isAdminVerified;
            _isEmailVerified = isEmailVerified;
        }

        #region Data

        private readonly bool _isBanned;
        private readonly bool _isAdminVerified;
        private readonly bool _isEmailVerified;

        public bool IsBanned()
        {
            return _isBanned;   
        }

        public bool IsAdminVerified()
        {
            return _isAdminVerified;
        }

        public bool IsEmailVerified()
        {
            return _isEmailVerified;
        }

        #endregion

        #region Form fields

        public int AccountId { get; set; }

        public AccountBanModel AccountBanModel { get; set; }

        #endregion
    }
}