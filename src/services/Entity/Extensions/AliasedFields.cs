namespace Serenity.Data;

/// <summary>
///   Contains extension method to create aliased fields
/// </summary>
public static class AliasedFields
{
    /// <summary>Aliases the fields with the specified alias.</summary>
    /// <typeparam name="TFields">The type of the fields.</typeparam>
    /// <param name="fields">The fields.</param>
    /// <param name="alias">The alias.</param>
    /// <returns>
    ///   An instance of RowFieldsBase with T0 replaced with specified alias
    /// </returns>
    /// <exception cref="ArgumentNullException">alias</exception>
    public static TFields As<TFields>(this TFields fields, string alias)
        where TFields : RowFieldsBase
    {
        if (string.IsNullOrWhiteSpace(alias))
            throw new ArgumentNullException(nameof(alias));

        if (alias == fields.AliasName)
            return fields;

        return (TFields)RowFieldsProvider.Current.ResolveWithAlias(typeof(TFields), alias);
    }
}