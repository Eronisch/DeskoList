using System.Web.Mvc;

namespace Web.Infrastructure.ViewEngines
{
    public class InstallerViewEngine : RazorViewEngine
    {
        public InstallerViewEngine()
        {
            ViewLocationFormats = new[]
            {
                "~/Install/Views/{0}.cshtml"
            };
        }
    }
}