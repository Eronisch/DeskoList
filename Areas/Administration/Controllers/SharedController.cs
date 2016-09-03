using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Account;
using Core.Business.Date;
using Core.Business.Reports;
using Core.Business.Widgets;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Account;
using Topsite.Areas.Administration.Models.Report;

namespace Topsite.Areas.Administration.Controllers
{
    public class SharedController : AdminController
    {
        public ActionResult GetUnverifiedUsers()
        {
            var accountService = new AccountService();

            int amountUnverifiedUsers = accountService.GetAmountUnverifiedUsers();
            var unverifiedUsers = accountService.GetUnverifiedAccounts(5);

            return View("_Users", new BundleUnverifiedUsersModel
            {
                AmountUnverifiedUsers = amountUnverifiedUsers,
                UnverifiedUsers = unverifiedUsers.Select(account => new UnverifiedUserModel
                {
                    Username = account.Username,
                    TimeAgo = DateHelper.GetNiceDate(account.RegistrationDate),
                    UserId = account.Id,
                    WebsiteTitle = account.Websites.Any() ? account.Websites.First().Title : Shared.NoWebsites
                })
            });
        }
        
        public ActionResult GetUnsolvedReports()
        {
            var reportService = new ReportsService();

            int amountReports = reportService.GetAmountReports();
            var openReports = reportService.GetReports(5);

            return View("_Reports", new ReportBundleModel
            {
                AmountReports = amountReports,
                Reports = openReports.Select(report => new ReportModel
                {
                  Reason = report.Message,
                  ReportId = report.Id,
                  TimeAgo = DateHelper.GetNiceDate(report.Date),
                  WebsiteTitle = report.Websites.Title
                })
            });
        }
        
        public PartialViewResult WidgetNavigation()
        {
            var widgetService = new WidgetService();

            return PartialView("_WidgetNavigation", widgetService.GetNavigation());
        }
    }
}
