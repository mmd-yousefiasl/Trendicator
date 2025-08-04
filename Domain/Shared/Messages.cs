namespace Domain.Shared
{
    public static class Messages
    {
        #region Error Messages

        public const string RequiredError = "{0} ضروری می باشد.";
        public const string OperationFailedError = "عملیات با خطا مواجه شد.";
        public const string ThisPhoneNumberAlreadyRegisteredError = "این شماره موبایل قبلا ثبت شده است.";
        public const string FileLengthError = "طول فایل بیش از حد مجاز است. لطفاً فایل کوچکتری انتخاب کنید.";
        public const string InvalidFileFormatError = "فرمت فایل نامعتبر است.";
        public const string RequiredAccessForRoleError = "حداقل یک دسترسی برای نقش انتخاب کنید.";
        public const string DuplicateUniqueNameForRoleError = "نام یکتا برای این نقش قبلاً ثبت شده است.";

        #endregion

        #region Success Messages
        public const string SuccessfullyDone = "عملیات با موفقیت به پایان رسید.";
        #endregion
    }
}
