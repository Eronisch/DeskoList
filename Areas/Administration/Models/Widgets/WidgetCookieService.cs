using Topsite.Areas.Administration.Models.ThirdParty;

namespace Topsite.Areas.Administration.Models.Widgets
{
    public class WidgetCookieService : AbstractThirdPartyCookie
    {
        public override string KeyCookieName
        {
            get { return "widgetAction"; }
        }

        public override string KeyIsSuccessName
        {
            get { return "isSuccess"; }
        }

        public override string KeyMessageName
        {
            get { return "message"; }
        }
    }
}