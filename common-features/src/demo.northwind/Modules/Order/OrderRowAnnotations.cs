namespace Serenity.Demo.Northwind.Annotations;

/// <summary>
/// This is annotation sample for OrderRow. Annotation types 
/// are normally not required, and you could put these attributes
/// directly on OrderRow properties, but such an annotation type
/// lets you to move out UI specific dependencies from the row,
/// so that you could move OrderRow itself into a separate 
/// class library, e.g. a DLL with just the entities, if desired.
/// </summary>
[AnnotationType(typeof(OrderRow))]
public sealed class OrderRowAnnotations
{
    [CustomerEditor]
    public string CustomerID { get; set; }

    [AsyncLookupEditor(typeof(EmployeeRow))]
    public int? EmployeeID { get; set; }

    [AsyncLookupEditor(typeof(ShipperRow))]
    public int? ShipVia { get; set; }
}