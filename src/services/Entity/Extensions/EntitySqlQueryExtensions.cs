using static Dapper.SqlMapper;

namespace Serenity.Data;

/// <summary>
///   Extensions for <see cref="SqlQuery"/> related to entities.
/// </summary>
public static class EntitySqlQueryExtensions
{
    private static TFields AdjustAlias<TFields>(SqlQuery query, TFields fields)
        where TFields : RowFieldsBase
    {
        ArgumentNullException.ThrowIfNull(fields);
        if (fields.AliasName is "T0" && query.HasAlias("T0"))
            fields = fields.As(query.AutoAlias());

        return fields;
    }

    private static void CheckNoIntoRow(SqlQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (((ISqlQueryExtensible)query).FirstIntoRow is not null)
            throw new InvalidOperationException(
                "The query already has an INTO row. Row-based From overloads can only be called once. " +
                "To add another source and materialize it, use query.From(secondRow.Fields.As(\"T2\")).Into(secondRow).");
    }

    /// <summary>
    /// Adds row fields as a FROM source and applies their dialect when the query dialect is not overridden.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="fields">The fields whose table is added to the query.</param>
    /// <returns>The query itself.</returns>
    public static SqlQuery From(this SqlQuery query, RowFieldsBase fields)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(fields);

        fields = AdjustAlias(query, fields);
        var alias = (IAlias)fields;

        ArgumentException.ThrowIfNullOrEmpty(alias.Table);

        if (query.HasAlias(alias.Name))
            throw new ArgumentOutOfRangeException(string.Format("{0} alias is used more than once in the query!", alias.Name));

        if (!query.IsDialectOverridden)
            query.Dialect(fields.Dialect);

        return query.From(alias.Table, alias);
    }

    /// <summary>
    /// Adds the entity as a FROM source and sets it as the query's INTO target.
    /// For row entities, applies the fields' dialect if the query dialect has not been overridden,
    /// and allocates another alias if the default T0 alias is already in use.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="entity">The entity.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">query or entity is null.</exception>
    /// <exception cref="InvalidOperationException">The query already has an INTO row.</exception>
    public static SqlQuery From(this SqlQuery query, IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        CheckNoIntoRow(query);
        
        if (entity is IRow row)
        {
            var fields = AdjustAlias(query, row.Fields);
            query.From(fields);
        }
        else if (entity is IAlias alias && (alias.Name == "t0" || alias.Name == "T0") && alias.Table == entity.Table)
            query.From(alias);
        else
            query.From(entity.Table, Alias.T0);

        return query.Into(entity);
    }

    /// <summary>
    /// Adds fields as a FROM source, adjusting the default T0 alias when it is already used.
    /// This overload does not set an INTO target.
    /// </summary>
    /// <typeparam name="TFields">The row fields type.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="fields">The fields whose table is added to the query.</param>
    /// <param name="configure">An action to configure the aliased fields and query.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">query or fields is null.</exception>
    public static SqlQuery From<TFields>(this SqlQuery query, TFields fields, Action<TFields, SqlQuery> configure)
        where TFields : RowFieldsBase
    {
        ArgumentNullException.ThrowIfNull(query);
        fields = AdjustAlias(query, fields);
        query.From(fields);
        configure?.Invoke(fields, query);
        return query;
    }

    /// <summary>
    /// Adds the row as a FROM source and sets it as the query's INTO target.
    /// Applies the row fields' dialect if the query dialect has not been overridden.
    /// </summary>
    /// <param name="query">The query</param>
    /// <typeparam name="TFields">The row fields type.</typeparam>
    /// <param name="row">The row whose table is added to the query.</param>
    /// <param name="configure">An optional callback to configure the aliased fields and query.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">query or row is null.</exception>
    /// <exception cref="InvalidOperationException">The query already has an INTO target.</exception>
    public static SqlQuery From<TFields>(this SqlQuery query, IRow<TFields> row, Action<TFields, SqlQuery>? configure = null)
        where TFields : RowFieldsBase
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(row);
        CheckNoIntoRow(query);
        var fields = AdjustAlias(query, row.Fields);
        query.From(fields).Into(row);
        configure?.Invoke(fields, query);
        return query;
    }

    /// <summary>
    /// Adds the row as a FROM source using the specified alias and sets it as the query's INTO target.
    /// Applies the row fields' dialect if the query dialect has not been overridden.
    /// </summary>
    /// <param name="query">The query</param>
    /// <typeparam name="TFields">The row fields type.</typeparam>
    /// <param name="row">The row whose table is added to the query.</param>
    /// <param name="alias">The alias to use for the row.</param>
    /// <param name="configure">An optional callback to configure the aliased fields and query.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">query or row is null.</exception>
    /// <exception cref="InvalidOperationException">The query already has an INTO target.</exception>
    public static SqlQuery From<TFields>(this SqlQuery query, IRow<TFields> row, string alias, Action<TFields, SqlQuery>? configure = null)
        where TFields : RowFieldsBase
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(row);
        CheckNoIntoRow(query);
        var fields = row.Fields;
        fields = alias != null ? fields.As(alias) : AdjustAlias(query, fields);
        query.From(fields).Into(row);
        configure?.Invoke(fields, query);
        return query;
    }

    /// <summary>
    /// Creates a subquery and adds the specified fields as its FROM source without setting an INTO target.
    /// If the fields use the default T0 alias, an available alias is allocated from the query tree.
    /// </summary>
    /// <typeparam name="TFields">The row fields type.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="fields">The fields.</param>
    /// <param name="configure">An optional callback to configure the aliased fields and subquery.</param>
    /// <returns>The created subquery.</returns>
    /// <exception cref="ArgumentNullException">query or fields is null.</exception>
    public static SqlQuery SubQueryFrom<TFields>(this QueryWithParams query, TFields fields, Action<TFields, SqlQuery>? configure = null)
        where TFields : RowFieldsBase
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(fields);
        if (fields.AliasName is "T0")
            fields = fields.As(query.AutoAlias());
        return query.CreateSubQuery<SqlQuery>().From(fields, configure!);
    }

    /// <summary>
    /// Adds the specified entity to the INTO list of the query, 
    /// and sets it as the current INTO row.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="into">The into.</param>
    /// <returns>The query itself.</returns>
    public static SqlQuery Into(this SqlQuery query, IEntity into)
    {
        var ext = (ISqlQueryExtensible)query;
        ext.IntoRowSelection(into);

        return query;
    }

    /// <summary>
    /// Adds a field's expression to the SELECT statement with its own column name. 
    /// If a join alias is referenced in the field expression, and the join is defined in 
    /// the field's entity class, it is automatically included in the query. 
    /// The field is marked as a target at the current index for future loading from a data reader.
    /// </summary>
    /// <param name="field">The field object.</param>
    /// <param name="query">The SQL query.</param>
    /// <returns>The query itself.</returns>
    public static SqlQuery Select(this SqlQuery query, IField field)
    {
        ArgumentNullException.ThrowIfNull(field);

        query.EnsureJoinsInExpression(field.Expression);
        _ = new SqlQuery.Column(query, field.Expression, field.ColumnAlias, field);
        return query;
    }

    /// <summary>
    /// Adds a field's expression to the SELECT statement with a given column name.
    /// If a join alias is referenced in the field expression, and the join is defined in
    /// the field's entity class, it is automatically included in the query.
    /// The field is marked as a target at the current index for future loading from a data reader.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="field">The field object.</param>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// field
    /// or
    /// columnName
    /// </exception>
    public static SqlQuery Select(this SqlQuery query, IField field, string columnName)
    {
        ArgumentNullException.ThrowIfNull(field);

        ArgumentNullException.ThrowIfNull(columnName);

        query.EnsureJoinsInExpression(field.Expression);
        _ = new SqlQuery.Column(query, field.Expression, columnName, field);
        return query;
    }

    /// <summary>
    /// Adds a field of a given table alias to the SELECT statement.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="alias">A table alias that will be prepended to the field name with "." between.</param>
    /// <param name="field">A field that only its name will be used. It won't be set as a target.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// alias
    /// or
    /// field
    /// </exception>
    /// <remarks>
    /// No column name is set for the selected field.
    /// Also the field is not set as a target, unlike the field only overload, only the field name is used.
    /// </remarks>
    public static SqlQuery Select(this SqlQuery query, IAlias alias, IField field)
    {
        ArgumentNullException.ThrowIfNull(alias);

        ArgumentNullException.ThrowIfNull(field);

        return query.Select(alias.NameDot + field);
    }


    /// <summary>
    /// Adds a field of a given table alias to the SELECT statement.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="alias">A table alias that will be prepended to the field name with "." between.</param>
    /// <param name="field">A field that only its field name will be used. It won't be set as a target.</param>
    /// <param name="columnName">A column name.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// alias
    /// or
    /// field
    /// or
    /// columnName
    /// </exception>
    /// <remarks>
    /// The field is not set as a target, unlike the field only overload, only the field name is used.
    /// </remarks>
    public static SqlQuery Select(this SqlQuery query, IAlias alias, IField field, string columnName)
    {
        ArgumentNullException.ThrowIfNull(alias);

        ArgumentNullException.ThrowIfNull(field);

        ArgumentNullException.ThrowIfNull(columnName);

        return query.Select(alias.NameDot + SqlSyntax.AutoBracket(field.Name, query.Dialect()), columnName);
    }

    /// <summary>
    /// For each field in the fields array, adds the expression of the field to
    /// the SELECT statement with a column name of its name.
    /// If a join alias is referenced in the field expression, and the join is defined in
    /// the field's entity class, it is automatically included in the query.
    /// The fields are marked as a target at the current index for future loading from a data reader.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="fields">The field objects.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">fields is null.</exception>
    public static SqlQuery Select(this SqlQuery query, params IField[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        foreach (IField field in fields)
            Select(query, field);

        return query;
    }

    /// <summary>
    /// Adds a field or an expression to the SELECT statement with a column name of a
    /// field's name. The field is marked as a target at the current index for future loading
    /// from a data reader.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="expression">A field name or an expression.</param>
    /// <param name="intoField">A field object whose name is to be used as a column name.</param>
    /// <returns>
    /// The query itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">expression is null or empty, or intoField is null.</exception>
    public static SqlQuery SelectAs(this SqlQuery query, string expression, IField intoField)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentNullException(nameof(expression));

        ArgumentNullException.ThrowIfNull(intoField);

        _ = new SqlQuery.Column(query, expression, intoField.ColumnAlias, intoField);
        return query;
    }

    /// <summary>
    /// Adds a field's expression to the order by list.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="field">The field.</param>
    /// <param name="desc">if set to <c>true</c>, sorts in descending order.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">field is null.</exception>
    public static SqlQuery OrderBy(this SqlQuery query, IField field, bool desc = false)
    {
        ArgumentNullException.ThrowIfNull(field);

        return query.OrderBy(field.Expression, desc);
    }

    /// <summary>
    /// Adds field expressions to the order by list.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="fields">The fields.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">fields is null.</exception>
    public static SqlQuery OrderBy(this SqlQuery query, params IField[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        foreach (IField field in fields)
            OrderBy(query, field);

        return query;
    }

    /// <summary>
    /// Adds a field's expression to the group by list.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="field">The field.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">field is null.</exception>
    public static SqlQuery GroupBy(this SqlQuery query, IField field)
    {
        ArgumentNullException.ThrowIfNull(field);

        return query.GroupBy(field.Expression);
    }

    /// <summary>
    /// Adds field expressions to the group by list.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="fields">The fields.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">fields is null.</exception>
    public static SqlQuery GroupBy(this SqlQuery query, params IField[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        foreach (IField f in fields)
            GroupBy(query, f);

        return query;
    }
}