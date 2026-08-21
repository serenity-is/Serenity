namespace Serenity.Data;

/// <summary>
/// Abstraction for row fields instance providers
/// </summary>
public interface IRowFieldsProvider
{
    /// <summary>
    /// Resolves the fields instance for the specified fields type.
    /// </summary>
    /// <param name="fieldsType">Type of the fields.</param>
    /// <returns>The resolved fields instance for the specified type.</returns>
    RowFieldsBase Resolve(Type fieldsType);
    /// <summary>
    /// Resolves the fields instance for the specified fields type with the given alias applied.
    /// </summary>
    /// <param name="fieldsType">Type of the fields.</param>
    /// <param name="alias">The alias.</param>
    /// <returns>The resolved fields instance with the specified alias applied.</returns>
    RowFieldsBase ResolveWithAlias(Type fieldsType, string alias);
}