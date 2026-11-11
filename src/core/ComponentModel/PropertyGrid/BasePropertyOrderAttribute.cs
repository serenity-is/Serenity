namespace Serenity.ComponentModel;

/// <summary>
/// Orders the properties a type inherits from its base types relative to the ones it declares itself in its property items,
/// instead of the order the types are declared in their assembly, which doesn't tell which of a type and its base type comes first.
/// A type hierarchy where none of the types has this attribute keeps the declaration order. The order of the properties
/// declared in the same type is always their declaration order.
/// <para>
/// All its parts are optional, and each type of a hierarchy that has it applies it to its own base types:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="Position"/> places the properties of all the base types before, or after, the ones of the type.</description></item>
/// <item><description><see cref="TypesBefore"/> and <see cref="TypesAfter"/> place the properties of the listed base types before or after the ones of
/// the type, in the order of the lists, which wins over what a base type asks for itself.</description></item>
/// <item><description><see cref="WhenInherited"/> is for the type itself: its properties are placed first or last in any type that inherits it, e.g.
/// the audit properties of a base type, unless that type lists it.</description></item>
/// </list>
/// </summary>
/// <example>
/// <code>
/// [BasePropertyOrder(WhenInherited = BasePropertyPosition.After)]
/// public abstract class AuditOptions { /* CreatedBy, ... */ }                // always after the others
///
/// [BasePropertyOrder(Position = BasePropertyPosition.After)]
/// public abstract class DatasetOptions : TitleOptions { /* Dataset, ... */ }  // Dataset, ..., then Title, ...
///
/// public class ChartOptions : DatasetOptions { /* ChartType, ... */ }         // Dataset, ..., Title, ..., ChartType, ...
///
/// [BasePropertyOrder(TypesBefore = [typeof(AuditOptions)])]
/// public class OtherOptions : AuditOptions { /* ... */ }                      // Audit first, as it is listed
/// </code>
/// </example>
/// <param name="position">Where the properties of the base types are placed, relative to the ones of the type.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class BasePropertyOrderAttribute(BasePropertyPosition position = BasePropertyPosition.Default) : Attribute
{
    /// <summary>
    /// Gets or sets where the properties of the base types of the type are placed, relative to the ones of the type. The default
    /// is before. The ones of the types in <see cref="TypesBefore"/> and <see cref="TypesAfter"/>, and the ones of the base types that are
    /// placed by <see cref="WhenInherited"/>, are not placed by this.
    /// </summary>
    public BasePropertyPosition Position { get; set; } = position;

    /// <summary>
    /// Gets or sets the base types whose properties are placed before the ones of the type, in this order, and before the properties of the
    /// other base types. A type that is not a base type is ignored.
    /// </summary>
    public Type[]? TypesBefore { get; set; }

    /// <summary>
    /// Gets or sets the base types whose properties are placed after the ones of the type, in this order, and after the properties of the
    /// other base types. A type that is not a base type is ignored.
    /// </summary>
    public Type[]? TypesAfter { get; set; }

    /// <summary>
    /// Gets or sets where the properties of the type are placed in a type that inherits it, instead of the position that type gives its
    /// base types, which is <see cref="BasePropertyPosition.Default"/>. <see cref="BasePropertyPosition.Before"/> is before all
    /// the other properties, and <see cref="BasePropertyPosition.After"/> is after all of them, unless the inheriting type lists it.
    /// </summary>
    public BasePropertyPosition WhenInherited { get; set; }
}
