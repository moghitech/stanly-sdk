using Refit;

namespace Fusion.Client.Attributes;

/// <summary>
/// PUT request source generator for Refit
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PutRequestAttribute(string path) : PutAttribute($"/{path}")
{
}