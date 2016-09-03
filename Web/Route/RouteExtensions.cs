using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Route
{
    public static class RouteExtensions
    {
        public static string GetActionRoute(this RequestContext requestContext, string action, string controller, object routeValues = null)
        {
            return new UrlHelper(requestContext).Action(action, controller, routeValues);
        }
    }
}
