namespace Serenity.Data;

/// <summary>
/// Abstraction for row fields instance providers
/// </summary>
public interface IRowFieldsProvider
{
    /// <summary>
    /// Resolves the specified fields type.
    /// </summary>
    /// <param name="fieldsType">Type of the fields.</param>
    /// <returns></returns>
    RowFieldsBase Resolve(Type fieldsType);
    /// <summary>
    /// Resolves the with alias.
    /// </summary>
    /// <param name="fieldsType">Type of the fields.</param>
    /// <param name="alias">The alias.</param>
    /// <returns></returns>
    RowFieldsBase ResolveWithAlias(Type fieldsType, string alias);
}