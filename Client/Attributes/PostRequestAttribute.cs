using Refit;

namespace Fusion.Client.Attributes;

/// <summary>
/// POST request source generator for Refit
/// </summary>
/// <param name="path"></param>
[AttributeUsage(AttributeTargets.Method)]
public class PostRequestAttribute(string path) : PostAttribute($"/{path}")
{
}