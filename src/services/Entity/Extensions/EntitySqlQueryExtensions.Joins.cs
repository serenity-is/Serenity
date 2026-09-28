namespace Serenity.Data;

public static partial class EntitySqlQueryExtensions
{
	private static SqlQuery JoinVia<TFields>(SqlQuery query, TFields fields, Field foreignKeyField,
		out TFields aliased, Func<IAlias, ICriteria, SqlQuery> addJoin)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		ArgumentNullException.ThrowIfNull(fields);
		ArgumentNullException.ThrowIfNull(foreignKeyField);

		var fieldName = foreignKeyField.PropertyName ?? foreignKeyField.Name;
		var sourceFieldsType = foreignKeyField.Fields.GetType();
		var fieldDescription = $"Field '{fieldName}' in row fields type '{sourceFieldsType.FullName ?? sourceFieldsType.Name}'";
		var foreignJoin = foreignKeyField.ForeignJoinAlias;
		var targetRowType = foreignJoin?.RowType ??
			(foreignJoin is null ? foreignKeyField.GetAttribute<ForeignKeyAttribute>()?.RowType : null);

		if (targetRowType is null)
		{
			var metadata = foreignJoin is null ? "ForeignKey metadata" : "ForeignJoinAlias";
			throw new InvalidOperationException($"{fieldDescription} has {metadata} without a row type; " +
				$"target fields type '{typeof(TFields).FullName}' cannot be validated.");
		}

		if (targetRowType != typeof(TFields).DeclaringType)
			throw new ArgumentException($"{fieldDescription} targets row type '{targetRowType.FullName}', which does not match " +
				$"the supplied target fields type '{typeof(TFields).FullName}'.", nameof(fields));

		aliased = AdjustAlias(query, fields);
		var alias = aliased.AliasName;

		string table;
		ICriteria onCriteria;
		if (foreignJoin is not null)
		{
			table = foreignJoin.Table ?? throw new InvalidOperationException($"{fieldDescription}'s ForeignJoinAlias must specify a table.");
			var declaredOnCriteria = foreignJoin.OnCriteria ??
				throw new InvalidOperationException($"{fieldDescription}'s ForeignJoinAlias must specify ON criteria.");
			string ReplaceJoinAlias(string joinAlias) =>
				string.Equals(joinAlias, foreignJoin.Name, StringComparison.OrdinalIgnoreCase) ? alias : joinAlias;
			onCriteria = declaredOnCriteria is BaseCriteria criteriaTree
				? JoinAliasLocator.ReplaceAliases(criteriaTree, ReplaceJoinAlias)
				: new Criteria(JoinAliasLocator.ReplaceAliases(declaredOnCriteria.ToStringIgnoreParams(), ReplaceJoinAlias));
		}
		else
		{
			table = foreignKeyField.ForeignTable ??
				throw new InvalidOperationException($"{fieldDescription} must specify a foreign table.");
			var foreignField = foreignKeyField.ForeignField ??
				throw new InvalidOperationException($"{fieldDescription} must specify a foreign field.");
			onCriteria = new Criteria(alias, foreignField) == new Criteria(foreignKeyField);
		}

		return addJoin((IAlias)aliased, onCriteria);
	}

	/// <summary>
	/// Adds a LEFT JOIN to the row referenced by the foreign key field.
	/// </summary>
	/// <typeparam name="TFields">The referenced row fields type.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="fields">The referenced row fields, used to infer and validate the target type.</param>
	/// <param name="foreignKeyField">The foreign key field.</param>
	/// <param name="aliased">Receives the referenced fields with the alias used by this query.</param>
	/// <returns>The query itself.</returns>
	public static SqlQuery LeftJoinVia<TFields>(this SqlQuery query, TFields fields, Field foreignKeyField, out TFields aliased)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		return JoinVia(query, fields, foreignKeyField, out aliased, query.LeftJoin);
	}

	/// <summary>
	/// Adds a RIGHT JOIN to the row referenced by the foreign key field.
	/// </summary>
	/// <typeparam name="TFields">The referenced row fields type.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="fields">The referenced row fields, used to infer and validate the target type.</param>
	/// <param name="foreignKeyField">The foreign key field.</param>
	/// <param name="aliased">Receives the referenced fields with the alias used by this query.</param>
	/// <returns>The query itself.</returns>
	public static SqlQuery RightJoinVia<TFields>(this SqlQuery query, TFields fields, Field foreignKeyField, out TFields aliased)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		return JoinVia(query, fields, foreignKeyField, out aliased, query.RightJoin);
	}

	/// <summary>
	/// Adds an INNER JOIN to the row referenced by the foreign key field.
	/// </summary>
	/// <typeparam name="TFields">The referenced row fields type.</typeparam>
	/// <param name="query">The query.</param>
	/// <param name="fields">The referenced row fields, used to infer and validate the target type.</param>
	/// <param name="foreignKeyField">The foreign key field.</param>
	/// <param name="aliased">Receives the referenced fields with the alias used by this query.</param>
	/// <returns>The query itself.</returns>
	public static SqlQuery InnerJoinVia<TFields>(this SqlQuery query, TFields fields, Field foreignKeyField, out TFields aliased)
		where TFields : RowFieldsBase
	{
		ArgumentNullException.ThrowIfNull(query);
		return JoinVia(query, fields, foreignKeyField, out aliased, query.InnerJoin);
	}

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