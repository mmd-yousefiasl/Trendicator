using System.ComponentModel.DataAnnotations;
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
            return value.ToString("dd MMMM yyyy");
        }

        public static string GetDateTime(this DateTime value)
        {
            return value.ToString("dd MMMM yyyy HH:mm");
        }
    }
}
