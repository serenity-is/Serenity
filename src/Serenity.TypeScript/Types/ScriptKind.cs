namespace Serenity.TypeScript;

public enum ScriptKind
{
    Unknown = 0,
    JS = 1,
    JSX = 2,
    TS = 3,
    TSX = 4,
    External = 5,
    JSON = 6,
    // Used on extensions that doesn't define the ScriptKind but the content defines it.
    // Deferred extensions are going to be included in all project contexts.
    Deferred = 7
}