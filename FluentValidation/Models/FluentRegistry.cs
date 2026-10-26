using System.Reflection;
using Fusion.FluentValidation.Extensions;

namespace Fusion.FluentValidation.Models;

/// <summary>
/// Fluent registry provides some property and functions those related with FluentValidation
/// </summary>
/************************************************************************
 * YOU CAN CONFIGURE SOME OBJECTS THOSE RELATED WITH FLUENT VALIDATION  *
 *          SUCH AS SOME VALUES THAT YOU NEEDS THEM IN RUNTIME          *
 * PLEASE AVOID WRITING FLUENT RULES OR TRYING TO CONFIGURE FLUENT HERE *
 ************************************************************************/
public static class FluentRegistry
{
    /// <summary>
    /// The property fills at runtime once
    /// It holds a Dictionary(Type,Type)
    /// First type is request model
    /// Second type is request model validator
    /// </summary>
    /// <returns>Dictionary(Type,Type)</returns>
    public static Dictionary<Type, Type> Validators =>
        Assembly.GetExecutingAssembly()!.GetFluentValidators();
}
