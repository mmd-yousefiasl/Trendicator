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

        private static readonly string[] PersianMonthNames =
        { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

        public static string GetDate(this DateTime value)
        {
            // تبدیل زمان به وقت محلی ایران با منطق اشتراکی
            DateTime localTime = value.NormalizeToIranTimeZone();

            PersianCalendar pc = new PersianCalendar();
            int day = pc.GetDayOfMonth(localTime);
            int month = pc.GetMonth(localTime);
            int year = pc.GetYear(localTime);

            return $"{day} {PersianMonthNames[month - 1]} {year}";
        }

        public static string GetDateTime(this DateTime value)
        {
            if (value == DateTime.MinValue || value.Year < 622)
            {
                return "ثبت نشده";
            }

            DateTime localTime = value.NormalizeToIranTimeZone();

            if (localTime.Year < 622)
            {
                return "ثبت نشده";
            }

            PersianCalendar pc = new PersianCalendar();
            int day = pc.GetDayOfMonth(localTime);
            int month = pc.GetMonth(localTime);
            int year = pc.GetYear(localTime);
            int hour = pc.GetHour(localTime);
            int minute = pc.GetMinute(localTime);

            return $"{day} {PersianMonthNames[month - 1]} {year} {hour:D2}:{minute:D2}";
        }

        public static string GetEndDateInDays(this DateTime utcDateTime)
        {
            var now = DateTime.UtcNow;

            if (now > utcDateTime) return "منقضی شده";

            var timeSpan = utcDateTime - now;
            var culture = new CultureInfo("fa-IR");

            if (timeSpan.TotalDays >= 1)
            {
                int days = (int)timeSpan.TotalDays;
                int hours = timeSpan.Hours;
                return hours > 0
                    ? $"{days.ToString(culture)} روز و {hours.ToString(culture)} ساعت"
                    : $"{days.ToString(culture)} روز";
            }

            if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)timeSpan.TotalHours;
                int minutes = timeSpan.Minutes;
                return minutes > 0
                    ? $"{hours.ToString(culture)} ساعت و {minutes.ToString(culture)} دقیقه"
                    : $"{hours.ToString(culture)} ساعت";
            }

            int totalMinutes = (int)timeSpan.TotalMinutes;
            int seconds = timeSpan.Seconds;

            if (totalMinutes == 0) return $"{seconds.ToString(culture)} ثانیه";

            return seconds > 0
                ? $"{totalMinutes.ToString(culture)} دقیقه و {seconds.ToString(culture)} ثانیه"
                : $"{totalMinutes.ToString(culture)} دقیقه";
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

        private static DateTime NormalizeToIranTimeZone(this DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc && value.Kind != DateTimeKind.Unspecified)
                return value;

            try
            {
                TimeZoneInfo winZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(value, winZone);
            }
            catch (TimeZoneNotFoundException)
            {
                TimeZoneInfo linuxZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
                return TimeZoneInfo.ConvertTime(value, linuxZone);
            }
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

        public static string ToToman(this decimal value)
        {
            return value.ToString("#,0 تومان");
        }

        public static string ToTomanFormat(this decimal value)
        {
            return value.ToString("#,0");
        }

        public static string ToTomanFormat(this int value)
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
    }
}
