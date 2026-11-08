namespace Serenity.Testing;

/// <summary>
/// Sample string enumeration used by the StringEnum tests.
/// </summary>
[EnumKey("My.WidgetCategory")]
public abstract class WidgetCategory : StringEnum
{
    [Description("Charts")]
    [IconClass("sv-ph-chart-bar-duotone")]
    public const string Charts = "charts";

    public const string Tables = "tables";

    [DescriptionText("Outlines", TextKey = "Site.Outlines")]
    public const string Outlines = "outlines";

    [IgnoreUIField]
    public const string Internal = "internal";

    [TransformIgnore]
    public const string System = "system";

    /// <summary>
    /// An alias of <see cref="Charts"/>, like an enum member with the same value.
    /// </summary>
    public const string ChartsAlias = "charts";

    /// <summary>
    /// Not a member, as it is not a string constant.
    /// </summary>
    public const int NotAString = 1;

    /// <summary>
    /// Not a member, as it is not a constant.
    /// </summary>
    public static readonly string NotAConstant = "notaconstant";
}

/// <summary>
/// A string enumeration deriving from another one. Only its own members are part of its set, like the
/// server typings generator, which reads the declared members only.
/// </summary>
public abstract class WidgetCategoryExtended : WidgetCategory
{
    public const string Maps = "maps";
}

/// <summary>
/// A string enumeration without members.
/// </summary>
public abstract class EmptyStringEnum : StringEnum
{
}
