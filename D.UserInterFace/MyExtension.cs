using System;
using System.Globalization;

namespace D.UserInterFace
{
    public static class MyExtension
    {
        public static int GetWeekNumberOfMonth(this DateTime date)
        {
            return GetWeekNumberOfMonth(date, CultureInfo.CurrentCulture);
        }

        public static int GetWeekNumberOfMonth(this DateTime date, CultureInfo culture)
        {
            return date.GetWeekNumber(culture)
                 - new DateTime(date.Year, date.Month, 1).GetWeekNumber(culture)
                 + 1; // Or skip +1 if you want the first week to be 0.
        }

        public static int GetWeekNumber(this DateTime date, CultureInfo culture)
        {
            return culture.Calendar.GetWeekOfYear(date,
                culture.DateTimeFormat.CalendarWeekRule,
                culture.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime UAETime()
        {
            TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            DateTime utc = DateTime.UtcNow;
            DateTime UAE = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);
            return UAE;
        }

        public static DateTime ConvertTimeZone(string timezone)
        {
            TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            DateTime utc = DateTime.Now;
            DateTime uae = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);
            return uae;
        }

        public static string ToUpperIgnoreNull(this string value)
        {
            if (value != null)
            {
                value = value.ToUpper(CultureInfo.InvariantCulture);
            }
            return value;
        }
    }


}