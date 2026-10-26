using System.Text.RegularExpressions;

namespace Fusion.Extensions;

/// <summary>
/// The class provides functionalities on String
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Convert english numbers to persian
    /// </summary>
    /// <param name="str">Target as string</param>
    /// <returns>String</returns>
    public static string En2Fa(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.Replace("0", "۰")
            .Replace("1", "۱")
            .Replace("2", "۲")
            .Replace("3", "۳")
            .Replace("4", "۴")
            .Replace("5", "۵")
            .Replace("6", "۶")
            .Replace("7", "۷")
            .Replace("8", "۸")
            .Replace("9", "۹");
    }

    /// <summary>
    /// Convert persian numbers to english
    /// </summary>
    /// <param name="str">Target string</param>
    /// <returns>String</returns>
    public static string Fa2En(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            //This part is so wired
            //iphone numeric
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9");
    }

    /// <summary>
    /// Fill the key values inside a text<br/>
    /// Sample text: This is a test for [name]
    /// </summary>
    /// <param name="baseMessage">String</param>
    /// <param name="param">Params</param>
    /// <returns>String</returns>
    public static string FillKeyValues(
        this string baseMessage,
        params (string key, string value)[] param
    )
    {
        return param.Aggregate(
            baseMessage,
            (current, parameter) => current.Replace($"[{parameter.key}]", parameter.value)
        );
    }

    /// <summary>
    /// Convert arabic chars to persian
    /// </summary>
    /// <param name="str">Target string</param>
    /// <returns>String</returns>
    public static string FixPersianChars(this string str)
    {
        return str.Replace("ﮎ", "ک")
            .Replace("ﮏ", "ک")
            .Replace("ﮐ", "ک")
            .Replace("ﮑ", "ک")
            .Replace("ك", "ک")
            .Replace("ي", "ی")
            .Replace(" ", " ")
            .Replace("‌", " ")
            .Replace("ھ", "ه"); //.Replace("ئ", "ی");
    }

    /// <summary>
    /// use this extension for replace message template variables for sms or notification
    /// note: pass a tuple of (string key, string value) as params
    /// note: key must be without of any additional characters like '' or [] or "" or {}
    /// </summary>
    /// <param name="baseMessage">the message template</param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static string ReplaceTemplateVariables(
        this string baseMessage,
        params (string key, string value)[] param
    )
    {
        baseMessage = baseMessage.Fa2En().FixPersianChars();
        return param.Aggregate(
            baseMessage,
            (current, parameter) =>
            {
                string key = parameter.key.Fa2En().FixPersianChars();
                return current
                    .Replace($"[{key}]", parameter.value)
                    .Replace("{" + key + "}", parameter.value);
            }
        );
    }

    /// <summary>
    /// Change input string to url like case
    /// <example>this-is-a-case</example>
    /// </summary>
    public static string UrlCase(this string input)
    {
        return Regex.Replace(input, "([a-z])([A-Z])", "$1-$2").ToLower();
    }

    /// <summary>
    /// Change input string to snake case
    /// <example>this_is_a_case</example>
    /// </summary>
    public static string SnakeCase(this string input)
    {
        return Regex.Replace(input, "([a-z])([A-Z])", "$1_$2").ToLower();
    }

    /// <summary>
    /// Change input string to upper snake case
    /// <example>THIS_IS_A_CASE</example>
    /// </summary>
    public static string KeyCase(this string input)
    {
        return Regex.Replace(input, "([a-z])([A-Z])", "$1_$2").Replace(".", "_").ToUpper();
    }
}
