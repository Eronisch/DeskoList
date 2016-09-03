using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core.Business.Date
{
    /// <summary>
    /// Date helper
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Returns a format like: 3 minutes, 1 hour, 2 years and 3 months depending on the date parameter
        /// Format is localized
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetNiceDate(DateTime dateTime)
        {
            var dateDifference = DateTime.Now - dateTime;
            string niceDate;
            int formattedTime;

            // minutes
            if (dateDifference.TotalMinutes < 60)
            {
                formattedTime = (int) dateDifference.TotalMinutes;
                niceDate = formattedTime == 1
                    ? String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.SingleMinute)
                    : String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.MultipleMinutes);
            }
                // Hours
            else if (dateDifference.TotalHours < 24)
            {
                formattedTime = (int) dateDifference.TotalHours;
                niceDate = formattedTime == 1
                     ? String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.SingleHour)
                     : String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.MultipleHours);
            }
                // Days
            else if (dateDifference.TotalDays < 7)
            {
                formattedTime = (int) dateDifference.TotalDays;
                niceDate = formattedTime == 1
                     ? String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.SingleDay)
                     : String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.MultipleDays);
            }
                // Weeks
            else if (dateDifference.TotalDays < 31)
            {
                formattedTime = (int) Math.Round(dateDifference.TotalDays/7, 0);
                niceDate = formattedTime == 1
                     ? String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.SingleWeek)
                     : String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.MultipleWeeks);
            }
                // Months
            else if (dateDifference.TotalDays >= 31 && dateDifference.TotalDays < 365)
            {
                formattedTime = (int) Math.Round(dateDifference.TotalDays/31, 0);
                niceDate = formattedTime == 1
                     ? String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.SingleMonth)
                     : String.Format("{0} {1}", formattedTime, Localization.Languages.Business.Date.Date.MultipleMonths);
            }
                // Years
            else
            {
                DateTime now = DateTime.Today;

                int years = now.Year - dateTime.Year;
                int months = now.Month - dateTime.Month;

                if (dateTime.Day >= Math.Round((double)DateTime.DaysInMonth(dateTime.Year, dateTime.Month) / 2, 0))
                {
                    months++;
                }

                niceDate = String.Format("{0} {1} ", years, years == 1 ? Localization.Languages.Business.Date.Date.Year : Localization.Languages.Business.Date.Date.Years);

                niceDate += months == 1
                    ? Localization.Languages.Business.Date.Date.addMonth.Replace("{0}",
                        months.ToString(CultureInfo.InvariantCulture))
                    : Localization.Languages.Business.Date.Date.addMonths.Replace("{0}",
                        months.ToString(CultureInfo.InvariantCulture));
            }

            return niceDate;
        }

        /// <summary>
        /// Get the timezones from the servers computer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TimeZoneInfo> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().ToArray();
        }
    }
}
