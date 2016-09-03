using Core.Business.Contact;
using Core.Business.Settings;
using Database.Entities;

namespace Core.Business.Account
{
    public class ResetService
    {
        private readonly AccountService _accountService = new AccountService();

        /// <summary>
        /// Sends an password reset email to the given user
        /// </summary>
        /// <param name="username"></param>
        public void SendPasswordResetEmail(string username)
        {
            var contactService = new ContactService();
            var settingsService = new SettingsService();

            var account = _accountService.GetUserByUsername(username);

            string password = UpdatePassword(account);

            contactService.SendPasswordResetEmail(password, account.Email, account.Username, settingsService.GetTitle());
        }

        private string UpdatePassword(Users account)
        {
            string generatedPassword = Generator.Generator.GenerateRandomCode(10, 25);
            string generatedHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            account.Password = generatedHash;
            _accountService.UpdateAccount(account);

            return generatedPassword;
        }
    }
}
