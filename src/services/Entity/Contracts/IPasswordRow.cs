namespace Serenity.Data;

/// <summary>
/// An interface that provides access to the password hash and salt fields.
/// </summary>
public interface IPasswordRow
{
    /// <summary>
    /// Gets the password hash field.
    /// </summary>
    StringField PasswordHashField { get; }

    /// <summary>
    /// Gets the password salt field.
    /// </summary>
    StringField PasswordSaltField { get; }
}