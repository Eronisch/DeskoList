using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Admin;
using Core.Business.Account;
using Core.Business.Websites;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Dashboard;

namespace Topsite.Areas.Administration.Controllers
{
    public class DashboardController : AdminController
    {
        public ActionResult Index()
        {
            var accountService = new AccountService();
            var websiteService = new WebsiteService();
            var websiteInService = new WebsiteInService();
            var websiteOutService = new WebsiteOutService();

            return View(new DashboardModel
            {
                AmountRegistrationsTotal = accountService.GetAmountUsers(),
                WebsitesTotal = websiteService.GetAmountWebsites(),
                UniqueInToday = websiteInService.GetUniqueIn(DateTime.Now),
                UniqueOutToday = websiteOutService.GetUniqueOut(DateTime.Now)
            });
        }

        public JsonResult GetUsers()
        {
            var accountService = new AccountService();

            return Json(from date in GetLastSixMonths()
                        select new
                        {
                            date = string.Format("{0}-{1}", date.Year, date.Month),
                            users = accountService.GetAmountUsers(date.Year, date.Month)
                        }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWebsites()
        {
            var websiteService = new WebsiteService();

            return Json(from date in GetLastSixMonths()
                        select new
                        {
                            date = string.Format("{0}-{1}", date.Year, date.Month),
                            websites = websiteService.GetAmountWebsites(date.Year, date.Month)
                        }, JsonRequestBehavior.AllowGet);
        }

        public string GetNews()
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    return webClient.DownloadString("http://deskolist.com/news");
                }
                catch (WebException)
                {
                    return Dashboard.NewsExternalError;
                }
            }
        }

        private IEnumerable<DateTime> GetLastSixMonths()
        {
            var dates = new Collection<DateTime>();

            for (int dateCounter = 0; 6 > dateCounter; dateCounter++)
            {
                var dateSubtractedMonths = DateTime.Now.AddMonths(-dateCounter);
                dates.Add(new DateTime(dateSubtractedMonths.Year, dateSubtractedMonths.Month, 1));
            }

            return dates;
        }

    }
}
