using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Business.Account;
using Core.Business.Connection;
using Core.Business.Date;
using Core.Models.Install;
using DatabaseXML;
using Localization.Languages.Controllers;
using Topsite.Models;
using Web.AutoInstaller;
using Web.Messages;

namespace Topsite.Controllers
{
    public class AutoInstallerController : Controller
    {
        private readonly InstallerService _installerService;
        private readonly Web.Account.AccountService _webAccountService;

        public AutoInstallerController()
        {
            _installerService = new InstallerService();
            new AccountService();
            _webAccountService = new Web.Account.AccountService();
        }

        public ActionResult Index()
        {
            if (!IsInstalled())
            {
                return View(new InstallModel
                {
                    SecurityQuestions = _webAccountService.GetSecurityQuestions(),
                    Languages = GetLanguages(),
                    Timezones = GetTimeZones(TimeZoneInfo.Local.Id), // use as default the systems timezone,
                    LanguageId = 1,
                    TimezoneId = TimeZoneInfo.Local.Id,
                    SettingsInAndOutCronJob = "0 0 12 ? * MON *",
                    SettingsCreateUserWebsiteThumbnailsCronjob = "0 0 0 ? * MON-FRI *",
                    SettingsEmailUserStatisticsCronjob = "0 0 0 ? * MON *",
                    SettingsMonitorUserWebsitesCronjob = "0 0 0/1 1/1 * ? *",
                    SettingsUpdateCronjob = "0 0 0 ? * MON *"
                });
            }

            return Content(AutoInstaller.UnAuthorized);
        }

        [HttpPost]
        public ActionResult Index(InstallModel installModel)
        {
            if (!IsInstalled())
            {
                if (ModelState.IsValid)
                {
                    var resultStatus = _installerService.Install(installModel.DatabaseHost, installModel.DatabaseName, installModel.DatabaseUsername, installModel.DatabasePassword,
                        installModel.NoReplyHost, (int)installModel.NoReplyPort, installModel.NoReplySecureConnection, installModel.NoReplyUsername, installModel.NoReplyPassword,
                        installModel.ReplyHost, (int)installModel.ReplyPort, installModel.ReplySecureConnection, installModel.ReplyUsername, installModel.ReplyPassword,
                        installModel.SettingsLongTitle, installModel.SettingsShortTitle, installModel.SettingsDescription, installModel.SettingsKeywords, (int)installModel.SettingsAmountWebsites, installModel.SettingsUrl,
                        installModel.SettingsLongTitle, installModel.LanguageId, installModel.TimezoneId, installModel.CaptchaSiteKey, installModel.CaptchaSecretKey, installModel.SettingsIsUserServerMonitoringEnabled,
                        installModel.SettingsIsEmailVerificationRequired, installModel.SettingsIsAutoUpdateEnabled, installModel.SettingsIsEmailingUserStatisticsEnabled, installModel.SettingsIsCreateUserWebsiteThumbnailsEnabled,
                        installModel.SettingsIsResetInAndOutsEnabled, installModel.IsUpdateWhenIncorrectVersionEnabled, installModel.SettingsIsAdminVerificationRequired, installModel.SettingsEmailUserStatisticsCronjob,
                        installModel.SettingsInAndOutCronJob, installModel.SettingsUpdateCronjob, installModel.SettingsCreateUserWebsiteThumbnailsCronjob, installModel.SettingsMonitorUserWebsitesCronjob,
                        installModel.Username, installModel.Password, installModel.Email, installModel.SecurityQuestion, installModel.SecurityAnswer);

                    if (resultStatus == InstallType.Success)
                    {
                        this.SetSuccess(Install.SuccessfullyInstalled);

                        return RedirectToAction("Index", "Home");
                    }

                    HandleErrorMessage(resultStatus);
                }

                installModel.SecurityQuestions = _webAccountService.GetSecurityQuestions();
                installModel.Languages = GetLanguages(installModel.LanguageId);
                installModel.Timezones = GetTimeZones(installModel.TimezoneId);

                return View(installModel);
            }

            return Content(AutoInstaller.UnAuthorized);
        }

        public bool ValidateDatabaseConnection(string host, string database, string username, string password)
        {
            return ConnectionTestService.IsValidDatabaseConnection(host, database, username, password);
        }

        public JsonResult ValidateEmailConnection(string noReplyHost, string noReplyPort, string replyHost, string replyPort)
        {
            bool isNoReplyValid = ConnectionTestService.IsValidEmailConnection(noReplyHost, noReplyPort);
            bool isReplyValid = ConnectionTestService.IsValidEmailConnection(replyHost, replyPort);

            return Json(new { isNoReplyValid, isReplyValid }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> GetLanguages(int id = 1)
        {
            const int ENGLISH_ID = 1;
          
            return new[]
            {
                new SelectListItem
                {
                    Selected = id == ENGLISH_ID,
                    Text = Install.English,
                    Value = ENGLISH_ID.ToString()
                }
            };
        }

        private IEnumerable<SelectListItem> GetTimeZones(string timezoneId)
        {
            return DateHelper.GetTimeZones().Select(tz => new SelectListItem
            {
                Text = tz.DisplayName,
                Value = tz.Id,
                Selected = timezoneId == tz.Id
            });
        }

        private void HandleErrorMessage(InstallType installType)
        {
            string errorMessage;

            switch (installType)
            {
                case InstallType.ExecuteDatabaseScriptFailed:
                    {
                        errorMessage = Install.CreatingDatabaseFailed;
                        break;
                    }
                case InstallType.AddingAdministratorFailed:
                    {
                        errorMessage = Install.AddingAdministratorFailed;
                        break;
                    }
                case InstallType.AddingWidgetsFailed:
                    {
                        errorMessage = Install.AddingWidgetsFailed;
                        break;
                    }
                case InstallType.AddingSettingsFailed:
                    {
                        errorMessage = Install.AddingSettingsFailed;
                        break;
                    }
                case InstallType.AddingStartUpDllsFailed:
                    {
                        errorMessage = Install.AddingStartUpDllsFailed;
                        break;
                    }
                case InstallType.AddingThemesFailed:
                    {
                        errorMessage = Install.AddingThemesFailed;
                        break;
                    }
                case InstallType.UpdateXmlDatabaseFailed:
                    {
                        errorMessage = Install.UpdateXmlDatabaseFailed;
                        break;
                    }
                case InstallType.AddingPagesFailed:
                    {
                        errorMessage = Install.UpdateXmlDatabaseFailed;
                        break;
                    }
                case InstallType.AddingEmailAccountsFailed:
                    {
                        errorMessage = Install.AddingEmailAccountsFailed;
                        break;
                    }
                case InstallType.AddingEmailTemplatesFailed:
                    {
                        errorMessage = Install.AddingEmailTemplatesFailed;
                        break;
                    }
                case InstallType.AddingLanguagesFailed:
                    {
                        errorMessage = Install.AddingLanguagesFailed;
                        break;
                    }
                default:
                    {
                        errorMessage = Install.SomethingFailed;
                        break;
                    }
            }

            this.SetError(errorMessage);
        }

        private bool IsInstalled()
        {
            return LocalDatabaseSettingsService.Manager.IsInstalled;
        }
    }
}
