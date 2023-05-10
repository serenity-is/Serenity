
namespace Serenity.Reporting;

/// <summary>
/// Base class for cell decorators, implementing all
/// <see cref="ICellDecorator"/> members.
/// </summary>
public abstract class BaseCellDecorator : ICellDecorator
{
    /// <inheritdoc/>
    public object Item { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public object Value { get; set; }

    /// <inheritdoc/>
    public string Background { get; set; }

    /// <inheritdoc/>
    public string Foreground { get; set; }

    /// <inheritdoc/>
    public string Format { get; set; }

    /// <inheritdoc/>
    public abstract void Decorate();
}
