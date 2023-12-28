namespace Serenity.TypeScript;



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


public enum Extension
{
    Ts,
    Tsx,
    Dts,
    Js,
    Jsx,
    LastTypeScriptExtension = Dts
}

