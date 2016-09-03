using System.Web;

namespace Web.Ip
{
    public static class IpHelper
    {
        public static string GetIpFromCurrentRequest()
        {
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
