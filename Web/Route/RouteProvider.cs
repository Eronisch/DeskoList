using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Route
{
    public static class RouteProvider
    {
        private const string ControllerKey = "controller";
        private const string ActionKey = "action";
        private const string AreaKey = "area";

        public static string GetAction()
        {
            return GetValueFromRouteData(HttpContext.Current.Request.RequestContext.RouteData.Values, ActionKey);
        }

        public static string GetController()
        {
            return GetValueFromRouteData(HttpContext.Current.Request.RequestContext.RouteData.Values, ControllerKey);
        }

        public static string GetControllerFromControllerContext(ControllerContext controllerContext)
        {
            return GetValueFromRouteData(controllerContext.RequestContext.RouteData.Values, ControllerKey);   
        }

        public static string GetActionFromContext(ControllerContext controllerContext)
        {
            return GetValueFromRouteData(controllerContext.RequestContext.RouteData.Values, ActionKey);
        }

        public static string GetAreaFromControllerAndDataTokens(ControllerContext controllerContext)
        {
            return GetValueFromRouteData(controllerContext.RequestContext.RouteData.DataTokens, AreaKey);
        }

        public static string GetArea()
        {
            var area =
                HttpContext.Current.Request.RequestContext.RouteData.Values[AreaKey];

            return (string)area ?? string.Empty;
        }

        public static string GetPreviousAction()
        {
            // Split the url to url + query string
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                var fullUrl = HttpContext.Current.Request.UrlReferrer.ToString();
                var questionMarkIndex = fullUrl.IndexOf('?');
                string queryString = null;
                string url = fullUrl;

                if (questionMarkIndex != -1) // There is a QueryString
                {
                    url = fullUrl.Substring(0, questionMarkIndex);
                    queryString = fullUrl.Substring(questionMarkIndex + 1);
                }

                // Arranges
                var request = new HttpRequest(null, url, queryString);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                // Extract the data    
                if (routeData != null) return GetValueFromRouteData(routeData.Values, ActionKey);
            }

            return string.Empty;
        }

        public static string GetPreviousController()
        {
            // Split the url to url + query string
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                var fullUrl = HttpContext.Current.Request.UrlReferrer.ToString();
                var questionMarkIndex = fullUrl.IndexOf('?');
                string queryString = null;
                string url = fullUrl;

                if (questionMarkIndex != -1) // There is a QueryString
                {
                    url = fullUrl.Substring(0, questionMarkIndex);
                    queryString = fullUrl.Substring(questionMarkIndex + 1);
                }

                // Arranges
                var request = new HttpRequest(null, url, queryString);
                var response = new HttpResponse(new StringWriter());
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                if (routeData != null) return GetValueFromRouteData(routeData.Values, ControllerKey);
            }

            return string.Empty;
        }

        private static string GetValueFromRouteData(RouteValueDictionary routeValueDictionary, string keyName)
        {
            var foundItem = routeValueDictionary[keyName];
            return foundItem != null ? routeValueDictionary[keyName].ToString().ToLower() : string.Empty;
        }
    }
}
