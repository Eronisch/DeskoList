using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Core.Business.Widgets;
using Elmah;
using Web.Infrastructure.Factory;

namespace Web.Widgets
{
    public static class WidgetHelper
    {
        public static void LoadWidgets(this HtmlHelper htmlHelper, string themeSectionName)
        {
            var widgetService = new WidgetService();

            widgetService.GetActiveWidgetsBySection(themeSectionName).ToList().ForEach(widget =>
            {
                try
                {
                    // Register referenced areas, check if area exists
                    htmlHelper.RenderAction(widget.StartIndex, widget.Controller,
                        new {thirdParty = widget.AreaName, type = ControllerSoftwareType.Widget});
                }
                catch (Exception ex)
                {
                    htmlHelper.ViewContext.Writer.WriteLine(GetErrorHtml(widget.Name));
                    ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                }
            });
        }

        private static string GetErrorHtml(string widgetName)
        {
            return string.Format("<div style='color: #D8000C;background-color: #FFBABA;padding: 10px 15px;margin-top: 25px;text-shadow: 0px 1px #FFF;border-radius: 6px;border: 1px solid #fff;'>{0}</div>",
                string.Format(Localization.Languages.Web.Widgets.WidgetHelper.WidgetCrashed, widgetName));
        }
    }
}

