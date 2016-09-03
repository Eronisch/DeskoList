using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Bootstrap
{
    public static class BootstrapExtension
    {
        public static IHtmlString GenerateError(this HtmlHelper htmlHelper, string title, string errorMessage, bool displayIcon, bool hasCloseIcon = true)
        {
            var alert = new StringBuilder();

            alert.Append(hasCloseIcon
                ? "<div class='alert alert-danger alert-dismissable'>"
                : "<div class='alert alert-danger'>");

            if (displayIcon)
            {
                alert.Append("<i class='fa fa-ban'></i>");
            }

            if (hasCloseIcon)
            {
                alert.Append("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>");    
            }
            
            alert.Append(string.Format("<h3>{0}</h3>", title));
            alert.Append(errorMessage);
            alert.Append("</div>");

            return htmlHelper.Raw(alert);
        }

        public static IHtmlString GenerateWarning(this HtmlHelper htmlHelper, string title, string errorMessage, bool displayIcon, bool hasCloseIcon = true)
        {
            var alert = new StringBuilder();

            alert.Append(hasCloseIcon
                ? "<div class='alert alert-warning alert-dismissable'>"
                : "<div class='alert alert-warning'>");

            if (displayIcon)
            {
                alert.Append("<i class='fa fa-ban'></i>");
            }

            if (hasCloseIcon)
            {
                alert.Append("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>");
            }

            alert.Append(string.Format("<h3>{0}</h3>", title));
            alert.Append(errorMessage);
            alert.Append("</div>");

            return htmlHelper.Raw(alert);
        }


        public static IHtmlString GenerateInfo(this HtmlHelper htmlHelper, string title, string infoMessage, bool displayIcon, bool hasCloseIcon = true)
        {
            var alert = new StringBuilder();

            alert.Append(hasCloseIcon
                ? "<div class='alert alert-info alert-dismissable'>"
                : "<div class='alert alert-info'>");

            if (displayIcon)
            {
                alert.Append("<i class='fa fa-ban'></i>");
            }

            if (hasCloseIcon)
            {
                alert.Append("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>");
            }

            alert.Append(string.Format("<h3>{0}</h3>", title));
            alert.Append(infoMessage);
            alert.Append("</div>");

            return htmlHelper.Raw(alert);
        }

        public static IHtmlString GenerateSuccess(this HtmlHelper htmlHelper, string title, string errorMessage, bool displayIcon, bool hasCloseIcon = true)
        {
            var alert = new StringBuilder();

            alert.Append(hasCloseIcon
              ? "<div class='alert alert-success alert-dismissable'>"
              : "<div class='alert alert-success'>");

            if (displayIcon)
            {
                alert.Append("<i class='fa fa-check'></i>");
            }

            if (hasCloseIcon)
            {
                alert.Append("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>");
            }

            alert.Append(string.Format("<h3>{0}</h3>", title));
            alert.Append(errorMessage);
            alert.Append("</div>");

            return htmlHelper.Raw(alert);
        }
    }
}
