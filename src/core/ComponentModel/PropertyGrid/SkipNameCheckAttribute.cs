namespace Serenity.ComponentModel;

/// <summary>
/// Skips validation of the property name against the row's field names when 
/// [BasedOnRow(CheckNames = true)] is used.
/// </summary>
public class SkipNameCheckAttribute : Attribute
{
}