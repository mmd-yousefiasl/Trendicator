using System.ComponentModel.DataAnnotations;

namespace Application.ValidationAttributes
{
    public class RequiredEachAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is IEnumerable<string> list)
            {
                return list.All(item => !string.IsNullOrWhiteSpace(item));
            }

            return false;
        }
    }
}
