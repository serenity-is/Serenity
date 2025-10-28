namespace Serenity.ComponentModel;

/// <summary>
/// Marks the column / form field as unbound (non-data / UI-only).
/// As it derives from SkipNameCheckAttribute it skips checking the property name when [BasedOnRow] 
/// attribute is used. When used on a column property, it will have no field name in the generated column, 
/// only a column id. This is useful for columns that do not correspond to actual data fields, 
/// such as ones that host action buttons, status indicators etc. When used on a form field property,
/// the form will not try to load or save any value for it, similar to how SkipOnSave / SkipOnLoad works.
/// This can be considered a mixture of [SkipNameCheck] and [SkipOnLoad]/[SkipOnSave].
/// </summary>
public class UnboundAttribute : SkipNameCheckAttribute
{
}