namespace Serenity.Data;

/// <summary>
/// An interface to provide access to a display name field
/// </summary>
public interface IDisplayNameRow
{
    /// <summary>
    /// Gets display name field
    /// </summary>
    StringField DisplayNameField { get; }
}