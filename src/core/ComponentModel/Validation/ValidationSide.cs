namespace Serenity.ComponentModel;

/// <summary>
/// Where a validation rule is checked, see <see cref="IClientValidationRule.Side"/>.
/// </summary>
public enum ValidationSide
{
    /// <summary>By the forms and on the server, which is the default.</summary>
    Both = 0,
    /// <summary>Only by the forms, e.g. a hint for the user that the server doesn't need to enforce.</summary>
    Client = 1,
    /// <summary>Only on the server, e.g. a rule that needs the database.</summary>
    Server = 2
}
