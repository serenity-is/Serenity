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
    private readonly object? value = value;

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object? Value => value;

    /// <summary>
    /// Lists longer than this inline values into the SQL instead of creating one
    /// parameter per item. Small lists always stay parameterized so execution
    /// plans stay reusable; only long lists are affected, and only with values
    /// that render safely (integers / enums, strings, GUIDs — see below).
    /// Other types (DateTime, bool, decimal, ...) stay parameterized and can
    /// still hit the parameter budget for very large lists.
    /// </summary>
    /// <remarks>
    /// 10 is a deliberately conservative cut-off, not a tuned value: this
    /// criteria renders in isolation and cannot know how many other parameters
    /// the rest of the query will add to the same command during
    /// <c>ToString()</c>, so the threshold stays far below budgets like SQL
    /// Server's 2100 to leave ample headroom.
    /// </remarks>
    private const int InlineThreshold = 10;

    /// <summary>
    /// Converts the criteria to string.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query to add params to.</param>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        if (value is IEnumerable enumerable && value is not string)
        {
            // The size is needed before rendering (see InlineThreshold). Real
            // collections report it for free via ICollection.Count — no
            // enumeration, no boxing. Anything else is buffered once: lazy /
            // single-pass sources (yield iterator, queryable) would throw or
            // re-execute on a second pass, so enumerable is reassigned to the
            // snapshot and the loop below always walks it exactly once.
            int count;
            if (enumerable is ICollection collection)
                count = collection.Count;
            else
            {
                var snapshot = enumerable.Cast<object?>().ToArray();
                count = snapshot.Length;
                enumerable = snapshot;
            }

            sb.Append('(');
            var index = 0;
            foreach (var item in enumerable)
            {
                if (index++ > 0)
                    sb.Append(',');

                if (count > InlineThreshold)
                {
                    if (IsIntegerType(item) || item is Enum)
                    {
                        // Provably numeric, so inlining cannot inject SQL. Render
                        // with the invariant culture: current-culture digits
                        // would not parse server-side. Enums go through their
                        // Int64 value (Convert.ToString on an enum would render
                        // its name). Kept dialect-independent (legacy behavior):
                        // numeric literals are compact everywhere.
                        sb.Append(item is Enum
                            ? Convert.ToInt64(item).ToString(CultureInfo.InvariantCulture)
                            : Convert.ToString(item, CultureInfo.InvariantCulture));
                        continue;
                    }

                    if (item is string s && CanInlineLiterals(query.Dialect))
                    {
                        // Quoted literal with dialect-aware escaping: cannot
                        // inject, and dodges the parameter budget (see helper).
                        sb.Append(s.ToSql(query.Dialect));
                        continue;
                    }

                    if (item is Guid g && CanInlineLiterals(query.Dialect) &&
                        HasStandardGuidLiteral(query.Dialect))
                    {
                        // Quoted D-format literal; only where it compares
                        // against GUID-ish columns and the budget matters.
                        sb.Append(((Guid?)g).ToSql());
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

    private static bool CanInlineLiterals(ISqlDialect dialect)
    {
        // Only dialects with a realistically reachable parameter budget inline
        // long string / GUID lists as literals: SQL Server (2100 params per
        // command) and SQLite (999 on pre-3.32 builds, 32766 newer) — a header
        // filter with thousands of selected codes would otherwise crash there.
        // Postgres (65535) and MySQL (65535 prepared) have budgets no such list
        // will reach, so they keep reusable parameterized plans. Oracle caps IN
        // lists at 1000 expressions either way (ORA-01795), which inlining
        // cannot avoid, so it stays parameterized too.
        return dialect?.ServerType switch
        {
            nameof(ServerType.SqlServer) or nameof(ServerType.Sqlite) => true,
            _ => false
        };
    }

    private static bool HasStandardGuidLiteral(ISqlDialect dialect)
    {
        // A quoted D-format GUID literal compares correctly against GUID-ish
        // columns here (uniqueidentifier coercion on SQL Server, plain text
        // compare on SQLite). Oracle typically stores GUIDs as RAW(16), where
        // hyphenated text is not valid hex, and Firebird has several competing
        // conventions (CHAR(36), CHAR(16) OCTETS) with no single standard, so
        // GUIDs stay parameterized on those dialects.
        return dialect?.ServerType switch
        {
            nameof(ServerType.SqlServer) or nameof(ServerType.Sqlite) => true,
            _ => false
        };
    }

    private static Parameter AddParam(IQueryWithParams query, object? value)
    {
        var param = query.AutoParam();
        query.AddParam(param.Name, value);
        return param;
    }
}