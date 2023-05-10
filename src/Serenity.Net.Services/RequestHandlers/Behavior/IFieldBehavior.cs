namespace Serenity.Services;

/// <summary>
/// Represents a request handler behavior that is targeted to a field.
/// </summary>
public interface IFieldBehavior
{
    /// <summary>
    /// Gets / sets the target field that current behavior should operate on
    /// </summary>
    Field Target { get; set; }
}