using System;
using Web.Cookies;

namespace Web.Language
{
    public class LanguageCookieService
    {
        private const string LanguageCookieKey = "Language";

        /// <summary>
        /// Gets the language culture from the users cookie
        /// </summary>
        /// <returns>Abbrevation or null</returns>
        public string GetCookie()
        {
            return CookieService.GetCookieValue(LanguageCookieKey);
        }

        /// <summary>
        /// Sets the language cookie in the users cookie for a year
        /// </summary>
        public void SetCookie(string culture)
        {
            CookieService.SetCookie(LanguageCookieKey, culture, DateTime.Now.AddYears(1));
        }
    }
}
