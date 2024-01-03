namespace Serenity.TypeScript;

internal enum ModifierFlags
{
    None = 0,
    Export = 1 << 0, // Declarations
    Ambient = 1 << 1, // Declarations
    Public = 1 << 2, // Property/Method
    Private = 1 << 3, // Property/Method
    Protected = 1 << 4, // Property/Method
    Static = 1 << 5, // Property/Method
    Readonly = 1 << 6, // Property/Method
    Abstract = 1 << 7, // Class/Method/ConstructSignature
    Async = 1 << 8, // Property/Method/Function
    Default = 1 << 9, // Function/Class (export default declaration)
    Const = 1 << 11, // Variable declaration
    HasComputedFlags = 1 << 29, // Modifier flags have been computed

    AccessibilityModifier = Public | Private | Protected,

    // Accessibility modifiers and 'readonly' can be attached to a parameter in a constructor to make it a property.
    ParameterPropertyModifier = AccessibilityModifier | Readonly,
    NonPublicAccessibilityModifier = Private | Protected,

    TypeScriptModifier = Ambient | Public | Private | Protected | Readonly | Abstract | Const,
    ExportDefault = Export | Default
}