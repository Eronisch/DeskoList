using System;
using System.Web;
using System.Web.SessionState;

namespace Web.Session
{
    public class SessionService
    {
        private static HttpSessionState Session
        {
            get
            {
                ThrowExceptionWhenSessionNotAvailable();
                return HttpContext.Current.Session;
            }
        }

        public static void AddItem(string name, object value)
        {
            Session.Add(name, value);
        }

        public static void RemoveItem(string name)
        {
            Session.Remove(name);
        }

        public static object GetItem(string name)
        {
            return Session[name];
        }

        private static void ThrowExceptionWhenSessionNotAvailable()
        {
            if (HttpContext.Current == null)
            {
                throw new Exception("HttpContext is not available!");
            }
        }
    }
}
