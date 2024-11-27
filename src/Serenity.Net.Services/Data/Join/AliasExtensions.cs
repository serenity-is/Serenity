namespace Serenity.Data;

/// <summary>
/// Contains extension methods for Alias objects
/// </summary>
public static class AliasExtensions
{
    /// <summary>
    /// Adds a WITH(NOLOCK) to the alias and returns a new alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns></returns>
    public static Alias WithNoLock(this IAlias alias)
    {
        if (string.IsNullOrEmpty(alias.Table))
            return new Alias(alias.Name + " WITH(NOLOCK)");

        return new Alias(alias.Table, alias.Name + " WITH(NOLOCK)");
    }
}