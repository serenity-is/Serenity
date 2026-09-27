namespace Serenity.Data;

/// <summary>
/// A reusable projected query that can be executed with per-call parameter overrides.
/// </summary>
/// <typeparam name="TResult">The flat projection result type.</typeparam>
public interface IProjectedQuery<TResult>
{
	/// <summary>
	/// Executes the projection and returns its results. Results are buffered by default.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="parameters">Optional parameter values to override for this execution.</param>
	/// <param name="buffered">Whether to buffer all results before returning.</param>
	/// <returns>The projected results.</returns>
	IEnumerable<TResult> Query(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null, bool buffered = true);

	/// <summary>
	/// Executes the projection and buffers its results.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="parameters">Optional parameter values to override for this execution.</param>
	/// <returns>The projected results.</returns>
	List<TResult> List(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null);

	/// <summary>
	/// Asynchronously streams the projected results.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="parameters">Optional parameter values to override for this execution.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An asynchronous stream of projected results.</returns>
	IAsyncEnumerable<TResult> QueryAsync(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously executes the projection and buffers its results.
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="parameters">Optional parameter values to override for this execution.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task representing the asynchronous operation. The task result is the projected list.</returns>
	Task<List<TResult>> ListAsync(IDbConnection connection,
		IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
}