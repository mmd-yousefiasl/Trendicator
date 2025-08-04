namespace Application.Tools
{
    public static class Filter
    {
        public static FilterConditions<TEntity> GenerateConditions<TEntity>()
        {
            FilterConditions<TEntity> filterConditions = new FilterConditions<TEntity>();
            return filterConditions;
        }

        public static bool HasDefaultOrEmptyProperties(this object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                var value = prop.GetValue(obj);
                if (value is null) return true; // Check for null

                var type = prop.PropertyType;

                // Check for default values
                if (type.IsValueType && Equals(value, Activator.CreateInstance(type)))
                    return true;

                // Special case for strings
                if (value is string str && string.IsNullOrWhiteSpace(str))
                    return true;
            }
            return false;
        }
    }
}
