namespace Serenity.CodeGeneration;

/// <summary>
/// Determines what the generated root Texts.ts file contains. Nested local text classes and
/// module Db texts are always generated in their own files, so that importing one of them
/// does not pull the whole local text tree into the importing ESM chunk.
/// </summary>
public enum ServerTypingsTextsMode
{
    /// <summary>
    /// Texts.ts keeps re-exporting every nested local texts class under its old name, and
    /// contains the full tree for the <c>Texts</c> export if a <c>Texts</c> class is declared.
    /// Existing imports from Texts.ts keep working, but such imports get no splitting benefit.
    /// </summary>
    Compat = 0,

    /// <summary>
    /// Texts.ts only contains the <c>Texts</c> export with the keys that are not part of
    /// any other generated texts file. It is not generated at all if no <c>Texts</c> class
    /// is declared, as the remaining keys would not be reachable from any export.
    /// </summary>
    Remaining = 1
}
