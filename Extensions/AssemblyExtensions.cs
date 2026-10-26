using System.Reflection;

namespace Fusion.Extensions;

/// <summary>
/// The class gives you some functionality to get your specified types
/// </summary>
public static class AssemblyExtensions
{
    private static readonly Dictionary<string, List<Type>> ChildTypes = new();

    /// <summary>
    /// Load all types based on parent type
    /// </summary>
    /// <returns>Child Types</returns>
    public static List<Type> LoadChildren(
        this AppDomain domain,
        Type parent,
        bool forceReload = false
    ) =>
        LoadChildren(
            domain,
            parent,
            t => t is { IsInterface: false, IsAbstract: false },
            forceReload
        );

    /// <summary>
    /// Load filtered types based on parent type
    /// </summary>
    /// <returns>Child Types</returns>
    public static List<Type> LoadChildren(
        this AppDomain domain,
        Type parent,
        Func<Type, bool> filter,
        bool forceReload = false
    ) => LoadChildren(domain, new[] { parent }, filter, forceReload);

    /// <summary>
    /// Load filtered types based on parent type
    /// </summary>
    /// <returns>Child Types</returns>
    public static List<Type> LoadChildren(
        this AppDomain domain,
        Type[] parents,
        Func<Type, bool> filter,
        bool forceReload = false
    )
    {
        var key = string.Join("_", parents.OrderBy(x => x.FullName).Select(x => x.FullName));

        if (forceReload || !ChildTypes.ContainsKey(key))
            ChildTypes[key] = domain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(t => parents.Any(p => p.IsAssignableFrom(t))))
                .ToList();

        return ChildTypes[key].Where(filter).ToList();
    }

    /// <summary>
    /// Returns available defined types
    /// The function is not efficient.Notice that don't use the function a lot in codebase.
    /// The usage should be once and on start of the application.
    /// </summary>
    /// <param name="domain">AppDomain</param>
    /// <returns>A list of TypeInfo</returns>
    public static List<TypeInfo> GetAvailableDefinedTypes(this AppDomain domain)
    {
        var assemblies = domain.GetAssemblies();

        var definedTypes = new List<TypeInfo>();

        foreach (var assembly in assemblies)
        {
            try
            {
                definedTypes.AddRange(assembly.DefinedTypes);
            }
            catch
            {
                //ignored
            }
        }

        return definedTypes;
    }

    /// <summary>
    /// Returns available types
    /// The function is not efficient.Notice that don't use the function a lot in codebase.
    /// The usage should be once and on start of the application.
    /// </summary>
    /// <param name="domain">AppDomain</param>
    /// <returns>A list of Type</returns>
    public static List<Type> GetAvailableTypes(this AppDomain domain)
    {
        var assemblies = domain.GetAssemblies();

        var types = new List<Type>();
        foreach (var assembly in assemblies)
        {
            try
            {
                types.AddRange(assembly.GetTypes());
            }
            catch
            {
                // ignored
            }
        }

        return types;
    }

    /// <summary>
    /// Get a list of all types matching whereClause from all loadable assemblies
    /// </summary>
    /// <param name="whereClause"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllLoadableAssemblyTypes(Func<Type, bool> whereClause)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(GetLoadableTypes)
            .Where(whereClause);
    }

    /// <summary>
    /// Get all types included in an assembly which are loadable
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <see>
    ///     <cref>https://stackoverflow.com/a/29379834/1818708</cref>
    /// </see>
    public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
        if (assembly is null)
            throw new ArgumentNullException(nameof(assembly));

        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null)!;
        }
    }

    /// <summary>
    /// Is used to determine if a type (Class A) implemented & inherited from another type (Class B)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="lookup">target type (class B) to check if inherited (by class A)</param>
    /// <returns></returns>
    public static bool IsInheritedFrom(this Type type, Type lookup)
    {
        while (true)
        {
            var baseType = type.BaseType;
            if (baseType is null)
                return false;

            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == lookup)
                return true;

            type = baseType;
        }
    }
}
