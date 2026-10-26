using Refit;

namespace Fusion.Client.Attributes;

/// <summary>
/// DELETE request source generator for Refit
/// </summary>

public class DeleteRequestAttribute(string path) : DeleteAttribute($"/{path}")
{
}