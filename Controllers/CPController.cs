using System.Web.Mvc;
using Topsite.Action_Filters;

namespace Topsite.Controllers
{
    public class CpController : Controller
    {
        //
        // GET: /CP/
        [LoginRequiredActionFilter]
        public ActionResult Index()
        {
            return View();
        }
    }
}