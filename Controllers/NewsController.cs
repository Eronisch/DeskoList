using System.Web.Mvc;
using Core.Business.News;
using Localization.Languages.Controllers;
using Web.Messages;
using Web.Seo;

namespace Topsite.Controllers
{
    public class NewsController : Controller
    {
        public ActionResult Index(int id)
        {
            var newsService = new NewsService();

            var newsArticle = newsService.GetArticleById(id);

            if (newsArticle == null)
            {
                this.SetError(News.NotFound);
                return RedirectToAction("Index", "Home");
            }

            this.SetSeo(newsArticle.Title, newsArticle.Description);

            return View(newsArticle);
        }
    }
}