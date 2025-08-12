using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Application.Tools
{
    public static class Convertor
    {
        public static long CalculateMB(this int mb)
        {
            return mb * 1024 * 1024;
        }

        public static string GetDisplayName(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DisplayAttribute attribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                return attribute.Name;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string GetDate(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();

            int day = pc.GetDayOfMonth(value);
            int month = pc.GetMonth(value);
            int year = pc.GetYear(value);

            string[] persianMonthNames = {
"فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
"مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"};

            string persianDate = $"{day} {persianMonthNames[month - 1]} {year}";
            return persianDate;
        }

        public static string GetDateTime(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();

            int day = pc.GetDayOfMonth(value);
            int month = pc.GetMonth(value);
            int year = pc.GetYear(value);
            int hour = pc.GetHour(value);
            int minute = pc.GetMinute(value);

            string[] persianMonthNames = {
"فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
"مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"};

            string persianDateTime = $"{day} {persianMonthNames[month - 1]} {year} {hour:D2}:{minute:D2}";
            return persianDateTime;
        }

        public static string ToToman(this long value)
        {
            return value.ToString("#,0 تومان");
        }
    }
}
