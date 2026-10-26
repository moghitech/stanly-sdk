using System.Reflection;
using FluentValidation;

namespace Fusion.FluentValidation.Extensions;

/**********************************************************************
 *             AS YOU KNOW HERE IS FLUENT VALIDATION AREA             *
 * PLEASE JUST DEVELOP FUNCTIONS THOSE RELATED WITH FLUENT VALIDATION *
 **********************************************************************/
/// <summary>
/// An extension class on Assembly object.
/// It provides some functionality to use
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Returns a dictionary that contains request models and its validators
    /// </summary>
    /// <returns></returns>
    public static Dictionary<Type, Type> GetFluentValidators(this Assembly assembly)
    {
        //Our base type is here
        var abstractValidatorType = typeof(AbstractValidator<>);

        //The dictionary that carry our types
        //Key is Request model
        //Value is Request model validator
        var genericTypeDictionary = new Dictionary<Type, Type>();

        //Here load types those inherited from AbstractValidator<T>
        var validatorTypes = assembly
            .GetTypes()
            .Where(
                type =>
                    type.BaseType is not null
                    && !type.IsAbstract
                    && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition() == abstractValidatorType
            );

        //Filling our dictionary
        foreach (var type in validatorTypes)
        {
            var genericArgument = type.BaseType!.GetGenericArguments().FirstOrDefault();
            if(genericArgument is null)
                continue;
            genericTypeDictionary.TryAdd(genericArgument, type);
        }

        return genericTypeDictionary;
    }
}
