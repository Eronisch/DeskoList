#region

using System.Web.Mvc;

#endregion

namespace Topsite.Areas.Administration
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Administration"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
             context.MapRoute(
                "Admin_default",
                "Administration/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "Topsite.Areas.Administration.Controllers" }
            );
        }
    }
}