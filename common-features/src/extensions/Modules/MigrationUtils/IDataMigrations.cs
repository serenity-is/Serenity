namespace Serenity.Extensions;

/// <summary>
/// Applies the migrations of the application. Implemented by the project template
/// (<c>AppServices.DataMigrations</c>) and resolved by <see cref="HostMigrationExtensions.RunMigrationsIf"/>
/// for the <c>migrate</c> command, and by <c>Startup</c> on every normal start.
/// </summary>
/// <remarks>
/// Lives next to <see cref="MigrationUtils"/> so that a host can offer the command without
/// knowing the application assembly, and so the Serene and StartSharp templates share one
/// definition instead of each shipping its own copy.
/// </remarks>
public interface IDataMigrations
{
    /// <summary>
    /// Ensures the databases exist and applies any pending migrations.
    /// </summary>
    void Initialize();
}
