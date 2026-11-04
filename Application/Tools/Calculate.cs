namespace Application.Tools
{
    public static class Calculate
    {
        public static int GetDiscountPercent(this decimal value, decimal afterDiscount)
        {
            if (value <= 0 || afterDiscount <= 0) return 0;

            int discountedPercent = (int)((afterDiscount * 100) / value);

            return 100 - discountedPercent;
        }

        public static decimal GetDiscountAmount(this decimal value, int percent)
        {
            return value * percent / 100;
        }

        public static decimal GetAmount(this decimal value, int taxRate)
        {
            return value * taxRate / 100;
        }
    }
}