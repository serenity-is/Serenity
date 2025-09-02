namespace Serenity.ComponentModel;

/// <summary>
/// Represents a customized column / form script
/// </summary>
public interface ICustomizePropertyItems
{
    /// <summary>
    /// Customizes the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    void Customize(List<PropertyItem> input);
}