namespace Serenity.Data;

/// <summary>
/// Interface for types that have a connection <see cref="StateChange"/> event (e.g. a connection).
/// </summary>
public interface IHasConnectionStateChange
{
    /// <summary>
    /// The state change event.
    /// </summary>
    event StateChangeEventHandler StateChange;
}