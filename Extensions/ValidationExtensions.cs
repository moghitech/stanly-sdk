using System.Text.RegularExpressions;
using Fusion.Constants;

namespace Fusion.Extensions;

/// <summary>
/// Validation Extensions
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Check is picture file (".jpg", ".jpeg", ".gif", ".png")
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsImageFile(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return ValidFileTypeConstants.ValidImageFileTypes.Contains(extension);
    }

    /// <summary>
    /// Check is sound file (".mp3", ".m4a", ".wave", ".mp4", ".avi", ".aac", ".ogg", ".acc")
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsSoundFile(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return ValidFileTypeConstants.ValidSoundFileTypes.Contains(extension);
    }

    /// <summary>
    /// Check is  validFile (".pdf", ".doc", ".docx", ".zip", ".rar")
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsAcceptableChatFile(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return ValidFileTypeConstants.ChatAcceptableFileTypes.Contains(extension);
    }

    /// <summary>
    /// Check is pdf file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsPdfFile(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return ValidFileTypeConstants.ValidPdfFileTypes.Contains(extension);
    }

    /// <summary>
    /// Check is excel file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsExcelFile(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return ValidFileTypeConstants.ValidExcelFileTypes.Contains(extension);
    }

    /// <summary>
    /// Check string for validation of email address
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        var isEmail = Regex.IsMatch(email, RegexConstants.Email, RegexOptions.IgnoreCase);
        return isEmail;
    }

    /// <summary>
    /// Check string for validation of phone (mobile) number
    /// </summary>
    /// <param name="mobileNumber"></param>
    /// <returns></returns>
    public static bool IsValidMobileNumber(this string mobileNumber)
    {
        if (string.IsNullOrEmpty(mobileNumber))
            return false;

        if (mobileNumber.Length is not 11)
            return false;

        var isValidNumber = long.TryParse(mobileNumber,out var mobileNumberInLongType);
        if(!isValidNumber)
            return false;
            
        switch (mobileNumberInLongType)
        {
            case < 9000000000
                or > 9999999999:
                return false;
        }

        return Regex.IsMatch(mobileNumber, RegexConstants.Mobile);
    }

    /// <summary>
    /// Check the input is a number or not
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsNumber(this string input)
    {
        return !string.IsNullOrEmpty(input) && input.All(char.IsDigit);
    }

    /// <summary>
    /// Check string for validation of persian national code
    /// </summary>
    /// <param name="nationalCode"></param>
    /// <returns></returns>
    public static bool IsValidNationalCode(this string nationalCode)
    {
        if (string.IsNullOrEmpty(nationalCode))
            return false;

        if (nationalCode.Length is not 10)
            return false;

        nationalCode = nationalCode.Fa2En();

        if (!nationalCode.IsNumber())
            return false;

        var allDigitEqual = new[]
        {
            "0000000000",
            "1111111111",
            "2222222222",
            "3333333333",
            "4444444444",
            "5555555555",
            "6666666666",
            "7777777777",
            "8888888888",
            "9999999999"
        };
        if (allDigitEqual.Contains(nationalCode))
            return false;

        var chArray = nationalCode.ToCharArray();
        var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
        var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
        var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
        var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
        var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
        var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
        var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
        var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
        var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
        var a = Convert.ToInt32(chArray[9].ToString());

        var b = num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
        var c = b % 11;

        return c < 2 && a == c || c >= 2 && 11 - c == a;
    }
}