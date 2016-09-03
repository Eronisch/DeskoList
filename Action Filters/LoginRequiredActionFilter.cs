using System.Web.Mvc;
using Web.Account;
using Web.Messages;

namespace Topsite.Action_Filters
{
    public class LoginRequiredActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!LoginHelper.IsLoggedIn())
            {
                const string urlLogin = "/Account/Login";

                filterContext.Controller.SetError(Localization.Languages.Filters.LoginRequired.LoginRequiredMessage);

                filterContext.Result = filterContext.HttpContext.Request != null
                    ? new RedirectResult(string.Format("{0}?returnUrl={1}", urlLogin, filterContext.HttpContext.Request.RawUrl))
                    : new RedirectResult(urlLogin);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}