using System.Collections;

namespace Serenity.Web;

/// <summary>
/// A dynamic script type for distinct values of a field
/// </summary>
/// <typeparam name="TRow"></typeparam>
public class DistinctValuesScript<TRow> : LookupScript
   where TRow : class, IRow, new()
{
    private readonly ISqlConnections connections;
    private readonly string propertyName;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="connections">Sql connections</param>
    /// <param name="propertyName">Property name</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DistinctValuesScript(ISqlConnections connections, string propertyName)
    {
        this.connections = connections ?? throw new ArgumentNullException(nameof(connections));
        this.propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

        var row = new TRow();
        var field = GetFieldFrom(row);

        var readPermission = field.GetAttribute<ReadPermissionAttribute>() ??
            typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

        if (readPermission != null)
            Permission = readPermission.Permission ?? "?";

        GroupKey = row.GetFields().GenerationKey;

        TextField = "v";
        IdField = "v";
    }

    private Field GetFieldFrom(IRow row)
    {
        Field field = row.FindFieldByPropertyName(propertyName) ??
            row.FindField(propertyName);

        if (field is null)
        {
            throw new InvalidProgramException(string.Format(CultureInfo.CurrentCulture,
                "Property '{0}' specified in a distinct values script on " +
                "row type {1} is not found!", propertyName, row.GetType().FullName));
        }

        return field;
    }

    /// <summary>
    /// Applies the sort order to the query
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void ApplyOrder(SqlQuery query)
    {
        var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;
        query.OrderBy(GetFieldFrom(row));
    }

    /// <summary>
    /// Prepares the sql query
    /// </summary>
    /// <param name="query">Sql query</param>
    protected virtual void PrepareQuery(SqlQuery query)
    {
        var row = (IRow)(query as ISqlQueryExtensible).FirstIntoRow;
        var field = GetFieldFrom(row);

        query.Select(field)
            .Distinct(true)
            .Where(field.IsNotNull());

        if (field is StringField)
            query.Where(field != "");
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        return "Q.ScriptData.set(" + ("Lookup." + LookupKey).ToSingleQuoted() +
            ", new Q.Lookup(" + LookupParams.ToJson() + ", " + GetItems().ToJson() + 
            ".map(function(x) { return { v: x }; })));";
    }

    /// <inheritdoc/>
    protected override IEnumerable GetItems()
    {
        var loader = new TRow();
        var field = GetFieldFrom(loader);
        var list = new List<object>();

        var query = new SqlQuery()
            .From(loader);

        PrepareQuery(query);
        ApplyOrder(query);

        using (var connection = connections.NewByKey(loader.Fields.ConnectionKey))
        {
            query.ForEach(connection, delegate ()
            {
                list.Add(field.AsObject(loader));
            });
        }

        return list;
    }
}