using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using Database;
using Database.Entities;
using Web.Cookies;

namespace Web.Account
{
    public class LoginService
    {
        private const string LoginCookieName = "LoginCookie";
        private const string LoginCookieSelector = "Selector";
        private const string LoginCookieToken = "Token";

        private readonly Random _random;
        private readonly Core.Business.Account.AccountService _accountService;

        public LoginService()
        {
            _accountService = new Core.Business.Account.AccountService();
            _random = new Random();
        }

        /// <summary>
        /// Logs the user out
        /// Removes login tokens, cookies and session object from the user
        /// </summary>
        public void LogOut()
        {
            if (LoginHelper.IsLoggedIn())
            {
                var cookie = CookieService.GetCookieValues(LoginCookieName);

                if (cookie == null) { return; }

                string selector = cookie[LoginCookieSelector];

                if (!string.IsNullOrEmpty(selector))
                {
                    _accountService.RemoveLoginToken(GetTokenFromDatabase(selector));
                }

                HttpContext.Current.Session.Remove(LoginHelper.GetSessionUserIdKey());
                HttpContext.Current.Session.Remove(LoginHelper.GetSessionUsernameKey());
            }
        }

        /// <summary>
        /// Compares the cookie login tokens and database tokens to login the user
        /// Removes the old login token from the user (not all the login tokens, just the previous one the user used)
        /// </summary>
        public void AutoLogin()
        {
            if (LoginHelper.IsLoggedIn()) { return; }

            var loginCookie = CookieService.GetCookieValues(LoginCookieName);

            if (loginCookie == null) return;

            string cookieSelector = loginCookie[LoginCookieSelector];
            string cookieToken = loginCookie[LoginCookieToken];

            var loginTokenItem = GetTokenFromDatabase(cookieSelector);

            if (_accountService.VerifyLoginToken(loginTokenItem, cookieToken))
            {
                string selector = _accountService.GetUniqueTokenSelector();
                string token = GetToken();

                DateTime rememberDate = DateTime.Now.AddYears(1);

                _accountService.AddLoginToken(loginTokenItem.Users.Id, selector, token, rememberDate);

                AddLoginCookie(selector, token, rememberDate);

                AddLoginSession(loginTokenItem.UserId, loginTokenItem.Users.Username);

                _accountService.RemoveLoginToken(loginTokenItem);
            }
            else
            {
                CookieService.RemoveCookie(LoginCookieName);
            }
        }

        private string GetToken()
        {
            return Membership.GeneratePassword(_random.Next(10, 25), 0);
        }

        private LoginTokens GetTokenFromDatabase(string selector)
        {
            using (var unitOfWorkRepository = new UnitOfWorkRepository())
            {
                return unitOfWorkRepository.LoginTokensRepository.Get(selector, DateTime.Now);
            }
        }

        private void AddLoginSession(int userId, string username)
        {
            HttpContext.Current.Session[LoginHelper.GetSessionUsernameKey()] = username;
            HttpContext.Current.Session[LoginHelper.GetSessionUserIdKey()] = userId;
        }

        private void AddLoginCookie(string selector, string token, DateTime expireDate)
        {
            var cookieValues = new NameValueCollection { { LoginCookieSelector, selector }, { LoginCookieToken, token } };

            CookieService.SetCookie(LoginCookieName, cookieValues, expireDate);
        }


        public void Login(string username, string password, string ip, bool rememberBe)
        {
            var account = _accountService.GetUserByUsername(username);

            string selector = _accountService.GetUniqueTokenSelector();
            string token = GetToken();

            DateTime rememberDate = DateTime.Now.AddYears(1);

            _accountService.AddLoginToken(account.Id, selector, token, rememberDate);

            AddLoginCookie(selector, token, rememberDate);

            AddLoginSession(account.Id, account.Username);
        }
    }
}
