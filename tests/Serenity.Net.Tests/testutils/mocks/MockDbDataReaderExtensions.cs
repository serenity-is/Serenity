namespace Serenity.TestUtils;

/// <summary>
/// Helpers to create <see cref="MockDbDataReader"/> instances aligned to the columns
/// of the query being intercepted, so tests can supply row values by column name
/// without worrying about select order or missing columns.
/// </summary>
public static class MockDbDataReaderExtensions
{
    private static string[]? GetColumnNames(SqlQuery? query)
    {
        if (query is not ISqlQueryExtensible ext || ext.Columns.Count == 0)
            return null;

        return [.. ext.Columns.Select(c =>
            c.ColumnName ?? (c.IntoField as IField)?.ColumnAlias ?? c.Expression)];
    }

    /// <summary>
    /// Creates a reader for the intercepted query, mapping anonymous items to its
    /// columns by name. Missing columns are read as DBNull.
    /// </summary>
    public static MockDbDataReader ToMockReader(this InterceptExecuteReaderArgs args, params object[] anonymousItems)
    {
        ArgumentNullException.ThrowIfNull(anonymousItems);
        var columnNames = GetColumnNames(args?.Query);
        return columnNames == null
            ? new MockDbDataReader(anonymousItems)
            : new MockDbDataReader(columnNames, anonymousItems);
    }

    /// <summary>
    /// Creates a reader for the intercepted query, mapping dictionary entries to its
    /// columns by key. Missing columns are read as DBNull.
    /// </summary>
    public static MockDbDataReader ToMockReader(this InterceptExecuteReaderArgs args,
        IEnumerable<IDictionary<string, object?>> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var columnNames = GetColumnNames(args?.Query) ?? items.FirstOrDefault()?.Keys.ToArray() ?? [];
        return new MockDbDataReader(columnNames, items);
    }

    /// <summary>
    /// Creates a single row reader for the intercepted query, mapping dictionary entries
    /// to its columns by key. Missing columns are read as DBNull.
    /// </summary>
    public static MockDbDataReader ToMockReader(this InterceptExecuteReaderArgs args, IDictionary<string, object?> item)
        => args.ToMockReader([item]);
}