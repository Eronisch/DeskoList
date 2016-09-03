using System.Web.Mvc;
using Core.Business.Pages;
using Localization.Languages.Controllers;
using Web.Messages;
using Web.Seo;

namespace Topsite.Controllers
{
    public class PageController : Controller
    {
        //
        // GET: /Page/

        [HttpGet]
        public ActionResult Index(int id)
        {
            var dynamicPageService = new DynamicPageService();

            var page = dynamicPageService.GetPageById(id);

            if (page == null)
            {
                this.SetError(Page.NoPageFound);
                return RedirectToAction("Index", "Home");
            }

            this.SetSeo(page.Title, page.Description, page.Keywords);

            return View(page);
        }
    }
}