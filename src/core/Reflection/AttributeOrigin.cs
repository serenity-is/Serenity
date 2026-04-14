namespace Serenity;

/// <summary>
/// Attribute source types for <see cref="Reflection.IPropertyInfo.GetAttribute"/> and <see cref="Reflection.IPropertyInfo.GetAttributes"/>
/// </summary>
public enum AttributeOrigin
{
    /// <summary>
    /// Only attributes explicitly declared on the property itself. This does not include attributes that are inherited from base classes or interfaces, intrinsic attributes, annotation-derived attributes, or attributes from based on fields. It only includes attributes that are directly applied to the property.
    /// </summary>
    Explicit = 0,

    /// <summary>
    /// Attributes inherited from base classes or interfaces. This includes attributes that are declared on base classes or interfaces and are inherited by the property. 
    /// </summary>
    Inherit = 1,

    /// <summary>
    /// Attributes intrinsicly defined by <see cref="Reflection.IIntrinsicPropertyAttributeProvider" />
    /// </summary>
    Intrinsic = 2,

    /// <summary>
    /// Attributes derived from annotations, such as <see cref="AnnotationTypeAttribute"/>.
    /// </summary>
    Annotation = 4,

    /// <summary>
    /// From the row field that the property is based on, if any. This is used for properties that are based on fields via BasedOnRow attribute. 
    /// If a property is based on a field, then attributes from that field can be considered as well. If this flag is included, then attributes from
    /// the based on field whether explicitly declared, inherited, intrinsic, or annotation-derived will be included in the search for attributes,
    /// as Field object has an attribute list that does not include source for the attributes and there can be dynamic
    /// fields that are not based on a property but still have attributes.
    /// </summary>
    BasedOnField = 8,

    /// <summary>
    /// Represents a combination of all available member types, including inherited, intrinsic, annotation, and members
    /// based on fields.
    /// </summary>
    /// <remarks>This value is typically used when an operation should consider every possible member type
    /// without restriction. It is a bitwise combination of the Inherit, Intrinsic, Annotation, and BasedOnField
    /// flags.</remarks>
    All = Inherit | Intrinsic | Annotation | BasedOnField,

    /// <summary>
    /// Excludes inherited attributes, but includes intrinsic, annotation-derived, and based on field attributes. This is a combination of the Intrinsic, Annotation, and BasedOnField flags, but does not include the Inherit flag.
    /// </summary>
    ExcludeInherit = Intrinsic | Annotation | BasedOnField,

    /// <summary>
    /// Excludes attributes from based on fields, but includes inherited, intrinsic, and annotation-derived attributes. This is a combination of the Inherit, Intrinsic, and Annotation flags, but does not include the BasedOnField flag.
    /// </summary>
    ExcludeBasedOnField = Inherit | Intrinsic | Annotation,
}