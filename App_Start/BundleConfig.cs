#region

using System.Web.Optimization;

#endregion

namespace Topsite
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var adminFooCssBundle = new StyleBundle("~/Areas/Administration/Content/Css/fooTable").Include(
                "~/Areas/Administration/Content/Css/fooTable/footable-core-min.css",
                "~/Areas/Administration/Content/Css/fooTable/footable-metro-min.css"
                );

            var adminFooJsFundle = new ScriptBundle("~/Areas/Administration/Content/Javascript/fooTable").Include(
                "~/Areas/Administration/Content/Javascript/fooTable/footable.js",
                "~/Areas/Administration/Content/Javascript/fooTable/footable-paginate.js",
                "~/Areas/Administration/Content/Javascript/fooTable/footable-filter.js",
                "~/Areas/Administration/Content/Javascript/fooTable/footable-sort.js",
                "~/Areas/Administration/Content/Javascript/fooTable/footable-striping.js",
                "~/Areas/Administration/Content/Javascript/CustomFooTable.js"
                );

            var adminStyleBundle = new StyleBundle("~/Areas/Administration/Content/Css").Include(
                "~/Areas/Administration/Content/Css/Bootstrap.css",
                "~/Areas/Administration/Content/Css/font-awesome/FontAwesome.css",
                "~/Areas/Administration/Content/Css/AdminLTE.css",
                "~/Areas/Administration/Content/Css/Custom.css"
                );

            var adminScriptsBundle = new ScriptBundle("~/Areas/Administration/Content/Javascript").Include(
                "~/Areas/Administration/Content/Javascript/JQuery.js",
                "~/Areas/Administration/Content/Javascript/Bootstrap.js",
                "~/Areas/Administration/Content/Javascript/AdminLTE/app.js",
                "~/Areas/Administration/Content/Javascript/SideNavigation.js"
                );

            var adminResponsiveScriptBundle = new ScriptBundle("~/Areas/Administration/Content/Javascript/Responsive").Include(
                "~/Areas/Administration/Content/Javascript/Responsive/html5Shiv.js",
                "~/Areas/Administration/Content/Javascript/Responsive/respond.js"
              );

            bundles.Add(adminFooCssBundle);
            bundles.Add(adminFooJsFundle);
            bundles.Add(adminStyleBundle);
            bundles.Add(adminResponsiveScriptBundle);
            bundles.Add(adminScriptsBundle);

            BundleTable.EnableOptimizations = false;
        }
    }
}