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

        public static string ToToman(this decimal value)
        {
            return value.ToString("#,0 تومان");
        }

        public static string ToTomanFormat(this decimal value)
        {
            return value.ToString("#,0");
        }

        public static int RoundToHigher(this decimal value)
        {
            return (int)Math.Ceiling(value);
        }

        public static decimal GetRateAvrage(this int sum, int total)
        {
            if (total < 1) return 0;

            return Math.Round((decimal)sum / total, 1);
        }

        public static DateTime ConvertToMiladi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            return new DateTime(value.Year, value.Month, value.Day, pc);
        }

        public static DateTime ConvertToShamsi(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();
            int year = pc.GetYear(value);
            int month = pc.GetMonth(value);
            int day = pc.GetDayOfMonth(value);

            return new DateTime(year, month, day, new GregorianCalendar());
        }

        public static DateTime ConvertToShamsi(this DateTime? value)
        {
            if (value is null) value = DateTime.UtcNow;

            PersianCalendar pc = new PersianCalendar();
            int year = pc.GetYear(value.Value);
            int month = pc.GetMonth(value.Value);
            int day = pc.GetDayOfMonth(value.Value);

            return new DateTime(year, month, day, new GregorianCalendar());
        }

        public static int GetRemainingCooldownSeconds(this DateTime createdDate, int cooldownSeconds = 120)
        {
            var expiryTime = createdDate.AddSeconds(cooldownSeconds);
            var remaining = (expiryTime - DateTime.UtcNow).TotalSeconds;

            return remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
        }

        public static string ToPersianRelativeTime(this DateTime utcDateTime)
        {
            var now = DateTime.UtcNow;

            if (utcDateTime > now) return "لحظاتی پیش";

            var timeSpan = now - utcDateTime;

            string result = timeSpan.TotalSeconds switch
            {
                < 60 => "لحظاتی پیش",
                < 3600 => $"{Math.Floor(timeSpan.TotalMinutes)} دقیقه قبل",
                < 86400 => $"{Math.Floor(timeSpan.TotalHours)} ساعت قبل",
                _ => $"{Math.Floor(timeSpan.TotalDays)} روز پیش"
            };

            return result;
        }
    }
}
