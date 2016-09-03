using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Blacklist;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Blacklist;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class BlacklistController : AdminController
    {
        private readonly BlackListService _blackListService;

        public BlacklistController()
        {
            _blackListService = new BlackListService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

         
        public JsonResult Get()
        {
            return Json(from item in _blackListService.GetAll().OrderByDescending(x=> x.Id)
                            select new
                            {
                                Id = item.Id,
                                Domain = item.Domain,
                                Edit =
                               BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "Blacklist", new {id = item.Id}),
                                   BootstrapButtonType.Warning,
                                   BootstrapSize.ExtraSmall),
                                Delete =
                                    BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Blacklist", new { id = item.Id }),
                                        BootstrapButtonType.Error, BootstrapSize.ExtraSmall, new Dictionary<string, string>
                                        {
                                            {"id", "deleteRecord"}
                                        })
                            }, JsonRequestBehavior.AllowGet);
        }

       public ActionResult Edit(int id)
        {
            var domain = _blackListService.GetById(id);

            if (domain == null) { this.SetError(Blacklist.NotFound); return RedirectToAction("Index");}

            return View(new EditBlacklistModel
            {
                Id = id,
                Domain = domain.Domain
            });
        }

        [HttpPost]
        public ActionResult Edit(EditBlacklistModel editBlacklistModel)
        {
            if (ModelState.IsValid)
            {
                _blackListService.Update(editBlacklistModel.Id, editBlacklistModel.Domain);

                this.SetSuccess(Blacklist.SuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            return View(editBlacklistModel);
        }

        [HttpPost]
        public ActionResult Add(BlacklistModel blacklistModel)
        {
            if (ModelState.IsValid)
            {
                _blackListService.AddWebsite(blacklistModel.Domain);

                this.SetSuccess(Blacklist.SuccessfullyAdded);

                return RedirectToAction("Index");
            }

            return View(blacklistModel);
        }

        public ActionResult Delete(int id)
        {
            if (_blackListService.RemoveWebsite(id))
            {
                this.SetSuccess(Blacklist.SuccessfullyRemoved);
            }
            else
            {
                this.SetSuccess(Blacklist.NotFound);
            }

            return RedirectToAction("Index");
        }

    }
}
