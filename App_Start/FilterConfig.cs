using System.Web.Mvc;
using Topsite.Action_Filters;

namespace Topsite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionActionFilter());
            filters.Add(new GlobalActionFilter());
            filters.Add(new LanguageActionFilter());
            filters.Add(new AutoLoginActionFilter());
            filters.Add(new PluginActionFilter());
        }
    }
}