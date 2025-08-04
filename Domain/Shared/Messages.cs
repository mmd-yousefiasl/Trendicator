namespace Domain.Shared
{
    public static class Messages
    {
        #region Error Messages

        public const string RequiredError = "{0} ضروری می باشد";
        public const string OperationFailedError = "عملیات با خطا مواجه شد";
        public const string ThisPhoneNumberAlreadyRegisteredError = "این شماره موبایل قبلا ثبت شده است";
        public const string FileLengthError = "طول فایل بیش از حد مجاز است. لطفاً فایل کوچکتری انتخاب کنید";
        public const string InvalidFileFormatError = "فرمت فایل نامعتبر است";
        public const string RequiredAccessForRoleError = "حداقل یک دسترسی برای نقش انتخاب کنید";
        public const string DuplicateUniqueNameForRoleError = "نام یکتا برای این نقش قبلاً ثبت شده است";
        public const string MaxLengthError = "تعداد کاراکتر بیشتر از حد مجاز می باشد ({1})";
        public const string MobileFormatError = "موبایل نامعتبر می باشد";
        public const string ToManyLoginAttemptError = "تعداد تلاش‌های شما برای ورود از حد مجاز فراتر رفته است. لطفاً بعد از دقایقی دوباره تلاش کنید";
        public const string InvalidLoginCodeError = "کد تایید نامعتبر می باشد.";
        public const string LoginFailedError = "ورود ناموفق، لطفا دوباره امتحان کنید";

        #endregion

        #region Success Messages
        public const string SuccessfullyDone = "عملیات با موفقیت به پایان رسید";
        public const string LogedInSuccessfully = "با موفقیت وارد حساب کاربری خود شدید";
        public const string LogedoutSuccessfully = "با موفقیت از حساب کاربری خود خارج شدید";
        public const string LoginCodeSentSuccessfully = "کد ورود با موفقیت ارسال شد.";
        #endregion
    }
}
