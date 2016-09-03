using System.Web.Mvc;
using Core.Business.Plugin;
using Core.Models.Account;
using Localization.Languages.Admin.Controllers;
using Topsite.Models;
using Web.Account;
using Web.Ip;
using Web.Messages;
using AccountService = Core.Business.Account.AccountService;

namespace Topsite.Areas.Administration.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccountService _accountService;
        private readonly LoginService _loginService;

        public HomeController()
        {
            _accountService = new AccountService();
            _loginService = new LoginService();
        }

        public ActionResult Index()
        {
            if (LoginHelper.IsLoggedIn() && _accountService.IsUserAdmin(LoginHelper.GetUserId()))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel loginModel)
        {
            if (!ModelState.IsValid) { return View(loginModel); }

            var loginResult = _accountService.ValidateAdminLogin(loginModel.Username, loginModel.Password,
                    IpHelper.GetIpFromCurrentRequest());

            if (loginResult != LoginType.Success) { return IncorrectGoToLogin(loginModel, loginResult); }

            var pluginsResult = PluginHookActivateService.FireHook(PluginHooks.Login, loginModel.Username,
                loginModel.Password);

            if (!pluginsResult.IsSuccess) { return IncorrectGoToLogin(loginModel, pluginsResult.ErrorMessage); }

            _loginService.Login(loginModel.Username, loginModel.Password, IpHelper.GetIpFromCurrentRequest(), loginModel.RememberMe);

            return RedirectToAction("Index", "Dashboard");
        }

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
                        this.SetError(Home.AccountBanned);
                        break;
                    }
                case LoginType.IncorrectPassword:
                    {
                        this.SetError(Home.IncorrectPassword);
                        break;
                    }
                case LoginType.NoAccountFound:
                    {
                        this.SetError(Home.NoAccountFound);
                        break;
                    }
                case LoginType.NotVerified:
                    {
                        this.SetError(Home.NotVerified);
                        break;
                    }
                case LoginType.NoPermission:
                    {
                        this.SetError(Home.NotAdmin);
                        break;
                    }
                case LoginType.IpBlocked:
                    {
                        this.SetError(Home.IpBlocked);
                        break;
                    }
            }
        }

        #endregion
    }
}