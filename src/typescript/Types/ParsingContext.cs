namespace Serenity.TypeScript;

internal enum ParsingContext
{
    SourceElements,            // Elements in source file
    BlockStatements,           // Statements in block
    SwitchClauses,             // Clauses in switch statement
    SwitchClauseStatements,    // Statements in switch clause
    TypeMembers,               // Members in interface or type literal
    ClassMembers,              // Members in class declaration
    EnumMembers,               // Members in enum declaration
    HeritageClauseElement,     // Elements in a heritage clause
    VariableDeclarations,      // Variable declarations in variable statement
    ObjectBindingElements,     // Binding elements in object binding list
    ArrayBindingElements,      // Binding elements in array binding list
    ArgumentExpressions,       // Expressions in argument list
    ObjectLiteralMembers,      // Members in object literal
    JsxAttributes,             // Attributes in jsx element
    JsxChildren,               // Things between opening and closing JSX tags
    ArrayLiteralMembers,       // Members in array literal
    Parameters,                // Parameters in parameter list
    JSDocParameters,           // JSDoc parameters in parameter list of JSDoc function type
    RestProperties,            // Property names in a rest type list
    TypeParameters,            // Type parameters in type parameter list
    TypeArguments,             // Type arguments in type argument list
    TupleElementTypes,         // Element types in tuple element type list
    HeritageClauses,           // Heritage clauses for a class or interface declaration.
    ImportOrExportSpecifiers,  // Named import clause's import specifier list,
    ImportAttributes,          // Import attributes
    JSDocComment,              // Parsing via JSDocParser
    Count,                     // Number of parsing contexts
}