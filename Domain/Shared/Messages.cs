namespace Domain.Shared
{
    public static class Messages
    {
        #region Error Messages

        public const string RequiredError = "{0} ضروری می باشد";
        public const string RangeError = "حداقل مقدار {0} {1} و حداکثر {2} می باشد";
        public const string OperationFailedError = "عملیات با خطا مواجه شد";
        public const string ThisPhoneNumberAlreadyRegisteredError = "این شماره موبایل قبلا ثبت شده است";
        public const string FileLengthError = "طول فایل بیش از حد مجاز است. لطفاً فایل کوچکتری انتخاب کنید";
        public const string InvalidFileFormatError = "فرمت فایل نامعتبر است";
        public const string RequiredAccessForRoleError = "حداقل یک دسترسی برای نقش انتخاب کنید";
        public const string DuplicateUniqueNameForRoleError = "نام یکتا برای این نقش قبلاً ثبت شده است";
        public const string MaxLengthError = "تعداد کاراکتر بیشتر از حد مجاز می باشد ({1})";
        public const string MinLengthError = "تعداد کاراکتر کمتر از حد مجاز می باشد ({1})";
        public const string MobileFormatError = "موبایل نامعتبر می باشد";
        public const string ToManyLoginAttemptError = "تعداد تلاش‌های شما برای ورود از حد مجاز فراتر رفته است. لطفاً بعد از دقایقی دوباره تلاش کنید";
        public const string InvalidLoginCodeError = "کد تایید نامعتبر می باشد.";
        public const string LoginFailedError = "ورود ناموفق، لطفا دوباره امتحان کنید";
        public const string CantDeleteThisRecordError = "نمی‌توان این رکورد را حذف کرد";
        public const string DuplicateSlugError = "عنوان در مرورگر تکراری می باشد";
        public const string ThisEmailAlreadyRegisterdError = "این ایمیل قبلا ثبت شده است";
        public const string FromDateCantBeGreaterThanToDateError = "تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد";
        public const string CouponCodeDuplicateError = "این کد تخفیف قبلا ثبت شده است";
        public const string ProductAlreadyInYourCartError = "این محصول قبلاً به سبد خرید شما اضافه شده است";
        public const string AlreadyUsedThisCouponError = "قبلا از این کد تخفیف استفاده کرده اید";
        public const string InvalidCouponCodeError = "کد تخفیف نامعتبر می باشد";
        public const string RemoveUnavailableItemsBeforeProceedToCheckout = "لطفاً اقلام ناموجود را قبل از ادامه فرایند پرداخت حذف کنید";
        public const string AddAProductAtLeastToCartBeforeProceedToCheckout = "لطفاً پیش از ادامه فرایند پرداخت، حداقل یک محصول را به سبد خرید اضافه کنید";
        public const string AddYourTradingViewAccountNameError = "لطفاً نام حساب تریدینگ ویو خود را وارد کنید";
        public const string PaymentFailedError = "پرداخت با شکست مواجه شد";
        public const string TransactionAlreadyVerified = "تراکنش قبلاً تأیید شده است";
        public const string InSufficentBalanceError = "موجودی کافی برای انجام تراکنش وجود ندارد";
        public const string CheckTermsAndConditionsError = "پذیرش قوانین و شرایط برای ادامه الزامی است";
        public const string CaptchaFailedError = "خطای کد امنیتی، لطفا دوباره امتحان کنید";
        public const string LoginCodeExpiredError = "کد تایید شما منقضی شده است. لطفا مجددا درخواست کد کنید";
        public const string InvalidEmailError = "لطفا یک ایمیل معتبر وارد کنید";

        #endregion

        #region Success Messages

        public const string SuccessfullyDone = "عملیات با موفقیت به پایان رسید";
        public const string LogedInSuccessfully = "با موفقیت وارد حساب کاربری خود شدید";
        public const string LogedoutSuccessfully = "با موفقیت از حساب کاربری خود خارج شدید";
        public const string LoginCodeSentSuccessfully = "کد ورود با موفقیت ارسال شد";
        public const string ReviewAddedSuccessfully = "نظر شما با موفقیت ثبت شد";
        public const string AddedToCartSuccessfully = "محصول با موفقیت به سبد خرید اضافه شد";
        public const string RemovedFromCartSuccessfully = "محصول با موفقیت از سبد خرید حذف شد";
        public const string DiscountAppliedSuccessfully = "تخفیف با موفقیت اعمال شد";
        public const string PaymentDoneSuccessfully = "پرداخت با موفقیت انجام شد";
        public const string TicketSentSuccessfully = "تیکت با موفقیت ارسال شد";
        public const string CommentSentSuccessfully = "دیدگاه شما ثبت شد و پس از تأیید نمایش داده می‌شود.";

        #endregion
    }
}
