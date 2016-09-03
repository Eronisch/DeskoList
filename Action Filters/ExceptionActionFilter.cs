using System.Web;
using System.Web.Mvc;
using Web.Messages;

namespace Topsite.Action_Filters
{
    public class ExceptionActionFilter : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            RedirectIfMaliciousInputClient(filterContext);
        }

        private static void RedirectIfMaliciousInputClient(ExceptionContext filterContext)
        {
            if (filterContext.Exception is HttpRequestValidationException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Controller.SetError(Localization.Languages.Filters.ExceptionAction.MaliciousInputDetected);
                filterContext.Result = new RedirectResult("/Home", false);
            }
        }
    }
}