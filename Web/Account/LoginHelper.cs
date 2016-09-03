using System;
using System.Web;

namespace Web.Account
{
    /// <summary>
    /// Helper to get information about the current logged in user
    /// </summary>
    public static class LoginHelper
    {
        private const string SessionUsernameKey = "Username";
        private const string SessionUserIdKey = "UserId";

        /// <summary>
        /// Get the logged in userid, throws an exception when the user is not logged in
        /// </summary>
        /// <returns></returns>
        public static int GetUserId()
        {
            if (!IsLoggedIn()) { throw new Exception("You can't get the userid, because the user is not logged in"); }

            return (int) HttpContext.Current.Session[SessionUserIdKey];
        }

        /// <summary>
        /// Gets the logged in username, throws an exception when the user is not logged in
        /// </summary>
        /// <returns></returns>
        public static string GetUsername()
        {
            if (!IsLoggedIn()) { throw new Exception("You can't get the username, because the user is not logged in"); }

            return HttpContext.Current.Session[SessionUsernameKey].ToString();
        }

        /// <summary>
        /// Checks if the current user is logged in
        /// </summary>
        /// <returns></returns>
        public static bool IsLoggedIn()
        {
            var userId = HttpContext.Current.Session[SessionUserIdKey];

            if (userId != null && !string.IsNullOrEmpty(userId.ToString()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the key that is used for storing the username in the session object
        /// </summary>
        /// <returns></returns>
        public static string GetSessionUsernameKey()
        {
            return SessionUsernameKey;
        }
        
        /// <summary>
        /// Gets the key that is used for storing the id in the session object
        /// </summary>
        /// <returns></returns>
        public static string GetSessionUserIdKey()
        {
            return SessionUserIdKey;
        }
    }
}
