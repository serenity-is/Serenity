using System.Collections;

namespace Serenity.Web;

/// <summary>
/// Generic lookup script type for rows.
/// </summary>
/// <typeparam name="TRow">The row type.</typeparam>
public class RowLookupScript<TRow> : LookupScript
    where TRow: class, IRow, new()
{
    /// <summary>
    /// The SQL connections.
    /// </summary>
    protected readonly ISqlConnections sqlConnections;

    /// <summary>
    /// Applies the sort order to the query.
    /// </summary>
    /// <param name="query">The query.</param>
    protected virtual void ApplyOrder(SqlQuery query)
    {
        var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;

        if (row.NameField is not null)
            query.OrderBy(row.NameField);
        else if (row.IdField is not null)
            query.OrderBy(row.IdField);
    }

    /// <summary>
    /// Prepares the SQL query to select fields.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    protected virtual void PrepareQuery(SqlQuery query)
    {
        var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;

        if (row.IdField is not null)
            query.Select(row.IdField);

        if (row.NameField is not null)
            query.Select(row.NameField);

        var list = new List<object>();

        foreach (var f in row.GetFields().Where(x => 
            x.CustomAttributes.OfType<LookupIncludeAttribute>().Any()))
        {
            query.Select(f);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RowLookupScript{TRow}"/> class.
    /// </summary>
    /// <param name="sqlConnections">The SQL connections.</param>
    /// <exception cref="ArgumentNullException"><paramref name="sqlConnections"/> is <c>null</c>.</exception>
    public RowLookupScript(ISqlConnections sqlConnections)
        : base()
    {
        this.sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

        var row = new TRow();

        if (row.IdField is not null)
        {
            IdField = row.IdField.PropertyName ?? row.IdField.Name;
        }

        if (row.NameField is Field nameField)
        {
            TextField = nameField.PropertyName ?? nameField.Name;
        }

        if (row is IParentIdRow treeRow)
        {
            ParentIdField = treeRow.ParentIdField.PropertyName ?? treeRow.ParentIdField.Name;
        }

        var readPermission = typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);
        if (readPermission != null)
            Permission = readPermission.Permission ?? "?";

        GroupKey = row.GetFields().GenerationKey;
    }

    /// <inheritdoc/>
    protected override IEnumerable GetItems()
    {
        var list = new List<TRow>();
        var loader = new TRow();

        var query = new SqlQuery()
            .From(loader);

        PrepareQuery(query);
        ApplyOrder(query);

        using (var connection = sqlConnections.NewByKey(loader.Fields.ConnectionKey))
        {
            query.ForEach(connection, delegate ()
            {
                list.Add(loader.Clone());
            });
        }

        return list;
    }
}