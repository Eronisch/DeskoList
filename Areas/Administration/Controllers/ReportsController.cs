using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Reports;
using Localization.Languages.Admin.Controllers;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class ReportsController : AdminController
    {
        private readonly ReportsService _reportsService;

        public ReportsController()
        {
            _reportsService = new ReportsService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetReports()
        {
            return Json(from report in _reportsService.GetReports().OrderByDescending(x => x.Id)
                        select new
                        {
                            Id = report.Id,
                            Reason = report.Message,
                            SenderIp = report.SenderIP,
                            Date = report.Date.ToShortDateString(),
                            ViewWebsite = BootstrapHelper.GetLinkButton(Reports.Info, "/Administration/Websites/View/" + report.WebsiteId,
                           BootstrapButtonType.Info,
                           BootstrapSize.ExtraSmall),
                            ViewReport = BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.View, ControllerContext.RequestContext.GetActionRoute("View", "Reports", new { id = report.Id }),
                        BootstrapButtonType.Success,
                        BootstrapSize.ExtraSmall),
                            DeleteReport = BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Reports", new { id = report.Id }),
                              BootstrapButtonType.Error,
                              BootstrapSize.ExtraSmall, new Dictionary<string, string>
                        {
                            {"id", "deleteReport"}
                        }),
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(int id)
        {
            var report = _reportsService.GetReport(id);

            if (report == null) { this.SetError(Reports.ReportNotFound); return RedirectToAction("Index"); }

            return View(report);
        }

        public ActionResult Delete(int id)
        {
            var report = _reportsService.GetReport(id);

            if (report != null)
            {
                _reportsService.RemoveReport(id);

                this.SetSuccess(Reports.ReportSuccessfullyRemoved);
            }
            else
            {
                this.SetError(Reports.ReportNotFound);
            }

            return RedirectToAction("Index");
        }
    }
}
