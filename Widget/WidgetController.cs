using System;
using System.Reflection;
using System.Web.Mvc;
using Core.Business.Widgets;
using Database.Entities;
using Web.Messages;

namespace Widget
{
    public class WidgetController : Controller
    {
        private readonly Lazy<Widgets> _widget;
        private readonly string _callingAssembly;
        private readonly WidgetConfigService _widgetConfigService;

        public WidgetController()
        {
            var widgetService = new WidgetService();
            _callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
            _widget = new Lazy<Widgets>(() => widgetService.GetByNameSpace(_callingAssembly));
            _widgetConfigService = new WidgetConfigService();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                filterContext.Controller.SetError(Localization.Languages.Widget.WidgetController.WidgetChildAction);
                filterContext.Result = new RedirectResult("/Home", false);
            }
        }

        public string GetWidgetPath(string pathFile)
        {
            return string.Format("/{0}/{1}", _widgetConfigService.GetRelativeAreaPath(WidgetAreaName), pathFile);
        }

        public string WidgetAreaName
        {
            get
            {
                return _widget.Value.AreaName;
            }
        }

        public int WidgetId
        {
            get
            {
                return _widget.Value.Id;
            }
        }
    }
}
