namespace Serenity.Data;

/// <summary>
/// Interface for types that have an <see cref="OpenedOnce"/> property that
/// determines if the connection has been opened at least once.
/// </summary>
public interface IHasOpenedOnce
{
    /// <summary>
    /// Gets the opened once info.
    /// </summary>
    bool OpenedOnce { get; }
}