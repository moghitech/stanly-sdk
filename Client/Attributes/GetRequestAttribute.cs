using Refit;

namespace Fusion.Client.Attributes;

/// <summary>
/// GET request source generator for Refit
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class GetRequestAttribute(string path) : GetAttribute($"/{path}")
{
}