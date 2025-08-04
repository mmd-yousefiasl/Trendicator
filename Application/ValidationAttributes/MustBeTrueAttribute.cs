using System.ComponentModel.DataAnnotations;

namespace Application.ValidationAttributes
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is bool boolValue)
            {
                return boolValue;
            }
            return false;
        }
    }
}
