namespace Serenity.Data;

/// <summary>
/// An interface used to abstract profilers like Mini Profiler.
/// </summary>
public interface IConnectionProfiler
{
    /// <summary>
    /// Wraps the specified connection with a profiled one.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The wrapped connection.</returns>
    IDbConnection Profile(IDbConnection connection);
}