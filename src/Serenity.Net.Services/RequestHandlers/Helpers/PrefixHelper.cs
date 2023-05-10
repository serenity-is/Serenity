namespace Serenity;

/// <summary>
/// Contains static methods to determine prefix length for a list
/// E.g. to find the prefix that all the columns of a table have
/// </summary>
public static class PrefixHelper
{
    /// <summary>
    /// Determines the prefix length
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="list">List of objects</param>
    /// <param name="getName">Gets the field name from a list element</param>
    /// <returns></returns>
    public static int DeterminePrefixLength<T>(IEnumerable<T> list, Func<T, string> getName)
    {
        if (!Enumerable.Any(list))
            return 0;
        string str1 = getName(Enumerable.First(list));
        int length = str1.IndexOf('_');
        if (length <= 0)
            return 0;
        string str2 = str1.Substring(0, length);
        foreach (T obj in list)
        {
            if (!getName(obj).StartsWith(str2))
                return 0;
        }
        return str2.Length + 1;
    }
}