using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Core.Business.Captcha;
using Core.Business.Contact;
using Core.Business.Plugin;
using Core.Business.Settings;
using Core.Business.Websites;
using Core.Models;
using Database.Entities;
using Localization.Languages.Controllers;
using Topsite.Models;
using Web.Account;
using Web.Ip;
using Web.Messages;
using AccountService = Core.Business.Account.AccountService;

namespace Topsite.Controllers
{
    public class JoinController : Controller
    {

        private readonly AccountService _accountService;
        private readonly Web.Category.CategoryService _categoryService;
        private readonly ContactService _contactService;
        private readonly WebsiteService _websiteService;
        private readonly CaptchaService _captchaService;
        private readonly SettingsService _settingsService;
        private readonly Web.Account.AccountService _webAccountService;

        public JoinController()
        {
            _captchaService = new CaptchaService();
            _websiteService = new WebsiteService();
            _contactService = new ContactService();
            _accountService = new AccountService();
            _categoryService = new Web.Category.CategoryService();
            _settingsService = new SettingsService();
            _webAccountService = new Web.Account.AccountService();
        }

        public ActionResult Index()
        {
            if (LoginHelper.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(GetBundleModel(new JoinusModel()));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(JoinusModel joinusModel)
        {
            if (ModelState.IsValid)
            {
                var validationErrorResult = ValidateRegister(joinusModel.Username, joinusModel.Account.Email, joinusModel.Website.BannerFile, joinusModel.Website.BannerUrl, joinusModel.Website.Url, joinusModel.Website.ServerIp, joinusModel.Website.ServerPort, Request.Form["g-recaptcha-response"], !joinusModel.Website.UseBannerUrl);

                if (!validationErrorResult.IsSuccess) { return GoToJoinView(validationErrorResult.ErrorMessage, GetBundleModel(joinusModel)); }

                var addWebsitePluginResult = PluginHookActivateService.FireHook(PluginHooks.AddWebsite, null,
                        joinusModel.Website.Title,
                        joinusModel.Website.Description, joinusModel.Website.Url, joinusModel.Website.BannerUrl,
                        joinusModel.Website.Keywords, joinusModel.Website.Category, joinusModel.Website.BannerFile,
                        joinusModel.Website.ServerIp, joinusModel.Website.ServerPort, !joinusModel.Website.UseBannerUrl);

                if (!addWebsitePluginResult.IsSuccess) { return GoToJoinView(addWebsitePluginResult.ErrorMessage, GetBundleModel(joinusModel)); }

                var addUserPluginResult = PluginHookActivateService.FireHook(PluginHooks.AddUser, Request,
                    joinusModel.Username,
                    joinusModel.Account.Email,
                    joinusModel.Account.QuestionId, joinusModel.Account.Answer);

                if (!addUserPluginResult.IsSuccess) { return GoToJoinView(addUserPluginResult.ErrorMessage, GetBundleModel(joinusModel)); }

                var user = _accountService.AddAccount(joinusModel.Username, joinusModel.Password,joinusModel.Account.Email, joinusModel.Account.QuestionId, joinusModel.Account.Answer);

                var website = _websiteService.AddWebsite(user.Id, joinusModel.Username,
                    joinusModel.Website.Title,
                    joinusModel.Website.Description,
                    joinusModel.Website.Url,
                    joinusModel.Website.BannerUrl,
                    joinusModel.Website.Keywords,
                    joinusModel.Website.Category,
                    joinusModel.Website.BannerFile.InputStream,
                    joinusModel.Website.BannerFile.FileName,
                    joinusModel.Website.ServerIp,
                    joinusModel.Website.ServerPort,
                    !joinusModel.Website.UseBannerUrl);

                SetUserSuccessRegistrationMessage(user);

                if (_accountService.IsUserVerified(user.Id))
                {
                    new LoginService().Login(user.Username, user.Password, IpHelper.GetIpFromCurrentRequest(),  false);
                    return RedirectToAction("Code", "Website", new { id = website.Id });
                }

                return RedirectToAction("Index", "Home");
            }

            return View(GetBundleModel(joinusModel));
        }

        #region Private Methods

        private ActionResult GoToJoinView(string errorMessage, BundleJoinModel bundleJoinUsModel)
        {
            this.SetError(errorMessage);

            return View(bundleJoinUsModel);
        }

        private void SetUserSuccessRegistrationMessage(Users user)
        {
            if (!user.IsAdminVerified && !user.IsEmailVerified)
            {
                this.SetSuccess(Join.RegisteredSuccesfullyBothVerificationRequired);
            }
            else if (!user.IsAdminVerified)
            {
                this.SetSuccess(Join.RegisteredSuccesfullyAdminVerificationRequired);
            }
            else if (!user.IsEmailVerified)
            {
                this.SetSuccess(Join.RegisteredSuccesfullyEmailVerificationRequired);
            }
            else
            {
                this.SetSuccess(Join.RegisteredSuccesfully);
            }
        }

        private BundleJoinModel GetBundleModel(JoinusModel joinusModel)
        {
            var bundleModel = new BundleJoinModel
            {
                Categories = _categoryService.GetListItemCategories(joinusModel.Website.Category),
                JoinusModel = joinusModel,
                IsPingEnabled = _settingsService.IsUserServerMonitoringEnabled()
            };

            bundleModel.JoinusModel.CaptchaKey = ConfigurationManager.AppSettings["RecaptchaKey"];

            bundleModel.JoinusModel.Account.Questions =
                _webAccountService.GetSecurityQuestions(bundleModel.JoinusModel.Account.QuestionId);

            return bundleModel;
        }

        private ResultModel ValidateRegister(string username, string email, HttpPostedFileBase banner, string bannerUrl, string websiteUrl, string serverIp, int? serverPort, string captchaResponse, bool useBannerFile)
        {
            // Validate captcha
            var validateCaptchaResult = _captchaService.ValidateCaptcha(captchaResponse, IpHelper.GetIpFromCurrentRequest());

            if (!validateCaptchaResult.IsSuccess) { return validateCaptchaResult; }

            // Validate account
            var validateAccountResult = _accountService.ValidateAccount(username, email);

            if (!validateAccountResult.IsSuccess) { return new ResultModel(validateAccountResult.ErrorMessage); }

            // Validate website
            var validateWebsiteResult = _websiteService.ValidateWebsite(banner?.InputStream, banner?.FileName, bannerUrl, websiteUrl, serverIp, serverPort, useBannerFile, true);

            if (!validateWebsiteResult.IsSuccess) { return new ResultModel(validateWebsiteResult.ErrorMessage); }

            // Success
            return new ResultModel();
        }

        #endregion
    }
}