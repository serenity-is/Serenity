namespace Serenity.Data;

/// <summary>
/// Interfaces for types that has a connection StateChange event (e.g. connection)
/// </summary>
public interface IHasConnectionStateChange
{
    /// <summary>
    /// State change event
    /// </summary>
    event StateChangeEventHandler StateChange;
}