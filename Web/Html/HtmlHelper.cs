using System.Linq;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Web.Html
{
    public static class HtmlHelper
    {
        public static void RenderLocalAction(this System.Web.Mvc.HtmlHelper htmlHelper, string action, string controller,
            RouteValueDictionary pRouteValueDictionary = null)
        {
            var routeValues = new RouteValueDictionary(new { area = string.Empty, thirdParty = string.Empty, type = string.Empty });

            if (pRouteValueDictionary != null)
            {
                foreach (var keyValuePair in pRouteValueDictionary.Where(r => !routeValues.ContainsKey(r.Key)))
                {
                    routeValues.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            htmlHelper.RenderAction(action, controller, routeValues);
        }

        public static string JsonSerializer(this System.Web.Mvc.HtmlHelper htmlHelper, object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }
    }
}
