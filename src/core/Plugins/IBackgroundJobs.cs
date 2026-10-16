namespace Serenity.Plugins;

/// <summary>
/// Interface for plugins that have a list of background jobs.
/// </summary>
/// <remarks>
/// This type is part of a legacy plugin system design and is no longer used by
/// the framework. Register background jobs through dependency injection instead.
/// </remarks>
public interface IBackgroundJobs
{
    /// <summary>
    /// Gets the list of background jobs.
    /// </summary>
    /// <returns>The list of background job types.</returns>
    IEnumerable<Type> GetBackgroundJobs();
}
