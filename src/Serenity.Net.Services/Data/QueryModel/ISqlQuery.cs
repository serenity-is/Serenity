namespace Serenity.Data;

/// <summary>
/// SqlQuery interface.
/// </summary>
/// <seealso cref="IQueryWithParams" />
/// <seealso cref="IChainable" />
public interface ISqlQuery : IQueryWithParams, IChainable
{
    /// <summary>
    /// Gets the columns.
    /// </summary>
    /// <value>
    /// The columns.
    /// </value>
    IEnumerable<SqlQuery.Column> Columns { get; }

    /// <summary>
    /// Gets if to count records
    /// </summary>
    bool CountRecords { get; }

    /// <summary>
    /// Gets if distinct query
    /// </summary>
    bool Distinct { get; }

    /// <summary>
    /// Gets FOR JSON part if any
    /// </summary>
    string ForJson { get; }

    /// <summary>
    /// Gets FOR XML part if any
    /// </summary>
    string ForXml { get; }

    /// <summary>
    /// Gets access to FROM part if any
    /// </summary>
    string From { get; }

    /// <summary>
    /// Gets access to GROUP BY part if any
    /// </summary>
    string GroupBy { get; }

    /// <summary>
    /// Gets access to HAVING part if any
    /// </summary>
    string Having { get; }

    /// <summary>
    /// Where to omit params while converting to string
    /// </summary>
    bool OmitParens { get; }

    /// <summary>
    /// Gets order by list
    /// </summary>
    IEnumerable<string> OrderBy { get; }

    /// <summary>
    /// Gets access to parent query if any
    /// </summary>
    IQueryWithParams Parent { get; }

    /// <summary>
    /// Gets skip number
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Gets take number
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Gets access to internal union query if any
    /// </summary>
    ISqlQuery UnionQuery { get; }

    /// <summary>
    /// Gets access to internal union type if any
    /// </summary>
    SqlUnionType UnionType { get; }

    /// <summary>
    /// Gets access to WHERE part if any
    /// </summary>
    string Where { get; }
}
