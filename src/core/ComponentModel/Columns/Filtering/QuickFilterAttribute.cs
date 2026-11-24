namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this property should have a quick filter on the grid.
/// </summary>
/// <seealso cref="Attribute" />
/// <seealso cref="QuickFilterOptionAttribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="QuickFilterAttribute"/> class.
/// The options of the quick filter that this attribute has, e.g. <see cref="Multiple"/>, <see cref="CheckList"/>,
/// <see cref="CascadeFrom"/> or <see cref="Placeholder"/>, are the commonly used ones, and the others are set with
/// <see cref="QuickFilterOptionAttribute"/>, which can also override the ones of this attribute, as it is applied
/// after them, e.g. <c>[QuickFilter(Multiple = true), QuickFilterOption("multiple", false)]</c> makes a single filter.
/// </remarks>
/// <param name="value">if set to <c>true</c> quick filter is enabled.</param>
public class QuickFilterAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="QuickFilterAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="QuickFilterAttribute"/>
    /// should have a separator before others, e.g. wrap onto a new line.
    /// </summary>
    /// <value>
    ///   <c>true</c> if separator is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Separator { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for generated quick filter div.
    /// </summary>
    /// <value>
    /// The CSS class to add to the quick filter div.
    /// </value>
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the quick filter stays visible while the quick filter bar
    /// is collapsed behind its "Filters" button, which it is when it is narrow, e.g. on a phone.
    /// Use it for the one or two filters that are used most.
    /// </summary>
    public bool Pinned { get; set; }

    private bool? multiple;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="QuickFilterAttribute"/> allows multiple selections.
    /// Only applicable to quick filters that support multiple selection. When it is not set, the quick filter is
    /// multiple when its editor is, e.g. a lookup editor with its Multiple option, so false makes the quick filter
    /// of such an editor a single one. See <see cref="GetMultiple"/> to tell whether it is set, as an attribute
    /// property can not be nullable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if multiple selections are allowed; otherwise, <c>false</c>.
    /// </value>
    public bool Multiple
    {
        get => multiple ?? false;
        set => multiple = value;
    }

    /// <summary>
    /// Returns the value of <see cref="Multiple"/>, or null when it is not set, so that the editor decides.
    /// </summary>
    public bool? GetMultiple() => multiple;

    private bool? checkList;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="QuickFilterAttribute"/> should use a checklist for multiple selections.
    /// Only applicable to quick filters that support multiple selection. When it is not set, the quick filter is a
    /// checklist when its editor is, so false makes the quick filter of a checklist editor a regular multiple one.
    /// See <see cref="GetCheckList"/> to tell whether it is set, as an attribute property can not be nullable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if checklist is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool CheckList
    {
        get => checkList ?? false;
        set => checkList = value;
    }

    /// <summary>
    /// Returns the value of <see cref="CheckList"/>, or null when it is not set, so that the editor decides.
    /// </summary>
    public bool? GetCheckList() => checkList;

    private bool? range;

    /// <summary>
    /// Gets or sets a value indicating whether the quick filter of a date property edits a range of dates, e.g. the
    /// orders of this month, instead of a single date, which matches an exact value, e.g. the orders of a day. Only
    /// applicable to properties that have a date quick filter, which edits a range when the value of this property is
    /// true, a single date when it is false, and the way the grid decides otherwise, e.g. an application can give a
    /// range editor to all of its date columns. See <see cref="GetRange"/> to tell whether it is set, as an attribute
    /// property can not be nullable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the quick filter edits a range; <c>false</c> if it edits a single date.
    /// </value>
    public bool Range
    {
        get => range ?? false;
        set => range = value;
    }

    /// <summary>
    /// Returns the value of <see cref="Range"/>, or null when it is not set, so that the grid decides.
    /// </summary>
    public bool? GetRange() => range;

    /// <summary>
    /// Gets or sets the field from which the quick filter should cascade.
    /// Only applicable to quick filters that support cascade from selection.
    /// </summary>
    /// <value>
    /// The field from which the quick filter should cascade.
    /// </value>
    public string? CascadeFrom { get; set; }

    /// <summary>
    /// Gets or sets the placeholder of the quick filter, which is shown in its editor when it has no value. It is only for
    /// the quick filter, so the placeholder of the property in a form, see <see cref="PlaceholderAttribute"/>, is not
    /// changed. Like the one of <see cref="PlaceholderAttribute"/>, it is a local text, whose key is the one of the
    /// property with a "_QuickFilter_Placeholder" suffix, e.g. "Db.Northwind.Order.ShipCity_QuickFilter_Placeholder"
    /// for a row field, or "Columns.Northwind.Order.ShipCity_QuickFilter_Placeholder" for a columns class, unless
    /// <see cref="PlaceholderKey"/> gives one. An empty text shows no placeholder.
    /// </summary>
    /// <value>
    /// The placeholder text to display in the quick filter input.
    /// </value>
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the local text key of the placeholder of the quick filter, see <see cref="Placeholder"/>, which is
    /// used instead of the one the property implies. When <see cref="Placeholder"/> is set too, it is the default text
    /// of the key.
    /// </summary>
    /// <value>
    /// The key for the placeholder text to display in the quick filter input.
    /// </value>
    public string? PlaceholderKey { get; set; }
}
