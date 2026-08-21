namespace Serenity.Services;

/// <summary>
/// Represents a request handler behavior that is targeted to a field.
/// </summary>
public interface IFieldBehavior
{
    /// <summary>
    /// Gets or sets the target field that the current behavior should operate on.
    /// </summary>
    Field Target { get; set; }
}