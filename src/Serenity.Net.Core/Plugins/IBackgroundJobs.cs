namespace Serenity.Plugins;

/// <summary>
/// Interface for plugins that has a list of background jobs
/// </summary>
public interface IBackgroundJobs
{
    /// <summary>
    /// Gets list of background jobs
    /// </summary>
    IEnumerable<Type> GetBackgroundJobs();
}
