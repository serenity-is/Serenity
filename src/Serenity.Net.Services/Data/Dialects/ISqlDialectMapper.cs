namespace Serenity.Data;

/// <summary>
/// The sql dialect mapper
/// </summary>
public interface ISqlDialectMapper
{
    /// <summary>
    /// Returns dialect for a dialect or provider name
    /// </summary>
    /// <param name="dialectOrProviderName">The dialect name or provider name</param>
    ISqlDialect TryGet(string dialectOrProviderName);
}
