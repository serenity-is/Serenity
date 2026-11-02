namespace Serenity.Web;

/// <summary>
/// Declares the client side types implementing the <c>LazyLoadInclude</c> marker of corelib and the module that
/// loads them, which is generated for a project that has such types. <c>Html.LazyTypes()</c> renders these for the
/// default <c>Config.lazyTypeLoader</c>, which uses them to load a type when it is first requested from a type registry.
/// </summary>
/// <param name="path">The app relative (<c>~/</c>) path of the module that exports the loaders of the types.</param>
/// <param name="keys">The keys the types are registered with on the client side.</param>
/// <exception cref="ArgumentNullException">One of the arguments is <c>null</c>.</exception>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class LazyLoadTypesAttribute(string path, params string[] keys) : Attribute
{
    /// <summary>
    /// Gets the app relative (<c>~/</c>) path of the module that exports the loaders of the types.
    /// </summary>
    public string Path { get; } = path ?? throw new ArgumentNullException(nameof(path));

    /// <summary>
    /// Gets the keys the types are registered with on the client side.
    /// </summary>
    public string[] Keys { get; } = keys ?? throw new ArgumentNullException(nameof(keys));
}
