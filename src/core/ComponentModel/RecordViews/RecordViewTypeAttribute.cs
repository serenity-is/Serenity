using System.Diagnostics.CodeAnalysis;

namespace Serenity.ComponentModel;

/// <summary>
/// Adds a record view to a property, a client side component that shows something about the record of the property,
/// e.g. a card with the details of the customer of an order when its customer name is hovered in a grid. The view is
/// shown where its <see cref="Placement"/> says, which the host of the property, e.g. a grid, supports or not. The
/// view classes generate their own attributes, which derive from this one, with their options as properties.
/// A property may have more than one view. A view that is not about the value of a property, but about the
/// records of the whole grid, e.g. a card view, is added to its columns class instead.
/// </summary>
/// <param name="viewType">The key of the view type, e.g. "MyProject.Northwind.CustomerHoverCard".</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
public class RecordViewTypeAttribute(string viewType) : Attribute
{
    private Dictionary<string, object?>? options;

    /// <summary>
    /// Gets the key of the view type.
    /// </summary>
    public string ViewType { get; } = viewType;

    /// <summary>
    /// Gets or sets where the view is shown (default: <see cref="RecordViewPlacement.Trigger"/>).
    /// </summary>
    public RecordViewPlacement Placement { get; set; }

    /// <summary>
    /// Gets or sets the property that has the id of the record the view shows, e.g. "CustomerID" for a
    /// "CustomerCompanyName" property. It is determined from the join of the property when not set, as
    /// it is for the <see cref="EditLinkAttribute.IdField"/>.
    /// </summary>
    public string? IdField { get; set; }

    /// <summary>
    /// Gets or sets whether the view is the one that is shown first, when it is on a columns class, e.g. a card view
    /// that a grid opens with instead of its table. A user who has chosen another view of the grid keeps it.
    /// </summary>
    public bool Default
    {
        get => GetOption<bool>("default");
        set => SetOption("default", value);
    }

    /// <summary>
    /// Gets or sets the width of the screen below which the view is shown instead of the table, when it is on a
    /// columns class, e.g. a card view on a phone: the name of a Bootstrap breakpoint, "sm", "md", "lg", "xl" or "xxl",
    /// for the screens that are narrower than it, or a width in pixels, e.g. "700". It applies until the user chooses a
    /// view of the grid, which they then keep.
    /// </summary>
    public string? Below
    {
        get => GetOption<string>("below");
        set => SetOption("below", value);
    }

    /// <summary>
    /// Gets or sets whether the buttons that switch between the views of the grid are hidden, when the view is on a
    /// columns class, e.g. a card view: the grid only follows the width of the screen, with <see cref="Below"/>, or keeps
    /// the view that is shown first, with <see cref="Default"/>, and does not save the view in its settings. Without them
    /// the grid shows its table.
    /// </summary>
    public bool HideSwitch
    {
        get => GetOption<bool>("hideSwitch");
        set => SetOption("hideSwitch", value);
    }

    /// <summary>
    /// Copies the options of the view to the parameters of its <see cref="RecordViewInfo"/>.
    /// </summary>
    /// <param name="viewParams">The parameters.</param>
    public virtual void SetParams(IDictionary<string, object?> viewParams)
    {
        if (viewParams is null)
            throw new ArgumentNullException(nameof(viewParams));

        if (options != null)
            foreach (var opt in options)
                viewParams[opt.Key] = opt.Value;
    }

    /// <summary>
    /// Sets an option of the view.
    /// </summary>
    /// <param name="key">The key of the option, as the view class names it.</param>
    /// <param name="value">The value.</param>
    protected void SetOption(string key, object? value)
    {
        options ??= new(StringComparer.Ordinal);
        options[key] = value;
    }

    /// <summary>
    /// Gets an option of the view.
    /// </summary>
    /// <typeparam name="TType">The type of the option.</typeparam>
    /// <param name="key">The key of the option.</param>
    [return: MaybeNull]
    protected TType GetOption<TType>(string key)
    {
        if (options == null || !options.TryGetValue(key, out object? obj) || obj == null)
            return default;

        return (TType)obj;
    }
}
