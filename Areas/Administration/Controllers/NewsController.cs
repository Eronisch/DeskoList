using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Admin;
using Core.Business.News;
using Localization.Languages.Admin.Controllers;
using Topsite.Areas.Administration.Models.News;
using Web.Account;
using Web.Bootstrap;
using Web.Messages;
using Web.Route;

namespace Topsite.Areas.Administration.Controllers
{
    public class NewsController : AdminController
    {
        private readonly NewsService _newsService;

        public NewsController()
        {
            _newsService = new NewsService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public JsonResult GetNews()
        {
            return Json(from news in _newsService.GetNewsArticles().OrderByDescending(x=> x.Id)
                            select new
                            {
                                Id = news.Id,
                                Subject = news.Subject,
                                Title = news.Title,
                                Author = news.AuthorName,
                                Date = news.Date.ToShortDateString(),
                                Edit =
                               BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Edit, ControllerContext.RequestContext.GetActionRoute("Edit", "News", new {id = news.Id}),
                                   BootstrapButtonType.Warning,
                                   BootstrapSize.ExtraSmall),
                                Delete =
                                    BootstrapHelper.GetLinkButton(Localization.Languages.Admin.Common.Delete, ControllerContext.RequestContext.GetActionRoute("Delete", "News", new { id = news.Id }),
                                        BootstrapButtonType.Error, BootstrapSize.ExtraSmall, new Dictionary<string, string>
                                        {
                                            {"id", "deleteArticle"}
                                        })
                            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var article = _newsService.GetArticleById(id);

            if (article == null) { this.SetError(News.ArticleNotFound); return RedirectToAction("Index");}

            return View(new EditNewsViewModel
            {
                Description = article.Description,
                Information = article.Information,
                NewsId = article.Id,
                Subject = article.Subject,
                Title = article.Title
            });
        }

        [HttpPost]
        public ActionResult Edit(EditNewsViewModel editNewsViewModel)
        {
            if (ModelState.IsValid)
            {
                _newsService.UpdateArticle(editNewsViewModel.NewsId, editNewsViewModel.Subject, editNewsViewModel.Title, editNewsViewModel.Description, editNewsViewModel.Information);

                this.SetSuccess(News.ArticleSuccessfullyUpdated);

                return RedirectToAction("Index");
            }

            return View(editNewsViewModel);
        }

        [HttpPost]
        public ActionResult Add(BaseViewNewsModel baseViewNewsModel)
        {
            if (ModelState.IsValid)
            {
                _newsService.AddArticle(baseViewNewsModel.Subject, baseViewNewsModel.Title, baseViewNewsModel.Description, baseViewNewsModel.Information, LoginHelper.GetUserId());

                this.SetSuccess(News.ArticleSuccessfullyAdded);

                return RedirectToAction("Index");
            }

            return View(baseViewNewsModel);
        }

        public ActionResult Delete(int id)
        {
            var article = _newsService.GetArticleById(id);

            if (article != null)
            {
                _newsService.RemoveArticle(id);

                this.SetSuccess(News.ArticleSuccessfullyRemoved);
            }
            else
            {
                this.SetError(News.ArticleNotFound);
            }
            
            return RedirectToAction("Index");
        }

    }
}
