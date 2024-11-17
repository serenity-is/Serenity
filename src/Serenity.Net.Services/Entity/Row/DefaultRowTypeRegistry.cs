namespace Serenity.Data;

/// <summary>
/// Default row type registry
/// </summary>
/// <seealso cref="IRowTypeRegistry" />
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultRowTypeRegistry"/> class.
/// </remarks>
/// <param name="typeSource">The type source.</param>
/// <exception cref="ArgumentNullException">typeSource</exception>
public class DefaultRowTypeRegistry(ITypeSource typeSource) : IRowTypeRegistry
{
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

    /// <summary>
    /// Gets all row types.
    /// </summary>
    /// <value>
    /// All row types.
    /// </value>
    public IEnumerable<Type> AllRowTypes => typeSource
        .GetTypesWithInterface(typeof(IRow))
        .Where(x => !x.IsAbstract && !x.IsInterface);

    /// <summary>
    /// Returns row types by the connection key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>Row types by the connection key</returns>
    public IEnumerable<Type> ByConnectionKey(string connectionKey)
    {
        if (string.IsNullOrEmpty(connectionKey))
            return [];

        return AllRowTypes.Where(x => x.GetCustomAttribute<ConnectionKeyAttribute>()?.Value == connectionKey);
    }
}