namespace Serenity.ComponentModel;

/// <summary>
/// Opts a type in to placing the properties it inherits from its base types before, or after, the properties it declares itself
/// in its property items, instead of the order the types are declared in their assembly, which doesn't tell which of a type
/// and its base type comes first.
/// <para>
/// It applies to each type of the hierarchy that has it, so a base type can place the properties of its own base types differently
/// than the type that inherits it. A type without it, along with the types that don't have it in its hierarchy, keeps the
/// declaration order. The order of the properties declared in the same type is always their declaration order.
/// </para>
/// </summary>
/// <example>
/// <code>
/// [BasePropertyOrder(BasePropertyPosition.After)]
/// public abstract class DatasetOptions : CommonOptions { /* Dataset, ... */ }  // Dataset, ..., then Title, ...
///
/// [BasePropertyOrder]
/// public class ChartOptions : DatasetOptions { /* ChartType, ... */ }          // Dataset, ..., Title, ..., ChartType, ...
/// </code>
/// </example>
/// <param name="position">Where the properties of the base types are placed.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class BasePropertyOrderAttribute(BasePropertyPosition position = BasePropertyPosition.Before) : Attribute
{
    /// <summary>
    /// Gets where the properties of the base types are placed, relative to the ones of the type.
    /// </summary>
    public BasePropertyPosition Position { get; } = position;
}
