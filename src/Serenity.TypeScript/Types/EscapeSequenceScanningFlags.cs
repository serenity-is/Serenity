namespace Serenity.TypeScript;

public enum EscapeSequenceScanningFlags
{
    String = 1 << 0,
    ReportErrors = 1 << 1,

    RegularExpression = 1 << 2,
    AnnexB = 1 << 3,
    AnyUnicodeMode = 1 << 4,
    AtomEscape = 1 << 5,

    ReportInvalidEscapeErrors = RegularExpression | ReportErrors,
    AllowExtendedUnicodeEscape = String | AnyUnicodeMode
}