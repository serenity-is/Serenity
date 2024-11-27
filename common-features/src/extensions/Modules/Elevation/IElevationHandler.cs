namespace Serenity.Abstractions;

/// <summary>
/// Interface for account elevation.
/// </summary>
public interface IElevationHandler
{
    /// <summary>
    /// Appends ElevationToken to cookies.
    /// </summary>
    void AppendElevationTokenToCookies();
    /// <summary>
    /// Checks to see if ElevationToken in cookies is valid.
    /// </summary>
    void ValidateElevationToken();
    /// <summary>
    /// Deletes ElevationToken.
    /// </summary>
    void DeleteToken();
}