namespace Serenity.ComponentModel;

/// <summary>
/// An attribute that indicates this type is based on another row type.
/// Used with form, columns etc. types to map their properties to
/// corresponding property in a row, so that they inherit attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class BasedOnRowAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasedOnRowAttribute"/> class.
    /// </summary>
    /// <param name="rowType">Type of the row.</param>
    public BasedOnRowAttribute(Type rowType)
    {
        RowType = rowType;
    }

    /// <summary>
    /// Gets the type of the row.
    /// </summary>
    /// <value>
    /// The type of the row.
    /// </value>
    public Type RowType { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether to check names.
    /// If this is true (by default false), if a property name doesn't
    /// exactly match a property in the RowType row, Serenity will
    /// raise error. In that case you might add [IgnoreName] attribute 
    /// to properties that shouldn't be checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should check names; otherwise, <c>false</c>.
    /// </value>
    public bool CheckNames { get; set; }
}
