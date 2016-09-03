using System;
using System.Collections.Generic;
using System.Web.Security;
using Core.Business.Contact;
using Core.Business.Settings;
using Core.Business.Url;
using Core.Models;
using Core.Models.Account;
using Database;
using Database.Entities;

namespace Core.Business.Account
{
    /// <summary>
    /// Account manager for updating accounts in the database
    /// </summary>
    public class AccountService
    {
        private readonly Random _random = new Random();

        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        /// <summary>
        /// Validates the login information from an administrator login
        /// Uses the method ValidateLogin and checks if the user is an administrator
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public LoginType ValidateAdminLogin(string username, string password, string ip)
        {
            var validateLogin = ValidateLogin(username, password, ip);

            if (validateLogin != LoginType.Success) { return validateLogin; }

            if (!IsAdministrator(username)) { return LoginType.NoPermission; }

            return LoginType.Success;
        }

        /// <summary>
        /// Validates the login from a normal account login
        /// Checks the users login info, email verification, admin verification, banned 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public LoginType ValidateLogin(string username, string password, string ip)
        {
            if (IsIpBlocked(ip))
            {
                return LoginType.IpBlocked;
            }

            var settingsService = new SettingsService();
            var accountService = new AccountService();

            var account = accountService.GetUserByUsername(username);

            if (account == null)
            {
                return LoginType.NoAccountFound;
            }

            if (settingsService.IsEmailVerificationRequired() && account.IsEmailVerified == false)
            {
                return LoginType.NotVerified;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return LoginType.IncorrectPassword;
            }

            if (account.BannedStartDate != null && account.BannedEndDate != null && DateTime.Now > account.BannedEndDate)
            {
                return LoginType.Banned;
            }

            return LoginType.Success;
        }

        /// <summary>
        /// Login the user, this doesn't validate the user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="loginValidateResult"></param>
        /// <param name="remememberMe"></param>
        /// <returns></returns>
        public LoginType Login(string username, string password, string ip, LoginType loginValidateResult, bool remememberMe)
        {
            if (loginValidateResult == LoginType.Success)
            {
                ClearLoginFails(ip);
            }
            else if (loginValidateResult == LoginType.IncorrectPassword
                || loginValidateResult == LoginType.NoAccountFound
                || loginValidateResult == LoginType.IncorrectPassword)
            {
                AddLoginFail(ip);

                /* We just added a login fail, check if the ip is blocked now.
                This is to avoid that you get the ip is blocked message on the 6th attempt when you have been locked out in the 5th. */
                if (IsIpBlocked(ip))
                {
                    return LoginType.IpBlocked;
                }
            }

            return loginValidateResult;
        }

        /// <summary>
        /// Returns true if the account is verified by an administrator and the user verified his email
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True if the user has verified his email and is admin verified</returns>
        public bool IsUserVerified(int userId)
        {
            var account = GetAccount(userId);

            return account.IsAdminVerified && account.IsEmailVerified;
        }

        public void AddLoginToken(int userId, string selector, string token, DateTime nextYear)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var loginToken = new LoginTokens
                {
                    UserId = userId,
                    Selector = selector,
                    Expires = nextYear,
                    Token = BCrypt.Net.BCrypt.HashPassword(token)
                };

                unitOfWorkRepository.LoginTokensRepository.Add(loginToken);

                unitOfWorkRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Set the user to admin verified
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>True if the user exists</returns>
        public bool VerifyUserAdminStatus(int accountId)
        {
            var user = GetAccount(accountId);

            if (user != null)
            {
                user.IsAdminVerified = true;

                _unitOfWorkRepository.SaveChanges();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Bans the user till the given time
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="endDate"></param>
        /// <returns>True if the user exists</returns>
        public bool BanUser(int accountId, DateTime endDate)
        {
            var user = GetAccount(accountId);

            if (user != null)
            {
                user.BannedStartDate = DateTime.Now;
                user.BannedEndDate = endDate;

                _unitOfWorkRepository.SaveChanges();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Unbans the user
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>True if the user exists</returns>
        public bool UnBanUser(int accountId)
        {
            var user = GetAccount(accountId);

            if (user != null)
            {
                user.BannedStartDate = null;
                user.BannedEndDate = null;

                _unitOfWorkRepository.SaveChanges();

                return true;
            }
            return false;
        }


        /// <summary>
        /// Permanently (Max DateTime) bans the user
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>True if the user exists</returns>
        public bool BanUser(int accountId)
        {
            return BanUser(accountId, DateTime.MaxValue);
        }

        public TypeEmailVerification VerifyEmail(int accountId, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return TypeEmailVerification.NotFound;
            }

            var account = _unitOfWorkRepository.AccountRepository.GetByEmailVerificationCodeAndUserId(accountId, code);

            if (account == null)
            {
                return TypeEmailVerification.NotFound;
            }
            if (account.IsEmailVerified)
            {
                return TypeEmailVerification.AlreadyActivated;
            }

            account.IsEmailVerified = true;
            account.EmailVerificationCode = null;

            _unitOfWorkRepository.SaveChanges();

            return TypeEmailVerification.Verified;
        }

        public TypeEmailVerification VerifyEmail(int accountId)
        {
            var account = GetAccount(accountId);

            if (account == null)
            {
                return TypeEmailVerification.NotFound;
            }

            if (account.IsEmailVerified)
            {
                return TypeEmailVerification.AlreadyActivated;
            }

            account.IsEmailVerified = true;
            account.EmailVerificationCode = null;

            _unitOfWorkRepository.SaveChanges();

            return TypeEmailVerification.Verified;
        }

        public AccountSecurity ValidateSecurityFromUsername(string username, string answer)
        {
            var account = GetUserByUsername(username);

            if (account == null) { return AccountSecurity.UserNotFound; }

            if (account.Answer.Equals(answer, StringComparison.CurrentCultureIgnoreCase))
            {
                return AccountSecurity.Success;
            }

            return AccountSecurity.IncorrectSecurity; ;
        }

        public AccountSecurity ValidateSecurityFromEmail(string email, string answer)
        {
            var account = GetUserByEmail(email);

            if (account == null) { return AccountSecurity.UserNotFound; }

            if (account.Answer.Equals(answer, StringComparison.CurrentCultureIgnoreCase))
            {
                return AccountSecurity.Success;
            }

            return AccountSecurity.IncorrectSecurity; ;
        }

        public ResultModel ValidateAccount(string username, string email)
        {
            if (!IsValidUsername(username))
            {
                return new ResultModel(Localization.Languages.Business.Account.AccountService.UsernameAlreadyTaken);
            }

            if (!IsUniqueEmail(email))
            {
                return new ResultModel(Localization.Languages.Business.Account.AccountService.EmailAlreadyTaken);
            }

            return new ResultModel();
        }

        public IEnumerable<Users> GetDatabaseUsers(bool includeBanned)
        {
            return _unitOfWorkRepository.AccountRepository.GetAll(includeBanned);
        }

        public TypeEmailVerification VerifyNewEmail(int userId, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return TypeEmailVerification.NotFound;
            }

            var account = _unitOfWorkRepository.AccountRepository.GetByNewEmailVerificationCodeAndUserId(userId, code);

            if (account == null)
            {
                return TypeEmailVerification.NotFound;
            }

            if (string.IsNullOrEmpty(account.NewEmail))
            {
                return TypeEmailVerification.AlreadyActivated;
            }

            account.Email = account.NewEmail;
            account.NewEmail = null;
            account.NewEmailVerificationCode = null;

            _unitOfWorkRepository.SaveChanges();

            return TypeEmailVerification.Verified;
        }

        public Users AddAccount(string username, string password, string email, int securityQuestionId,
            string securityAnswer)
        {
            var settingsService = new SettingsService();

            const bool IS_ADMIN = false;

            bool isEmailVerificationRequired = settingsService.IsEmailVerificationRequired();

            bool isAdminVerificationRequired = settingsService.IsAdminVerificationRequired();

            string emailVerificationCode = isEmailVerificationRequired ? GetUniqueEmailVerificationCode() : null;

            var account = _unitOfWorkRepository.AccountRepository.AddAccount(username, GetHashedPassword(password),
                email,
                securityQuestionId, securityAnswer, !isEmailVerificationRequired, IS_ADMIN, !isAdminVerificationRequired, emailVerificationCode);

            _unitOfWorkRepository.SaveChanges();

            SendActivateAccountEmail(account, emailVerificationCode, isEmailVerificationRequired);

            return account;
        }

        public Users AddAdministrator(string username, string password, string email, int securityQuestionId,
            string securityAnswer)
        {
            var account = _unitOfWorkRepository.AccountRepository.AddAccount(username, GetHashedPassword(password),
                email,
                securityQuestionId, securityAnswer, false, true, true);

            _unitOfWorkRepository.SaveChanges();

            return account;
        }

        public AccountModel GetAccountModel(int id)
        {
            var account = _unitOfWorkRepository.AccountRepository.GetById(id);

            if (account != null)
            {
                return new AccountModel
                {
                    BannedEndDate = account.BannedEndDate,
                    BannedStartDate = account.BannedStartDate,
                    Email = account.Email,
                    IsAdmin = account.IsAdmin,
                    IsBanned = account.BannedEndDate != null && account.BannedEndDate > DateTime.Now,
                    IsEmailVerified = account.IsEmailVerified,
                    LastLoginDate = account.LastLoginDate,
                    RegistrationDate = account.RegistrationDate,
                    Username = account.Username,
                    SecurityAnswer = account.Answer,
                    SecurityQuestion = account.Question,
                    Id = account.Id,
                    IsAdminVerified = account.IsAdminVerified
                };
            }

            return null;
        }

        public Users GetAccount(int id)
        {
            return _unitOfWorkRepository.AccountRepository.GetById(id);
        }

        public int GetAmountActiveUsers()
        {
            return _unitOfWorkRepository.AccountRepository.GetAmountActiveUsers();
        }

        public int GetAmountUsers()
        {
            return _unitOfWorkRepository.AccountRepository.GetAmountUsers();
        }

        public int GetAmountUsers(int year, int month)
        {
            return _unitOfWorkRepository.AccountRepository.GetAmountUsers(year, month);
        }

        public Users GetUserByUsername(string username)
        {
            return _unitOfWorkRepository.AccountRepository.GetByUsername(username);
        }

        public Users GetUserByEmail(string email)
        {
            return _unitOfWorkRepository.AccountRepository.GetByEmail(email);
        }

        public void UpdateAccount(Users account)
        {
            _unitOfWorkRepository.UpdateEntity(account);

            _unitOfWorkRepository.SaveChanges();
        }

        public IEnumerable<AccountSecurityQuestionModel> GetSecurityQuestions()
        {
            return new[]
            {
                new AccountSecurityQuestionModel(0, Localization.Languages.Business.Account.AccountService.QuestionNameSchool),
                new AccountSecurityQuestionModel(0, Localization.Languages.Business.Account.AccountService.QuestionMothersName),
                new AccountSecurityQuestionModel(0, Localization.Languages.Business.Account.AccountService.QuestionBornWhere),
                new AccountSecurityQuestionModel(0, Localization.Languages.Business.Account.AccountService.QuestionPetName)
            };
        }

        public EditAccountType UpdateAccount(int userId, string email, string oldPassword, string newPassword,
            int securityQuestion, string securityAnswer, bool validatePassword)
        {
            var account = GetAccount(userId);

            var returnAccountType = EditAccountType.Success;

            var updateEmailResult = UpdateNewEmail(account, email);

            if (updateEmailResult != EditAccountType.Success) { return updateEmailResult; }

            UpdateSecurity(account, securityQuestion, securityAnswer);

            if (!UpdatePassword(account, newPassword, oldPassword, validatePassword: validatePassword))
            {
                returnAccountType = EditAccountType.IncorrectPassword;
            }

            UpdateAccount(account);

            return returnAccountType;
        }

        public IEnumerable<Users> GetUnverifiedAccounts(int amountUsers)
        {
            return _unitOfWorkRepository.AccountRepository.GetUnverifiedUsers(amountUsers);
        }

        public int GetAmountUnverifiedUsers()
        {
            return _unitOfWorkRepository.AccountRepository.GetAmountUnverifiedUsers();
        }

        public bool IsUserAdmin(int userId)
        {
            return _unitOfWorkRepository.AccountRepository.IsAdmin(userId);
        }

        public void DeleteAccount(int userId)
        {
            _unitOfWorkRepository.RatingRepository.RemoveRatingsFromUserId(userId); // circulair constraint cascade error
            _unitOfWorkRepository.AccountRepository.DeleteAccount(userId);
            _unitOfWorkRepository.SaveChanges();
        }

        /// <summary>
        /// Updates the email record (Email or NewEmail) and sends an email if the email adress has to verified by the user.
        /// </summary>
        /// <returns></returns>
        private EditAccountType UpdateNewEmail(Users account, string newEmail)
        {
            if (!IsUniqueEmail(newEmail, account.Id)) { return EditAccountType.EmailAlreadyTaken; }

            var settingsService = new SettingsService();

            if (!account.Email.Equals(newEmail, StringComparison.CurrentCultureIgnoreCase))
            {
                bool isEmailVerificationRequired = settingsService.IsEmailVerificationRequired();

                if (isEmailVerificationRequired)
                {
                    var contactService = new ContactService();

                    string verificationCode = GetUniqueNewEmailVerificationCode();

                    SetNewEmail(account, newEmail, verificationCode);

                    contactService.SendActivateEmail(account.Id, account.Username, newEmail, UrlHelpers.GetCurrentBaseUrl(), verificationCode, settingsService.GetTitle());
                }
                else
                {
                    SetNewEmail(account, newEmail, false);
                }

            }

            return EditAccountType.Success;
        }

        private void SetNewEmail(Users account, string newEmail, bool isEmailVericationRequired)
        {
            if (isEmailVericationRequired)
            {
                account.NewEmail = newEmail;
            }
            else
            {
                account.Email = newEmail;
            }
        }

        private void SetNewEmail(Users account, string newEmail, string newEmailVerificationCode)
        {
            SetNewEmail(account, newEmail, true);

            account.NewEmailVerificationCode = newEmailVerificationCode;
        }

        private string GetUniqueEmailVerificationCode()
        {
            var accountService = new AccountService();
            const int minLength = 15;
            const int maxLength = 30;

            while (true)
            {
                string verificationCode = Generator.Generator.GenerateRandomCode(minLength, maxLength);

                if (_unitOfWorkRepository.AccountRepository.IsUniqueEmailVerificationCode(verificationCode))
                {
                    return verificationCode;
                }
            }
        }

        private string GetUniqueNewEmailVerificationCode()
        {
            const int minLength = 15;
            const int maxLength = 30;

            while (true)
            {
                string verificationCode = Generator.Generator.GenerateRandomCode(minLength, maxLength);

                if (_unitOfWorkRepository.AccountRepository.IsUniqueNewEmailVerificationCode(verificationCode))
                {
                    return verificationCode;
                }
            }
        }

        private bool IsValidUsername(string username)
        {
            return _unitOfWorkRepository.AccountRepository.IsUniqueUsername(username);
        }

        private bool IsUniqueEmail(string email, int? excludeUserId = null)
        {
            return _unitOfWorkRepository.AccountRepository.IsUniqueEmail(email, excludeUserId);
        }

        private void UpdateSecurity(Users account, int securityQuestion, string securityAnswer)
        {
            account.Question = securityQuestion;

            if (!string.IsNullOrEmpty(securityAnswer))
            {
                account.Answer = securityAnswer;
            }
        }

        private bool ValidatePassword(Users account, string givenPassword, bool validatePassword)
        {
            if (!validatePassword)
            {
                return true;
            }

            return BCrypt.Net.BCrypt.Verify(givenPassword, account.Password);
        }

        /// <summary>
        /// Updates the account property with the given password
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPassword"></param>
        /// <param name="oldPassword"></param>
        /// <param name="validatePassword"></param>
        /// <returns>True if the new password has been set</returns>
        private bool UpdatePassword(Users account, string newPassword, string oldPassword, bool validatePassword)
        {
            bool returnStatus = true;

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (ValidatePassword(account, oldPassword, validatePassword: validatePassword))
                {
                    account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                }
                else
                {
                    returnStatus = false;
                }

            }
            return returnStatus;
        }

        private string GetHashedPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private void SendActivateAccountEmail(Users account, string emailVerificationCode, bool isEmailVerificationRequired)
        {
            var contactService = new ContactService();
            var settingsService = new SettingsService();

            contactService.SendActivateAccountEmail(account.Email, account.Id, account.Username,
                UrlHelpers.GetCurrentBaseUrl(), emailVerificationCode, settingsService.GetTitle(), isEmailVerificationRequired);
        }

        private void AddNewLoginFail(string ip)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                unitOfWorkRepository.LoginFailsRepository.AddOrUpdate(new LoginFails
                {
                    Attempts = 1,
                    ExpireDate = GetLoginFailExpireDate(),
                    Ip = ip
                });

                unitOfWorkRepository.SaveChanges();
            }
        }

        private bool IsIpBlocked(string ip)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var loginFail = unitOfWorkRepository.LoginFailsRepository.GetByIp(ip);

                return loginFail != null && loginFail.Attempts >= 5 && loginFail.ExpireDate > DateTime.Now;
            }
        }

        private void ClearLoginFails(string ip)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                var loginFail = GetLoginFail(ip);

                if (loginFail != null)
                {
                    unitOfWorkRepository.LoginFailsRepository.Remove(loginFail.Id);

                    unitOfWorkRepository.SaveChanges();
                }
            }
        }

        private LoginFails GetLoginFail(string ip)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.LoginFailsRepository.GetByIp(ip);
            }
        }

        private bool IsAdministrator(string username)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.AccountRepository.IsAdmin(username);
            }
        }

        public bool VerifyLoginToken(LoginTokens loginTokenItem, string token)
        {
            return loginTokenItem != null && BCrypt.Net.BCrypt.Verify(token, loginTokenItem.Token);
        }


        public void RemoveLoginToken(LoginTokens loginTokenItem)
        {
            if (loginTokenItem == null) { return; }

            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                unitOfWorkRepository.LoginTokensRepository.RemoveLogin(loginTokenItem.UserId,
                    loginTokenItem.Selector);

                unitOfWorkRepository.SaveChanges();
            }
        }

        public string GetUniqueTokenSelector()
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                while (true)
                {
                    string selector = Membership.GeneratePassword(_random.Next(5, 25), 0);

                    if (unitOfWorkRepository.LoginTokensRepository.IsUniqueSelector(selector))
                    {
                        return selector;
                    }
                }
            }
        }

        private DateTime GetLoginFailExpireDate()
        {
            return DateTime.Now.AddMinutes(15);
        }

        private void AddLoginFail(string ip)
        {
            var loginFail = GetLoginFail(ip);

            if (loginFail == null)
            {
                AddNewLoginFail(ip);
            }
            else
            {
                UpdateLoginFail(loginFail);
            }
        }

        private void UpdateLoginFail(LoginFails loginFail)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                if (DateTime.Now > loginFail.ExpireDate)
                {
                    loginFail.Attempts = 1;
                }
                else
                {
                    loginFail.Attempts++;
                }

                loginFail.ExpireDate = GetLoginFailExpireDate();

                unitOfWorkRepository.LoginFailsRepository.AddOrUpdate(loginFail);

                unitOfWorkRepository.SaveChanges();
            }
        }
    }
}