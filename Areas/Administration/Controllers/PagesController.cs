using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Pages;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Pages;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class PagesController : AdminController
    {
        private readonly DynamicPageService _dynamicPageService;

        public PagesController()
        {
            _dynamicPageService = new DynamicPageService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPages()
        {
            return Json(from page in _dynamicPageService.GetPages().OrderByDescending(x => x.Id)
                        select new
                        {
                            Id = page.Id,
                            Title = page.Title,
                            Description = page.Description,
                            Edit =
                          BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "Pages", new { id = page.Id }),
                              BootstrapButtonType.Warning,
                              BootstrapSize.ExtraSmall),
                            Delete =
                                BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Pages", new { id = page.Id }),
                                    BootstrapButtonType.Error, BootstrapSize.ExtraSmall, new Dictionary<string, string>
                                    {
                                        {"id", "deletePage"}
                                    })
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var page = _dynamicPageService.GetPageById(id);

            if (page != null)
            {
                _dynamicPageService.DeletePage(id);

                this.SetSuccess(Pages.PageSuccessfullyDeleted);
            }
            else
            {
                this.SetError(Pages.PageNotFound);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var page = _dynamicPageService.GetPageById(id);

            if (page == null) { this.SetError(Pages.PageNotFound); return RedirectToAction("Index"); }

            return View(new EditPageViewModel
            {
                Content = page.Message,
                Description = page.Description,
                Keywords = page.Keywords,
                Title = page.Title,
                PageId = page.Id
            });
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(EditPageViewModel editPageViewModel)
        {
            if (ModelState.IsValid)
            {
                _dynamicPageService.UpdatePage(editPageViewModel.PageId, editPageViewModel.Title, editPageViewModel.Description, editPageViewModel.Keywords, editPageViewModel.Content);

                this.SetSuccess(Pages.PageSuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            return View(editPageViewModel);
        }

        [HttpPost]
        public ActionResult Add(BasicPageViewModel basicPageViewModel)
        {
            if (ModelState.IsValid)
            {
                _dynamicPageService.AddPage(basicPageViewModel.Title, basicPageViewModel.Description, basicPageViewModel.Keywords, basicPageViewModel.Content);

                this.SetSuccess(Pages.PageSuccessfullyCreated);

                return RedirectToAction("Index");
            }

            return View(basicPageViewModel);
        }

    }
}
