using System.Globalization;

namespace Serenity.Tests.Helpers;

public class StringHelperTests
{
    [Fact]
    public void IsEmptyOrNull_NullString_ReturnsTrue()
    {
        Assert.True(((string)null).IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_EmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_NonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, world!".IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_WhitespaceString_ReturnsFalse()
    {
        Assert.False("   ".IsEmptyOrNull());
    }

    [Fact]
    public void IsNullOrEmpty_NullString_ReturnsTrue()
    {
        Assert.True(((string)null).IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_EmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_NonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, world!".IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_WhitespaceString_ReturnsFalse()
    {
        Assert.False("   ".IsNullOrEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_NullString_ReturnsTrue()
    {
        Assert.True(((string)null).IsTrimmedEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_EmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.IsTrimmedEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_NonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, world!".IsTrimmedEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_WhitespaceString_ReturnsTrue()
    {
        Assert.True("   ".IsTrimmedEmpty());
    }

    [Fact]
    public void TrimToNull_NullString_ReturnsNull()
    {
        Assert.Null(((string)null).TrimToNull());
    }

    [Fact]
    public void TrimToNull_EmptyString_ReturnsNull()
    {
        Assert.Null(string.Empty.TrimToNull());
    }

    [Fact]
    public void TrimToNull_WhiteSpace_ReturnsNull()
    {
        Assert.Null("   ".TrimToNull());
    }

    [Fact]
    public void TrimToNull_NonEmptyString_ReturnsTrimmedString()
    {
        Assert.Equal("Hello, world!", "  Hello, world!  ".TrimToNull());
    }

    [Fact]
    public void TrimToEmpty_NullString_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ((string)null).TrimToEmpty());
    }

    [Fact]
    public void TrimToEmpty_EmptyString_ReturnsNull()
    {
        Assert.Equal(string.Empty, string.Empty.TrimToEmpty());
    }

    [Fact]
    public void TrimToEmpty_WhiteSpace_ReturnsNull()
    {
        Assert.Equal(string.Empty, "   ".TrimToEmpty());
    }

    [Fact]
    public void TrimToEmpty_NonEmptyString_ReturnsTrimmedString()
    {
        Assert.Equal("Hello, world!", "  Hello, world!  ".TrimToEmpty());
    }

    [Fact]
    public void IsTrimmedSame_NullStrings_ReturnsTrue()
    {
        Assert.True(((string)null).IsTrimmedSame(null));
    }

    [Fact]
    public void IsTrimmedSame_NullAndEmptyStrings_ReturnsTrue()
    {
        Assert.True(((string)null).IsTrimmedSame(string.Empty));
    }

    [Fact]
    public void IsTrimmedSame_EmptyStrings_ReturnsTrue()
    {
        Assert.True(string.Empty.IsTrimmedSame(string.Empty));
    }

    [Fact]
    public void IsTrimmedSame_WhiteSpaceStrings_ReturnsTrue()
    {
        string whiteSpaceString1 = "   ";
        string whiteSpaceString2 = "  \t  ";
        Assert.True(whiteSpaceString1.IsTrimmedSame(whiteSpaceString2));
    }

    [Fact]
    public void IsTrimmedSame_NonEmptyStrings_ReturnsTrue()
    {
        string nonEmptyString1 = "  Hello, world!  ";
        string nonEmptyString2 = "Hello, world!";
        Assert.True(nonEmptyString1.IsTrimmedSame(nonEmptyString2));
    }

    [Fact]
    public void IsTrimmedSame_DifferentStrings_ReturnsFalse()
    {
        string differentString1 = "Hello, world!";
        string differentString2 = "Goodbye, world!";
        Assert.False(differentString1.IsTrimmedSame(differentString2));
    }

    [Fact]
    public void ThreeDots_NullString_ReturnsEmptyString()
    {
        string nullString = null;
        Assert.Equal(string.Empty, nullString.ThreeDots(10));
    }

    [Fact]
    public void ThreeDots_EmptyString_ReturnsEmptyString()
    {
        string emptyString = string.Empty;
        Assert.Equal(string.Empty, emptyString.ThreeDots(10));
    }

    [Fact]
    public void ThreeDots_StringShorterThanMaxLength_ReturnsSameString()
    {
        string shortString = "Hello";
        Assert.Equal(shortString, shortString.ThreeDots(10));
    }

    [Fact]
    public void ThreeDots_StringEqualToMaxLength_ReturnsSameString()
    {
        string equalLengthString = "Hello, world";
        Assert.Equal(equalLengthString, equalLengthString.ThreeDots(12));
    }

    [Fact]
    public void ThreeDots_StringLongerThanMaxLength_ReturnsTrimmedAndThreeDottedString()
    {
        string longString = "Hello, world!";
        Assert.Equal("Hell...", longString.ThreeDots(7));
    }

    [Fact]
    public void ThreeDots_MaxLengthLessThan3_ReturnsThreeDots()
    {
        string longString = "Hello, world!";
        Assert.Equal("...", longString.ThreeDots(2));
    }

    [Fact]
    public void ToSingleLine_NullString_ReturnsEmptyString()
    {
        string nullString = null;
        Assert.Equal(string.Empty, nullString.ToSingleLine());
    }

    [Fact]
    public void ToSingleLine_EmptyString_ReturnsEmptyString()
    {
        string emptyString = string.Empty;
        Assert.Equal(string.Empty, emptyString.ToSingleLine());
    }

    [Fact]
    public void ToSingleLine_SingleLineString_ReturnsSameString()
    {
        string singleLineString = "Hello, world!";
        Assert.Equal(singleLineString, singleLineString.ToSingleLine());
    }

    [Fact]
    public void ToSingleLine_MultiLineString_ReturnsSingleLinedString()
    {
        string multiLineString = "Hello,\nworld!";
        Assert.Equal("Hello, world!", multiLineString.ToSingleLine());
    }

    [Fact]
    public void ToSingleLine_MultiLineStringWithCarriageReturn_ReturnsSingleLinedString()
    {
        string multiLineStringWithCarriageReturn = "Hello,\r\nworld!";
        Assert.Equal("Hello, world!", multiLineStringWithCarriageReturn.ToSingleLine());
    }

    [Fact]
    public void QuoteString_NullString_ReturnsEmptyDoubleQuote()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString(null, sb, doubleQuote: true);
        Assert.Equal("\"\"", sb.ToString());
    }

    [Fact]
    public void QuoteString_EmptyString_ReturnsEmptyDoubleQuote()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString(string.Empty, sb, doubleQuote: true);
        Assert.Equal("\"\"", sb.ToString());
    }

    [Fact]
    public void QuoteString_SingleQuote_NullString_ReturnsEmptySingleQuote()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString(null, sb, doubleQuote: false);
        Assert.Equal("''", sb.ToString());
    }

    [Fact]
    public void QuoteString_SingleQuote_EmptyString_ReturnsEmptySingleQuote()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString(string.Empty, sb, doubleQuote: false);
        Assert.Equal("''", sb.ToString());
    }

    [Fact]
    public void QuoteString_SingleQuote_ReturnsSingleQuotedString()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString("\"Hello, 'world'!\"", sb, doubleQuote: false);
        Assert.Equal("'\"Hello, \\'world\\'!\"'", sb.ToString());
    }

    [Fact]
    public void QuoteString_DoubleQuote_ReturnsDoubleQuotedString()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString("'Hello, \"world\"!'", sb, doubleQuote: true);
        Assert.Equal("\"'Hello, \\\"world\\\"!'\"", sb.ToString());
    }

    [Fact]
    public void QuoteString_EscapeCharacters_ReturnsEscapedCharacters()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString("\r\n\t", sb, doubleQuote: true);
        Assert.Equal("\"\\r\\n\\t\"", sb.ToString());
    }

    [Fact]
    public void QuoteString_Escapes_Backslash()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString("\\Test\\", sb, doubleQuote: true);
        Assert.Equal("\"\\\\Test\\\\\"", sb.ToString());
    }

    [Fact]
    public void QuoteString_Escapes_SpecialCharsBeforeSpace()
    {
        StringBuilder sb = new();
        StringHelper.QuoteString("\u0003", sb, doubleQuote: true);
        Assert.Equal("\"\\u0003\"", sb.ToString());
    }

    [Fact]
    public void ToSingleQuoted_NullString_ReturnsEmptyQuote()
    {
        Assert.Equal("''", StringHelper.ToSingleQuoted(null));
    }

    [Fact]
    public void ToSingleQuoted_EmptyString_ReturnsEmptyQuote()
    {
        Assert.Equal("''", StringHelper.ToSingleQuoted(string.Empty));
    }

    [Fact]
    public void ToSingleQuoted_SingleQuote_ReturnsQuotedString()
    {
        var str = "\"Hello, 'world'!\"";
        Assert.Equal("'\"Hello, \\'world\\'!\"'", StringHelper.ToSingleQuoted(str));
    }

    [Fact]
    public void ToSingleQuoted_EscapeCharacters_ReturnsEscapedCharacters()
    {
        var str = "\\\r\n\t\u0003";
        Assert.Equal("'\\\\\\r\\n\\t\\u0003'", StringHelper.ToSingleQuoted(str));
    }

    [Fact]
    public void ToDoubleQuoted_NullString_ReturnsEmptyQuote()
    {
        Assert.Equal("\"\"", StringHelper.ToDoubleQuoted(null));
    }

    [Fact]
    public void ToDoubleQuoted_EmptyString_ReturnsEmptyQuote()
    {
        Assert.Equal("\"\"", StringHelper.ToDoubleQuoted(string.Empty));
    }

    [Fact]
    public void ToDoubleQuoted_SingleQuote_ReturnsQuotedString()
    {
        var str = "'Hello, \"world\"!'";
        Assert.Equal("\"'Hello, \\\"world\\\"!'\"", StringHelper.ToDoubleQuoted(str));
    }

    [Fact]
    public void ToDoubleQuoted_EscapeCharacters_ReturnsEscapedCharacters()
    {
        var str = "\\\r\n\t\u0003";
        Assert.Equal("\"\\\\\\r\\n\\t\\u0003\"", StringHelper.ToDoubleQuoted(str));
    }

    [Fact]
    public void IsEmptyOrNull_NullCollection_ReturnsTrue()
    {
        System.Collections.ICollection nullCollection = null;
        Assert.True(nullCollection.IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_EmptyCollection_ReturnsTrue()
    {
        System.Collections.ICollection emptyCollection = new List<int>();
        Assert.True(emptyCollection.IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_NonEmptyCollection_ReturnsFalse()
    {
        System.Collections.ICollection nonEmptyCollection = new List<int>() { 1, 2, 3 };
        Assert.False(nonEmptyCollection.IsEmptyOrNull());
    }

    [Fact]
    public void SafeSubstring_NullString_ReturnsEmptyString()
    {
        string nullString = null;
        Assert.Equal(string.Empty, nullString.SafeSubstring(0, 5));
    }

    [Fact]
    public void SafeSubstring_EmptyString_ReturnsEmptyString()
    {
        string emptyString = string.Empty;
        Assert.Equal(string.Empty, emptyString.SafeSubstring(0, 5));
    }

    [Fact]
    public void SafeSubstring_StartIndexGreaterThanLength_ReturnsEmptyString()
    {
        string value = "Hello, world!";
        Assert.Equal(string.Empty, value.SafeSubstring(20, 5));
    }

    [Fact]
    public void SafeSubstring_MaxLengthLessThanOrEqualToZero_ReturnsEmptyString()
    {
        string value = "Hello, world!";
        Assert.Equal(string.Empty, value.SafeSubstring(0, -1));
    }

    [Fact]
    public void SafeSubstring_StartIndexPlusMaxLengthGreaterThanLength_ReturnsSubstringFromStartIndexToEnd()
    {
        string value = "Hello, world!";
        Assert.Equal("world!", value.SafeSubstring(7, 10));
    }

    [Fact]
    public void SafeSubstring_ValidParameters_ReturnsSubstring()
    {
        string value = "Hello, world!";
        Assert.Equal("world", value.SafeSubstring(7, 5));
    }

    [Fact]
    public void SanitizeFilename_NullString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelper.SanitizeFilename(null));
    }

    [Fact]
    public void SanitizeFilename_EmptyString_ReturnsEmptyString()
    {
        string emptyString = string.Empty;
        Assert.Equal(string.Empty, StringHelper.SanitizeFilename(emptyString));
    }

    [Fact]
    public void SanitizeFilename_ReturnsStringWithoutDiacriticsByDefault()
    {
        string diacriticString = "Héllo, wórld!";
        Assert.Equal("Hello, world!", StringHelper.SanitizeFilename(diacriticString));
    }

    [Fact]
    public void SanitizeFilename_ReplaceInvalidCharacters_ReturnsStringWithReplacedCharacters()
    {
        string invalidCharacterString = "Hello, world?*";
        Assert.Equal("Hello, world__", StringHelper.SanitizeFilename(invalidCharacterString));
    }

    [Fact]
    public void SanitizeFilename_RemoveDiacriticsAndReplaceInvalidCharacters_ReturnsSanitizedString()
    {
        string mixedString = " Héllo, wórld?*ıüğş ";
        Assert.Equal("Hello, world__iugs", StringHelper.SanitizeFilename(mixedString));
    }

    [Fact]
    public void SanitizeFilename_DoesNotRemoveDiacritics_IfRemoveDiacriticsIsFalse()
    {
        string mixedString = " Héllo, wórld?*ıüğş ";
        Assert.Equal("Héllo, wórld__ıüğş", StringHelper.SanitizeFilename(mixedString, removeDiacritics: false));
    }

    [Fact]
    public void SanitizeFilePath_NullString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelper.SanitizeFilePath(null));
    }

    [Fact]
    public void SanitizeFilePath_EmptyString_ReturnsEmptyString()
    {
        string emptyString = string.Empty;
        Assert.Equal(string.Empty, StringHelper.SanitizeFilePath(emptyString));
    }

    readonly char slash = System.IO.Path.DirectorySeparatorChar;

    [Fact]
    public void SanitizeFilePath_ReturnsStringWithoutDiacriticsByDefault()
    {
        string diacriticString = $"Héllo{slash}, wórld!";
        Assert.Equal($"Hello{slash}, world!", StringHelper.SanitizeFilePath(diacriticString));
    }

    [Fact]
    public void SanitizeFilePath_ReplaceInvalidCharacters_ReturnsStringWithReplacedCharacters()
    {
        string invalidCharacterString = $"Hello{slash}, world?*";
        Assert.Equal($"Hello{slash}, world__", StringHelper.SanitizeFilePath(invalidCharacterString));
    }

    [Fact]
    public void SanitizeFilePath_RemoveDiacriticsAndReplaceInvalidCharacters_ReturnsSanitizedString()
    {
        string mixedString = $" Héllo{slash}, wórld?*ıüğş ";
        Assert.Equal($"Hello{slash}, world__iugs", StringHelper.SanitizeFilePath(mixedString));
    }

    [Fact]
    public void SanitizeFilePath_DoesNotRemoveDiacritics_IfRemoveDiacriticsIsFalse()
    {
        string mixedString = $" Héllo{slash}, wórld?*ıüğş ";
        Assert.Equal($"Héllo{slash}, wórld__ıüğş", StringHelper.SanitizeFilePath(mixedString, removeDiacritics: false));
    }

    [Fact]
    public void RemoveDiacritics_NullString_ThrowsArgumentNull()
    {
        string nullString = null;
        Assert.Throws<ArgumentNullException>(() => StringHelper.RemoveDiacritics(nullString));
    }

    [Fact]
    public void RemoveDiacritics_EmptyString_ReturnsEmptyString()
    {
        string emptyString = string.Empty;
        Assert.Equal(string.Empty, StringHelper.RemoveDiacritics(emptyString));
    }

    [Fact]
    public void RemoveDiacritics_StringWithDiacritics_ReturnsStringWithoutDiacritics()
    {
        string diacriticString = "Héllo, wórld!";
        Assert.Equal("Hello, world!", StringHelper.RemoveDiacritics(diacriticString));
    }

    [Fact]
    public void RemoveDiacritics_StringWithDotlessI_ReturnsStringWithI()
    {
        string dotlessIString = "İstanbul ısparta";
        Assert.Equal("Istanbul isparta", StringHelper.RemoveDiacritics(dotlessIString));
    }

    [Fact]
    public void ToStringDefault_NullSource_ReturnsEmptyString()
    {
        int? nullInt = null;
        Assert.Equal(string.Empty, nullInt.ToStringDefault());
    }

    [Fact]
    public void ToStringDefault_EmptyFormat_ReturnsFormattedString()
    {
        int? number = 123;
        Assert.Equal("123", number.ToStringDefault(string.Empty));
    }

    [Fact]
    public void ToStringDefault_NullFormat_ReturnsFormattedString()
    {
        int? number = 123;
        Assert.Equal("123", number.ToStringDefault(null));
    }

    [Fact]
    public void ToStringDefault_CustomFormat_ReturnsFormattedString()
    {
        int? number = 123;
        Assert.Equal("123.00", number.ToStringDefault("F2"));
    }

    [Fact]
    public void ToStringDefault_CustomProvider_ReturnsFormattedString()
    {
        double? number = 123.456;
        CultureInfo cultureInfo = new("fr-FR");
        Assert.Equal("123,46", number.ToStringDefault("F2", cultureInfo));
    }

    [Fact]
    public void ToStringDefault_EmptyString_ReturnsEmptyString()
    {
        int? nullableInt = null;
        Assert.Equal(string.Empty, nullableInt.ToStringDefault(null, null, string.Empty));
    }

    [Fact]
    public void ToStringDefault_NullEmptyString_ReturnsEmptyString()
    {
        int? nullableInt = null;
        Assert.Equal(string.Empty, nullableInt.ToStringDefault(null, null, null));
    }

    class Obj<T>(T value) : IFormattable
        where T: struct, IFormattable
    {
        public T Value { get; set; } = value;

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Value.ToString(format, formatProvider);
        }
    }

    [Fact]
    public void ToStringDefault_WithClass_NullSource_ReturnsEmptyString()
    {
        Obj<int> nullInt = null;
        Assert.Equal(string.Empty, nullInt.ToStringDefault());
    }

    [Fact]
    public void ToStringDefault_WithClass_EmptyFormat_ReturnsFormattedString()
    {
        Obj<int> number = new(123);
        Assert.Equal("123", number.ToStringDefault(string.Empty));
    }

    [Fact]
    public void ToStringDefault_WithClass_NullFormat_ReturnsFormattedString()
    {
        Obj<int> number = new(123);
        Assert.Equal("123", number.ToStringDefault(null));
    }

    [Fact]
    public void ToStringDefault_WithClass_CustomFormat_ReturnsFormattedString()
    {
        Obj<int> number = new(123);
        Assert.Equal("123.00", number.ToStringDefault("F2"));
    }

    [Fact]
    public void ToStringDefault_WithClass_CustomProvider_ReturnsFormattedString()
    {
        Obj<double> number = new(123.456);
        CultureInfo cultureInfo = new("fr-FR");
        Assert.Equal("123,46", number.ToStringDefault("F2", cultureInfo));
    }

    [Fact]
    public void ToStringDefault_WithClass_EmptyString_ReturnsEmptyString()
    {
        Obj<int> nullableInt = null;
        Assert.Equal(string.Empty, nullableInt.ToStringDefault(null, null, string.Empty));
    }

    [Fact]
    public void ToStringDefault_WithClass_NullEmptyString_ReturnsEmptyString()
    {
        Obj<int> nullableInt = null;
        Assert.Equal(string.Empty, nullableInt.ToStringDefault(null, null, null));
    }

    [Fact]
    public void Join_NullStrings_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, StringHelper.Join(null, "-", null));
    }

    [Fact]
    public void Join_EmptyStrings_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, StringHelper.Join(string.Empty, "-", string.Empty));
    }

    [Fact]
    public void Join_FirstStringEmpty_ReturnsSecondString()
    {
        Assert.Equal("world", StringHelper.Join(string.Empty, "-", "world"));
    }

    [Fact]
    public void Join_SecondStringEmpty_ReturnsFirstString()
    {
        Assert.Equal("hello", StringHelper.Join("hello", "-", string.Empty));
    }

    [Fact]
    public void Join_NonEmptyStrings_ReturnsJoinedString()
    {
        Assert.Equal("hello-world", StringHelper.Join("hello", "-", "world"));
    }

    [Fact]
    public void JoinNonEmpty_NullValues_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, StringHelper.JoinNonEmpty("-", null, null));
    }

    [Fact]
    public void JoinNonEmpty_EmptyValues_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, StringHelper.JoinNonEmpty("-", string.Empty, string.Empty));
    }

    [Fact]
    public void JoinNonEmpty_NonEmptyValues_ReturnsJoinedString()
    {
        Assert.Equal("hello-world", StringHelper.JoinNonEmpty("-", "hello", "world"));
    }

    [Fact]
    public void JoinNonEmpty_Enumerable_NullValues_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, StringHelper.JoinNonEmpty("-", (IEnumerable<string>)[null, null]));
    }

    [Fact]
    public void JoinNonEmpty_Enumerable_EmptyValues_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, StringHelper.JoinNonEmpty("-", (IEnumerable<string>)[string.Empty, string.Empty]));
    }

    [Fact]
    public void JoinNonEmpty_Enumerable_NonEmptyValues_ReturnsJoinedString()
    {
        Assert.Equal("hello-world", StringHelper.JoinNonEmpty("-", (IEnumerable<string>)["hello", "world"]));
    }


}