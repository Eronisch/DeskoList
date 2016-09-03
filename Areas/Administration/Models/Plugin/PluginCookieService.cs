using Topsite.Areas.Administration.Models.ThirdParty;

namespace Topsite.Areas.Administration.Models.Plugin
{
    public class PluginCookieService : AbstractThirdPartyCookie
    {
        public override string KeyCookieName
        {
            get { return "pluginAction"; }
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