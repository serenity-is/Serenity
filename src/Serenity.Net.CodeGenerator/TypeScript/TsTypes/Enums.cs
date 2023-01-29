namespace Serenity.TypeScript.TsTypes;

[Flags]
public enum NodeFlags
{
    None = 0,
    Let = 1 << 0, // Variable declaration
    Const = 1 << 1, // Variable declaration
    NestedNamespace = 1 << 2, // Namespace declaration
    Synthesized = 1 << 3, // Node was synthesized during transformation
    Namespace = 1 << 4, // Namespace declaration
    ExportContext = 1 << 5, // Export context (initialized by binding)
    ContainsThis = 1 << 6, // Interface contains references to "this"
    HasImplicitReturn = 1 << 7, // If function implicitly returns on one of codepaths (initialized by binding)

    HasExplicitReturn =
        1 << 8, // If function has explicit reachable return on one of codepaths (initialized by binding)
    GlobalAugmentation = 1 << 9, // Set if module declaration is an augmentation for the global scope
    HasAsyncFunctions = 1 << 10, // If the file has async functions (initialized by binding)
    DisallowInContext = 1 << 11, // If node was parsed in a context where 'in-expressions' are not allowed
    YieldContext = 1 << 12, // If node was parsed in the 'yield' context created when parsing a generator
    DecoratorContext = 1 << 13, // If node was parsed as part of a decorator
    AwaitContext = 1 << 14, // If node was parsed in the 'await' context created when parsing an async function
    ThisNodeHasError = 1 << 15, // If the parser encountered an error when parsing the code that created this node
    JavaScriptFile = 1 << 16, // If node was parsed in a JavaScript
    ThisNodeOrAnySubNodesHasError = 1 << 17, // If this node or any of its children had an error
    HasAggregatedChildData = 1 << 18, // If we've computed data from children and cached it in this node

    BlockScoped = Let | Const,

    ReachabilityCheckFlags = HasImplicitReturn | HasExplicitReturn,
    ReachabilityAndEmitFlags = ReachabilityCheckFlags | HasAsyncFunctions,

    // Parsing context flags
    ContextFlags = DisallowInContext | YieldContext | DecoratorContext | AwaitContext | JavaScriptFile,

    // Exclude these flags when parsing a Type
    TypeExcludesFlags = YieldContext | AwaitContext
}

public enum ParsingContext
{
    SourceElements, // Elements in source file
    BlockStatements, // Statements in block
    SwitchClauses, // Clauses in switch statement
    SwitchClauseStatements, // Statements in switch clause
    TypeMembers, // Members in interface or type literal
    ClassMembers, // Members in class declaration
    EnumMembers, // Members in enum declaration
    HeritageClauseElement, // Elements in a heritage clause
    VariableDeclarations, // Variable declarations in variable statement
    ObjectBindingElements, // Binding elements in object binding list
    ArrayBindingElements, // Binding elements in array binding list
    ArgumentExpressions, // Expressions in argument list
    ObjectLiteralMembers, // Members in object literal
    JsxAttributes, // Attributes in jsx element
    JsxChildren, // Things between opening and closing JSX tags
    ArrayLiteralMembers, // Members in array literal
    Parameters, // Parameters in parameter list
    RestProperties, // Property names in a rest type list
    TypeParameters, // Type parameters in type parameter list
    TypeArguments, // Type arguments in type argument list
    TupleElementTypes, // Element types in tuple element type list
    HeritageClauses, // Heritage clauses for a class or interface declaration.
    ImportOrExportSpecifiers, // Named import clause's import specifier list
    JSDocFunctionParameters,
    JSDocTypeArguments,
    JSDocRecordMembers,
    JSDocTupleTypes,
    Count // Number of parsing contexts
}

public enum Tristate
{
    False,
    True,
    Unknown
}

public enum JSDocState
{
    BeginningOfLine,
    SawAsterisk,
    SavingComments
}

public enum ModifierFlags
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

public enum JsxFlags
{
    None = 0,

    /** An element from a named property of the JSX.IntrinsicElements interface */
    IntrinsicNamedElement = 1 << 0,

    /** An element inferred from the string index signature of the JSX.IntrinsicElements interface */
    IntrinsicIndexedElement = 1 << 1,

    IntrinsicElement = IntrinsicNamedElement | IntrinsicIndexedElement
}

public enum RelationComparisonResult
{
    Succeeded = 1, // Should be truthy
    Failed = 2,
    FailedAndReported = 3
}

public enum GeneratedIdentifierKind
{
    None, // Not automatically generated.
    Auto, // Automatically generated identifier.
    Loop, // Automatically generated identifier with a preference for '_i'.
    Unique, // Unique name based on the 'text' property.
    Node // Unique name based on the node in the 'original' property.
}

public enum FlowFlags
{
    Unreachable = 1 << 0, // Unreachable code
    Start = 1 << 1, // Start of flow graph
    BranchLabel = 1 << 2, // Non-looping junction
    LoopLabel = 1 << 3, // Looping junction
    Assignment = 1 << 4, // Assignment
    TrueCondition = 1 << 5, // Condition known to be true
    FalseCondition = 1 << 6, // Condition known to be false
    SwitchClause = 1 << 7, // Switch statement clause
    ArrayMutation = 1 << 8, // Potential array mutation
    Referenced = 1 << 9, // Referenced as antecedent once
    Shared = 1 << 10, // Referenced as antecedent more than once
    PreFinally = 1 << 11, // Injected edge that links pre-finally label and pre-try flow
    AfterFinally = 1 << 12, // Injected edge that links post-finally flow with the rest of the graph
    Label = BranchLabel | LoopLabel,
    Condition = TrueCondition | FalseCondition
}

public enum ExitStatus
{
    // Compiler ran successfully.  Either this was a simple do-nothing compilation (for example,
    // when -version or -help was provided, or this was a normal compilation, no diagnostics
    // were produced, and all outputs were generated successfully.
    Success = 0,

    // Diagnostics were produced and because of them no code was generated.
    DiagnosticsPresentOutputsSkipped = 1,

    // Diagnostics were produced and outputs were generated in spite of them.
    DiagnosticsPresentOutputsGenerated = 2
}

public enum NodeBuilderFlags
{
    None = 0,
    AllowThisInObjectLiteral = 1 << 0,
    AllowQualifedNameInPlaceOfIdentifier = 1 << 1,
    AllowTypeParameterInQualifiedName = 1 << 2,
    AllowAnonymousIdentifier = 1 << 3,
    AllowEmptyUnionOrIntersection = 1 << 4,
    AllowEmptyTuple = 1 << 5
}

public enum TypeFormatFlags
{
    None = 0x00000000,
    WriteArrayAsGenericType = 0x00000001, // Write Array<T> instead T[]
    UseTypeOfFunction = 0x00000002, // Write typeof instead of function type literal
    NoTruncation = 0x00000004, // Don't truncate typeToString result
    WriteArrowStyleSignature = 0x00000008, // Write arrow style signature

    WriteOwnNameForAnyLike =
        0x00000010, // Write symbol's own name instead of 'any' for any like types (eg. unknown, __resolving__ etc)

    WriteTypeArgumentsOfSignature =
        0x00000020, // Write the type arguments instead of type parameters of the signature
    InElementType = 0x00000040, // Writing an array or union element type

    UseFullyQualifiedType =
        0x00000080, // Write out the fully qualified type name (eg. Module.Type, instead of Type)
    InFirstTypeArgument = 0x00000100, // Writing first type argument of the instantiated type
    InTypeAlias = 0x00000200, // Writing type in type alias declaration

    UseTypeAliasValue =
        0x00000400, // Serialize the type instead of using type-alias. This is needed when we emit declaration file.
    SuppressAnyReturnType = 0x00000800, // If the return type is any-like, don't offer a return type.
    AddUndefined = 0x00001000 // Add undefined to types of initialized, non-optional parameters
}

public enum SymbolFormatFlags
{
    None = 0x00000000,

    // Write symbols's type argument if it is instantiated symbol
    // eg. class C<T> { p: T }   <-- Show p as C<T>.p here
    //     var a: C<number>;
    //     var p = a.p;  <--- Here p is property of C<number> so show it as C<number>.p instead of just C.p
    WriteTypeParametersOrArguments = 0x00000001,

    // Use only external alias information to get the symbol name in the given context
    // eg.  module m { export class c { } } import x = m.c;
    // When this flag is specified m.c will be used to refer to the class instead of alias symbol x
    UseOnlyExternalAliasing = 0x00000002
}

public enum SymbolAccessibility
{
    Accessible,
    NotAccessible,
    CannotBeNamed
}

public enum SyntheticSymbolKind
{
    UnionOrIntersection,
    Spread
}

public enum TypePredicateKind
{
    This,
    Identifier
}

public enum TypeReferenceSerializationKind
{
    Unknown, // The TypeReferenceNode could not be resolved. The type name

    // should be emitted using a safe fallback.
    TypeWithConstructSignatureAndValue, // The TypeReferenceNode resolves to a type with a constructor

    // function that can be reached at runtime (e.g. a `class`
    // declaration or a `var` declaration for the static side
    // of a type, such as the global `Promise` type in lib.d.ts).
    VoidNullableOrNeverType, // The TypeReferenceNode resolves to a Void-like, Nullable, or Never type.
    NumberLikeType, // The TypeReferenceNode resolves to a Number-like type.
    StringLikeType, // The TypeReferenceNode resolves to a String-like type.
    BooleanType, // The TypeReferenceNode resolves to a Boolean-like type.
    ArrayLikeType, // The TypeReferenceNode resolves to an Array-like type.
    EsSymbolType, // The TypeReferenceNode resolves to the ESSymbol type.
    Promise, // The TypeReferenceNode resolved to the global Promise constructor symbol.
    TypeWithCallSignature, // The TypeReferenceNode resolves to a Function type or a type

    // with call signatures.
    ObjectType // The TypeReferenceNode resolves to any other type.
}

public enum SymbolFlags
{
    None = 0,
    FunctionScopedVariable = 1 << 0, // Variable (var) or parameter
    BlockScopedVariable = 1 << 1, // A block-scoped variable (let or const)
    Property = 1 << 2, // Property or enum member
    EnumMember = 1 << 3, // Enum member
    Function = 1 << 4, // Function
    Class = 1 << 5, // Class
    Interface = 1 << 6, // Interface
    ConstEnum = 1 << 7, // Const enum
    RegularEnum = 1 << 8, // Enum
    ValueModule = 1 << 9, // Instantiated module
    NamespaceModule = 1 << 10, // Uninstantiated module
    TypeLiteral = 1 << 11, // Type Literal or mapped type
    ObjectLiteral = 1 << 12, // Object Literal
    Method = 1 << 13, // Method
    Constructor = 1 << 14, // Constructor
    GetAccessor = 1 << 15, // Get accessor
    SetAccessor = 1 << 16, // Set accessor
    Signature = 1 << 17, // Call, construct, or index signature
    TypeParameter = 1 << 18, // Type parameter
    TypeAlias = 1 << 19, // Type alias
    ExportValue = 1 << 20, // Exported value marker (see comment in declareModuleMember in binder)
    ExportType = 1 << 21, // Exported type marker (see comment in declareModuleMember in binder)
    ExportNamespace = 1 << 22, // Exported namespace marker (see comment in declareModuleMember in binder)
    Alias = 1 << 23, // An alias for another symbol (see comment in isAliasSymbolDeclaration in checker)
    Prototype = 1 << 24, // Prototype property (no source representation)
    ExportStar = 1 << 25, // Export * declaration
    Optional = 1 << 26, // Optional property
    Transient = 1 << 27, // Transient symbol (created during type check)

    Enum = RegularEnum | ConstEnum,
    Variable = FunctionScopedVariable | BlockScopedVariable,

    Value = Variable | Property | EnumMember | Function | Class | Enum | ValueModule | Method | GetAccessor |
            SetAccessor,
    Type = Class | Interface | Enum | EnumMember | TypeLiteral | ObjectLiteral | TypeParameter | TypeAlias,
    Namespace = ValueModule | NamespaceModule | Enum,
    Module = ValueModule | NamespaceModule,
    Accessor = GetAccessor | SetAccessor,

    // Variables can be redeclared, but can not redeclare a block-scoped declaration with the
    // same name, or any other value that is not a variable, e.g. ValueModule or Class
    FunctionScopedVariableExcludes = Value & ~FunctionScopedVariable,

    // Block-scoped declarations are not allowed to be re-declared
    // they can not merge with anything in the value space
    BlockScopedVariableExcludes = Value,

    ParameterExcludes = Value,
    PropertyExcludes = None,
    EnumMemberExcludes = Value | Type,
    FunctionExcludes = Value & ~(Function | ValueModule),
    ClassExcludes = (Value | Type) & ~(ValueModule | Interface), // class-interface mergability done in checker.ts
    InterfaceExcludes = Type & ~(Interface | Class),

    RegularEnumExcludes =
        (Value | Type) & ~(RegularEnum | ValueModule), // regular enums merge only with regular enums and modules
    ConstEnumExcludes = (Value | Type) & ~ConstEnum, // const enums merge only with const enums
    ValueModuleExcludes = Value & ~(Function | Class | RegularEnum | ValueModule),
    NamespaceModuleExcludes = 0,
    MethodExcludes = Value & ~Method,
    GetAccessorExcludes = Value & ~SetAccessor,
    SetAccessorExcludes = Value & ~GetAccessor,
    TypeParameterExcludes = Type & ~TypeParameter,
    TypeAliasExcludes = Type,
    AliasExcludes = Alias,

    ModuleMember = Variable | Function | Class | Interface | Enum | Module | TypeAlias | Alias,

    ExportHasLocal = Function | Class | Enum | ValueModule,

    HasExports = Class | Enum | Module,
    HasMembers = Class | Interface | TypeLiteral | ObjectLiteral,

    BlockScoped = BlockScopedVariable | Class | Enum,

    PropertyOrAccessor = Property | Accessor,
    Export = ExportNamespace | ExportType | ExportValue,

    ClassMember = Method | Accessor | Property,

    /* @internal */
    // The set of things we consider semantically classifiable.  Used to speed up the LS during
    // classification.
    Classifiable = Class | Enum | TypeAlias | Interface | TypeParameter | Module
}

public enum CheckFlags
{
    Instantiated = 1 << 0, // Instantiated symbol
    SyntheticProperty = 1 << 1, // Property in union or intersection type
    SyntheticMethod = 1 << 2, // Method in union or intersection type
    Readonly = 1 << 3, // Readonly transient symbol
    Partial = 1 << 4, // Synthetic property present in some but not all constituents
    HasNonUniformType = 1 << 5, // Synthetic property with non-uniform type in constituents
    ContainsPublic = 1 << 6, // Synthetic property with public constituent(s)
    ContainsProtected = 1 << 7, // Synthetic property with protected constituent(s)
    ContainsPrivate = 1 << 8, // Synthetic property with private constituent(s)
    ContainsStatic = 1 << 9, // Synthetic property with static constituent(s)
    Synthetic = SyntheticProperty | SyntheticMethod
}

public enum NodeCheckFlags
{
    TypeChecked = 0x00000001, // Node has been type checked
    LexicalThis = 0x00000002, // Lexical 'this' reference
    CaptureThis = 0x00000004, // Lexical 'this' used in body
    CaptureNewTarget = 0x00000008, // Lexical 'new.target' used in body
    SuperInstance = 0x00000100, // Instance 'super' reference
    SuperStatic = 0x00000200, // Static 'super' reference
    ContextChecked = 0x00000400, // Contextual types have been assigned
    AsyncMethodWithSuper = 0x00000800, // An async method that reads a value from a member of 'super'.
    AsyncMethodWithSuperBinding = 0x00001000, // An async method that assigns a value to a member of 'super'.
    CaptureArguments = 0x00002000, // Lexical 'arguments' used in body (for async functions)

    EnumValuesComputed =
        0x00004000, // Values for enum members have been computed, and any errors have been reported for them.

    LexicalModuleMergesWithClass =
        0x00008000, // Instantiated lexical module declaration is merged with a previous class declaration.
    LoopWithCapturedBlockScopedBinding = 0x00010000, // Loop that contains block scoped variable captured in closure
    CapturedBlockScopedBinding = 0x00020000, // Block-scoped binding that is captured in some function

    BlockScopedBindingInLoop =
        0x00040000, // Block-scoped binding with declaration nested inside iteration statement

    ClassWithBodyScopedClassBinding =
        0x00080000, // Decorated class that contains a binding to itself inside of the class body.
    BodyScopedClassBinding = 0x00100000, // Binding to a decorated class inside of the class's body.

    NeedsLoopOutParameter =
        0x00200000, // Block scoped binding whose value should be explicitly copied outside of the converted loop
    AssignmentsMarked = 0x00400000, // Parameter assignments have been marked

    ClassWithConstructorReference =
        0x00800000, // Class that contains a binding to its constructor inside of the class body.
    ConstructorReferenceInClass = 0x01000000 // Binding to a class constructor inside of the class's body.
}

public enum TypeFlags
{
    Any = 1 << 0,
    String = 1 << 1,
    Number = 1 << 2,
    Boolean = 1 << 3,
    Enum = 1 << 4,
    StringLiteral = 1 << 5,
    NumberLiteral = 1 << 6,
    BooleanLiteral = 1 << 7,
    EnumLiteral = 1 << 8,
    EsSymbol = 1 << 9, // Type of symbol primitive introduced in ES6
    Void = 1 << 10,
    Undefined = 1 << 11,
    Null = 1 << 12,
    Never = 1 << 13, // Never type
    TypeParameter = 1 << 14, // Type parameter
    Object = 1 << 15, // Object type
    Union = 1 << 16, // Union (T | U)
    Intersection = 1 << 17, // Intersection (T & U)
    Index = 1 << 18, // keyof T
    IndexedAccess = 1 << 19, // T[K]

    /* @internal */
    FreshLiteral = 1 << 20, // Fresh literal type

    /* @internal */
    ContainsWideningType = 1 << 21, // Type is or contains undefined or null widening type

    /* @internal */
    ContainsObjectLiteral = 1 << 22, // Type is or contains object literal type

    /* @internal */
    ContainsAnyFunctionType = 1 << 23, // Type is or contains object literal type
    NonPrimitive = 1 << 24, // intrinsic object type

    /* @internal */
    JsxAttributes = 1 << 25, // Jsx attributes type

    /* @internal */
    Nullable = Undefined | Null,
    Literal = StringLiteral | NumberLiteral | BooleanLiteral | EnumLiteral,
    StringOrNumberLiteral = StringLiteral | NumberLiteral,

    /* @internal */
    DefinitelyFalsy = StringLiteral | NumberLiteral | BooleanLiteral | Void | Undefined | Null,
    PossiblyFalsy = DefinitelyFalsy | String | Number | Boolean,

    /* @internal */
    Intrinsic = Any | String | Number | Boolean | BooleanLiteral | EsSymbol | Void | Undefined | Null | Never |
                NonPrimitive,

    /* @internal */
    Primitive = String | Number | Boolean | Enum | EsSymbol | Void | Undefined | Null | Literal,
    StringLike = String | StringLiteral | Index,
    NumberLike = Number | NumberLiteral | Enum | EnumLiteral,
    BooleanLike = Boolean | BooleanLiteral,
    EnumLike = Enum | EnumLiteral,
    UnionOrIntersection = Union | Intersection,
    StructuredType = Object | Union | Intersection,
    StructuredOrTypeVariable = StructuredType | TypeParameter | Index | IndexedAccess,
    TypeVariable = TypeParameter | IndexedAccess,

    // 'Narrowable' types are types where narrowing actually narrows.
    // This *should* be every type other than null, undefined, void, and never
    Narrowable = Any | StructuredType | TypeParameter | Index | IndexedAccess | StringLike | NumberLike |
                 BooleanLike | EsSymbol | NonPrimitive,
    NotUnionOrUnit = Any | EsSymbol | Object | NonPrimitive,

    /* @internal */
    RequiresWidening = ContainsWideningType | ContainsObjectLiteral,

    /* @internal */
    PropagatingFlags = ContainsWideningType | ContainsObjectLiteral | ContainsAnyFunctionType
}

public enum ObjectFlags
{
    Class = 1 << 0, // Class
    Interface = 1 << 1, // Interface
    Reference = 1 << 2, // Generic type reference
    Tuple = 1 << 3, // Synthesized generic tuple type
    Anonymous = 1 << 4, // Anonymous
    Mapped = 1 << 5, // Mapped
    Instantiated = 1 << 6, // Instantiated anonymous or mapped type
    ObjectLiteral = 1 << 7, // Originates in an object literal
    EvolvingArray = 1 << 8, // Evolving array type
    ObjectLiteralPatternWithComputedProperties = 1 << 9, // Object literal pattern with computed properties
    ClassOrInterface = Class | Interface
}

public enum SignatureKind
{
    Call,
    Construct
}

public enum IndexKind
{
    String,
    Number
}

public enum SpecialPropertyAssignmentKind
{
    None,

    /// exports.name = expr
    ExportsProperty,

    /// module.exports = expr
    ModuleExports,

    /// className.prototype.name = expr
    PrototypeProperty,

    /// this.name = expr
    ThisProperty,

    // F.name = expr
    Property
}

public enum DiagnosticCategory
{
    Warning,
    Error,
    Message,
    Unknown
}

public enum ModuleResolutionKind
{
    Classic = 1,
    NodeJs = 2
}

public enum ModuleKind
{
    None = 0,
    CommonJs = 1,
    Amd = 2,
    Umd = 3,
    System = 4,
    Es2015 = 5
}

public enum JsxEmit
{
    None = 0,
    Preserve = 1,
    React = 2,
    ReactNative = 3
}

public enum NewLineKind
{
    CarriageReturnLineFeed = 0,
    LineFeed = 1
}

public enum ScriptKind
{
    Unknown = 0,
    Js = 1,
    Jsx = 2,
    Ts = 3,
    Tsx = 4,
    External = 5
}

public enum LanguageVariant
{
    Standard,
    Jsx
}

public enum DiagnosticStyle
{
    Simple,
    Pretty
}

public enum WatchDirectoryFlags
{
    None = 0,
    Recursive = 1 << 0
}

public enum CharacterCodes
{
    MaxAsciiCharacter = 0x7F,
    LineSeparator = 0x2028,
    ParagraphSeparator = 0x2029,
    NextLine = 0x0085,
    NonBreakingSpace = 0x00A0,
    EnQuad = 0x2000,
    EmQuad = 0x2001,
    EnSpace = 0x2002,
    EmSpace = 0x2003,
    ThreePerEmSpace = 0x2004,
    FourPerEmSpace = 0x2005,
    SixPerEmSpace = 0x2006,
    FigureSpace = 0x2007,
    PunctuationSpace = 0x2008,
    ThinSpace = 0x2009,
    HairSpace = 0x200A,
    ZeroWidthSpace = 0x200B,
    NarrowNoBreakSpace = 0x202F,
    IdeographicSpace = 0x3000,
    MathematicalSpace = 0x205F,
    Ogham = 0x1680,
    ByteOrderMark = 0xFEFF,
}

public enum Extension
{
    Ts,
    Tsx,
    Dts,
    Js,
    Jsx,
    LastTypeScriptExtension = Dts
}

public enum TransformFlags
{
    None = 0,

    // Facts
    // - Flags used to indicate that a node or subtree contains syntax that requires transformation.
    TypeScript = 1 << 0,
    ContainsTypeScript = 1 << 1,
    ContainsJsx = 1 << 2,
    ContainsEsNext = 1 << 3,
    ContainsEs2017 = 1 << 4,
    ContainsEs2016 = 1 << 5,
    Es2015 = 1 << 6,
    ContainsEs2015 = 1 << 7,
    Generator = 1 << 8,
    ContainsGenerator = 1 << 9,
    DestructuringAssignment = 1 << 10,
    ContainsDestructuringAssignment = 1 << 11,

    // Markers
    // - Flags used to indicate that a subtree contains a specific transformation.
    ContainsDecorators = 1 << 12,
    ContainsPropertyInitializer = 1 << 13,
    ContainsLexicalThis = 1 << 14,
    ContainsCapturedLexicalThis = 1 << 15,
    ContainsLexicalThisInComputedPropertyName = 1 << 16,
    ContainsDefaultValueAssignments = 1 << 17,
    ContainsParameterPropertyAssignments = 1 << 18,
    ContainsSpread = 1 << 19,
    ContainsObjectSpread = 1 << 20,
    ContainsRest = ContainsSpread,
    ContainsObjectRest = ContainsObjectSpread,
    ContainsComputedPropertyName = 1 << 21,
    ContainsBlockScopedBinding = 1 << 22,
    ContainsBindingPattern = 1 << 23,
    ContainsYield = 1 << 24,
    ContainsHoistedDeclarationOrCompletion = 1 << 25,

    HasComputedFlags = 1 << 29, // Transform flags have been computed.

    // Assertions
    // - Bitmasks that are used to assert facts about the syntax of a node and its subtree.
    AssertTypeScript = TypeScript | ContainsTypeScript,
    AssertJsx = ContainsJsx,
    AssertEsNext = ContainsEsNext,
    AssertEs2017 = ContainsEs2017,
    AssertEs2016 = ContainsEs2016,
    AssertEs2015 = Es2015 | ContainsEs2015,
    AssertGenerator = Generator | ContainsGenerator,
    AssertDestructuringAssignment = DestructuringAssignment | ContainsDestructuringAssignment,

    // Scope Exclusions
    // - Bitmasks that exclude flags from propagating out of a specific context
    //   into the subtree flags of their container.
    NodeExcludes = TypeScript | Es2015 | DestructuringAssignment | Generator | HasComputedFlags,

    ArrowFunctionExcludes = NodeExcludes | ContainsDecorators | ContainsDefaultValueAssignments |
                            ContainsLexicalThis | ContainsParameterPropertyAssignments |
                            ContainsBlockScopedBinding | ContainsYield | ContainsHoistedDeclarationOrCompletion |
                            ContainsBindingPattern | ContainsObjectRest,

    FunctionExcludes = NodeExcludes | ContainsDecorators | ContainsDefaultValueAssignments |
                       ContainsCapturedLexicalThis | ContainsLexicalThis | ContainsParameterPropertyAssignments |
                       ContainsBlockScopedBinding | ContainsYield | ContainsHoistedDeclarationOrCompletion |
                       ContainsBindingPattern | ContainsObjectRest,

    ConstructorExcludes = NodeExcludes | ContainsDefaultValueAssignments | ContainsLexicalThis |
                          ContainsCapturedLexicalThis | ContainsBlockScopedBinding | ContainsYield |
                          ContainsHoistedDeclarationOrCompletion | ContainsBindingPattern | ContainsObjectRest,

    MethodOrAccessorExcludes = NodeExcludes | ContainsDefaultValueAssignments | ContainsLexicalThis |
                               ContainsCapturedLexicalThis | ContainsBlockScopedBinding | ContainsYield |
                               ContainsHoistedDeclarationOrCompletion | ContainsBindingPattern | ContainsObjectRest,

    ClassExcludes = NodeExcludes | ContainsDecorators | ContainsPropertyInitializer | ContainsLexicalThis |
                    ContainsCapturedLexicalThis | ContainsComputedPropertyName |
                    ContainsParameterPropertyAssignments | ContainsLexicalThisInComputedPropertyName,

    ModuleExcludes = NodeExcludes | ContainsDecorators | ContainsLexicalThis | ContainsCapturedLexicalThis |
                     ContainsBlockScopedBinding | ContainsHoistedDeclarationOrCompletion,
    TypeExcludes = ~ContainsTypeScript,

    ObjectLiteralExcludes = NodeExcludes | ContainsDecorators | ContainsComputedPropertyName |
                            ContainsLexicalThisInComputedPropertyName | ContainsObjectSpread,
    ArrayLiteralOrCallOrNewExcludes = NodeExcludes | ContainsSpread,
    VariableDeclarationListExcludes = NodeExcludes | ContainsBindingPattern | ContainsObjectRest,
    ParameterExcludes = NodeExcludes,
    CatchClauseExcludes = NodeExcludes | ContainsObjectRest,
    BindingPatternExcludes = NodeExcludes | ContainsRest,

    // Masks
    // - Additional bitmasks
    TypeScriptClassSyntaxMask = ContainsParameterPropertyAssignments | ContainsPropertyInitializer |
                                ContainsDecorators,
    Es2015FunctionSyntaxMask = ContainsCapturedLexicalThis | ContainsDefaultValueAssignments
}

public enum EmitFlags
{
    SingleLine = 1 << 0, // The contents of this node should be emitted on a single line.
    AdviseOnEmitNode = 1 << 1, // The printer should invoke the onEmitNode callback when printing this node.
    NoSubstitution = 1 << 2, // Disables further substitution of an expression.
    CapturesThis = 1 << 3, // The function captures a lexical `this`
    NoLeadingSourceMap = 1 << 4, // Do not emit a leading source map location for this node.
    NoTrailingSourceMap = 1 << 5, // Do not emit a trailing source map location for this node.
    NoSourceMap = NoLeadingSourceMap | NoTrailingSourceMap, // Do not emit a source map location for this node.
    NoNestedSourceMaps = 1 << 6, // Do not emit source map locations for children of this node.
    NoTokenLeadingSourceMaps = 1 << 7, // Do not emit leading source map location for token nodes.
    NoTokenTrailingSourceMaps = 1 << 8, // Do not emit trailing source map location for token nodes.

    NoTokenSourceMaps =
        NoTokenLeadingSourceMaps |
        NoTokenTrailingSourceMaps, // Do not emit source map locations for tokens of this node.
    NoLeadingComments = 1 << 9, // Do not emit leading comments for this node.
    NoTrailingComments = 1 << 10, // Do not emit trailing comments for this node.
    NoComments = NoLeadingComments | NoTrailingComments, // Do not emit comments for this node.
    NoNestedComments = 1 << 11,
    HelperName = 1 << 12,

    ExportName =
        1 <<
        13, // Ensure an export prefix is added for an identifier that points to an exported declaration with a local name (see SymbolFlags.ExportHasLocal).

    LocalName =
        1 << 14, // Ensure an export prefix is not added for an identifier that points to an exported declaration.

    Indented =
        1 <<
        15, // Adds an explicit extra indentation level for class and function bodies when printing (used to match old emitter).
    NoIndentation = 1 << 16, // Do not indent the node.
    AsyncFunctionBody = 1 << 17,
    ReuseTempVariableScope = 1 << 18, // Reuse the existing temp variable scope during emit.

    CustomPrologue =
        1 <<
        19, // Treat the statement as if it were a prologue directive (NOTE: Prologue directives are *not* transformed).
    NoHoisting = 1 << 20, // Do not hoist this declaration in --module system

    HasEndOfDeclarationMarker =
        1 << 21 // Declaration has an associated NotEmittedStatement to mark the end of the declaration
}

public enum ExternalEmitHelpers
{
    Extends = 1 << 0, // __extends (used by the ES2015 class transformation)
    Assign = 1 << 1, // __assign (used by Jsx and ESNext object spread transformations)
    Rest = 1 << 2, // __rest (used by ESNext object rest transformation)
    Decorate = 1 << 3, // __decorate (used by TypeScript decorators transformation)
    Metadata = 1 << 4, // __metadata (used by TypeScript decorators transformation)
    Param = 1 << 5, // __param (used by TypeScript decorators transformation)
    Awaiter = 1 << 6, // __awaiter (used by ES2017 async functions transformation)
    Generator = 1 << 7, // __generator (used by ES2015 generator transformation)
    Values = 1 << 8, // __values (used by ES2015 for..of and yield* transformations)
    Read = 1 << 9, // __read (used by ES2015 iterator destructuring transformation)
    Spread = 1 << 10, // __spread (used by ES2015 array spread and argument list spread transformations)
    AsyncGenerator = 1 << 11, // __asyncGenerator (used by ES2017 async generator transformation)
    AsyncDelegator = 1 << 12, // __asyncDelegator (used by ES2017 async generator yield* transformation)
    AsyncValues = 1 << 13, // __asyncValues (used by ES2017 for..await..of transformation)

    // Helpers included by ES2015 for..of
    ForOfIncludes = Values,

    // Helpers included by ES2017 for..await..of
    ForAwaitOfIncludes = AsyncValues,

    // Helpers included by ES2015 spread
    SpreadIncludes = Read | Spread,

    FirstEmitHelper = Extends,
    LastEmitHelper = AsyncValues
}

public enum EmitHint
{
    SourceFile, // Emitting a SourceFile
    Expression, // Emitting an Expression
    IdentifierName, // Emitting an IdentifierName
    Unspecified // Emitting an otherwise unspecified node
}