namespace Serenity.ComponentModel;

/// <summary>
/// Determines label with of target property, and optionally 
/// the properties following it until another of this attribute
/// is used.
/// </summary>
/// <seealso cref="Attribute" />
public class LabelWidthAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelWidthAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public LabelWidthAttribute(int value)
    {
        Value = value + "px";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelWidthAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public LabelWidthAttribute(string? value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this attribute should apply 
    /// to following properties until next occurence of this attribute 
    /// without JustThis flag.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [until next]; otherwise, <c>false</c>.
    /// </value>
    public bool UntilNext { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this attribute
    /// shouldn't break effectiveness of another LabelWidth attribute 
    /// with UntilNext flag.
    /// For example, if you set all fields to 100px by adding a
    /// [LabelWidth("100px", UntilNext = true)] attribute to the first 
    /// property of a form, but just want to change one property in 
    /// the middle to 200px, and if you add [LabelWidth("200px")] 
    /// to that property, 100px would not apply to following ones.
    /// But if you did [LabelWidth("200px", JustThis = true)], 100px
    /// will still apply to following ones.
    /// </summary>
    /// <value>
    ///   <c>true</c> if it should break effectiveness of a prior LabelWidth
    ///   attribute with UntilNext flag, otherwise, <c>false</c>.
    /// </value>
    public bool JustThis { get; set; }
}