using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Criteria object with one value
/// </summary>
/// <seealso cref="BaseCriteria" />
/// <remarks>
/// Initializes a new instance of the <see cref="ValueCriteria"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class ValueCriteria(object? value) : BaseCriteria
{
    private readonly object? value = Snapshot(value);

    private static object? Snapshot(object? value)
    {
        if (value is Array array)
            return array.Clone();

        var valueType = value?.GetType();
        if (value is IList && valueType?.IsGenericType == true &&
            valueType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var toArray = valueType.GetMethod(nameof(List<int>.ToArray), Type.EmptyTypes)!;
            return toArray.Invoke(value, null);
        }

        return value is IEnumerable enumerable && value is not string
            ? enumerable.Cast<object?>().ToArray()
            : value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value => value;

    /// <summary>
    /// List values may be inlined into SQL instead of creating one parameter per
    /// item when the target server's reserved parameter budget would be exceeded.
    /// Only values with a safe dialect-aware literal representation are inlined.
    /// Other types (DateTime, bool, decimal, ...) stay parameterized and can
    /// still hit the parameter budget for very large lists.
    /// </summary>
    /// <remarks>
    /// The inlining decision uses the current query parameter count and a
    /// dialect-specific hard limit, reserving 500 parameters for query parts
    /// that have not rendered yet. Oracle lists over 1000 values are inlined
    /// independently of the bind-variable budget.
    /// </remarks>
    private const int ReservedParameterCount = 500;

    /// <summary>
    /// Converts the criteria to string.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query to add params to.</param>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        if (value is IEnumerable enumerable && value is not string)
        {
            // Constructor snapshots every non-string enumerable into an array,
            // so both the count check and rendering use stable immutable input.
            var count = ((ICollection)enumerable).Count;

            var inlineValues = ShouldInlineListValues(query, count);

            sb.Append('(');
            var index = 0;
            foreach (var item in enumerable)
            {
                if (index++ > 0)
                    sb.Append(',');

                if (inlineValues)
                {
                    if (IsIntegerType(item) || item is Enum)
                    {
                        // Provably numeric, so inlining cannot inject SQL. Render
                        // with the invariant culture: current-culture digits
                        // would not parse server-side. Enum values are rendered
                        // using their underlying integral type to preserve ulong
                        // values above long.MaxValue.
                        sb.Append(FormatInteger(item!));
                        continue;
                    }

                    if (item is string s)
                    {
                        // Quoted literal with dialect-aware escaping: cannot
                        // inject, and dodges the parameter budget (see helper).
                        sb.Append(s.ToSql(query.Dialect));
                        continue;
                    }

                    if (item is Guid g)
                    {
                        // Dialect-aware GUID literal; formatting accounts for
                        // dialect-specific GUID storage conventions.
                        sb.Append(((Guid?)g).ToSql(query.Dialect));
                        continue;
                    }
                }
                sb.Append(AddParam(query, item).Name);
            }
            sb.Append(')');
        }
        else
        {
            sb.Append(AddParam(query, value).Name);
        }
    }

    private static bool IsIntegerType(object? k)
    {
        // Exactly the eight CLR integer types. bool, char, float, double and
        // nint / nuint are excluded: only decimal-digit rendering is safe to inline.
        return k is byte or sbyte or short or ushort or int or uint or long or ulong;
    }

    private static string FormatInteger(object value)
    {
        if (value is Enum enumValue)
            value = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumValue.GetType()), CultureInfo.InvariantCulture);

        return Convert.ToString(value, CultureInfo.InvariantCulture)!;
    }

    private static bool ShouldInlineListValues(IQueryWithParams query, int listCount)
    {
        var serverType = query.Dialect.ServerType;

        // Oracle's requested large-list behavior is based on the 1000-item IN
        // list boundary, independently of its bind-variable budget.
        if (serverType == nameof(ServerType.Oracle) && listCount > 1000)
            return true;

        // Reserve the same fixed headroom for query parts that may render later.
        var parameterLimit = serverType switch
        {
            nameof(ServerType.SqlServer) => 2100,
            // Microsoft.Data.Sqlite uses a current SQLite version with a 32766
            // variable limit by default.
            nameof(ServerType.Sqlite) => 32766,
            nameof(ServerType.Postgres) or nameof(ServerType.MySql) or nameof(ServerType.Oracle) => 65535,
            nameof(ServerType.Firebird) => 32767,
            _ => 0
        };

        if (parameterLimit == 0)
            return false;

        var currentParameterCount = query.Params?.Count ?? 0;
        var inlineAtCount = parameterLimit - ReservedParameterCount;
        return currentParameterCount + (long)listCount > inlineAtCount;
    }

    private static Parameter AddParam(IQueryWithParams query, object? value)
    {
        var param = query.AutoParam();
        query.AddParam(param.Name, value);
        return param;
    }
}
