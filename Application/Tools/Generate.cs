namespace Application.Tools
{
    public static class Generate
    {
        public static string GenerateUniqueStringCode() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
