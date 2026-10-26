namespace Fusion.Constants;

internal static class RegexConstants
{
    public const string Mobile = @"(09)[01239][0-9]\d{7}";

    public const string PersianDigitMobile =
        @"([\u06F0\u06F9])[\u06F0\u06F1\u06F2\u06F3\u06F9][\u06F0-\u06F9][\u06F0-\u06F9]{7}";

    public const string Email =
        @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
}
