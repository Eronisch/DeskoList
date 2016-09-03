using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.Route
{
    public class RouteConfig
    {
        public static void RegisterRoutes()
        {
            RouteRegisterService.Ignore("*.html|*.js|*.css|*.gif|*.jpg|*.jpeg|*.png|*.swf");

            RouteRegisterService.Ignore("elfinder.connector");

            RouteRegisterService.Ignore("{resource}.axd/{*pathInfo}");

            RouteRegisterService.Add("Category", "Category/{CategoryId}/{Link}", "Category", "Index", new Dictionary<string, UrlParameter>(), new Dictionary<string, string>
            {
                {"CategoryId", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("Search", "Search/{Text}", "Search", "Index", new Dictionary<string, UrlParameter>
            {
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("News", "News/{Id}/{Title}", "News", "Index", new Dictionary<string, UrlParameter>
            {
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("Page", "Page/{Id}/{Title}", "Page", "Index", new Dictionary<string, UrlParameter>
            {
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("EditWebsite", "Website/Edit/{Id}", "Website", "Edit", new Dictionary<string, UrlParameter>
            {
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("Code", "Website/View/{User}/{Id}", "Website", "View", new Dictionary<string, UrlParameter>
            {
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("Go", "Website/Go/{User}/{Id}", "Website", "Go", new Dictionary<string, UrlParameter>
            {
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("Vote", "Website/Vote/{Username}/{Id}/{Redirect}", "Website", "Vote", new Dictionary<string, UrlParameter>
            {
                {"Redirect", UrlParameter.Optional},
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });

            RouteRegisterService.Add("Website", "Website/Code/{Id}", "Website", "Code", new Dictionary<string, UrlParameter>
            {
            }, new Dictionary<string, string>()
            {
                {"Id", @"\d+"}
            }, new[] { "Topsite.Controllers" });


            RouteRegisterService.Add("ThirdParty", "{thirdParty}/{controller}/{action}/{type}", "Home", "Index", new Dictionary<string, string>
            {
              {"controller", "Home"},
              {"action", "Index"}
            }, new Dictionary<string, string>()
            {
                 {"thirdParty", @"^[a-zA-Z0-9]*$"},
                {"type", @"\d+"},
            }, new string[0]);

               RouteRegisterService.Add("Default", "{controller}/{action}", "Home", "Index", new Dictionary<string, UrlParameter>
         {
         }, new[] { "Topsite.Controllers" });
        }
    }
}