using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Core.Business.Url
{
    public class UrlHelpers
    {
        public static string GetCleanUrl(string input)
        {
            var r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            var strRemovedSpecialCharacters = r.Replace(input, String.Empty);
            return strRemovedSpecialCharacters.Replace(" ", "-");
        }

        public static string GetHost(string url, bool includeExtension)
        {
            if (includeExtension)
            {
                return new Uri(url).Host;
            }

            var host = new Uri(url).Host;
            return host.Substring(0, host.LastIndexOf('.'));
        }

        /// <summary>
        /// Gets the base of the current url (example: http://datop100.com)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentBaseUrl()
        {
            return "http://" + HttpContext.Current.Request.Url.Authority;
        }
    }
}
