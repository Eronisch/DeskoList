using System.Globalization;

namespace Core.Extensions
{
    public static class DoubleExtension
    {
        /// <summary>
        /// Returns a string that uses a point instead of a comma
        /// </summary>
        /// <returns></returns>
        public static string ToZeroGbString(this double value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("en-GB"));
        }
    }
}
