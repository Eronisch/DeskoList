using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Account;
using Core.Business.Contact;
using Core.Business.Settings;
using Core.Business.Websites;
using Core.Models.Account;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Account;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class AccountsController : AdminController
    {
        private readonly AccountService _accountService;
        private readonly WebsiteService _websiteService;
        private readonly Web.Account.AccountService _webAccountService;

        public AccountsController()
        {
            _accountService = new AccountService();
            _websiteService = new WebsiteService();
            _webAccountService = new Web.Account.AccountService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUsers()
        {
            return Json(from user in _accountService.GetDatabaseUsers(includeBanned: true).OrderByDescending(x => x.Id)
                        select new
                        {
                            Id = user.Id,
                            Username = user.Username,
                            Email = user.Email,
                            RegistrationDate = user.RegistrationDate.ToString(CultureInfo.InvariantCulture),
                            LastLoggedInDate = user.LastLoginDate.ToString(CultureInfo.InvariantCulture),
                            View = BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.View, ControllerContext.RequestContext.GetActionRoute("View", "Accounts", new { id = user.Id }),
                               BootstrapButtonType.Info,
                               BootstrapSize.ExtraSmall),
                            Edit =
                                BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "Accounts", new { id = user.Id }),
                                    BootstrapButtonType.Warning,
                                    BootstrapSize.ExtraSmall),
                            Delete =
                                BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Accounts", new { id = user.Id }),
                                    BootstrapButtonType.Error, BootstrapSize.ExtraSmall)
                        }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UnBan(int accountId)
        {
            if (_accountService.UnBanUser(accountId))
            {
                this.SetSuccess(Account.UserUnbannedSuccessfully);
            }
            else
            {
                this.SetError(Account.NoAccountFound);
            }

            return ToEditView(accountId);
        }

        public ActionResult ActivateAdmin(int accountId)
        {
            if (_accountService.VerifyUserAdminStatus(accountId))
            {
                SendAccountVerifiedByAdministratorEmail(accountId);

                this.SetSuccess(Account.UserAdminStatusVerifiedSuccessfully);
            }
            else
            {
                this.SetError(Account.NoAccountFound);
            }

            return ToEditView(accountId);
        }

        public ActionResult ActivateEmail(int accountId)
        {
            switch (_accountService.VerifyEmail(accountId))
            {
                case TypeEmailVerification.Verified:
                {
                    this.SetSuccess(Account.UserEmailVerified);
                    break;
                }
                case TypeEmailVerification.AlreadyActivated:
                {
                    this.SetError(Account.UserEmailAlreadyVerified);
                    break;
                }
                case TypeEmailVerification.NotFound:
                {
                    this.SetError(Account.NoAccountFound);
                    break;
                }
            }

            return ToEditView(accountId);
        }

        [HttpPost]
        public ActionResult Ban(AccountBanModel accountBanModel)
        {
            if ((!ModelState.IsValid) || (!accountBanModel.IsPermanent && !accountBanModel.EndDate.HasValue)) { this.SetError(Account.InvalidEndDate); return ToEditView(accountBanModel.UserId); }

            bool success = accountBanModel.IsPermanent
                ? _accountService.BanUser(accountBanModel.UserId)
                : _accountService.BanUser(accountBanModel.UserId, accountBanModel.EndDate.Value);

            if (success)
            {
                this.SetSuccess(Account.UserBannedSuccessfully);
            }
            else
            {
                this.SetError(Account.NoAccountFound);
            }

            return ToEditView(accountBanModel.UserId);
        }

        private ActionResult ToEditView(int userId)
        {
            return RedirectToAction("Edit", new { id = userId });
        }


        public ActionResult View(int id)
        {
            var account = _accountService.GetAccountModel(id);

            if (account == null) { this.SetError(Account.NoAccountFound); return RedirectToAction("Index"); }

            return View(new AccountViewBundleModel
            {
                Account = account,
                Websites = _websiteService.GetWebsitesFromUser(account.Id)
            });
        }

        public ActionResult Edit(int id)
        {
            var account = _accountService.GetAccountModel(id);

            if (account == null) { this.SetError(Account.NoAccountFound); return RedirectToAction("Index"); }

            var accountModel = new EditAccountAdminModel(account.Id, account.Email, account.SecurityQuestion,
                account.SecurityAnswer, _webAccountService.GetSecurityQuestions(account.SecurityQuestion), account.IsBanned, account.IsAdminVerified, account.IsEmailVerified);

            return View(accountModel);
        }

        [HttpPost]
        public ActionResult Edit(EditAccountAdminModel editAccountAdminModel)
        {
            if (ModelState.IsValid)
            {
                var updateAccountResult = _accountService.UpdateAccount(editAccountAdminModel.AccountId, editAccountAdminModel.Email,
                     editAccountAdminModel.OldPassword, editAccountAdminModel.NewPassword, editAccountAdminModel.QuestionId,
                     editAccountAdminModel.Answer, validatePassword: false);

                switch (updateAccountResult)
                {
                    case EditAccountType.EmailAlreadyTaken:
                        {
                            this.SetError(Account.EmailAlreadyExists);
                            break;
                        }
                    case EditAccountType.Success:
                        {

                            this.SetSuccess(Account.AccountUpdatedSuccesfully);
                            return RedirectToAction("Index");
                        }
                }
            }

            editAccountAdminModel.Questions = _webAccountService.GetSecurityQuestions(editAccountAdminModel.QuestionId);

            return View(editAccountAdminModel);
        }

        public string GetUsername(int id)
        {
            var user = _accountService.GetAccount(id);

            if (user == null) { return string.Empty; }

            return user.Username;
        }

        private void SendAccountVerifiedByAdministratorEmail(int accountId)
        {
            var account = _accountService.GetAccount(accountId);

            var contactService = new ContactService();
            var settingsService = new SettingsService();

            contactService.SendAccountVerifiedByAnAdministratorEmail(account.Email, account.Username, settingsService.GetTitle());
        }
    }
}
