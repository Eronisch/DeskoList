using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Elmah;
using Web.Messages;

namespace Topsite.Areas.Administration.Controllers
{
    public class ElmahController : AdminController
    {
        private readonly ElmahService _elmahService;

        public ElmahController()
        {
            _elmahService = new ElmahService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult View(Guid id)
        {
            return View(_elmahService.GetById(id));
        }

        public ActionResult Clean()
        {
            _elmahService.CleanErrors();

            this.SetSuccess(Localization.Languages.Admin.Controllers.Elmah.ErrorsCleaned);

            return RedirectToAction("Index", "Elmah");
        }

        public JsonResult GetErrors()
        {
            return Json(from error in _elmahService.GetAll()
                        select new
                        {
                            Type = error.Type,
                            Message = error.Message,
                            Sequence = error.Sequence,
                            StatusCode = error.StatusCode,
                            Date = error.TimeUtc.ToLocalTime().ToString(CultureInfo.CurrentCulture)
                        }, JsonRequestBehavior.AllowGet);
        }
    }
}