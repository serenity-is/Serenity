using Serenity.TypeScript.TsTypes;
#if ISSOURCEGENERATOR
#else
using CharSpan = System.ReadOnlySpan<char>;
#endif
using static Serenity.TypeScript.TsParser.Core;
using Debug = System.Diagnostics.Debug;

namespace Serenity.TypeScript.TsParser;

public delegate void ErrorCallback(DiagnosticMessage message, int? length);

public partial class Scanner
{
    private string _text;
    // Current position (end position of text of current token)
    private int _pos;

    // end of text
    private int _end;

    // Start position of whitespace before current token
    private int _startPos;

    // Start position of text of current token
    private int _tokenPos;

    private SyntaxKind _token;
    private string _tokenValue;
    private bool _precedingLineBreak;
    private bool _tokenIsUnterminated;
    public event ErrorCallback OnError;
    private LanguageVariant _languageVariant;

    public int StartPos => _startPos;
    public int TextPos
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            _pos = value;
            _startPos = value;
            _tokenPos = value;
            _token = SyntaxKind.Unknown;
            _precedingLineBreak = false;
            _tokenValue = null;
            _tokenIsUnterminated = false;
        }
    }

    public int TokenPos => _tokenPos;
    public string TokenText => _text[_tokenPos.._pos];
    public string TokenValue => _tokenValue;
    public bool HasPrecedingLineBreak => _precedingLineBreak;
    public bool IsUnterminated => _tokenIsUnterminated;

    public Scanner(LanguageVariant languageVariant, string text, int start = 0, int? length = null)
    {
        _languageVariant = languageVariant;
        _startPos = start;
        SetText(text, start, length);
    }


    public static bool IsIdentifierStart(int ch)
    {
        return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || ch == '$' || ch == '_' ||
            (ch > CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierStart(ch));
    }

    public static bool IsIdentifierPart(int ch)
    {
        return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') ||
            (ch >= '0' && ch <= '9') || ch == '$' || ch == '_' ||
            (ch > CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierPart(ch));
    }

    public void Error(DiagnosticMessage message, int length = 0)
    {
        OnError?.Invoke(message, length);
    }

    public string ScanNumber()
    {
        var start = _pos;
        while (IsDigit(_text[_pos]))
        {
            _pos++;
        }
        if (_text[_pos] == '.')
        {
            _pos++;
            while (IsDigit(_text[_pos]))
            {
                _pos++;
            }
        }
        var end = _pos;
        if (_text[_pos] == 'E' || _text[_pos] == 'e')
        {
            _pos++;
            if (_text[_pos] == '+' || _text[_pos] == '-')
            {
                _pos++;
            }
            if (IsDigit(_text[_pos]))
            {
                _pos++;
                while (IsDigit(_text[_pos]))
                {
                    _pos++;
                }
                end = _pos;
            }
            else
            {
                Error(DiagnosticMessage.Digit_expected);
            }
        }
        return _text[start..end];
    }

    public int ScanOctalDigits()
    {
        var start = _pos;
        while (IsOctalDigit(_text[_pos]))
        {
            _pos++;
        }
#if ISSOURCEGENERATOR
        return int.Parse(_text[start.._pos]);
#else
        return int.Parse(_text.AsSpan(start, _pos - start));
#endif
    }

    public int ScanExactNumberOfHexDigits(int count)
    {
        return ScanHexDigits(/*minCount*/ count, /*scanAsManyAsPossible*/ false);
    }

    public int ScanMinimumNumberOfHexDigits(int count)
    {
        return ScanHexDigits(/*minCount*/ count, /*scanAsManyAsPossible*/ true);
    }

    public int ScanHexDigits(int minCount, bool scanAsManyAsPossible)
    {
        var digits = 0;
        var value = 0;
        while (digits < minCount || scanAsManyAsPossible)
        {
            var ch = _text[_pos];
            if (ch >= '0' && ch <= '9')
            {
                value = value * 16 + ch - '0';
            }
            else
        if (ch >= 'A' && ch <= 'f')
            {
                value = value * 16 + ch - 'A' + 10;
            }
            else
        if (ch >= 'a' && ch <= 'f')
            {
                value = value * 16 + ch - 'a' + 10;
            }
            else
            {
                break;
            }
            _pos++;
            digits++;
        }
        if (digits < minCount)
        {
            value = -1;
        }
        return value;
    }

    public string ScanString(bool allowEscapes = true)
    {
        var quote = _text[_pos];
        _pos++;
        StringBuilder result = null;
        var start = _pos;
        while (true)
        {
            if (_pos >= _end)
            {
                _tokenIsUnterminated = true;
                Error(DiagnosticMessage.Unterminated_string_literal);
                if (result is null)
                    return _text[start.._pos];

                result.Append(_text, start, _pos - start);
                break;
            }
            var ch = _text[_pos];
            if (ch == quote)
            {
                if (result is null)
                    return _text[start.._pos++];

                result.Append(_text, start, _pos - start);
                _pos++;
                break;
            }
            if (ch == '\\' && allowEscapes)
            {
                result ??= new();
                result.Append(_text, start, _pos - start);
                ScanEscapeSequence(result);
                start = _pos;
                continue;
            }
            if (IsLineBreak(ch))
            {
                _tokenIsUnterminated = true;
                Error(DiagnosticMessage.Unterminated_string_literal);
                if (result is null)
                    return _text[start.._pos];

                result.Append(_text, start, _pos - start);
                break;
            }
            _pos++;
        }
        return result.ToString();
    }

    public SyntaxKind ScanTemplateAndSetTokenValue()
    {
        var startedWithBacktick = _text[_pos] == '`';
        _pos++;
        var start = _pos;
        var contents = new StringBuilder();
        SyntaxKind resultingToken;
        while (true)
        {
            if (_pos >= _end)
            {
                contents.Append(_text, start, _pos - start);
                _tokenIsUnterminated = true;
                Error(DiagnosticMessage.Unterminated_template_literal);
                resultingToken = startedWithBacktick ? SyntaxKind.NoSubstitutionTemplateLiteral : SyntaxKind.TemplateTail;
                break;
            }
            var currChar = _text[_pos];
            if (currChar == '`')
            {
                contents.Append(_text, start, _pos - start);
                _pos++;
                resultingToken = startedWithBacktick ? SyntaxKind.NoSubstitutionTemplateLiteral : SyntaxKind.TemplateTail;
                break;
            }
            if (currChar == '$' && _pos + 1 < _end && _text[_pos + 1] == '{')
            {
                contents.Append(_text, start, _pos - start);
                _pos += 2;
                resultingToken = startedWithBacktick ? SyntaxKind.TemplateHead : SyntaxKind.TemplateMiddle;
                break;
            }
            if (currChar == '\\')
            {
                contents.Append(_text, start, _pos - start);
                ScanEscapeSequence(contents);
                start = _pos;
                continue;
            }
            if (currChar == '\r')
            {
                contents.Append(_text, start, _pos - start);
                _pos++;
                if (_pos < _end && _text[_pos] == '\n')
                {
                    _pos++;
                }
                contents.Append('\n');
                start = _pos;
                continue;
            }
            _pos++;
        }
        //Debug.assert(resultingToken != null);
        _tokenValue = contents.ToString();
        return resultingToken;
    }

    public void ScanEscapeSequence(StringBuilder sb)
    {
        _pos++;
        if (_pos >= _end)
        {
            Error(DiagnosticMessage.Unexpected_end_of_text);
        }
        var ch = _text[_pos];
        _pos++;
        switch (ch)
        {
            case '0':
                sb.Append('\0');
                return;
            case 'b':
                sb.Append('\b');
                return;
            case 't':
                sb.Append('\t');
                return;
            case 'n':
                sb.Append('\n');
                return;
            case 'v':
                sb.Append('\v');
                return;
            case 'f':
                sb.Append('\f');
                return;
            case 'r':
                sb.Append('\r');
                return;
            case '\'':
                sb.Append('\'');
                return;
            case '"':
                sb.Append('"');
                return;
            case 'u':
                if (_pos < _end && _text[_pos] == '{')
                {
                    _pos++;
                    ScanExtendedUnicodeEscape(sb);
                    return;
                }
                // '\uDDDD'
                ScanHexadecimalEscape(sb, /*numDigits*/ 4);
                return;
            case 'x':
                // '\xDD'
                ScanHexadecimalEscape(sb, /*numDigits*/ 2);
                return;
            case '\r':
                if (_pos < _end && _text[_pos] == '\n')
                {
                    _pos++;
                }
                return;
            case '\n':
            case (char)CharacterCodes.LineSeparator:
            case (char)CharacterCodes.ParagraphSeparator:
                return;
            default:
                sb.Append(ch);
                return;
        }
    }

    public void ScanHexadecimalEscape(StringBuilder sb, int numDigits)
    {
        var escapedValue = ScanExactNumberOfHexDigits(numDigits);
        if (escapedValue >= 0)
        {
            sb.Append((char)escapedValue);
        }
        else
        {
            Error(DiagnosticMessage.Hexadecimal_digit_expected);
        }
    }

    public void ScanExtendedUnicodeEscape(StringBuilder sb)
    {
        var codePoint = ScanMinimumNumberOfHexDigits(1);
        if (codePoint < 0)
        {
            Error(DiagnosticMessage.Hexadecimal_digit_expected);
            return;
        }

        if (codePoint > 0x10FFFF)
        {
            Error(DiagnosticMessage.An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive);
            return;
        }

        if (_pos >= _end)
        {
            Error(DiagnosticMessage.Unexpected_end_of_text);
            return;
        }

        if (_text[_pos] == '}')
        {
            // Only swallow the following character up if it's a '}'.
            _pos++;
        }
        else
        {
            Error(DiagnosticMessage.Unterminated_Unicode_escape_sequence);
            return;
        }

        if (codePoint <= 65535)
        {
            sb.Append((char)codePoint);
            return;
        }

        var codeUnit1 = (int)Math.Floor(((double)codePoint - 65536) / 1024) + 0xD800;
        var codeUnit2 = ((codePoint - 65536) % 1024) + 0xDC00;
        sb.Append((char)codeUnit1);
        sb.Append((char)codeUnit2);
    }

    public int PeekUnicodeEscape()
    {
        if (_pos + 5 < _end && _text[_pos + 1] == 'u')
        {
            var start = _pos;
            _pos += 2;
            var value = ScanExactNumberOfHexDigits(4);
            _pos = start;
            return value;
        }
        return -1;
    }

    public string ScanIdentifierParts()
    {
        var result = new StringBuilder();
        var start = _pos;
        while (_pos < _end)
        {
            var ch = _text[_pos];
            if (IsIdentifierPart(ch))
            {
                _pos++;
            }
            else
            if (ch == '\\')
            {
                ch = (char)PeekUnicodeEscape();
                if (!(ch >= 0 && IsIdentifierPart(ch)))
                {
                    break;
                }
                result.Append(_text, start, _pos - start);
                result.Append(ch);
                // Valid Unicode escape is always six characters
                _pos += 6;
                start = _pos;
            }
            else
            {
                break;
            }
        }
        result.Append(_text, start, _pos - start);
        return result.ToString();
    }

    public SyntaxKind GetIdentifierToken()
    {
        var len = _tokenValue.Length;
        if (len >= 2 && len <= 11)
        {
            var ch = _tokenValue[0];
            if (ch >= 'a' && ch <= 'z')
            {
                if (textToToken.TryGetValue(_tokenValue, out SyntaxKind value))
                {
                    _token = value;
                    return _token;
                }
            }
        }
        _token = SyntaxKind.Identifier;
        return _token;
    }

    public int ScanBinaryOrOctalDigits(int @base)
    {
        Debug.Assert(@base == 2 || @base == 8, "Expected either @base 2 or @base 8");
        var value = 0;
        var numberOfDigits = 0;
        while (true)
        {
            var ch = _text[_pos];
            var valueOfCh = ch - '0';
            if (!IsDigit(ch) || valueOfCh >= @base)
            {
                break;
            }
            value = value * @base + valueOfCh;
            _pos++;
            numberOfDigits++;
        }
        if (numberOfDigits == 0)
        {
            return -1;
        }
        return value;
    }

    public SyntaxKind Scan()
    {
        _startPos = _pos;
        _precedingLineBreak = false;
        _tokenIsUnterminated = false;
        while (true)
        {
            _tokenPos = _pos;
            if (_pos >= _end)
            {
                _token = SyntaxKind.EndOfFileToken;
                return _token;
            }
            var ch = _text[_pos];
            if (ch == '#' && _pos == 0 && IsShebangTrivia(_text, _pos))
            {
                _pos = ScanShebangTrivia(_text, _pos);
                continue;
            }
            switch (ch)
            {
                case '\n':
                case '\r':
                    _precedingLineBreak = true;
                    _pos++;
                    continue;
                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    _pos++;
                    continue;
                case '!':
                    if (_text[_pos + 1] == '=')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.ExclamationEqualsEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.ExclamationEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.ExclamationToken;
                    return _token;
                case '"':
                case '\'':
                    _tokenValue = ScanString();
                    _token = SyntaxKind.StringLiteral;
                    return _token;
                case '`':
                    _token = ScanTemplateAndSetTokenValue();
                    return _token;
                case '%':
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.PercentEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.PercentToken;
                    return _token;
                case '&':
                    if (_text[_pos + 1] == '&')
                    {
                        _pos += 2;
                        _token = SyntaxKind.AmpersandAmpersandToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.AmpersandEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.AmpersandToken;
                    return _token;
                case '(':
                    _pos++;
                    _token = SyntaxKind.OpenParenToken;
                    return _token;
                case ')':
                    _pos++;
                    _token = SyntaxKind.CloseParenToken;
                    return _token;
                case '*':
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.AsteriskEqualsToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '*')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.AsteriskAsteriskEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.AsteriskAsteriskToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.AsteriskToken;
                    return _token;
                case '+':
                    if (_text[_pos + 1] == '+')
                    {
                        _pos += 2;
                        _token = SyntaxKind.PlusPlusToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.PlusEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.PlusToken;
                    return _token;
                case ',':
                    _pos++;
                    _token = SyntaxKind.CommaToken;
                    return _token;
                case '-':
                    if (_text[_pos + 1] == '-')
                    {
                        _pos += 2;
                        _token = SyntaxKind.MinusMinusToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.MinusEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.MinusToken;
                    return _token;
                case '.':
                    if (IsDigit(_text[_pos + 1]))
                    {
                        _tokenValue = ScanNumber();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    if (_text[_pos + 1] == '.' && _text[_pos + 2] == '.')
                    {
                        _pos += 3;
                        _token = SyntaxKind.DotDotDotToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.DotToken;
                    return _token;
                case '/':
                    if (_text[_pos + 1] == '/')
                    {
                        _pos += 2;
                        while (_pos < _end)
                        {
                            if (IsLineBreak(_text[_pos]))
                            {
                                break;
                            }
                            _pos++;
                        }
                        continue;
                    }

                    if (_text[_pos + 1] == '*')
                    {
                        _pos += 2;
                        var commentClosed = false;
                        while (_pos < _end)
                        {
                            var ch2 = _text[_pos];
                            if (ch2 == '*' && _text[_pos + 1] == '/')
                            {
                                _pos += 2;
                                commentClosed = true;
                                break;
                            }
                            if (IsLineBreak(ch2))
                            {
                                _precedingLineBreak = true;
                            }
                            _pos++;
                        }
                        if (!commentClosed)
                        {
                            Error(DiagnosticMessage.Asterisk_Slash_expected);
                        }
                        continue;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.SlashEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.SlashToken;
                    return _token;
                case '0':
                    if (_pos + 2 < _end && (_text[_pos + 1] == 'x' || _text[_pos + 1] == 'x'))
                    {
                        _pos += 2;
                        var value = ScanMinimumNumberOfHexDigits(1);
                        if (value < 0)
                        {
                            Error(DiagnosticMessage.Hexadecimal_digit_expected);
                            value = 0;
                        }
                        _tokenValue = value.ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    else
            if (_pos + 2 < _end && (_text[_pos + 1] == 'b' || _text[_pos + 1] == 'b'))
                    {
                        _pos += 2;
                        var value = ScanBinaryOrOctalDigits(/* base */ 2);
                        if (value < 0)
                        {
                            Error(DiagnosticMessage.Binary_digit_expected);
                            value = 0;
                        }
                        _tokenValue = value.ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    else
            if (_pos + 2 < _end && (_text[_pos + 1] == 'O' || _text[_pos + 1] == 'o'))
                    {
                        _pos += 2;
                        var value = ScanBinaryOrOctalDigits(/* base */ 8);
                        if (value < 0)
                        {
                            Error(DiagnosticMessage.Octal_digit_expected);
                            value = 0;
                        }
                        _tokenValue = value.ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    if (_pos + 1 < _end && IsOctalDigit(_text[_pos + 1]))
                    {
                        _tokenValue = ScanOctalDigits().ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    goto caseLabel30;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                caseLabel30: _tokenValue = ScanNumber();
                    _token = SyntaxKind.NumericLiteral;
                    return _token;
                case ':':
                    _pos++;
                    _token = SyntaxKind.ColonToken;
                    return _token;
                case ';':
                    _pos++;
                    _token = SyntaxKind.SemicolonToken;
                    return _token;
                case '<':
                    if (IsConflictMarkerTrivia(_text, _pos))
                    {
                        _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                        continue;
                    }
                    if (_text[_pos + 1] == '<')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.LessThanLessThanEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.LessThanLessThanToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.LessThanEqualsToken;
                        return _token;
                    }
                    if (_languageVariant == LanguageVariant.Jsx &&
                                                    _text[_pos + 1] == '/' &&
                                                    _text[_pos + 2] != '*')
                    {
                        _pos += 2;
                        _token = SyntaxKind.LessThanSlashToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.LessThanToken;
                    return _token;
                case '=':
                    if (IsConflictMarkerTrivia(_text, _pos))
                    {
                        _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                        continue;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.EqualsEqualsEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.EqualsEqualsToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '>')
                    {
                        _pos += 2;
                        _token = SyntaxKind.EqualsGreaterThanToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.EqualsToken;
                    return _token;
                case '>':
                    if (IsConflictMarkerTrivia(_text, _pos))
                    {
                        _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                        continue;
                    }
                    _pos++;
                    _token = SyntaxKind.GreaterThanToken;
                    return _token;
                case '?':
                    _pos++;
                    _token = SyntaxKind.QuestionToken;
                    return _token;
                case '[':
                    _pos++;
                    _token = SyntaxKind.OpenBracketToken;
                    return _token;
                case ']':
                    _pos++;
                    _token = SyntaxKind.CloseBracketToken;
                    return _token;
                case '^':
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.CaretEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.CaretToken;
                    return _token;
                case '{':
                    _pos++;
                    _token = SyntaxKind.OpenBraceToken;
                    return _token;
                case '|':
                    if (_text[_pos + 1] == '|')
                    {
                        _pos += 2;
                        _token = SyntaxKind.BarBarToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.BarEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.BarToken;
                    return _token;
                case '}':
                    _pos++;
                    _token = SyntaxKind.CloseBraceToken;
                    return _token;
                case '~':
                    _pos++;
                    _token = SyntaxKind.TildeToken;
                    return _token;
                case '@':
                    _pos++;
                    _token = SyntaxKind.AtToken;
                    return _token;
                case '\\':
                    var cookedChar = PeekUnicodeEscape();
                    if (cookedChar >= 0 && IsIdentifierStart(cookedChar))
                    {
                        _pos += 6;
                        _tokenValue = (char)cookedChar + ScanIdentifierParts();
                        _token = GetIdentifierToken();
                        return _token;
                    }
                    Error(DiagnosticMessage.Invalid_character);
                    _pos++;
                    _token = SyntaxKind.Unknown;
                    return _token;
                default:
                    if (IsIdentifierStart(ch))
                    {
                        _pos++;
                        while (_pos < _end && IsIdentifierPart(ch = _text[_pos])) _pos++;
                        _tokenValue = _text[_tokenPos.._pos];
                        if (ch == '\\')
                        {
                            _tokenValue += ScanIdentifierParts();
                        }
                        return _token = GetIdentifierToken();
                    }
                    else if (IsWhiteSpaceSingleLine(ch))
                    {
                        _pos++;
                        continue;
                    }
                    else if (IsLineBreak(ch))
                    {
                        _precedingLineBreak = true;
                        _pos++;
                        continue;
                    }
                    Error(DiagnosticMessage.Invalid_character);
                    _pos++;
                    _token = SyntaxKind.Unknown;
                    return _token;
            }
        }
    }

    public SyntaxKind ReScanGreaterToken()
    {
        if (_token == SyntaxKind.GreaterThanToken)
        {
            if (_text[_pos] == '>')
            {
                if (_text[_pos + 1] == '>')
                {
                    if (_text[_pos + 2] == '=')
                    {
                        _pos += 3;
                        _token = SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken;
                        return _token;
                    }
                    _pos += 2;
                    _token = SyntaxKind.GreaterThanGreaterThanGreaterThanToken;
                    return _token;
                }
                if (_text[_pos + 1] == '=')
                {
                    _pos += 2;
                    _token = SyntaxKind.GreaterThanGreaterThanEqualsToken;
                    return _token;
                }
                _pos++;
                _token = SyntaxKind.GreaterThanGreaterThanToken;
                return _token;
            }
            if (_text[_pos] == '=')
            {
                _pos++;
                _token = SyntaxKind.GreaterThanEqualsToken;
                return _token;
            }
        }
        return _token;
    }

    public SyntaxKind ReScanSlashToken()
    {
        if (_token == SyntaxKind.SlashToken || _token == SyntaxKind.SlashEqualsToken)
        {
            var p = _tokenPos + 1;
            var inEscape = false;
            var inCharacterClass = false;
            while (true)
            {
                if (p >= _end)
                {
                    _tokenIsUnterminated = true;
                    Error(DiagnosticMessage.Unterminated_regular_expression_literal);
                    break;
                }
                var ch = _text[p];
                if (IsLineBreak(ch))
                {
                    _tokenIsUnterminated = true;
                    Error(DiagnosticMessage.Unterminated_regular_expression_literal);
                    break;
                }
                if (inEscape)
                {
                    // Parsing an escape character;
                    // reset the flag and just advance to the next char.
                    inEscape = false;
                }
                else
                if (ch == '/' && !inCharacterClass)
                {
                    // A slash within a character class is permissible,
                    // but in general it signals the end of the regexp literal.
                    p++;
                    break;
                }
                else
                if (ch == '[')
                {
                    inCharacterClass = true;
                }
                else
                if (ch == '\\')
                {
                    inEscape = true;
                }
                else
                if (ch == ']')
                {
                    inCharacterClass = false;
                }
                p++;
            }
            while (p < _end && IsIdentifierPart(_text[p]))
            {
                p++;
            }
            _pos = p;
            _tokenValue = _text[_tokenPos.._pos];
            _token = SyntaxKind.RegularExpressionLiteral;
        }
        return _token;
    }

    public SyntaxKind ReScanTemplateToken()
    {
        Debug.Assert(_token == SyntaxKind.CloseBraceToken, "'reScanTemplateToken' should only be called on a '}'");
        _pos = _tokenPos;
        _token = ScanTemplateAndSetTokenValue();
        return _token;
    }

    public SyntaxKind ReScanJsxToken()
    {
        _pos = _tokenPos = _startPos;
        _token = ScanJsxToken();
        return _token;
    }

    public SyntaxKind ScanJsxToken()
    {
        _startPos = _tokenPos = _pos;
        if (_pos >= _end)
        {
            _token = SyntaxKind.EndOfFileToken;
            return _token;
        }
        var @char = _text[_pos];
        if (@char == '<')
        {
            if (_text[_pos + 1] == '/')
            {
                _pos += 2;
                _token = SyntaxKind.LessThanSlashToken;
                return _token;
            }
            _pos++;
            _token = SyntaxKind.LessThanToken;
            return _token;
        }
        if (@char == '{')
        {
            _pos++;
            _token = SyntaxKind.OpenBraceToken;
            return _token;
        }
        while (_pos < _end)
        {
            _pos++;
            @char = _text[_pos];
            if (@char == '{')
            {
                break;
            }
            if (@char == '<')
            {
                if (IsConflictMarkerTrivia(_text, _pos))
                {
                    _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                    _token = SyntaxKind.ConflictMarkerTrivia;
                    return _token;
                }
                break;
            }
        }
        _token = SyntaxKind.JsxText;
        return _token;
    }

    public SyntaxKind ScanJsxIdentifier()
    {
        if (TokenIsIdentifierOrKeyword(_token))
        {
            var firstCharPosition = _pos;
            while (_pos < _end)
            {
                var ch = _text[_pos];
                if (ch == '-' || ((firstCharPosition == _pos) ? IsIdentifierStart(ch) : IsIdentifierPart(ch)))
                {
                    _pos++;
                }
                else
                {
                    break;
                }
            }
            _tokenValue += _text[firstCharPosition.._pos];
        }
        return _token;
    }

    public SyntaxKind ScanJsxAttributeValue()
    {
        _startPos = _pos;
        switch (_text[_pos])
        {
            case '"':
            case '\'':
                _tokenValue = ScanString(/*allowEscapes*/ false);
                _token = SyntaxKind.StringLiteral;
                return _token;
            default:
                // If this scans anything other than `{`, it's a parse error.
                return Scan();
        }
    }

    public SyntaxKind ScanJsDocToken()
    {
        if (_pos >= _end)
        {
            _token = SyntaxKind.EndOfFileToken;
            return _token;
        }
        _startPos = _pos;
        _tokenPos = _pos;
        var ch = _text[_pos];
        switch (ch)
        {
            case '\t':
            case '\v':
            case '\f':
            case ' ':
                while (_pos < _end && IsWhiteSpaceSingleLine(_text[_pos]))
                {
                    _pos++;
                }
                _token = SyntaxKind.WhitespaceTrivia;
                return _token;
            case '@':
                _pos++;
                _token = SyntaxKind.AtToken;
                return _token;
            case '\n':
            case '\r':
                _pos++;
                _token = SyntaxKind.NewLineTrivia;
                return _token;
            case '*':
                _pos++;
                _token = SyntaxKind.AsteriskToken;
                return _token;
            case '{':
                _pos++;
                _token = SyntaxKind.OpenBraceToken;
                return _token;
            case '}':
                _pos++;
                _token = SyntaxKind.CloseBraceToken;
                return _token;
            case '[':
                _pos++;
                _token = SyntaxKind.OpenBracketToken;
                return _token;
            case ']':
                _pos++;
                _token = SyntaxKind.CloseBracketToken;
                return _token;
            case '=':
                _pos++;
                _token = SyntaxKind.EqualsToken;
                return _token;
            case ',':
                _pos++;
                _token = SyntaxKind.CommaToken;
                return _token;
            case '.':
                _pos++;
                _token = SyntaxKind.DotToken;
                return _token;
        }
        if (IsIdentifierStart(ch))
        {
            _pos++;
            while (IsIdentifierPart(_text[_pos]) && _pos < _end)
            {
                _pos++;
            }
            _token = SyntaxKind.Identifier;
            return _token;
        }
        else
        {
            _pos += 1;
            _token = SyntaxKind.Unknown;
            return _token;
        }
    }

    public T SpeculationHelper<T>(Func<T> callback, bool isLookahead)
    {
        var savePos = _pos;
        var saveStartPos = _startPos;
        var saveTokenPos = _tokenPos;
        var saveToken = _token;
        var saveTokenValue = _tokenValue;
        var savePrecedingLineBreak = _precedingLineBreak;
        var result = callback();
        if (result == null || ((result is bool) && Convert.ToBoolean(result) == false) || isLookahead)
        {
            _pos = savePos;
            _startPos = saveStartPos;
            _tokenPos = saveTokenPos;
            _token = saveToken;
            _tokenValue = saveTokenValue;
            _precedingLineBreak = savePrecedingLineBreak;
        }
        return result;
    }

    public T ScanRange<T>(int? start, int length, Func<T> callback)
    {
        var saveEnd = _end;
        var savePos = _pos;
        var saveStartPos = _startPos;
        var saveTokenPos = _tokenPos;
        var saveToken = _token;
        var savePrecedingLineBreak = _precedingLineBreak;
        var saveTokenValue = _tokenValue;
        var saveTokenIsUnterminated = _tokenIsUnterminated;
        SetText(_text, start, length);
        var result = callback();
        _end = saveEnd;
        _pos = savePos;
        _startPos = saveStartPos;
        _tokenPos = saveTokenPos;
        _token = saveToken;
        _precedingLineBreak = savePrecedingLineBreak;
        _tokenValue = saveTokenValue;
        _tokenIsUnterminated = saveTokenIsUnterminated;
        return result;
    }

    public T LookAhead<T>(Func<T> callback)
    {
        return SpeculationHelper(callback, /*isLookahead*/ true);
    }

    public T TryScan<T>(Func<T> callback)
    {
        return SpeculationHelper(callback, /*isLookahead*/ false);
    }

    public void SetText(string newText, int? start = null, int? length = null)
    {
        _text = newText ?? "";
        _end = length == null ? _text.Length : (int)start + (int)length;
        TextPos = start ?? 0;
    }

    public void SetOnError(ErrorCallback errorCallback)
    {
        OnError = errorCallback;
    }

    public void SetLanguageVariant(LanguageVariant variant)
    {
        _languageVariant = variant;
    }
}
