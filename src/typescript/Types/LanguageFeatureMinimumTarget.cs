namespace Serenity.TypeScript;

public enum LanguageFeatureMinimumTarget
{
    // ES2015 Features
    Classes = ScriptTarget.ES2015,
    ForOf = ScriptTarget.ES2015,
    Generators = ScriptTarget.ES2015,
    Iteration = ScriptTarget.ES2015,
    SpreadElements = ScriptTarget.ES2015,
    RestElements = ScriptTarget.ES2015,
    TaggedTemplates = ScriptTarget.ES2015,
    DestructuringAssignment = ScriptTarget.ES2015,
    BindingPatterns = ScriptTarget.ES2015,
    ArrowFunctions = ScriptTarget.ES2015,
    BlockScopedVariables = ScriptTarget.ES2015,
    ObjectAssign = ScriptTarget.ES2015,
    RegularExpressionFlagsUnicode = ScriptTarget.ES2015,
    RegularExpressionFlagsSticky = ScriptTarget.ES2015,

    // ES2016 Features
    Exponentiation = ScriptTarget.ES2016, // `x ** y`

    // ES2017 Features
    AsyncFunctions = ScriptTarget.ES2017, // `async function f() {}`

    // ES2018 Features
    ForAwaitOf = ScriptTarget.ES2018, // `for await (const x of y)`
    AsyncGenerators = ScriptTarget.ES2018, // `async function * f() { }`
    AsyncIteration = ScriptTarget.ES2018, // `Symbol.asyncIterator`
    ObjectSpreadRest = ScriptTarget.ES2018, // `{ ...obj }`
    RegularExpressionFlagsDotAll = ScriptTarget.ES2018,

    // ES2019 Features
    BindinglessCatch = ScriptTarget.ES2019, // `try { } catch { }`

    // ES2020 Features
    BigInt = ScriptTarget.ES2020, // `0n`
    NullishCoalesce = ScriptTarget.ES2020, // `a ?? b`
    OptionalChaining = ScriptTarget.ES2020, // `a?.b`

    // ES2021 Features
    LogicalAssignment = ScriptTarget.ES2021, // `a ||= b`, `a &&= b`, `a ??= b`

    // ES2022 Features
    TopLevelAwait = ScriptTarget.ES2022,
    ClassFields = ScriptTarget.ES2022,
    PrivateNamesAndClassStaticBlocks = ScriptTarget.ES2022, // `class C { static {} #x = y, #m() {} }`, `#x in y`
    RegularExpressionFlagsHasIndices = ScriptTarget.ES2022,

    // ES2023 Features
    ShebangComments = ScriptTarget.ESNext,

    // Upcoming Features
    // NOTE: We must reevaluate the target for upcoming features when each successive TC39 edition is ratified in
    //       June of each year. This includes changes to `LanguageFeatureMinimumTarget`, `ScriptTarget`,
    //       transformers/esnext.ts, commandLineParser.ts, and the contents of each lib/esnext.*.d.ts file.
    UsingAndAwaitUsing = ScriptTarget.ESNext, // `using x = y`, `await using x = y`
    ClassAndClassElementDecorators = ScriptTarget.ESNext, // `@dec class C {}`, `class C { @dec m() {} }`
    RegularExpressionFlagsUnicodeSets = ScriptTarget.ESNext,
}