using System.Web.Mvc;
using Core.Business.Plugin;

namespace Topsite.Action_Filters
{
    public class PluginActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            PluginActionActivateService.FireAction(PluginActions.PageLoadOnActionExecuting, filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            PluginActionActivateService.FireAction(PluginActions.PageLoadOnActionExecuted, filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            PluginActionActivateService.FireAction(PluginActions.PageLoadOnResultExecuting, filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            PluginActionActivateService.FireAction(PluginActions.PageLoadOnResultExecuted, filterContext);
        }
    }
}