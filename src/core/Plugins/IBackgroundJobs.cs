namespace Serenity.Plugins;

/// <summary>
/// Interface for plugins that have a list of background jobs.
/// </summary>
public interface IBackgroundJobs
{
    /// <summary>
    /// Gets the list of background jobs.
    /// </summary>
    /// <returns>The list of background job types.</returns>
    IEnumerable<Type> GetBackgroundJobs();
}
