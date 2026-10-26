using FluentValidation;
using FluentValidation.Validators;

namespace Fusion.FluentValidation.CustomValidators;

internal class NationalCodeValidator<T> : PropertyValidator<T, string>
{
    private const string NationalCodeNullOrEmptyErrorMessage = "شماره ملی خالی است";
    private const string NationalCodeMustBeDigitOnlyErrorMessage =
        "شماره ملی باید تنها شامل اعداد باشد.";
    private const string NationalCodeFormatErrorMessage = "فرمت شماره ملی وارد شده صحیح نیست.";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                NationalCodeNullOrEmptyErrorMessage
            );
            return false;
        }

        const int initialZeros = 2;
        const int nationalCodeLength = 10;
        if (value.Length is < nationalCodeLength - initialZeros or > nationalCodeLength)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", NationalCodeFormatErrorMessage);
            return false;
        }

        value = value.PadLeft(10, '0');

        if (!value.All(char.IsDigit))
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                NationalCodeMustBeDigitOnlyErrorMessage
            );
            return false;
        }

        var j = nationalCodeLength;
        var sum = 0;
        for (var i = 0; i < value.Length - 1; i++)
        {
            sum += (int)char.GetNumericValue(value[i]) * j--;
        }

        var remainder = sum % 11;
        var controlNumber = (int)char.GetNumericValue(value[9]);
        var isCorrectFormat =
            (remainder < 2 && controlNumber == remainder)
            || (remainder >= 2 && controlNumber == 11 - remainder);

        if (!isCorrectFormat)
            context.MessageFormatter.AppendArgument("ErrorMessage", NationalCodeFormatErrorMessage);

        return isCorrectFormat;
    }

    public override string Name => nameof(NationalCodeValidator<T>);

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "{ErrorMessage}";
    }
}

/// <summary>
/// NationalCode FluentValidation Custom Validator Extension
/// </summary>
public static class ValidNationalCodeExtensions
{
    /// <summary>
    /// validate current string is in NationalCode format <br />
    /// also check: string.Empty, null, string length
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> ValidNationalCode<T>(
        this IRuleBuilder<T, string> ruleBuilder
    ) => ruleBuilder.SetValidator(new NationalCodeValidator<T>());
}
