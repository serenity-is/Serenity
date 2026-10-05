namespace Serenity;

/// <summary>
/// Contains static methods to determine prefix length for a list
/// E.g. to find the prefix that all the columns of a table have
/// </summary>
public static class PrefixHelper
{
    /// <summary>
    /// Determines the prefix length.
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="list">List of objects</param>
    /// <param name="getName">Gets the field name from a list element</param>
    /// <returns>The length of the common prefix, or <c>0</c> if there is none.</returns>
    /// <exception cref="ArgumentNullException">list or getName is null.</exception>
    public static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(getName);

        string? prefix = null;

        foreach (T obj in list)
        {
            string name = getName(obj);
            int length = name.IndexOf('_');
            if (length <= 0)
                return 0;

            if (prefix is null)
                prefix = name[..length];
            else if (!name.StartsWith(prefix, StringComparison.Ordinal))
                return 0;
        }

        return prefix is null ? 0 : prefix.Length + 1;
    }
}