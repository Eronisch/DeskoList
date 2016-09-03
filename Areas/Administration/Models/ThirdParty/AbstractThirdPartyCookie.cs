using System.Collections.Specialized;
using Web.Cookies;

namespace Topsite.Areas.Administration.Models.ThirdParty
{
    public abstract class AbstractThirdPartyCookie
    {
        abstract public string KeyCookieName { get; }
        abstract public string KeyIsSuccessName { get; }
        abstract public string KeyMessageName { get; }

        public ThirdPartyCookie Get()
        {
            var cookieValues = CookieService.GetCookieValues(KeyCookieName);

            if (cookieValues.AllKeys.Length == 0) { return null;}

            return new ThirdPartyCookie(cookieValues.Get(KeyMessageName), bool.Parse(cookieValues.Get(KeyIsSuccessName)));
        }

        public void Set(string message, bool isSuccess)
        {
            CookieService.SetCookie(KeyCookieName, new NameValueCollection
            {
              {KeyMessageName, message},
              {KeyIsSuccessName, isSuccess.ToString()}
            });
        }

        public void Set(NameValueCollection nameValueCollection)
        {
            CookieService.SetCookie(KeyCookieName, nameValueCollection);
        }

        public void Remove()
        {
            CookieService.RemoveCookie(KeyCookieName);
        }
    }
}