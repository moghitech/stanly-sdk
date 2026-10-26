using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Fusion.Constants;

namespace Fusion.FluentValidation.CustomValidators;

/// <summary>
/// PhoneNumber FluentValidation Custom validator
/// </summary>
/// <typeparam name="T"></typeparam>
internal class MobileNumberValidator<T> : PropertyValidator<T, string>
{
    private const string MobileNumberNullOrEmptyErrorMessage = "شماره تلفن همراه خالی است";
    private const string MobileNumberMustBeDigitOnlyErrorMessage =
        "شماره تلفن همراه باید تنها شامل اعداد باشد.";
    private const string MobileNumberLengthErrorMessage =
        "طول شماره تلفن همراه باید 11 کاراکتر باشد.";
    private const string MobileNumberFormatErrorMessage =
        "فرمت شماره تلفن همراه وارد شده صحیح نیست.";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                MobileNumberNullOrEmptyErrorMessage
            );
            return false;
        }

        if (value.Length != 11)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", MobileNumberLengthErrorMessage);
            return false;
        }

        if (!long.TryParse(value, out var longPhoneNumber))
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                MobileNumberMustBeDigitOnlyErrorMessage
            );
            return false;
        }

        if (longPhoneNumber is < 9000000000 or > 9999999999)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", MobileNumberFormatErrorMessage);
            return false;
        }

        // ReSharper disable once InvertIf
        if (!Regex.IsMatch(value, RegexConstants.Mobile))
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", MobileNumberFormatErrorMessage);
            return false;
        }

        return true;
    }

    public override string Name => nameof(MobileNumberValidator<T>);

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "{ErrorMessage}";
    }
}

/// <summary>
/// PhoneNumber FluentValidation Custom Validator Extension
/// </summary>
public static class ValidMobileNumberExtensions
{
    /// <summary>
    /// validate current string is in MobileNumber format. <br />
    /// also check: string.Empty, null, string length
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> ValidMobileNumber<T>(
        this IRuleBuilder<T, string> ruleBuilder
    ) => ruleBuilder.SetValidator(new MobileNumberValidator<T>());
}
