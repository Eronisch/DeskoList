using System;
using System.Collections.Specialized;
using System.Web;

namespace Web.Cookies
{
    /// <summary>
    /// Manager for user cookies
    /// </summary>
    public static class CookieService
    {
        /// <summary>
        /// Set a new cookie with a single value
        /// The value will be stored encoded
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="endDate"></param>
        public static void SetCookie(string name, string value, DateTime endDate)
        {
            var cookie = new HttpCookie(name) {Name = name, Expires = endDate, Value = EncodeCookieValue(value)};
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Set a new cookie with multiple values
        /// The values will be stored encoded
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="endDate"></param>
        public static void SetCookie(string name, NameValueCollection values, DateTime endDate)
        {
            var cookie = new HttpCookie(name) {Name = name};

            cookie.Values.Add(EncodeCookieValues(values));

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Set a new cookie with a single value
        /// Cookies gets removed after the browser session ends(behaves differently on browsers)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetCookie(string name, string value)
        {
            var cookie = new HttpCookie(name) {Name = name, Value = EncodeCookieValue(value) };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Set a new cookie with a single value
        /// Cookies gets removed after the browser session ends(behaves differently on browsers)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        public static void SetCookie(string name, NameValueCollection values)
        {
            var cookie = new HttpCookie(name) {Name = name};

            cookie.Values.Add(EncodeCookieValues(values));

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Updates the cookie value
        /// Won't update the cookies value if the cookie doesn't exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void UpdateCookie(string name, string value)
        {
            HttpCookie httpCookie = HttpContext.Current.Response.Cookies[name];
            if (httpCookie != null)
                httpCookie.Value = EncodeCookieValue(value);
        }

        /// <summary>
        /// Sets the cookie expiration date to minus a year
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveCookie(string name)
        {
            var cookie = new HttpCookie(name)
            {
                Value = null,
                Expires = DateTime.Now.AddYears(-1) // or any other time in the past
            };

            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// Gets the decoded value of the cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCookieValue(string name)
        {
            return DecodedCookieValue(GetCookie(name).Value);
        }

        /// <summary>
        /// Gets multiple decoded values of the cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NameValueCollection GetCookieValues(string name)
        {
            return DecodeCookieValues(GetCookie(name).Values);
        }

        private static HttpCookie GetCookie(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                return HttpContext.Current.Request.Cookies[name];
            }

            return new HttpCookie(name);
        }

        private static NameValueCollection EncodeCookieValues(NameValueCollection values)
        {
            foreach (var valueKey in values.AllKeys)
            {
                string value = values[valueKey];

                values[valueKey] = EncodeCookieValue(value);
            }

            return values;
        }

        private static string EncodeCookieValue(string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        private static string DecodedCookieValue(string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        private static NameValueCollection DecodeCookieValues(NameValueCollection values)
        {
            foreach (var valueKey in values.AllKeys)
            {
                string value = values[valueKey];

                values[valueKey] = DecodedCookieValue(value);
            }

            return values;
        }
    }
}