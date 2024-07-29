namespace Serenity.TypeScript;

public enum RegularExpressionFlags
{
    None = 0,
    HasIndices = 1 << 0, // d
    Global = 1 << 1, // g
    IgnoreCase = 1 << 2, // i
    Multiline = 1 << 3, // m
    DotAll = 1 << 4, // s
    Unicode = 1 << 5, // u
    UnicodeSets = 1 << 6, // v
    Sticky = 1 << 7, // y
    AnyUnicodeMode = Unicode | UnicodeSets,
    Modifiers = IgnoreCase | Multiline | DotAll
}