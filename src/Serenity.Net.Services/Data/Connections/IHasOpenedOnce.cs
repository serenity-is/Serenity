namespace Serenity.Data;

/// <summary>
/// Interfaces for types that has an OpenedOnce property that
/// determines if the connection has opened at least once
/// </summary>
public interface IHasOpenedOnce
{
    /// <summary>
    /// Gets opened once info
    /// </summary>
    bool OpenedOnce { get; }
}