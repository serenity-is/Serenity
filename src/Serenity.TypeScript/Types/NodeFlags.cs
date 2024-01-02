namespace Serenity.TypeScript;

[Flags]
public enum NodeFlags
{
    None = 0,
    Let = 1 << 0,  // Variable declaration
    Const = 1 << 1,  // Variable declaration
    Using = 1 << 2,  // Variable declaration
    AwaitUsing = Const | Using, // Variable declaration (NOTE: on a single node these flags would otherwise be mutually exclusive)
    NestedNamespace = 1 << 3,  // Namespace declaration
    Synthesized = 1 << 4,  // Node was synthesized during transformation
    Namespace = 1 << 5,  // Namespace declaration
    OptionalChain = 1 << 6,  // Chained MemberExpression rooted to a pseudo-OptionalExpression
    ExportContext = 1 << 7,  // Export context (initialized by binding)
    ContainsThis = 1 << 8,  // Interface contains references to "this"
    HasImplicitReturn = 1 << 9,  // If function implicitly returns on one of codepaths (initialized by binding)
    HasExplicitReturn = 1 << 10,  // If function has explicit reachable return on one of codepaths (initialized by binding)
    GlobalAugmentation = 1 << 11,  // Set if module declaration is an augmentation for the global scope
    HasAsyncFunctions = 1 << 12, // If the file has async functions (initialized by binding)
    DisallowInContext = 1 << 13, // If node was parsed in a context where 'in-expressions' are not allowed
    YieldContext = 1 << 14, // If node was parsed in the 'yield' context created when parsing a generator
    DecoratorContext = 1 << 15, // If node was parsed as part of a decorator
    AwaitContext = 1 << 16, // If node was parsed in the 'await' context created when parsing an async function
    DisallowConditionalTypesContext = 1 << 17, // If node was parsed in a context where conditional types are not allowed
    ThisNodeHasError = 1 << 18, // If the parser encountered an error when parsing the code that created this node
    JavaScriptFile = 1 << 19, // If node was parsed in a JavaScript
    ThisNodeOrAnySubNodesHasError = 1 << 20, // If this node or any of its children had an error
    HasAggregatedChildData = 1 << 21, // If we've computed data from children and cached it in this node

    // These flags will be set when the parser encounters a dynamic import expression or 'import.meta' to avoid
    // walking the tree if the flags are not set. However, these flags are just a approximation
    // (hence why it's named "PossiblyContainsDynamicImport") because once set, the flags never get cleared.
    // During editing, if a dynamic import is removed, incremental parsing will *NOT* clear this flag.
    // This means that the tree will always be traversed during module resolution, or when looking for external module indicators.
    // However, the removal operation should not occur often and in the case of the
    // removal, it is likely that users will add the import anyway.
    // The advantage of this approach is its simplicity. For the case of batch compilation,
    // we guarantee that users won't have to pay the price of walking the tree if a dynamic import isn't used.
    // @internal
    PossiblyContainsDynamicImport = 1 << 22,
    // @internal
    PossiblyContainsImportMeta = 1 << 23,

    JSDoc = 1 << 24, // If node was parsed inside jsdoc
    // @internal
    Ambient = 1 << 25, // If node was inside an ambient context -- a declaration file, or inside something with the `declare` modifier.
    // @internal
    InWithStatement = 1 << 26, // If any ancestor of node was the `statement` of a WithStatement (not the `expression`)
    JsonFile = 1 << 27, // If node was parsed in a Json
    // @internal
    TypeCached = 1 << 28, // If a type was cached for node at any point
    // @internal
    Deprecated = 1 << 29, // If has '@deprecated' JSDoc tag

    BlockScoped = Let | Const | Using,
    Constant = Const | Using,

    ReachabilityCheckFlags = HasImplicitReturn | HasExplicitReturn,
    ReachabilityAndEmitFlags = ReachabilityCheckFlags | HasAsyncFunctions,

    // Parsing context flags
    ContextFlags = DisallowInContext | DisallowConditionalTypesContext | YieldContext | DecoratorContext | AwaitContext | JavaScriptFile | InWithStatement | Ambient,

    // Exclude these flags when parsing a Type
    TypeExcludesFlags = YieldContext | AwaitContext,

    // Represents all flags that are potentially set once and
    // never cleared on SourceFiles which get re-used in between incremental parses.
    // See the comment above on `PossiblyContainsDynamicImport` and `PossiblyContainsImportMeta`.
    // @internal
    PermanentlySetIncrementalFlags = PossiblyContainsDynamicImport | PossiblyContainsImportMeta,

    // The following flags repurpose other NodeFlags as different meanings for Identifier nodes
    // @internal
    IdentifierHasExtendedUnicodeEscape = ContainsThis, // Indicates whether the identifier contains an extended unicode escape sequence
    // @internal
    IdentifierIsInJSDocNamespace = HasAsyncFunctions, // Indicates whether the identifier is part of a JSDoc namespace
}