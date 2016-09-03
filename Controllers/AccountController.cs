using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Core.Business.Account;
using Core.Business.Plugin;
using Core.Business.Settings;
using Core.Models.Account;
using Localization.Languages.Controllers;
using Topsite.Action_Filters;
using Topsite.Models;
using Web.Account;
using Web.Ip;
using Web.Messages;
using AccountService = Core.Business.Account.AccountService;

namespace Topsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly Web.Account.AccountService _webAccountService;

        public AccountController()
        {
            _accountService = new AccountService();
            _webAccountService = new Web.Account.AccountService();
        }

        #region Activate Account

        [HttpGet]
        public ActionResult Activate(int accountId, string code)
        {
            var verificationResult = _accountService.VerifyEmail(accountId, code);

            switch (verificationResult)
            {
                case TypeEmailVerification.Verified:
                    this.SetSuccess(Account.AccountVerified);
                    break;

                case TypeEmailVerification.AlreadyActivated:
                    this.SetError(Account.AlreadyActivated);
                    break;

                case TypeEmailVerification.NotFound:
                    this.SetError(Account.NoAccountFoundCode);
                    break;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ActivateNewEmail(int accountId, string code)
        {
            var verificationResult = _accountService.VerifyNewEmail(accountId, code);

            switch (verificationResult)
            {
                case TypeEmailVerification.Verified:
                    this.SetSuccess(Account.NewEmailActivated);
                    break;

                case TypeEmailVerification.AlreadyActivated:
                    this.SetError(Account.AlreadyActivated);
                    break;

                case TypeEmailVerification.NotFound:
                    this.SetError(Account.NoAccountFoundCode);
                    break;
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Login

        public ActionResult Login(string returnUrl)
        {
            return View(new LoginModel(returnUrl));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            var loginService = new LoginService();

            if (!ModelState.IsValid) { return View(loginModel); }

            var loginValidateResult = _accountService.ValidateLogin(loginModel.Username, loginModel.Password,
                IpHelper.GetIpFromCurrentRequest());

            if (loginValidateResult != LoginType.Success) { return IncorrectGoToLogin(loginModel, loginValidateResult); }

            var pluginValidateResult = PluginHookActivateService.FireHook(PluginHooks.Login, loginModel.Username);

            if (!pluginValidateResult.IsSuccess) { return IncorrectGoToLogin(loginModel, pluginValidateResult.ErrorMessage); }

            loginService.Login(loginModel.Username, loginModel.Password, IpHelper.GetIpFromCurrentRequest(), loginModel.RememberMe);

            return RedirectAway(loginModel.ReturnUrl);
        }

        #endregion

        #region Edit Account

        [LoginRequiredActionFilter]
        public ActionResult Edit()
        {
            var account = _accountService.GetAccountModel(LoginHelper.GetUserId());
            var settingsService = new SettingsService();

            var accountModel = new EditAccountModel(settingsService.IsEmailVerificationRequired())
            {
                Email = account.Email,
                QuestionId = account.SecurityQuestion,
                Answer = account.SecurityAnswer,
                Questions = _webAccountService.GetSecurityQuestions(account.SecurityQuestion)
            };

            return View(accountModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoginRequiredActionFilter]
        public ActionResult Edit(EditAccountModel editAccountModel)
        {
            if (ModelState.IsValid)
            {
                var pluginValidateResult = PluginHookActivateService.FireHook(PluginHooks.EditUser, editAccountModel.Email, LoginHelper.GetUsername(), editAccountModel.QuestionId, editAccountModel.Answer);

                if (!pluginValidateResult.IsSuccess)
                {
                    this.SetError(pluginValidateResult.ErrorMessage);
                    return RedirectToAction("Index", "Cp");
                }

                var updateAccountResult = _accountService.UpdateAccount(LoginHelper.GetUserId(), editAccountModel.Email,
                     editAccountModel.OldPassword, editAccountModel.NewPassword, editAccountModel.QuestionId,
                     editAccountModel.Answer, validatePassword: true);

                switch (updateAccountResult)
                {
                    case EditAccountType.IncorrectPassword:
                        {
                            this.SetError(Account.IncorrectOldPassword);
                            break;
                        }
                    case EditAccountType.EmailAlreadyTaken:
                        {
                            this.SetError(Account.EmailAlreadyUsed);
                            break;
                        }
                    case EditAccountType.Success:
                        {
                            this.SetSuccess(Account.AccountUpdated);
                            return RedirectToAction("Index", "cp");
                        }
                }
            }

            editAccountModel.Questions = _webAccountService.GetSecurityQuestions(editAccountModel.QuestionId);

            return View(editAccountModel);
        }

        #endregion

        #region Reset

        public ActionResult ResetStep1()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetStep1(ResetModel resetModel)
        {
            if (ModelState.IsValid)
            {
                var accountService = new AccountService();

                if (accountService.GetUserByUsername(resetModel.Username) == null)
                {
                    this.SetError(Account.NoAccountFoundUsername);
                }
                else
                {
                    return RedirectToAction("ResetStep2", new { username = resetModel.Username });
                }
            }

            return View(resetModel);
        }

        public ActionResult ResetStep2(string username)
        {
            var accountService = new AccountService();
            var webAccountService = new Web.Account.AccountService();

            var account = accountService.GetUserByUsername(username);

            if (account == null) { return RedirectToAction("Index", "Home"); }

            return View(new ResetStepTwoModel
            {
                QuestionId = account.Question,
                Username = account.Username,
                Question = webAccountService.GetSecurityQuestions().First(x => x.Value == account.Question.ToString(CultureInfo.InvariantCulture)).Text
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetStep2(ResetStepTwoModel resetStepTwoModel)
        {
            var accountService = new AccountService();
            var webAccountService = new Web.Account.AccountService();
            var resetService = new ResetService();

            if (ModelState.IsValid)
            {
                var validationResult = accountService.ValidateSecurityFromUsername(resetStepTwoModel.Username, resetStepTwoModel.Answer);

                switch (validationResult)
                {
                    case AccountSecurity.UserNotFound:
                        {
                            this.SetError(Account.NoAccountFoundUsername);
                            break;
                        }
                    case AccountSecurity.IncorrectSecurity:
                        {
                            this.SetError(Account.IncorrectSecurity);
                            break;
                        }
                    case AccountSecurity.Success:
                        {
                            resetService.SendPasswordResetEmail(resetStepTwoModel.Username);
                            this.SetSuccess(Account.PasswordResetSuccessfully);
                            return RedirectToAction("Index", "Home");
                        }
                }
            }

            var account = _accountService.GetUserByUsername(resetStepTwoModel.Username);

            resetStepTwoModel.QuestionId = resetStepTwoModel.QuestionId;
            resetStepTwoModel.Question = webAccountService.GetSecurityQuestions().First(x => int.Parse(x.Value) == account.Question).Text;

            return View(resetStepTwoModel);
        }

        #endregion

        public ActionResult RequestStep1()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestStep1(RequestModel requestModel)
        {
            if (ModelState.IsValid)
            {
                var account = _accountService.GetUserByEmail(requestModel.Email);

                if (account == null)
                {
                    this.SetError(Account.NoAccountFoundEmail);
                }
                else
                {
                    return RedirectToAction("RequestStep2", new { email = requestModel.Email });
                }
            }

            return View(requestModel);
        }

        public ActionResult RequestStep2(string email)
        {
            var accountService = new AccountService();

            var account = accountService.GetUserByEmail(email);

            if (account == null) { return RedirectToAction("Index", "Home"); }

            return View(new RequestStepTwoModel
            {
                QuestionId = account.Question,
                Email = email,
                Question = _webAccountService.GetSecurityQuestions().First(x => x.Value == account.Question.ToString(CultureInfo.InvariantCulture)).Text
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestStep2(RequestStepTwoModel requestStepTwoModel)
        {
            var accountService = new AccountService();
            var requestService = new RequestService();

            if (ModelState.IsValid)
            {
                var validationResult = accountService.ValidateSecurityFromEmail(requestStepTwoModel.Email, requestStepTwoModel.Answer);

                switch (validationResult)
                {
                    case AccountSecurity.UserNotFound:
                        {
                            this.SetError(Account.NoAccountFoundUsername);
                            break;
                        }
                    case AccountSecurity.IncorrectSecurity:
                        {
                            this.SetError(Account.IncorrectSecurity);
                            break;
                        }
                    case AccountSecurity.Success:
                        {
                            requestService.SendUserNameForgotEmail(requestStepTwoModel.Email);
                            this.SetSuccess(Account.UsernameSuccessfullySend);
                            return RedirectToAction("Index", "Home");
                        }
                }
            }

            var account = _accountService.GetUserByEmail(requestStepTwoModel.Email);

            requestStepTwoModel.QuestionId = account.Question;
            requestStepTwoModel.Question = _webAccountService.GetSecurityQuestions().First(x => int.Parse(x.Value) == account.Question).Text;

            return View(requestStepTwoModel);
        }

        #region Logout

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Logout()
        {
            new LoginService().LogOut();

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Private Methods

        private ActionResult IncorrectGoToLogin(LoginModel loginModel, LoginType loginType)
        {
            DisplayLoginError(loginType);

            return View(loginModel);
        }

        private ActionResult IncorrectGoToLogin(LoginModel loginModel, string errorMessage)
        {
            this.SetError(errorMessage);

            return View(loginModel);
        }

        private void DisplayLoginError(LoginType loginType)
        {
            switch (loginType)
            {
                case LoginType.Banned:
                    {
                        this.SetError(Account.AccountBanned);
                        break;
                    }
                case LoginType.IncorrectPassword:
                    {
                        this.SetError(Account.IncorrectPassword);
                        break;
                    }
                case LoginType.NoAccountFound:
                    {
                        this.SetError(Account.NoAccountFoundUsername);
                        break;
                    }
                case LoginType.NotVerified:
                    {
                        this.SetError(Account.NotVerified);
                        break;
                    }
                case LoginType.IpBlocked:
                    {
                        this.SetError(Account.IpBlocked);
                        break;
                    }
            }
        }

        private ActionResult RedirectAway(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
