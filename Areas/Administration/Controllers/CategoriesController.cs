using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.Categories;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.Categories;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class CategoriesController : AdminController
    {
        private readonly CategoryService _categoryService;

        public CategoriesController()
        {
            _categoryService = new CategoryService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategories()
        {
            return Json(from category in _categoryService.GetCategories()
                        select new
                        {
                            Id = category.CategoryId,
                            Name = category.Name,
                            Keywords = string.IsNullOrEmpty(category.Keywords) ? string.Empty : category.Keywords,
                            Edit = BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "Categories", new { id = category.CategoryId }), BootstrapButtonType.Info,
                           BootstrapSize.ExtraSmall),
                            Delete = BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "Categories", new { id = category.CategoryId }), BootstrapButtonType.Error,
                           BootstrapSize.ExtraSmall, new Dictionary<string, string>
                               {
                                   {"id", "deleteCategory"}
                               })
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategory(id);

            if (category == null) { this.SetError(Categories.CategoryNotFound); return RedirectToAction("Index"); }

            return View(new EditCategoryViewModel
            {
                Keywords = category.Keywords,
                Name = category.Name,
                CategoryId = category.Id
            });
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(EditCategoryViewModel editCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(editCategoryViewModel.CategoryId, editCategoryViewModel.Name, editCategoryViewModel.Keywords);

                this.SetSuccess(Categories.CategorySuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            return View(editCategoryViewModel);
        }

        [HttpPost]
        public ActionResult Add(BaseCategoryViewModel baseCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                _categoryService.AddCategory(baseCategoryViewModel.Name, baseCategoryViewModel.Keywords);

                this.SetSuccess(Categories.CategorySuccessfullyAdded);

                return RedirectToAction("Index");
            }

            return View(baseCategoryViewModel);
        }
    }
}
