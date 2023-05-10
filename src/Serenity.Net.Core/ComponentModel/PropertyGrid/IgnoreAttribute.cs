namespace Serenity.ComponentModel;

/// <summary>
/// Skips a property while generating grid column or form field list.
/// Use this to ignore a property for UI, but still use it for other 
/// purposes like JSON serialization.
/// This might be useful when a type is used as a Service Request and Form
/// Declaration at the same time.
/// </summary>
public class IgnoreAttribute : Attribute
{
}