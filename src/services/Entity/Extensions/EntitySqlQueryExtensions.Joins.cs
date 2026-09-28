namespace Serenity.Data;

public static partial class EntitySqlQueryExtensions
{
	private static SqlQuery JoinWithFields<TFields>(SqlQuery query, TFields fields,
		Func<TFields, ICriteria> onCriteria, out TFields aliased,
		Func<IAlias, ICriteria, SqlQuery> addJoin)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(fields);
		ArgumentNullException.ThrowIfNull(onCriteria);

		aliased = AdjustAlias(query, fields);
		return addJoin((IAlias)aliased, onCriteria(aliased));
	}

	/// <summary>
	/// Adds an INNER JOIN using fields aliased for this query and builds its ON criteria from those fields.
	/// </summary>
	/// <typeparam name="TFields">The row fields type.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="fields">The fields whose table is joined.</param>
	/// <param name="onCriteria">Creates the ON criteria from the fields with their query alias.</param>
	/// <param name="aliased">Receives the fields instance with the alias used by the join.</param>
	/// <returns>The query itself.</returns>
	public static SqlQuery InnerJoin<TFields>(this SqlQuery query, TFields fields,
		Func<TFields, ICriteria> onCriteria, out TFields aliased)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		return JoinWithFields(query, fields, onCriteria, out aliased, query.InnerJoin);
	}

	/// <summary>
	/// Adds a LEFT JOIN using fields aliased for this query and builds its ON criteria from those fields.
	/// </summary>
	/// <typeparam name="TFields">The row fields type.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="fields">The fields whose table is joined.</param>
	/// <param name="onCriteria">Creates the ON criteria from the fields with their query alias.</param>
	/// <param name="aliased">Receives the fields instance with the alias used by the join.</param>
	/// <returns>The query itself.</returns>
	public static SqlQuery LeftJoin<TFields>(this SqlQuery query, TFields fields,
		Func<TFields, ICriteria> onCriteria, out TFields aliased)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		return JoinWithFields(query, fields, onCriteria, out aliased, query.LeftJoin);
	}

	/// <summary>
	/// Adds a RIGHT JOIN using fields aliased for this query and builds its ON criteria from those fields.
	/// </summary>
	/// <typeparam name="TFields">The row fields type.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="fields">The fields whose table is joined.</param>
	/// <param name="onCriteria">Creates the ON criteria from the fields with their query alias.</param>
	/// <param name="aliased">Receives the fields instance with the alias used by the join.</param>
	/// <returns>The query itself.</returns>
	public static SqlQuery RightJoin<TFields>(this SqlQuery query, TFields fields,
		Func<TFields, ICriteria> onCriteria, out TFields aliased)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		return JoinWithFields(query, fields, onCriteria, out aliased, query.RightJoin);
	}
}