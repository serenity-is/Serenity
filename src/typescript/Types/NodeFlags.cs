namespace Serenity.TypeScript;

// The upstream TypeScript compiler uses a 32-bit NodeFlags enum, but most of those
// flags are only needed by the binder/checker/incremental parser. This parser port
// only uses 12 bits, so the flags are compacted into bits 0..11 and the enum fits
// in a short (2 bytes). Bits 12..15 are currently unused.
[Flags]
public enum NodeFlags : short
{
    None = 0,

    NestedNamespace = 1 << 0,  // Namespace declaration
    Namespace = 1 << 1,  // Namespace declaration
    OptionalChain = 1 << 2,  // Chained MemberExpression rooted to a pseudo-OptionalExpression
    DisallowInContext = 1 << 3, // If node was parsed in a context where 'in-expressions' are not allowed
    YieldContext = 1 << 4, // If node was parsed in the 'yield' context created when parsing a generator
    DecoratorContext = 1 << 5, // If node was parsed as part of a decorator
    AwaitContext = 1 << 6, // If node was parsed in the 'await' context created when parsing an async function
    DisallowConditionalTypesContext = 1 << 7, // If node was parsed in a context where conditional types are not allowed
    JavaScriptFile = 1 << 8, // If node was parsed in a JavaScript
    // @internal
    PossiblyContainsImportMeta = 1 << 9,
    // @internal
    Ambient = 1 << 10, // If node was inside an ambient context -- a declaration file, or inside something with the `declare` modifier.
    // @internal
    InWithStatement = 1 << 11, // If any ancestor of node was the `statement` of a WithStatement (not the `expression`)

    // Exclude these flags when parsing a Type
    TypeExcludesFlags = YieldContext | AwaitContext,

    // ---------------------------------------------------------------------
    // The following upstream flags are not used by this parser port. They are
    // kept commented out instead of renumbered so the upstream layout stays
    // recognizable. Re-enable and repack only after checking all usages.
    // ---------------------------------------------------------------------

    // Let = 1 << 0,  // Variable declaration
    // Const = 1 << 1,  // Variable declaration
    // Using = 1 << 2,  // Variable declaration
    // AwaitUsing = Const | Using, // Variable declaration (NOTE: on a single node these flags would otherwise be mutually exclusive)
    // Synthesized = 1 << 4,  // Node was synthesized during transformation
    // ExportContext = 1 << 7,  // Export context (initialized by binding)
    // ContainsThis = 1 << 8,  // Interface contains references to "this"
    // HasImplicitReturn = 1 << 9,  // If function implicitly returns on one of codepaths (initialized by binding)
    // HasExplicitReturn = 1 << 10,  // If function has explicit reachable return on one of codepaths (initialized by binding)
    // GlobalAugmentation = 1 << 11,  // Set if module declaration is an augmentation for the global scope
    // HasAsyncFunctions = 1 << 12, // If the file has async functions (initialized by binding)
    // ThisNodeHasError = 1 << 18, // Incremental parsing only
    // ThisNodeOrAnySubNodesHasError = 1 << 20, // Incremental parsing only
    // HasAggregatedChildData = 1 << 21, // Incremental parsing only
    // PossiblyContainsDynamicImport = 1 << 22, // Only assigned in upstream; never checked by this port
    // JSDoc = 1 << 24, // If node was parsed inside jsdoc
    // JsonFile = 1 << 27, // If node was parsed in a Json
    // TypeCached = 1 << 28, // If a type was cached for node at any point
    // Deprecated = 1 << 29, // If has '@deprecated' JSDoc tag
    // Unreachable = 1 << 30, // If node is unreachable according to the binder

    // BlockScoped = Let | Const | Using,
    // Constant = Const | Using,
    // ReachabilityCheckFlags = HasImplicitReturn | HasExplicitReturn,
    // ReachabilityAndEmitFlags = ReachabilityCheckFlags | HasAsyncFunctions,
    // PermanentlySetIncrementalFlags = PossiblyContainsDynamicImport | PossiblyContainsImportMeta,

    // Parsing context flags (used by incremental parsing only)
    // ContextFlags = DisallowInContext | DisallowConditionalTypesContext | YieldContext | DecoratorContext | AwaitContext | JavaScriptFile | InWithStatement | Ambient,

    // The following flags repurpose other NodeFlags as different meanings for Identifier nodes
    // IdentifierHasExtendedUnicodeEscape = ContainsThis, // Indicates whether the identifier contains an extended unicode escape sequence
    // IdentifierIsInJSDocNamespace = HasAsyncFunctions, // Indicates whether the identifier is part of a JSDoc namespace
}
