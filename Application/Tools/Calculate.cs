namespace Application.Tools
{
    public static class Calculate
    {
        public static int GetDiscountPercent(this long value, long afterDiscount)
        {
            if (value <= 0 || afterDiscount <= 0) return 0;

            int discountedPercent = (int)((afterDiscount * 100) / value);

            return 100 - discountedPercent;
        }

        public static long GetTaxAmount(this long value, int taxRate)
        {
            return value * taxRate / 100;
        }
    }
}
