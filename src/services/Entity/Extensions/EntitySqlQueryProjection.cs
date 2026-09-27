using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Serenity.Data;

/// <summary>
/// Extensions for SQL query projections.
/// </summary>
/// <remarks>
/// Projection selectors currently support flat anonymous types, constructor projections,
/// and object initializers whose values are direct row field accesses. Unbuffered results
/// keep the data reader open until enumeration completes or the enumerator is disposed.
/// </remarks>
public static class EntitySqlQueryProjection
{
	private readonly record struct ProjectionColumn(Expression ProjectionExpression, Field Field, string Name, int Index);

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// The query must not already have SELECT columns, and its into rows must match the selector
	/// parameters in count and compatible row type.
	/// </summary>
	/// <typeparam name="TRow">The row type of the query's single into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="buffered">Whether to buffer all results before returning.</param>
	/// <returns>The projected results.</returns>
	public static IEnumerable<TResult> QueryProjected<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection, bool buffered = true)
		where TRow : class, IRow
	{
		return QueryProjectedCore<TResult>(query, connection, projection, buffered);
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// </summary>
	/// <typeparam name="TRow">The row type of the query's single into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <returns>The projected results.</returns>
	public static List<TResult> ListProjected<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection)
		where TRow : class, IRow
	{
		return [.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false)];
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// When <paramref name="buffered"/> is false, the data reader remains open until enumeration
	/// completes or the enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the query's first into row.</typeparam>
	/// <typeparam name="TRow2">The row type of the query's second into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="buffered">Whether to buffer all results before returning.</param>
	/// <returns>The projected results.</returns>
	public static IEnumerable<TResult> QueryProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection, bool buffered = true)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		return QueryProjectedCore<TResult>(query, connection, projection, buffered);
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the query's first into row.</typeparam>
	/// <typeparam name="TRow2">The row type of the query's second into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <returns>The projected results.</returns>
	public static List<TResult> ListProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		return [.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false)];
	}

	/// <summary>
	/// Asynchronously executes the query and buffers the flat projection results.
	/// </summary>
	/// <typeparam name="TRow">The row type of the query's single into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
	public static Task<List<TResult>> ListProjectedAsync<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		CancellationToken cancellationToken = default)
		where TRow : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var materializer = PrepareProjection<TResult>(query, projection);
		return BufferProjectedAsync(query, connection, materializer, cancellationToken);
	}

	/// <summary>
	/// Asynchronously executes the query and buffers the flat projection results.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the query's first into row.</typeparam>
	/// <typeparam name="TRow2">The row type of the query's second into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
	public static Task<List<TResult>> ListProjectedAsync<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		CancellationToken cancellationToken = default)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var materializer = PrepareProjection<TResult>(query, projection);
		return BufferProjectedAsync(query, connection, materializer, cancellationToken);
	}

	/// <summary>
	/// Asynchronously streams the flat projection results. The data reader remains open until
	/// enumeration completes, is cancelled, or the async enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow">The row type of the query's single into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An asynchronous stream of projected results.</returns>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		CancellationToken cancellationToken = default)
		where TRow : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var materializer = PrepareProjection<TResult>(query, projection);
		return EnumerateProjectedAsync(query, connection, materializer, cancellationToken);
	}

	/// <summary>
	/// Asynchronously streams the flat projection results. The data reader remains open until
	/// enumeration completes, is cancelled, or the async enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the query's first into row.</typeparam>
	/// <typeparam name="TRow2">The row type of the query's second into row.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An asynchronous stream of projected results.</returns>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		CancellationToken cancellationToken = default)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var materializer = PrepareProjection<TResult>(query, projection);
		return EnumerateProjectedAsync(query, connection, materializer, cancellationToken);
	}

	private static IEnumerable<TResult> QueryProjectedCore<TResult>(SqlQuery query,
		IDbConnection connection, LambdaExpression projection, bool buffered)
	{
		ArgumentNullException.ThrowIfNull(connection);
		var materializer = PrepareProjection<TResult>(query, projection);
		var results = EnumerateProjected(query, connection, materializer);
		return buffered ? [.. results] : results;
	}

	private static Func<IDataReader, TResult> PrepareProjection<TResult>(SqlQuery query, LambdaExpression projection)
	{
		ArgumentNullException.ThrowIfNull(query);
		ArgumentNullException.ThrowIfNull(projection);

		var extensible = (ISqlQueryExtensible)query;
		if (extensible.Columns.Count != 0)
			throw new InvalidOperationException("QueryProjected requires a query without existing SELECT columns.");

		if (projection.Parameters.Count != extensible.IntoRows.Count)
			throw new InvalidOperationException("The projection parameter count must match the query's into row count.");

		var sourceRows = new IRow[projection.Parameters.Count];
		for (var index = 0; index < sourceRows.Length; index++)
		{
			if (extensible.IntoRows[index] is not IRow row)
				throw new InvalidOperationException("Every query into row used by ListProjected must implement IRow.");

			if (!projection.Parameters[index].Type.IsAssignableFrom(row.GetType()))
				throw new InvalidOperationException(string.Format(
					"Projection parameter {0} has type '{1}', which is not compatible with query into row type '{2}'.",
					index, projection.Parameters[index].Type.FullName, row.GetType().FullName));

			sourceRows[index] = row;
		}

		var columns = GetProjectionColumns(projection.Body);
		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var column in columns)
		{
			if (!names.Add(column.Name))
				throw new NotSupportedException("Projection member names must be unique, ignoring case.");
		}

		var selectedColumns = new List<ProjectionColumn>(columns.Count);
		foreach (var column in columns)
		{
			var path = GetFieldPath(column.Expression, projection.Parameters);
			var sourceFields = ((ISqlQueryProjectionExtensible)query).GetIntoRowSource(path.ParameterIndex) as RowFieldsBase ??
				sourceRows[path.ParameterIndex].Fields;
			var field = ResolveField(query, sourceFields, path.MemberNames);
			selectedColumns.Add(new ProjectionColumn(column.Expression, field, column.Name, selectedColumns.Count));
		}

		var readerParameter = Expression.Parameter(typeof(IDataReader), "reader");
		var replacements = new Dictionary<Expression, Queue<ProjectionColumn>>(ReferenceEqualityComparer.Instance);
		foreach (var column in selectedColumns)
		{
			if (!replacements.TryGetValue(column.ProjectionExpression, out var matches))
				replacements.Add(column.ProjectionExpression, matches = new Queue<ProjectionColumn>());

			matches.Enqueue(column);
		}

		var materializerBody = new ProjectionReaderVisitor(readerParameter, replacements).Visit(projection.Body)!;
		var materializer = Expression.Lambda<Func<IDataReader, TResult>>(materializerBody, readerParameter).Compile();

		extensible.IntoRowSelection(null);
		foreach (var column in selectedColumns)
			query.Select(column.Field.Expression, column.Name);

		return materializer;
	}

	private static IEnumerable<TResult> EnumerateProjected<TResult>(SqlQuery query,
		IDbConnection connection, Func<IDataReader, TResult> materializer)
	{
		using var reader = query.ExecuteReader(connection);
		while (reader.Read())
			yield return materializer(reader);
	}

	private static async Task<List<TResult>> BufferProjectedAsync<TResult>(SqlQuery query,
		IDbConnection connection, Func<IDataReader, TResult> materializer, CancellationToken cancellationToken)
	{
		var results = new List<TResult>();
		await foreach (var result in EnumerateProjectedAsync(query, connection, materializer, cancellationToken)
			.ConfigureAwait(false))
			results.Add(result);

		return results;
	}

	private static async IAsyncEnumerable<TResult> EnumerateProjectedAsync<TResult>(SqlQuery query,
		IDbConnection connection, Func<IDataReader, TResult> materializer,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		using var reader = await query.ExecuteReaderAsync(connection, cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
			yield return materializer(reader);
	}

	private readonly record struct ProjectionMember(Expression Expression, string Name);

	private static List<ProjectionMember> GetProjectionColumns(Expression body)
	{
		body = StripConvert(body);
		var result = new List<ProjectionMember>();

		if (body is NewExpression newExpression)
		{
			var constructorParameters = newExpression.Constructor?.GetParameters();
			for (var index = 0; index < newExpression.Arguments.Count; index++)
			{
				string? name = null;
				if (newExpression.Members is { } members && members.Count == newExpression.Arguments.Count)
					name = members[index].Name;
				else if (constructorParameters != null && constructorParameters.Length == newExpression.Arguments.Count)
					name = constructorParameters[index].Name;

				if (string.IsNullOrWhiteSpace(name))
					throw new NotSupportedException("Projection constructor arguments must have corresponding member or parameter names.");

				result.Add(new ProjectionMember(newExpression.Arguments[index], name));
			}
		}
		else if (body is MemberInitExpression memberInit)
		{
			foreach (var binding in memberInit.Bindings)
			{
				if (binding is not MemberAssignment assignment)
					throw new NotSupportedException("Only direct member assignments are supported in a flat projection.");

				result.Add(new ProjectionMember(assignment.Expression, binding.Member.Name));
			}
		}
		else
			throw new NotSupportedException("A flat projection must create an anonymous type, use a constructor, or use an object initializer.");

		if (result.Count == 0)
			throw new NotSupportedException("A flat projection must select at least one member.");

		return result;
	}

	private readonly record struct FieldPath(int ParameterIndex, string[] MemberNames);

	private static FieldPath GetFieldPath(Expression expression, ReadOnlyCollection<ParameterExpression> parameters)
	{
		expression = StripConvert(expression);
		var names = new Stack<string>();

		while (expression is MemberExpression member)
		{
			if (member.Member is not PropertyInfo)
				throw new NotSupportedException("Projection values must use row properties.");

			names.Push(member.Member.Name);
			expression = StripConvert(member.Expression ?? throw new NotSupportedException(
				"Static properties are not supported in a row projection."));
		}

		if (expression is not ParameterExpression parameter)
			throw new NotSupportedException("Projection values must be direct row field accesses.");

		var parameterIndex = -1;
		for (var index = 0; index < parameters.Count; index++)
		{
			if (parameters[index] == parameter)
			{
				parameterIndex = index;
				break;
			}
		}

		if (parameterIndex < 0 || names.Count == 0)
			throw new NotSupportedException("Projection values must be direct row field accesses.");

		return new FieldPath(parameterIndex, [.. names]);
	}

	private static Field ResolveField(SqlQuery query, RowFieldsBase sourceFields, string[] memberNames)
	{
		var fields = sourceFields;
		for (var index = 0; index < memberNames.Length; index++)
		{
            var field = (fields.FindFieldByPropertyName(memberNames[index]) ?? fields.FindField(memberNames[index])) ?? throw new ArgumentException(string.Format(
                    "Property '{0}' does not map to a field on row type '{1}'.",
                    memberNames[index], fields.GetType().DeclaringType?.FullName ?? fields.GetType().FullName));
            if (index == memberNames.Length - 1)
				return field;

			if (!typeof(IRow).IsAssignableFrom(field.ValueType) ||
				field.GetAttribute<ForeignRowAttribute>()?.ForeignKeyProperty is not string foreignKeyProperty)
				throw new NotSupportedException(string.Format(
					"Property path '{0}' contains a member that is not a configured foreign row.",
					string.Join('.', memberNames)));

			var foreignKeyField = fields.FindFieldByPropertyName(foreignKeyProperty) ?? fields.FindField(foreignKeyProperty);
			var foreignJoin = foreignKeyField?.ForeignJoinAlias ?? throw new InvalidOperationException(
				"The foreign row's foreign key field must have a ForeignJoinAlias.");

			if (Activator.CreateInstance(field.ValueType) is not IRow foreignRow)
				throw new InvalidOperationException("The foreign row field value type must be constructible as an IRow.");

			if (!string.Equals(foreignKeyField.ForeignTable, foreignRow.Table, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("The foreign key target table does not match the foreign row field type.");

			query.EnsureJoin(foreignJoin);
			fields = foreignRow.Fields;
			if (!string.Equals(fields.AliasName, foreignJoin.Name, StringComparison.Ordinal))
				fields = RowFieldsProvider.Current.ResolveWithAlias(fields.GetType(), foreignJoin.Name);
		}

		throw new InvalidOperationException("A projection field path could not be resolved.");
	}

	private static Expression StripConvert(Expression expression)
	{
		while (expression is UnaryExpression unary &&
			(unary.NodeType == ExpressionType.Convert || unary.NodeType == ExpressionType.ConvertChecked))
			expression = unary.Operand;

		return expression;
	}

	private static readonly MethodInfo ReadProjectedValueMethod = typeof(EntitySqlQueryProjection)
		.GetMethod(nameof(ReadProjectedValue), BindingFlags.NonPublic | BindingFlags.Static)!;

	private static object? ReadProjectedValue(IDataReader reader, int index, Field field)
	{
		var value = reader.IsDBNull(index) ? null : reader.GetValue(index);
		return field.ConvertValue(value, CultureInfo.InvariantCulture);
	}

	private sealed class ProjectionReaderVisitor(ParameterExpression reader,
        Dictionary<Expression, Queue<ProjectionColumn>> replacements) : ExpressionVisitor
	{
		private readonly ParameterExpression reader = reader;
		private readonly Dictionary<Expression, Queue<ProjectionColumn>> replacements = replacements;

        public override Expression? Visit(Expression? node)
		{
			if (node != null && replacements.TryGetValue(node, out var matches))
			{
				var column = matches.Dequeue();
				return Expression.Convert(Expression.Call(ReadProjectedValueMethod,
					reader, Expression.Constant(column.Index), Expression.Constant(column.Field)), node.Type);
			}

			return base.Visit(node);
		}
	}

}