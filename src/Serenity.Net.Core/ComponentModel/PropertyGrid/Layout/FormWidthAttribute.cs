namespace Serenity.ComponentModel;

/// <summary>
/// Sets CSS class for field on forms only. Useful for Bootstrap grid, e.g. col-md-4 etc.
/// </summary>
public class FormWidthAttribute : Attribute
{
    /// <summary>
    /// Creates a FormWidth attribute with no initial column classes.
    /// Make sure to set properties to make it useful.
    /// </summary>
    public FormWidthAttribute()
    {
    }

    /// <summary>
    /// Creates a FormWidth attribute with given custom css class,
    /// like col-md-6 col-sm-4 etc.
    /// </summary>
    /// <param name="cssClass"></param>
    public FormWidthAttribute(string? cssClass)
    {
        Value = cssClass;
    }

    /// <summary>
    /// Gets / sets cols (1..12) in extra small devices (width &lt; 768px, mobile phones)
    /// </summary>
    public int XSmall
    {
        get { return Get("col-") ?? 0; }
        set { Set("col-", value); }
    }

    /// <summary>
    /// Gets / sets cols (1..12) in small devices (width >= 768px, tablets)
    /// </summary>
    public int Small
    {
        get { return Get("col-sm-") ?? 0; }
        set { Set("col-sm-", value); }
    }

    /// <summary>
    /// Gets / sets cols (1..12) in medium devices (width >= 992px, medium desktops)
    /// </summary>
    public int Medium
    {
        get { return Get("col-md-") ?? 0; }
        set { Set("col-md-", value); }
    }

    /// <summary>
    /// Gets / sets cols (1..12) in large devices (width >= 1200px, large desktops)
    /// </summary>
    public int Large
    {
        get { return Get("col-lg-") ?? 0; }
        set { Set("col-lg-", value); }
    }

    /// <summary>
    /// Gets the css class that will be applied to field
    /// </summary>
    public string? Value { get; protected set; }

    /// <summary>
    /// Applies this form width (e.g. bootstrap grid size) to all 
    /// following fields until next another FormWidth attribute
    /// </summary>
    public bool UntilNext { get; set; }

    /// <summary>
    /// Applies this form width (e.g. bootstrap grid size) to just 
    /// current field and doesn't cancel prior form width attribute
    /// with a UntilNext attribute. Don't set UntilNext = true if
    /// you set JustThis = true, as they are exclusive
    /// </summary>
    public bool JustThis { get; set; }

    private void Set(string prefix, int cols)
    {
        var parts = (Value ?? "").Split(' ');
        var index = Array.FindIndex(parts, x =>
            x.Length > prefix.Length &&
            x.StartsWith(prefix) &&
            x[prefix.Length] >= '0' &&
            x[prefix.Length] <= '9');

        if (index < 0)
        {
            if (cols <= 0)
                return;

            if (!string.IsNullOrEmpty(Value))
                Value += " ";
            Value += prefix + cols;
        }
        else
        {
            if (cols <= 0)
                Value = string.Join(" ", parts.Take(index).Concat(parts.Skip(index + 1)));
            else
            {
                parts[index] = prefix + cols;
                Value = string.Join(" ", parts);
            }
        }
    }

    private int? Get(string prefix)
    {
        var klass = (Value ?? "").Split(' ')
            .FirstOrDefault(x =>
                x.Length > prefix.Length &&
                x.StartsWith(prefix) &&
                x[prefix.Length] >= '0' &&
                x[prefix.Length] <= '9');

        if (klass == null)
            return null;

        if (!int.TryParse(klass[prefix.Length..], out int cols))
            return null;

        return cols;
    }
}