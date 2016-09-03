using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Links;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Links;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class LinkController : AdminController
    {
        private readonly LinkService _linksService;

        public LinkController()
        {
            _linksService = new LinkService();
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
            return Json(from record in _linksService.GetLinks()
                            select new
                            {
                                Id = record.Id,
                                Name = record.Name,
                                Link = record.Link,
                                Edit =
                               BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "Links", new {id = record.Id}),
                                   BootstrapButtonType.Warning,
                                   BootstrapSize.ExtraSmall),
                                Delete =
                                    BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Links", new { id = record.Id }),
                                        BootstrapButtonType.Error, BootstrapSize.ExtraSmall, new Dictionary<string, string>
                                        {
                                            {"id", "deleteRecord"}
                                        })
                            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var record = _linksService.GetById(id);

            if (record == null) { this.SetError(Links.NotFound); return RedirectToAction("Index");}

            return View(new EditLinkModel
            {
                Id = record.Id,
                Name = record.Name,
                Url = record.Link
            });
        }

        [HttpPost]
        public ActionResult Edit(EditLinkModel editLinkModel)
        {
            if (ModelState.IsValid)
            {
                _linksService.Update(editLinkModel.Id, editLinkModel.Name, editLinkModel.Url);

                this.SetSuccess(Links.SuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            return View(editLinkModel);
        }

        [HttpPost]
        public ActionResult Add(LinkModel linkModel)
        {
            if (ModelState.IsValid)
            {
                _linksService.Add(linkModel.Name, linkModel.Url);

                this.SetSuccess(Links.SuccessfullyAdded);

                return RedirectToAction("Index");
            }

            return View(linkModel);
        }

        public ActionResult Delete(int id)
        {
            if (_linksService.Remove(id))
            {
                this.SetSuccess(Links.SuccessfullyRemoved);
            }
            else
            {
                this.SetError(Links.NotFound);
            }
            
            return RedirectToAction("Index");
        }

    }
}
