using System;

namespace Core.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Returns a string in the format: 2015-09-08
        /// </summary>
        /// <returns></returns>
        public static string ToIso8601FormatString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}
