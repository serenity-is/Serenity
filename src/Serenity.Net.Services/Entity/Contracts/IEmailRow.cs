namespace Serenity.Data;

/// <summary>
/// An interface that provides access to email field
/// </summary>
public interface IEmailRow
{
    /// <summary>
    /// Gets email field
    /// </summary>
    StringField EmailField { get; }
}