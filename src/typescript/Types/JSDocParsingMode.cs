namespace Serenity.TypeScript;

public enum JSDocParsingMode
{
    /// <summary>
    /// Always parse JSDoc comments and include them in the AST.
    /// This is the default if no mode is provided.
    /// </summary>
    ParseAll,

    /// <summary>
    /// Never parse JSDoc comments, mo matter the file type.
    /// </summary>
    ParseNone,

    /// <summary>
    /// Parse only JSDoc comments which are needed to provide correct type errors.
    /// 
    /// This will always parse JSDoc in non-TS files, but only parse JSDoc comments
    /// containing `@see` and `@link` in TS files.
    /// </summary>
    ParseForTypeErrors,

    /// <summary>
    /// Parse only JSDoc comments which are needed to provide correct type info.
    /// 
    /// This will always parse JSDoc in non-TS files, but never in TS files.
    /// 
    /// Note: Do not use this mode if you require accurate type errors; use {@link ParseForTypeErrors} instead.
    /// </summary>
    ParseForTypeInfo
}
