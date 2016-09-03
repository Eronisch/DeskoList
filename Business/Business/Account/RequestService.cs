using Core.Business.Contact;
using Core.Business.Settings;
using Core.Models.Account;

namespace Core.Business.Account
{
    public class RequestService
    {
        /// <summary>
        /// Sends an email to the given email address with his username
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public RequestUsernameType SendUserNameForgotEmail(string email)
        {
            var accountService = new AccountService();
            var contactService = new ContactService();
            var settingsService = new SettingsService();

            var account = accountService.GetUserByEmail(email);

            if (account == null) { return RequestUsernameType.UserNotFound; }

            contactService.SendRequestUsernameEmail(account, settingsService.GetTitle());

            return RequestUsernameType.Success;;
        }
    }
}
