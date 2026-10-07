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
/// and object initializers whose values are direct row field accesses. Sources are inferred
/// from query row fields when each selector parameter has one unique match, or can be supplied
/// explicitly. Unbuffered results keep the data reader open until enumeration completes or
/// the enumerator is disposed.
/// </remarks>
public static class EntitySqlQueryProjection
{
	private readonly record struct PreparedProjection<TResult>(SqlQuery Query,
		Func<IDataReader, TResult> Materializer);

	private readonly record struct ProjectionColumn(Expression ProjectionExpression, Field? Field,
		string SqlExpression, string Name, int Index);

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// The query must not already have SELECT columns. Each selector parameter must match one
	/// unambiguous row-fields source in the query, or sources can be supplied explicitly.
	/// </summary>
	/// <typeparam name="TRow">The row type of the projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="buffered">Whether to buffer all results before returning.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <returns>The projected results.</returns>
	public static IEnumerable<TResult> QueryProjected<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection, bool buffered = true,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow : class, IRow
	{
		return QueryProjectedCore<TResult>(query, connection, projection, buffered, parameters,
			takeOwnership: takeOwnership);
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// </summary>
	/// <typeparam name="TRow">The row type of the projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <returns>The projected results.</returns>
	public static List<TResult> ListProjected<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow : class, IRow
	{
		return [.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false, parameters,
			takeOwnership: takeOwnership)];
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// When <paramref name="buffered"/> is false, the data reader remains open until enumeration
	/// completes or the enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="buffered">Whether to buffer all results before returning.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <returns>The projected results.</returns>
	public static IEnumerable<TResult> QueryProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection, bool buffered = true,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		return QueryProjectedCore<TResult>(query, connection, projection, buffered, parameters,
			takeOwnership: takeOwnership);
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <returns>The projected results.</returns>
	public static List<TResult> ListProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		return [.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false, parameters,
			takeOwnership: takeOwnership)];
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// When <paramref name="buffered"/> is false, the data reader remains open until enumeration
	/// completes or the enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TRow3">The row type of the third projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="buffered">Whether to buffer all results before returning.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <returns>The projected results.</returns>
	public static IEnumerable<TResult> QueryProjected<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection, bool buffered = true,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
		where TRow3 : class, IRow
	{
		return QueryProjectedCore<TResult>(query, connection, projection, buffered, parameters,
			takeOwnership: takeOwnership);
	}

	/// <summary>
	/// Executes the query and materializes each result row into the specified flat projection.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TRow3">The row type of the third projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <returns>The projected results.</returns>
	public static List<TResult> ListProjected<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
		where TRow3 : class, IRow
	{
		return [.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false, parameters,
			takeOwnership: takeOwnership)];
	}

	/// <summary>
	/// Prepares a reusable flat projection. The source query must be a root query without existing SELECT columns.
	/// </summary>
	/// <typeparam name="TRow">The row type of the projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to prepare.</param>
	/// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; an already-frozen source cannot be taken over.</param>
	/// <returns>A reusable projection that can be executed with different parameter values.</returns>
	public static IProjectedQuery<TResult> AsReusableProjected<TRow, TResult>(this SqlQuery query,
		Expression<Func<TRow, TResult>> projection, bool takeOwnership = false)
		where TRow : class, IRow
	{
		return CreateReusableProjection<TResult>(query, projection, takeOwnership: takeOwnership);
	}

	/// <summary>
	/// Prepares a reusable flat projection. The source query must be a root query without existing SELECT columns.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to prepare.</param>
	/// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; an already-frozen source cannot be taken over.</param>
	/// <returns>A reusable projection that can be executed with different parameter values.</returns>
	public static IProjectedQuery<TResult> AsReusableProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		Expression<Func<TRow1, TRow2, TResult>> projection, bool takeOwnership = false)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		return CreateReusableProjection<TResult>(query, projection, takeOwnership: takeOwnership);
	}

	/// <summary>
	/// Prepares a reusable flat projection. The source query must be a root query without existing SELECT columns.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TRow3">The row type of the third projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to prepare.</param>
	/// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; an already-frozen source cannot be taken over.</param>
	/// <returns>A reusable projection that can be executed with different parameter values.</returns>
	public static IProjectedQuery<TResult> AsReusableProjected<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		Expression<Func<TRow1, TRow2, TRow3, TResult>> projection, bool takeOwnership = false)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
		where TRow3 : class, IRow
	{
		return CreateReusableProjection<TResult>(query, projection, takeOwnership: takeOwnership);
	}

    /// <summary>
    /// Asynchronously executes the query and buffers the flat projection results.
    /// </summary>
    /// <typeparam name="TRow">The row type of the projected source.</typeparam>
    /// <typeparam name="TResult">The flat projection result type.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="projection">A flat projection built from direct row field accesses.</param>
    /// <param name="parameters">Values that override the source query's parameters for this execution.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
    public static Task<List<TResult>> ListProjectedAsync<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false,
        CancellationToken cancellationToken = default)
		where TRow : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, takeOwnership: takeOwnership);
		return BufferProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>
	/// Asynchronously executes the query and buffers the flat projection results.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
	public static Task<List<TResult>> ListProjectedAsync<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false,
        CancellationToken cancellationToken = default)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, takeOwnership: takeOwnership);
		return BufferProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>
	/// Asynchronously executes the query and buffers the flat projection results.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TRow3">The row type of the third projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
	public static Task<List<TResult>> ListProjectedAsync<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false, 
        CancellationToken cancellationToken = default)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
		where TRow3 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, takeOwnership: takeOwnership);
		return BufferProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>
	/// Asynchronously streams the flat projection results. The data reader remains open until
	/// enumeration completes, is cancelled, or the async enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow">The row type of the projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An asynchronous stream of projected results.</returns>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false,
        CancellationToken cancellationToken = default)
		where TRow : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, takeOwnership: takeOwnership);
		return EnumerateProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>
	/// Asynchronously streams the flat projection results. The data reader remains open until
	/// enumeration completes, is cancelled, or the async enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An asynchronous stream of projected results.</returns>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false, 
        CancellationToken cancellationToken = default)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, takeOwnership: takeOwnership);
		return EnumerateProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>
	/// Asynchronously streams the flat projection results. The data reader remains open until
	/// enumeration completes, is cancelled, or the async enumerator is disposed.
	/// </summary>
	/// <typeparam name="TRow1">The row type of the first projected source.</typeparam>
	/// <typeparam name="TRow2">The row type of the second projected source.</typeparam>
	/// <typeparam name="TRow3">The row type of the third projected source.</typeparam>
	/// <typeparam name="TResult">The flat projection result type.</typeparam>
	/// <param name="query">The query to execute.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="projection">A flat projection built from direct row field accesses.</param>
	/// <param name="parameters">Values that override the source query's parameters for this execution.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place instead of cloning it. The source must not be frozen.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An asynchronous stream of projected results.</returns>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false, 
        CancellationToken cancellationToken = default)
		where TRow1 : class, IRow
		where TRow2 : class, IRow
		where TRow3 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, takeOwnership: takeOwnership);
		return EnumerateProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>Executes a projection using the specified row-fields source aliases.</summary>
	public static IEnumerable<TResult> QueryProjected<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, bool buffered = true,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow : class, IRow =>
		QueryProjectedCore<TResult>(query, connection, projection, buffered, parameters, sources, takeOwnership);

	/// <summary>Executes a projection using the specified row-fields source aliases.</summary>
	public static IEnumerable<TResult> QueryProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, bool buffered = true,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow1 : class, IRow where TRow2 : class, IRow =>
		QueryProjectedCore<TResult>(query, connection, projection, buffered, parameters, sources, takeOwnership);

	/// <summary>Executes a projection using the specified row-fields source aliases.</summary>
	public static IEnumerable<TResult> QueryProjected<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, bool buffered = true,
		IReadOnlyDictionary<string, object?>? parameters = null, bool takeOwnership = false)
		where TRow1 : class, IRow where TRow2 : class, IRow where TRow3 : class, IRow =>
		QueryProjectedCore<TResult>(query, connection, projection, buffered, parameters, sources, takeOwnership);

	/// <summary>Executes and buffers a projection using the specified row-fields source aliases.</summary>
	public static List<TResult> ListProjected<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false)
		where TRow : class, IRow =>
		[.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false, parameters, sources, takeOwnership)];

	/// <summary>Executes and buffers a projection using the specified row-fields source aliases.</summary>
	public static List<TResult> ListProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false)
		where TRow1 : class, IRow where TRow2 : class, IRow =>
		[.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false, parameters, sources, takeOwnership)];

	/// <summary>Executes and buffers a projection using the specified row-fields source aliases.</summary>
	public static List<TResult> ListProjected<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false)
		where TRow1 : class, IRow where TRow2 : class, IRow where TRow3 : class, IRow =>
		[.. QueryProjectedCore<TResult>(query, connection, projection, buffered: false, parameters, sources, takeOwnership)];

	/// <summary>Prepares a reusable projection using the specified row-fields source aliases.</summary>
	/// <param name="query">The query to prepare.</param>
	/// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
	/// <param name="sources">The row-fields sources for the projection parameters.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
	/// <returns>A reusable projection that can be executed with different parameter values.</returns>
	public static IProjectedQuery<TResult> AsReusableProjected<TRow, TResult>(this SqlQuery query,
		Expression<Func<TRow, TResult>> projection, IReadOnlyList<RowFieldsBase> sources, bool takeOwnership = false)
		where TRow : class, IRow => CreateReusableProjection<TResult>(query, projection, sources, takeOwnership);

	/// <summary>Prepares a reusable projection using the specified row-fields source aliases.</summary>
	/// <param name="query">The query to prepare.</param>
	/// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
	/// <param name="sources">The row-fields sources for the projection parameters.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
	/// <returns>A reusable projection that can be executed with different parameter values.</returns>
	public static IProjectedQuery<TResult> AsReusableProjected<TRow1, TRow2, TResult>(this SqlQuery query,
		Expression<Func<TRow1, TRow2, TResult>> projection, IReadOnlyList<RowFieldsBase> sources, bool takeOwnership = false)
		where TRow1 : class, IRow where TRow2 : class, IRow => CreateReusableProjection<TResult>(query, projection, sources, takeOwnership);

	/// <summary>Prepares a reusable projection using the specified row-fields source aliases.</summary>
	/// <param name="query">The query to prepare.</param>
	/// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
	/// <param name="sources">The row-fields sources for the projection parameters.</param>
	/// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
	/// <returns>A reusable projection that can be executed with different parameter values.</returns>
	public static IProjectedQuery<TResult> AsReusableProjected<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		Expression<Func<TRow1, TRow2, TRow3, TResult>> projection, IReadOnlyList<RowFieldsBase> sources, bool takeOwnership = false)
		where TRow1 : class, IRow where TRow2 : class, IRow where TRow3 : class, IRow => CreateReusableProjection<TResult>(query, projection, sources, takeOwnership);

	/// <summary>Executes and buffers a projection asynchronously using the specified row-fields source aliases.</summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
    /// <param name="sources">The row-fields sources for the projection parameters.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
	public static Task<List<TResult>> ListProjectedAsync<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
        bool takeOwnership = false, CancellationToken cancellationToken = default)
		where TRow : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		return BufferProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

    /// <summary>Executes and buffers a projection asynchronously using the specified row-fields source aliases.</summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
    /// <param name="sources">The row-fields sources for the projection parameters.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task<List<TResult>> ListProjectedAsync<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false, CancellationToken cancellationToken = default)
		where TRow1 : class, IRow where TRow2 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		return BufferProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>Executes and buffers a projection asynchronously using the specified row-fields source aliases.</summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
    /// <param name="sources">The row-fields sources for the projection parameters.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
	public static Task<List<TResult>> ListProjectedAsync<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false, CancellationToken cancellationToken = default)
		where TRow1 : class, IRow where TRow2 : class, IRow where TRow3 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		return BufferProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

    /// <summary>Streams a projection asynchronously using the specified row-fields source aliases.</summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
    /// <param name="sources">The row-fields sources for the projection parameters.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false, CancellationToken cancellationToken = default)
		where TRow : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		return EnumerateProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>Streams a projection asynchronously using the specified row-fields source aliases.</summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
    /// <param name="sources">The row-fields sources for the projection parameters.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow1, TRow2, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false, CancellationToken cancellationToken = default)
		where TRow1 : class, IRow where TRow2 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		return EnumerateProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	/// <summary>Streams a projection asynchronously using the specified row-fields source aliases.</summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="connection">The database connection.</param>
    /// <param name="projection">A flat projection built from row field accesses or SQL expressions.</param>
    /// <param name="sources">The row-fields sources for the projection parameters.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="takeOwnership">Whether to prepare the source query in place. If false, a clone is prepared; a frozen source cannot be taken over.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
	public static IAsyncEnumerable<TResult> QueryProjectedAsync<TRow1, TRow2, TRow3, TResult>(this SqlQuery query,
		IDbConnection connection, Expression<Func<TRow1, TRow2, TRow3, TResult>> projection,
		IReadOnlyList<RowFieldsBase> sources, IReadOnlyDictionary<string, object?>? parameters = null,
		bool takeOwnership = false, CancellationToken cancellationToken = default)
		where TRow1 : class, IRow where TRow2 : class, IRow where TRow3 : class, IRow
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		return EnumerateProjectedAsync(prepared.Query, connection, prepared.Materializer, parameters, cancellationToken);
	}

	private static IEnumerable<TResult> QueryProjectedCore<TResult>(SqlQuery query,
		IDbConnection connection, LambdaExpression projection, bool buffered,
		IReadOnlyDictionary<string, object?>? parameters, IReadOnlyList<RowFieldsBase>? sources = null,
		bool takeOwnership = false)
	{
		ArgumentNullException.ThrowIfNull(connection);
		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		var results = EnumerateProjected(prepared.Query, connection, prepared.Materializer, parameters);
		return buffered ? [.. results] : results;
	}

	private static PreparedProjection<TResult> PrepareProjection<TResult>(SqlQuery query,
		LambdaExpression projection, IReadOnlyList<RowFieldsBase>? sources = null, bool takeOwnership = false)
	{
		ArgumentNullException.ThrowIfNull(query);
		ArgumentNullException.ThrowIfNull(projection);

		if (takeOwnership && query.IsFrozen)
			throw new InvalidOperationException("Cannot take ownership of a frozen query.");

		var projectedQuery = takeOwnership ? query : query.Clone();
		var extensible = (ISqlQueryExtensible)projectedQuery;
		if (extensible.Columns.Count != 0)
			throw new InvalidOperationException("QueryProjected requires a query without existing SELECT columns.");

		var projectionSources = ResolveProjectionSources(projectedQuery, projection.Parameters, sources);

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
			if (TryGetSqlExpression(column.Expression, out var sqlExpression))
			{
				selectedColumns.Add(new ProjectionColumn(column.Expression, null, sqlExpression,
					column.Name, selectedColumns.Count));
				continue;
			}

			var path = GetFieldPath(column.Expression, projection.Parameters);
			var field = ResolveField(projectedQuery,
				projectionSources[path.ParameterIndex], path.MemberNames);
			selectedColumns.Add(new ProjectionColumn(column.Expression, field, field.Expression,
				column.Name, selectedColumns.Count));
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
			projectedQuery.Select(column.SqlExpression, column.Name);

		return new PreparedProjection<TResult>(projectedQuery, materializer);
	}

	private static RowFieldsBase[] ResolveProjectionSources(SqlQuery query,
		ReadOnlyCollection<ParameterExpression> parameters, IReadOnlyList<RowFieldsBase>? explicitSources)
	{
		var candidates = GetProjectionSources(query);
		if (explicitSources is not null)
		{
			if (explicitSources.Count != parameters.Count)
				throw new ArgumentException("The source fields count must match the projection parameter count.", nameof(explicitSources));

			var result = new RowFieldsBase[parameters.Count];
			for (var index = 0; index < parameters.Count; index++)
			{
				var fields = explicitSources[index] ?? throw new ArgumentException(
					"Projection source fields cannot be null.", nameof(explicitSources));
				if (!candidates.Any(candidate =>
					string.Equals(candidate.AliasName, fields.AliasName, StringComparison.OrdinalIgnoreCase) &&
					candidate.GetType() == fields.GetType()))
					throw new ArgumentException(string.Format(
						"Projection source alias '{0}' is not present in the query.", fields.AliasName), nameof(explicitSources));

				EnsureCompatibleSource(parameters[index], fields, index);
				result[index] = fields;
			}

			return result;
		}

		var assignments = new RowFieldsBase[parameters.Count];
		var current = new RowFieldsBase[parameters.Count];
		var usedAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var matchCount = 0;

		void FindMatches(int parameterIndex)
		{
			if (matchCount > 1)
				return;

			if (parameterIndex == parameters.Count)
			{
				matchCount++;
				if (matchCount == 1)
					Array.Copy(current, assignments, current.Length);
				return;
			}

			var parameterType = parameters[parameterIndex].Type;
			foreach (var fields in candidates)
			{
				var rowType = fields.GetType().DeclaringType;
				if (rowType is null || !parameterType.IsAssignableFrom(rowType) ||
					!usedAliases.Add(fields.AliasName))
					continue;

				current[parameterIndex] = fields;
				FindMatches(parameterIndex + 1);
				usedAliases.Remove(fields.AliasName);
			}
		}

		FindMatches(0);
		if (matchCount == 1)
			return assignments;

		throw new InvalidOperationException(matchCount == 0
			? "No unique mapping was found between projection parameters and query row-fields sources."
			: "Projection parameters match multiple query row-fields sources; supply source fields explicitly.");
	}

	private static List<RowFieldsBase> GetProjectionSources(SqlQuery query)
	{
		var extensible = (ISqlQueryExtensible)query;
		var sources = new List<RowFieldsBase>();
		var sourceAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		void AddSource(RowFieldsBase fields)
		{
			if (sourceAliases.Add(fields.AliasName))
				sources.Add(fields);
		}

		void AddMetadataJoins(RowFieldsBase fields)
		{
			foreach (var join in fields.Joins.Values)
			{
				if (!query.HasAlias(join.Name) || join.RowType is not Type rowType)
					continue;

				var fieldsType = rowType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
					.FirstOrDefault(t => typeof(RowFieldsBase).IsAssignableFrom(t) && !t.IsAbstract);
				if (fieldsType is null)
					continue;

				var joinedFields = RowFieldsProvider.Current.ResolveWithAlias(fieldsType, join.Name);
				if (sourceAliases.Add(join.Name))
				{
					sources.Add(joinedFields);
					AddMetadataJoins(joinedFields);
				}
			}
		}

		foreach (var source in extensible.FromSources.Concat(extensible.JoinSources))
			if (source is RowFieldsBase fields)
			{
				AddSource(fields);
				AddMetadataJoins(fields);
			}

		return sources;
	}

	private static void EnsureCompatibleSource(ParameterExpression parameter,
		RowFieldsBase fields, int index)
	{
		var rowType = fields.GetType().DeclaringType;
		if (rowType is null || !parameter.Type.IsAssignableFrom(rowType))
			throw new ArgumentException(string.Format(
				"Projection source {0} has row type '{1}', which is not compatible with projection parameter type '{2}'.",
				index, rowType?.FullName ?? fields.GetType().FullName, parameter.Type.FullName), nameof(parameter));
	}

	private static ReusableProjectedQuery<TResult> CreateReusableProjection<TResult>(SqlQuery query,
		LambdaExpression projection, IReadOnlyList<RowFieldsBase>? sources = null, bool takeOwnership = false)
	{
		ArgumentNullException.ThrowIfNull(query);
		if (((ISqlQuery)query).Parent is not null)
			throw new NotSupportedException("Reusable projections require a root SqlQuery with independent parameters.");

		var prepared = PrepareProjection<TResult>(query, projection, sources, takeOwnership);
		prepared.Query.Freeze();

		return new ReusableProjectedQuery<TResult>(prepared.Query, prepared.Materializer);
	}

	internal static IEnumerable<TResult> EnumerateProjected<TResult>(SqlQuery query,
		IDbConnection connection, Func<IDataReader, TResult> materializer,
		IReadOnlyDictionary<string, object?>? parameters = null)
	{
		using var reader = query.ExecuteReader(connection, parameters);
		while (reader.Read())
			yield return materializer(reader);
	}

	internal static async Task<List<TResult>> BufferProjectedAsync<TResult>(SqlQuery query,
		IDbConnection connection, Func<IDataReader, TResult> materializer,
		IReadOnlyDictionary<string, object?>? parameters, CancellationToken cancellationToken)
	{
		var results = new List<TResult>();
		await foreach (var result in EnumerateProjectedAsync(query, connection, materializer, parameters, cancellationToken)
			.ConfigureAwait(false))
			results.Add(result);

		return results;
	}

	internal static async IAsyncEnumerable<TResult> EnumerateProjectedAsync<TResult>(SqlQuery query,
		IDbConnection connection, Func<IDataReader, TResult> materializer,
		IReadOnlyDictionary<string, object?>? parameters = null,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		using var reader = await query.ExecuteReaderAsync(connection, parameters, cancellationToken: cancellationToken)
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

	private static bool TryGetSqlExpression(Expression expression, out string sqlExpression)
	{
		expression = StripConvert(expression);
		if (expression is not MethodCallExpression methodCall ||
			methodCall.Method.DeclaringType != typeof(Sql) ||
			methodCall.Method.Name != nameof(Sql.Expr) ||
			!methodCall.Method.IsGenericMethod ||
			methodCall.Arguments.Count != 1)
		{
			sqlExpression = null!;
			return false;
		}

		if (GetCapturedValue(methodCall.Arguments[0]) is not string value || string.IsNullOrWhiteSpace(value))
			throw new NotSupportedException("Sql.Expr requires a non-empty SQL string literal or captured string variable.");

		sqlExpression = value;
		return true;
	}

	private static object? GetCapturedValue(Expression expression)
	{
		expression = StripConvert(expression);
		if (expression is ConstantExpression constant)
			return constant.Value;

		if (expression is MemberExpression { Member: FieldInfo field } member)
		{
			var target = member.Expression is null ? null : GetCapturedValue(member.Expression);
			return field.GetValue(target);
		}

		throw new NotSupportedException("Sql.Expr requires a SQL string literal or captured string variable.");
	}

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

	private static object? ReadProjectedValue(IDataReader reader, int index, Field? field, Type targetType)
	{
		var value = reader.IsDBNull(index) ? null : reader.GetValue(index);
		if (value is not null && field is not null)
			value = field.ConvertValue(value, CultureInfo.InvariantCulture);

		// Like Dapper and EF Core, materialize default(T) when the column is DBNull and the
		// target member is a non-nullable value type. Unboxing null into such a member would
		// otherwise throw a NullReferenceException. Nullable and reference members still get null.
		if (value is null && targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null)
			return Activator.CreateInstance(targetType);

		return value;
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
					reader, Expression.Constant(column.Index), Expression.Constant(column.Field, typeof(Field)),
					Expression.Constant(node.Type, typeof(Type))), node.Type);
			}

			return base.Visit(node);
		}
	}

}

/// <summary>
/// A prepared projected query that reuses its compiled materializer and prepared SQL query.
/// Each execution uses an independent parameter dictionary and can run concurrently.
/// </summary>
/// <typeparam name="TResult">The flat projection result type.</typeparam>
internal sealed class ReusableProjectedQuery<TResult> : IProjectedQuery<TResult>
{
	private readonly SqlQuery preparedQuery;
	private readonly Func<IDataReader, TResult> materializer;

	internal ReusableProjectedQuery(SqlQuery preparedQuery,
		Func<IDataReader, TResult> materializer)
	{
		this.preparedQuery = preparedQuery;
		this.materializer = materializer;
	}

	public IEnumerable<TResult> Query(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool buffered = true)
	{
		ArgumentNullException.ThrowIfNull(connection);
		return buffered ? List(connection, parameters) : Enumerate(connection, parameters);
	}

	/// <summary>
	/// Executes the prepared projection and buffers its results.
	/// Parameters are reset to the values captured when the projection was prepared, then overridden by <paramref name="parameters"/>.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="parameters">Optional parameter values to override for this execution.</param>
	/// <returns>The projected results.</returns>
	public List<TResult> List(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null)
	{
		ArgumentNullException.ThrowIfNull(connection);
		return [.. Enumerate(connection, parameters)];
	}

	public IAsyncEnumerable<TResult> QueryAsync(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(connection);
		return EnumerateAsync(connection, parameters, cancellationToken);
	}

	/// <summary>
	/// Asynchronously executes the prepared projection and buffers its results.
	/// Parameters are reset to the values captured when the projection was prepared, then overridden by <paramref name="parameters"/>.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="parameters">Optional parameter values to override for this execution.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
	public async Task<List<TResult>> ListAsync(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(connection);
		var results = new List<TResult>();
		await foreach (var result in EnumerateAsync(connection, parameters, cancellationToken).ConfigureAwait(false))
			results.Add(result);
		return results;
	}

	private static Dictionary<string, object?> CreateParameterOverrides(
		IReadOnlyDictionary<string, object?>? overrides)
	{
		var parameters = new Dictionary<string, object?>();

		if (overrides is not null)
			foreach (var (name, value) in overrides)
				parameters.Add(name, value);

		return parameters;
	}

	private IEnumerable<TResult> Enumerate(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters)
	{
		using var reader = preparedQuery.ExecuteReader(connection, CreateParameterOverrides(parameters));
		while (reader.Read())
			yield return materializer(reader);
	}

	private async IAsyncEnumerable<TResult> EnumerateAsync(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		using var reader = await preparedQuery.ExecuteReaderAsync(connection,
			CreateParameterOverrides(parameters), cancellationToken: cancellationToken).ConfigureAwait(false);
		while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
			yield return materializer(reader);
	}
}