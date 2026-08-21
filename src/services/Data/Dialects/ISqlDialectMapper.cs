namespace Serenity.Data;

/// <summary>
/// Maps a dialect or provider name to an <see cref="ISqlDialect"/> instance.
/// </summary>
public interface ISqlDialectMapper
{
    /// <summary>
    /// Returns the dialect for a dialect or provider name, or <c>null</c> if none is found.
    /// </summary>
    /// <param name="dialectOrProviderName">The dialect name or provider name.</param>
    /// <returns>The matching <see cref="ISqlDialect"/>, or <c>null</c> if no match is found.</returns>
    ISqlDialect TryGet(string dialectOrProviderName);
}
