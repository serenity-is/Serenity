namespace Serenity.Data;

/// <summary>
/// An interface that provides access to password hash and salt fields
/// </summary>
public interface IPasswordRow
{
    /// <summary>
    /// Gets password hash field
    /// </summary>
    StringField PasswordHashField { get; }

    /// <summary>
    /// Gets password salt field
    /// </summary>
    StringField PasswordSaltField { get; }
}