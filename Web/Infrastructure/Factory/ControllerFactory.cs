using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Core.Business.Plugin;
using Core.Business.Widgets;
using DatabaseXML;

namespace Web.Infrastructure.Factory
{
    public class ControllerFactory : DefaultControllerFactory
    {
        private readonly Collection<ControllerModel> _controllers;
        private readonly WidgetService _widgetService;
        private readonly PluginService _pluginService;
        private const string WidgetControllerFullName = "Widget.WidgetController";
        private const string PluginControllerFullName = "Plugin.PluginController";

        public ControllerFactory()
        {
            _controllers = new Collection<ControllerModel>();
            _widgetService = new WidgetService();
            _pluginService = new PluginService();

            LoadControllers();
        }

        private void LoadControllers()
        {
            var widgetController = GetType(WidgetControllerFullName);
            var pluginController = GetType(PluginControllerFullName);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.IsDynamic == false))
            {
                foreach (var exportedType in assembly.GetExportedTypes().Where(type => type.BaseType != null
                    && !type.IsAbstract
                    && (type.IsSubclassOf(typeof(Controller)))
                    && type.FullName != WidgetControllerFullName
                    && type.FullName != PluginControllerFullName
                    && !type.FullName.StartsWith("Elmah")))
                {
                    string controllerName = exportedType.Name.Replace("Controller", "");

                    var controllerType = GetSoftwareType(exportedType, widgetController, pluginController);

                    _controllers.Add(new ControllerModel
                    {
                        Controller = (Controller)Activator.CreateInstance(exportedType),
                        Name = controllerName,
                        Type = controllerType,
                        Area = GetArea(controllerType, exportedType.FullName)
                    });
                }
            }
        }

        private Type GetType(string typeFullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.IsDynamic == false))
            {
                foreach (var exportedType in assembly.GetExportedTypes().Where(type => type.BaseType != null
                                                                                       && !type.IsAbstract
                                                                                       && (type.IsSubclassOf(typeof(Controller)))))
                {
                    if (exportedType.FullName == typeFullName)
                    {
                        return exportedType;
                    }
                }
            }
            throw new Exception(string.Format("{0} controller not found", typeFullName));
        }

        private ControllerSoftwareType GetSoftwareType(Type type, Type widgetController, Type pluginController)
        {
            if (type.FullName.StartsWith("Topsite.Controllers"))
            {
                return ControllerSoftwareType.NonArea;
            }
            if (type.IsSubclassOf(widgetController))
            {
                return ControllerSoftwareType.Widget;
            }
            if (type.IsSubclassOf(pluginController))
            {
                return ControllerSoftwareType.Plugin;
            }

            return ControllerSoftwareType.LocalArea;
        }

        private string GetArea(ControllerSoftwareType softwareType, string typeFullName)
        {
            if (LocalDatabaseSettingsService.Manager.IsInstalled)
            {
                if (softwareType == ControllerSoftwareType.Widget)
                {
                    return _widgetService.GetByNameSpace(GetProjectNamespace(typeFullName)).AreaName;
                }
                if (softwareType == ControllerSoftwareType.Plugin)
                {
                    return _pluginService.GetByNameSpace(GetProjectNamespace(typeFullName)).Area;
                }
            }
          
            if (softwareType == ControllerSoftwareType.LocalArea)
            {
                return "Administration";
            }

            return string.Empty;
        }

        private string GetProjectNamespace(string assemblyFullName)
        {
            return assemblyFullName.Substring(0, assemblyFullName.IndexOf("."));
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controller = _controllers.FirstOrDefault(x => x.Name.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
            && x.Type == GetControllerSoftwareType(requestContext)
            && x.Area.Equals(GetConstraint(requestContext), StringComparison.CurrentCultureIgnoreCase));

            if (controller != null)
            {
                var controllerType = controller.Controller.GetType();

                return (IController)Activator.CreateInstance(controllerType);
            }

            return null;
        }

        private string GetConstraint(RequestContext requestContext)
        {
            string thirdPartyConstraint = GetThirdPartyName(requestContext);

            if (!string.IsNullOrEmpty(thirdPartyConstraint))
            {
                return thirdPartyConstraint;
            }

            string localAreaConstrainst = GetLocalAreaName(requestContext);

            if (!string.IsNullOrEmpty(GetLocalAreaName(requestContext)))
            {
                return localAreaConstrainst;
            }

            return string.Empty;
        }

        private string GetThirdPartyName(RequestContext requestContext)
        {
            var thirdPartyConstrainst = requestContext.RouteData.Values["thirdParty"];

            if (thirdPartyConstrainst != null)
            {
                return thirdPartyConstrainst.ToString();
            }

            return string.Empty;
        }

        private string GetLocalAreaName(RequestContext requestContext)
        {
            return (string)requestContext.RouteData.DataTokens["area"];
        }

        private ControllerSoftwareType GetControllerSoftwareType(RequestContext requestContext)
        {
            var dataType = requestContext.RouteData.Values["type"];

            if (dataType is ControllerSoftwareType)
            {
                return (ControllerSoftwareType)dataType;
            }

            if (dataType != null && !string.IsNullOrEmpty(dataType.ToString()))
            {
                return ((ControllerSoftwareType)Convert.ToInt16(dataType));
            }

            if (requestContext.RouteData.DataTokens["area"] != null)
            {
                return ControllerSoftwareType.LocalArea;
            }

            return ControllerSoftwareType.NonArea;
        }
    }
}

