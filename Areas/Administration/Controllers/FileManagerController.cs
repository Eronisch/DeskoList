using System.Web.Mvc;
using Admin;
using Core.Business.Languages;

namespace Topsite.Areas.Administration.Controllers
{
    public class FileManagerController : AdminController
    {
        //
        // GET: /FileManager/

        public ActionResult Index()
        {
            var languageService = new LanguageService();

            return View((object)languageService.GetDisplayLanguage(new ElFinderConfig().GetInstalledLanguages()));
        }

    }
}
