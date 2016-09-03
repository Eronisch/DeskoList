using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using Autofac;
using Core.Business.Schedule;
using Core.Business.ThirdParty;
using DatabaseXML;
using elFinder.Connector.Integration.Autofac;
using Web.Infrastructure.Factory;
using Web.Infrastructure.ViewEngines;
using Web.Route;

namespace Topsite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        // Every page visit
        protected void Application_BeginRequest()
        {
           // Protection against click jacking
            HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");
        }

        private void RegisterIoC()
        {
            // register IoC
            var builder = new ContainerBuilder();
            // add elFinder connector registration
            builder.RegisterElFinderConnectorDefault();
            // create container
            var  container = builder.Build();
            // need also to set container in elFinder module
            container.SetAsElFinderDependencyResolver();
        }

        protected void Application_Start()
        {
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();

            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());

            ViewEngines.Engines.Clear();

            if (LocalDatabaseSettingsService.Manager.IsInstalled)
            {
                new ThirdPartyActivator().Start();
                new BuiltInTasksScheduler().Schedule();
                ViewEngines.Engines.Add(new CustomViewEngine());
            }
            else
            {
                ViewEngines.Engines.Add(new InstallerViewEngine());
            }
            
            RouteConfig.RegisterRoutes();

            RegisterIoC();
        }
    }
}