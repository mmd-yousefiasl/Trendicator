using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Enums.Payment
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VerifyPaymentResultStatusEnum
    {
        [Display(Name = "در انتظار پردخت")]
        PendingPayment = -1,
        [Display(Name = "خطای داخلی")]
        InternalError = -2,
        [Display(Name = "پرداخت شده - تاییدشده")]
        PaidAndVerified = 1,
        [Display(Name = "پرداخت شده - تاییدنشده")]
        PaidNotVerified = 2,
        [Display(Name = "لغوشده توسط کاربر")]
        CancelledByUser = 3,
        [Display(Name = "شماره کارت نامعتبر می‌باشد.")]
        InvalidCardNumber = 4,
        [Display(Name = "موجودی حساب کافی نمی‌باشد.")]
        InsufficientFunds = 5,
        [Display(Name = "رمز واردشده اشتباه می‌باشد.")]
        IncorrectPassword = 6,
        [Display(Name = "تعداد درخواست‌ها بیش از حد مجاز می‌باشد.")]
        TooManyRequests = 7,
        [Display(Name = "تعداد پرداخت اینترنتی روزانه بیش از حد مجاز می‌باشد.")]
        DailyPaymentLimitExceeded = 8,
        [Display(Name = "مبلغ پرداخت اینترنتی روزانه بیش از حد مجاز می‌باشد.")]
        DailyAmountLimitExceeded = 9,
        [Display(Name = "صادرکننده‌ی کارت نامعتبر می‌باشد.")]
        InvalidCardIssuer = 10,
        [Display(Name = "خطای سوییچ")]
        SwitchError = 11,
        [Display(Name = "کارت قابل دسترسی نمی‌باشد.")]
        CardNotAccessible = 12,
        [Display(Name = "تراکنش استرداد شده")]
        TransactionRefunded = 15,
        [Display(Name = "تراکنش در حال استرداد")]
        TransactionRefundInProgress = 16,
        [Display(Name = "تراکنش ریورس شده")]
        TransactionReversed = 18,
        [Display(Name = "پذیرنده نامعتبر است")]
        InvalidMerchant = 21
    }
}
