using System.Web.Mvc;
using Core.Business.Breadcrumbs;
using Web.Account;
using Web.Breadcrumbs;
using Web.Route;
using AccountService = Core.Business.Account.AccountService;

namespace Admin
{
    public class AdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RedirectWhenNoPermission(filterContext);

            SetBreadCrumbs(filterContext.Controller, RouteProvider.GetController(), RouteProvider.GetAction(), filterContext.IsChildAction, filterContext.RequestContext.HttpContext.Request.IsAjaxRequest());
        }

        private void RedirectWhenNoPermission(ActionExecutingContext filterContext)
        {
            var accountService = new AccountService();

            if (!LoginHelper.IsLoggedIn() || !accountService.IsUserAdmin(LoginHelper.GetUserId()))
            {
                filterContext.Result = new RedirectResult("/Administration");
            }
        }

        private void SetBreadCrumbs(ControllerBase controllerBase, string controller, string action, bool isChildAction, bool isAjaxRequest)
        {
            if (!isChildAction && !isAjaxRequest)
            {
                var breadCrumbsService = new AdminBreadCrumbsService();

                AdminPageBreadCrumbsHelper.SetBreadCrumbs(controllerBase, breadCrumbsService.GetBreadCrumbs(controller, action));
            }
        }
    }
}
