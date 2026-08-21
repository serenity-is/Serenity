namespace Serenity.Data;

/// <summary>
/// An interface that provides access to the email field.
/// </summary>
public interface IEmailRow
{
    /// <summary>
    /// Gets the email field.
    /// </summary>
    StringField EmailField { get; }
}