using System.Web.Mvc;
using DatabaseXML;
using Web.Account;

namespace Topsite.Action_Filters
{
    public class AutoLoginActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!LocalDatabaseSettingsService.Manager.IsInstalled) { return; }

            if (!filterContext.IsChildAction && !filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                new LoginService().AutoLogin();
            }
        }
    }
}