using Refit;

namespace Fusion.Client.Attributes;

/// <summary>
/// PATCH request source generator for Refit
/// </summary>
/// <param name="path"></param>
public class PatchRequestAttribute(string path) : PatchAttribute($"/{path}")
{
}