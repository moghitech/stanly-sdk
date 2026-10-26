using Fusion.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fusion.Microservice.Attributes;

/// <summary>
/// The attribute use to generate response type in swagger when you have 200 status.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ProducesSuccessAttribute : ProducesResponseTypeAttribute
{
    /// <summary>
    /// Initialize an instance by given type
    /// </summary>
    /// <param name="type">Response Type</param>
    /// <param name="isCollectionType">Set true if your type is collection. The value is false by default</param>
    /// <example>
    /// Checkout the example blow if you have a collection type
    /// <code>
    /// [ProducesSuccess(typeof(Foo),isCollectionType=true)]
    /// </code>
    /// if you have a non list response type see the example blow
    /// <code>
    /// [ProducesSuccess(typeof(Foo))]
    /// </code>
    /// </example>
    public ProducesSuccessAttribute(Type type, bool isCollectionType = false)
        : base(GetResponseType(type, isCollectionType), StatusCodes.Status200OK) { }

    /// <summary>
    /// Initialize an instance by given type
    /// </summary>
    /// <param name="type">Response Type</param>
    /// <param name="isCollectionType">Set true if your type is collection</param>
    /// <param name="contentType">Specified ContentType</param>
    public ProducesSuccessAttribute(Type type, bool isCollectionType, string contentType)
        : base(GetResponseType(type, isCollectionType), StatusCodes.Status200OK, contentType) { }

    /// <summary>
    /// Initialize an instance by given type
    /// </summary>
    /// <param name="type">Response Type</param>
    /// <param name="isCollectionType">Set true if your type is collection</param>
    /// <param name="contentType">Specified content type</param>
    /// <param name="additionalParams">Additional Params</param>
    public ProducesSuccessAttribute(
        Type type,
        bool isCollectionType,
        string contentType,
        params string[] additionalParams
    )
        : base(
            GetResponseType(type, isCollectionType),
            StatusCodes.Status200OK,
            contentType,
            additionalParams
        ) { }

    /// <summary>
    /// The function returns response type that use for swagger schema
    /// Notice that the responses that the function generates, are inside the ApiResult class
    /// </summary>
    /// <param name="type">The original type</param>
    /// <param name="isCollectionType">A flag that demonstrate is the type collection or not. if the value set as true the response will generate inside a list</param>
    /// <returns>A type to generate swagger schema</returns>
    private static Type GetResponseType(Type type, bool isCollectionType = false)
    {
        return isCollectionType
            ? typeof(ApiResult<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(type))
            : typeof(ApiResult<>).MakeGenericType(type);
    }
}
