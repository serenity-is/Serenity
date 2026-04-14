
namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether the CSS class for selected cells (default "selected") is applied
/// to cells in this column when the containing row is selected. By default, the selected
/// CSS class is applied to all columns in a selected row. Set this attribute to false
/// to exclude specific columns from receiving the selected styling. This is useful for
/// columns with custom formatters or styling that don't work well with the selected appearance.
/// </summary>
/// <remarks>
/// This attribute only affects the visual styling of cells when rows are selected.
/// It does not control whether cells can be navigated to or edited.
/// </remarks>
/// <param name="value">if set to <c>true</c> (default), the selected CSS class will be applied
/// to cells in this column when the row is selected; otherwise, it will not.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]

public class ShowSelectionAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether the selected CSS class should be applied
    /// to cells in this column when the containing row is selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if the selected CSS class should be applied; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}
