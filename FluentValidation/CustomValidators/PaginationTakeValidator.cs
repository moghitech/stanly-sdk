using FluentValidation;
using FluentValidation.Validators;
using Fusion.Microservice.Models;

namespace Fusion.FluentValidation.CustomValidators;

internal class ValidatePagination<T> : PropertyValidator<T, PaginationFilter>
{
    private const int MaxPaginationTake = 100;
    private const int MinPaginationTake = 1;
    private const int MinPaginationSkip = 0;

    private readonly string _takeIsNotInRangeMessageConstants =
        $"تعداد آیتم درخواستی باید بین {MinPaginationTake} و {MaxPaginationTake} باشد.";

    private readonly string _skipIsNotInRangeMessageConstants =
        $"تعداد پرش آیتم ها باید بزرگتر از {MinPaginationSkip} باشد.";

    public override bool IsValid(ValidationContext<T> context, PaginationFilter value)
    {
        if (value.Take is < MinPaginationTake or > MaxPaginationTake)
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                _takeIsNotInRangeMessageConstants
            );
            return false;
        }

        if (value.Take < MinPaginationSkip)
        {
            context.MessageFormatter.AppendArgument(
                "ErrorMessage",
                _skipIsNotInRangeMessageConstants
            );
            return false;
        }

        return true;
    }

    public override string Name => nameof(ValidatePagination<T>);

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "{ErrorMessage}";
    }
}

/// <summary>
/// Pagination Take FluentValidation Custom Validator Extension
/// </summary>
public static class ValidPaginationTakeExtensions
{
    /// <summary>
    /// validate pagination take skip. <br/>
    /// Take: it should be less-than 100 and gather-than 0 <br/>
    /// Skip: it should be gather-than or equal 0
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, PaginationFilter> Pagination<T>(
        this IRuleBuilder<T, PaginationFilter> ruleBuilder
    ) => ruleBuilder.SetValidator(new ValidatePagination<T>());
}
