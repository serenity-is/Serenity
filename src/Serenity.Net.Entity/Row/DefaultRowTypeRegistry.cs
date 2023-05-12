namespace Serenity.Data;

/// <summary>
/// Default row type registry
/// </summary>
/// <seealso cref="IRowTypeRegistry" />
public class DefaultRowTypeRegistry : IRowTypeRegistry
{
    private readonly ITypeSource typeSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRowTypeRegistry"/> class.
    /// </summary>
    /// <param name="typeSource">The type source.</param>
    /// <exception cref="ArgumentNullException">typeSource</exception>
    public DefaultRowTypeRegistry(ITypeSource typeSource)
    {
        this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    }

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
            return Array.Empty<Type>();

        return AllRowTypes.Where(x => x.GetCustomAttribute<ConnectionKeyAttribute>()?.Value == connectionKey);
    }
}