namespace Serenity.Data;

/// <summary>
/// Fallback row fields provider for cases where a IServiceProvider 
/// is not available.
/// </summary>
/// <seealso cref="IRowFieldsProvider" />
public class FallbackRowFieldsProvider : IRowFieldsProvider
{
    /// <summary>
    /// The instance
    /// </summary>
    public static FallbackRowFieldsProvider Instance = new FallbackRowFieldsProvider();
    private readonly ConcurrentDictionary<Type, RowFieldsBase> byType;
    private readonly ConcurrentDictionary<(Type type, string alias),
        RowFieldsBase> byTypeAndAlias;

    private FallbackRowFieldsProvider()
    {
        byType = new ConcurrentDictionary<Type, RowFieldsBase>();
        byTypeAndAlias = new ConcurrentDictionary<(Type, string), RowFieldsBase>();
    }

    /// <summary>
    /// Resolves the specified fields type.
    /// </summary>
    /// <param name="fieldsType">Type of the fields.</param>
    /// <returns></returns>
    public RowFieldsBase Resolve(Type fieldsType)
    {
        return byType.GetOrAdd(fieldsType, CreateType);
    }

    /// <summary>
    /// Resolves the with alias.
    /// </summary>
    /// <param name="fieldsType">Type of the fields.</param>
    /// <param name="alias">The alias.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">alias</exception>
    public RowFieldsBase ResolveWithAlias(Type fieldsType, string alias)
    {
        if (string.IsNullOrEmpty(alias))
            throw new ArgumentNullException(nameof(alias));

        return byTypeAndAlias.GetOrAdd((fieldsType, alias),
            tuple => CreateType(tuple.type, tuple.alias));
    }

    private RowFieldsBase CreateType(Type fieldsType)
    {
        return CreateType(fieldsType, null);
    }

    private RowFieldsBase CreateType(Type fieldsType, string alias)
    {
        var fields = (RowFieldsBase)Activator.CreateInstance(fieldsType);
        fields.Initialize(annotations: null, SqlSettings.DefaultDialect);

        if (alias != null)
            fields.ReplaceAliasWith(alias);
        fields.LockAlias();

        return fields;
    }
}