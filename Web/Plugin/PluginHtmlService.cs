using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Core.Business.Plugin;
using Web.Route;

namespace Web.Plugin
{
    public static class PluginHtmlService
    {
        public static List<MvcHtmlString> GetHtmlPlugins(this HtmlHelper htmlHelper)
        {
            return
                PluginHtmlSubscribeService.Get(RouteProvider.GetController(), RouteProvider.GetAction())
                    .Select(p => GetHtml(htmlHelper, p.Area, p.ViewName)).ToList();
        }

        private static MvcHtmlString GetHtml(this HtmlHelper htmlHelper, string area, string viewName)
        {
            return htmlHelper.Partial(GetViewPath(area, viewName));
        }

        private static string GetViewPath(string area, string viewName)
        {
            var pluginConfigService = new PluginConfigService();

            return string.Format("~/{0}/{1}", pluginConfigService.GetRelativeAreaPath(area), viewName);
        }
    }
}
