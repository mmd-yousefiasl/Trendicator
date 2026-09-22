namespace Application.Tools
{
    public static class Generate
    {
        public static string UserLoginCode()
        {
            Random rnd = new Random();
            string code = "";

            for (int i = 0; i < 5; i++)
            {
                code += rnd.Next(0, 10);
            }

            return code;
        }

        public static string GenerateUniqueStringCode() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
