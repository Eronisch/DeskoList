using System;
using System.Web.Mvc;
using System.Web.Routing;
using Core.Business.Settings;
using Core.Business.Widgets;
using Web.Infrastructure.Factory;
using Web.Route;

namespace Web.Infrastructure.ViewEngines
{
    public class CustomViewEngine : RazorViewEngine
    {
        private readonly SettingsService _settingsService = new SettingsService();
        private readonly WidgetService _widgetService = new WidgetService();

        public CustomViewEngine()
        {
            string themeName = _settingsService.GetActiveThemeMap();

            ViewLocationFormats = new[]
            {
                "~/Themes/" + themeName + "/Views/{1}/{0}.cshtml",
                "~/Themes/" + themeName + "/Views/Shared/{0}.cshtml",
                "~/Install/Views/%1/{0}.cshtml"
            };


            MasterLocationFormats = new[]
            {
                "~/Themes/" + themeName + "/Views/%1/{1}/{0}.cshtml",
                "~/Themes/" + themeName + "/Views/Shared/{0}.cshtml"
            };


            PartialViewLocationFormats = new[]
            {
                "~/Themes/" + themeName + "/Views/%1/{1}/{0}.cshtml",
                "~/Themes/" + themeName + "/Views/Shared/{0}.cshtml",
            };

            FileExtensions = new[] { "cshtml" };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var softwareType = GetControllerSoftwareType(controllerContext.RequestContext);

            IView view;

            switch (softwareType)
            {
                case ControllerSoftwareType.Widget:
                    {
                        view = CreateView(controllerContext,
                     string.Format("~/Widgets/{0}/Views/{1}/{2}.cshtml", GetThirdPartyName(controllerContext.RequestContext),
                         RouteProvider.GetControllerFromControllerContext(controllerContext), viewName), masterName);

                        break;
                    }
                case ControllerSoftwareType.Plugin:
                    {
                        view = CreateView(controllerContext,
                         string.Format("~/Plugins/{0}/Views/{1}/{2}.cshtml", GetThirdPartyName(controllerContext.RequestContext),
                             RouteProvider.GetControllerFromControllerContext(controllerContext), viewName), masterName);

                        break;
                    }
                default:
                    {
                        return base.FindView(controllerContext, viewName, masterName, useCache);
                    }
            }

            return new ViewEngineResult(view, this);
        }

        private string GetThirdPartyName(RequestContext requestContext)
        {
            var dataRouteArea = requestContext.RouteData.Values["thirdParty"];

            if (dataRouteArea != null)
            {
                return dataRouteArea.ToString();
            }

            return string.Empty;
        }

        private ControllerSoftwareType GetControllerSoftwareType(RequestContext requestContext)
        {
            var dataType = requestContext.RouteData.Values["type"];

            if (dataType != null)
            {
                return ((ControllerSoftwareType)Convert.ToInt16(dataType));
            }

            if (requestContext.RouteData.Values["area"] != null)
            {
                return ControllerSoftwareType.LocalArea;
            }

            return ControllerSoftwareType.NonArea;
        }
    }
}