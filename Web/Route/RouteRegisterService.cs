using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Infrastructure.Factory;

namespace Web.Route
{
    public static class RouteRegisterService
    {
        private static RouteCollection RouteTable
        {
            get { return System.Web.Routing.RouteTable.Routes; }
        }

        /// <summary>
        /// Adds the route, throws an exception when a route with the same name already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        public static void Add(string name, string url, string controller, string action, Dictionary<string, string> defaults, Dictionary<string, string> constraints, string[] namespaces)
        {
            var routeDefaults = GetRouteDefaults(controller, action, defaults);

            RouteTable.MapRoute(name, url,
                                     (object)routeDefaults,
                                     (object)GetContstraints(constraints),
                                     namespaces);
        }

        /// <summary>
        /// Adds the route, throws an exception when a route with the same name already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="defaults"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        public static void Add(string name, string url, string controller, string action, Dictionary<string, UrlParameter> defaults, Dictionary<string, string> constraints, string[] namespaces)
        {
            var routeDefaults = GetRouteDefaults(controller, action, defaults);

            RouteTable.MapRoute(name, url,
                                     (object)routeDefaults,
                                     (object)GetContstraints(constraints),
                                     namespaces);
        }

        /// <summary>
        /// Adds the route, throws an exception when a route with the same name already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="defaults"></param>
        /// <param name="namespaces"></param>
        public static void Add(string name, string url, string controller, string action, Dictionary<string, UrlParameter> defaults, string[] namespaces)
        {
            var routeDefaults = GetRouteDefaults(controller, action, defaults);

            RouteTable.MapRoute(name, url,
                                   (object)routeDefaults,
                                   namespaces);
        }

        /// <summary>
        /// Adds a third party route, throws an exception when a route with the same name already exists
        /// </summary>
        public static void AddThirdParty(string name, string @namespace, ControllerSoftwareType softwareType)
        {
            RouteTable.MapRoute(name, name + "/{controller}/{type}",
             new { action = "Index", type = (int)softwareType, thirdParty = name },
               new { thirdParty = "^[a-zA-Z0-9]*$", type = @"\d+" },
             new[] { @namespace });

            RouteTable.MapRoute(name + "(2)", name + "/{controller}/{action}/{type}",
                new { action = "Index", type = (int)softwareType, thirdParty = name },
                  new { thirdParty = "^[a-zA-Z0-9]*$", type = @"\d+" },
                new[] { @namespace });
        }

        /// <summary>
        /// Adds the widget route, throws an exception when a route with the same name already exists
        /// </summary>
        public static void AddThirdParty(string name, string controller, string action, string @namespace, ControllerSoftwareType softwareType)
        {
            RouteTable.MapRoute(name, "{thirdParty}/{controller}",
             new { action = "Index", type = (int)softwareType },
               new { thirdParty = "^[a-zA-Z0-9]*$", type = @"\d+" },
             new[] { @namespace });

            RouteTable.MapRoute(name + "(2)", "{thirdParty}/{controller}/{action}",
                new { type = (int)softwareType },
                  new { thirdParty = "^[a-zA-Z0-9]*$", type = @"\d+" },
                new[] { @namespace });
        }


        /// <summary>
        /// Adds the area route with the given parameters, throws an exception when a route with the same name already exists
        /// <param name="urlParams">Url to match the route, {controller}/{/action} is already added. So add only the url after the the controller/action part.</param>
        /// </summary>
        public static void AddThirdParty(string name, string controller, string action, string urlParams, Dictionary<string, string> constraints, string @namespace, ControllerSoftwareType softwareType)
        {
            var thirdPartyConstraints = GetDynamicObject(constraints);
            thirdPartyConstraints.thirdParty = "^[a-zA-Z0-9]*$";
            thirdPartyConstraints.type = @"\d+";

            string thirdPartyUrlWithoutAction = string.Format("{{thirdParty}}/{{controller}}/{0}",  urlParams);
            string thirdPartyUrlWithAction = string.Format("{{thirdParty}}/{{controller}}/{{action}}/{0}", urlParams);

            RouteTable.MapRoute(name, thirdPartyUrlWithoutAction,
             new { action = action, type = (int) softwareType },
             (object) thirdPartyConstraints,
             new[] { @namespace });

            RouteTable.MapRoute(name + "(2)", thirdPartyUrlWithAction,
                new {type = (int) softwareType },
                (object) thirdPartyConstraints ,
                new[] { @namespace });
        }

        public static void Ignore(string url)
        {
            RouteTable.IgnoreRoute(url);
        }

        private static dynamic GetRouteDefaults(string controller, string action, Dictionary<string, string> defaults)
        {
            var routeDefaults = GetDynamicObject(defaults);
            routeDefaults.controller = controller;
            routeDefaults.action = action;
            return routeDefaults;
        }

        private static dynamic GetRouteDefaults(string controller, string action, Dictionary<string, UrlParameter> defaults)
        {
            var routeDefaults = GetDynamicObject(defaults);
            routeDefaults.controller = controller;
            routeDefaults.action = action;
            return routeDefaults;
        }

        private static dynamic GetContstraints(Dictionary<string, string> constraints)
        {
            return GetDynamicObject(constraints);
        }

        private static dynamic GetDynamicObject<T>(Dictionary<string, T> properties)
        {
            var dynamicObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in properties)
            {
                dynamicObject.Add(property.Key, property.Value);
            }
            return dynamicObject;
        }
    }
}
