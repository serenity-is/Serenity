namespace Serenity.TypeScript;

public delegate void ErrorCallback(DiagnosticMessage message, int length, object arg0);

public partial class Scanner
{
    private string text;

    // Current position (end position of text of current token)
    private int pos;

    // end of text
    private int end;

    // Start position of whitespace before current token
    private int fullStartPos;

    // Start position of text of current token
    private int tokenStart;

    private SyntaxKind token;
    private string tokenValue;
    private TokenFlags tokenFlags;

    private List<CommentDirective> commentDirectives;
    private int inJSDocType;

    private ScriptKind scriptKind = ScriptKind.Unknown;
    private JSDocParsingMode jsDocParsingMode = JSDocParsingMode.ParseAll;

    private ScriptTarget languageVersion = ScriptTarget.Latest;
    private LanguageVariant languageVariant;
    private readonly bool skipTrivia;

    private ErrorCallback onError;

    public Scanner(ScriptTarget languageVersion, bool skipTrivia, LanguageVariant languageVariant = LanguageVariant.Standard,
        string textInitial = null, ErrorCallback onError = null, int? start = null, int? length = null)
    {
        this.languageVersion = languageVersion;
        this.skipTrivia = skipTrivia;
        this.languageVariant = languageVariant;
        SetText(textInitial, start, length);
        this.onError = onError;
    }

    public SyntaxKind GetToken() => token;
    public int GetTokenFullStart() => fullStartPos;
    public int GetTokenStart() => tokenStart;
    public int GetTokenEnd() => pos;
    public string GetTokenText() => text[tokenStart..pos];
    public string GetTokenValue() => tokenValue;
    public bool HasUnicodeEscape() => (tokenFlags & TokenFlags.UnicodeEscape) != 0;
    public bool HasExtendedUnicodeEscape() => (tokenFlags & TokenFlags.ExtendedUnicodeEscape) != 0;
    public bool HasPrecedingLineBreak() => (tokenFlags & TokenFlags.PrecedingLineBreak) != 0;
    internal bool HasPrecedingJSDocComment() => (tokenFlags & TokenFlags.PrecedingJSDocComment) != 0;
    public bool IsIdentifier() => token == SyntaxKind.Identifier || token > SyntaxKindMarker.LastReservedWord;
    public bool IsReservedWord() => token >= SyntaxKindMarker.FirstReservedWord && token <= SyntaxKindMarker.LastReservedWord;
    public bool IsUnterminated() => (tokenFlags & TokenFlags.Unterminated) != 0;
    internal TokenFlags GetNumericLiteralFlags() => tokenFlags & TokenFlags.NumericLiteralFlags;
    internal IEnumerable<CommentDirective> GetCommentDirectives() => commentDirectives;
    internal TokenFlags GetTokenFlags() => tokenFlags;

    private void Error(DiagnosticMessage message, int? errPos = null, int length = 0, object arg0 = null)
    {
        if (onError != null)
        {
            var oldPos = pos;
            pos = errPos ?? pos;
            onError?.Invoke(message, length, arg0);
            pos = oldPos;
        }
    }

    private string ScanNumberFragment()
    {
        var start = pos;
        var allowSeparator = false;
        var isPreviousTokenSeparator = false;
        var result = "";
        while (pos < end)
        {
            var ch = text[pos];
            if (ch == CharacterCodes._)
            {
                tokenFlags |= TokenFlags.ContainsSeparator;
                if (allowSeparator)
                {
                    allowSeparator = false;
                    isPreviousTokenSeparator = true;
                    result += text[start..pos];
                }
                else
                {
                    tokenFlags |= TokenFlags.ContainsInvalidSeparator;
                    if (isPreviousTokenSeparator)
                    {
                        Error(Diagnostics.Multiple_consecutive_numeric_separators_are_not_permitted, pos, 1);
                    }
                    else
                    {
                        Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos, 1);
                    }
                }
                pos++;
                start = pos;
                continue;
            }
            if (IsDigit(ch))
            {
                allowSeparator = true;
                isPreviousTokenSeparator = false;
                pos++;
                continue;
            }
            break;
        }
        if (pos > 0 && text[pos - 1] == CharacterCodes._)
        {
            tokenFlags |= TokenFlags.ContainsInvalidSeparator;
            Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos - 1, 1);
        }
        return result + text[start..pos];
    }

    // Extract from Section 12.9.3
    // NumericLiteral ::=
    //     | DecimalLiteral
    //     | DecimalBigIntegerLiteral
    //     | NonDecimalIntegerLiteral 'n'?
    //     | LegacyOctalIntegerLiteral
    // DecimalBigIntegerLiteral ::=
    //     | '0n'
    //     | [1-9] DecimalDigits? 'n'
    //     | [1-9] '_' DecimalDigits 'n'
    // DecimalLiteral ::=
    //     | DecimalIntegerLiteral? '.' DecimalDigits? ExponentPart?
    //     | '.' DecimalDigits ExponentPart?
    //     | DecimalIntegerLiteral ExponentPart?
    // DecimalIntegerLiteral ::=
    //     | '0'
    //     | [1-9] '_'? DecimalDigits
    //     | NonOctalDecimalIntegerLiteral
    // LegacyOctalIntegerLiteral ::= '0' [0-7]+
    // NonOctalDecimalIntegerLiteral ::= '0' [0-7]* [89] [0-9]*
    private SyntaxKind ScanNumber()
    {
        var start = pos;
        string mainFragment;
        if (text[pos] == CharacterCodes._0)
        {
            pos++;
            if (text[pos] == CharacterCodes._)
            {
                tokenFlags |= TokenFlags.ContainsSeparator | TokenFlags.ContainsInvalidSeparator;
                Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos, 1);
                // treat it as a normal number literal
                pos--;
                mainFragment = ScanNumberFragment();
            }
            // Separators are not allowed in the below cases
            else if (!ScanDigits())
            {
                // NonOctalDecimalIntegerLiteral, emit error later
                // Separators in decimal and exponent parts are still allowed according to the spec
                tokenFlags |= TokenFlags.ContainsLeadingZero;
                mainFragment = "" + (long.TryParse(tokenValue, out var v) ? v.ToString(CultureInfo.InvariantCulture) : tokenValue);
            }
            else if (string.IsNullOrEmpty(tokenValue))
            {
                // a single zero
                mainFragment = "0";
            }
            else
            {
                // LegacyOctalIntegerLiteral
                var value = Convert.ToInt64(tokenValue, 8);
                tokenValue = "" + value.ToString(CultureInfo.InvariantCulture);
                tokenFlags |= TokenFlags.Octal;
                var withMinus = token == SyntaxKind.MinusToken;
                var literal = (withMinus ? "-" : "") + "0o" + Convert.ToString(value, 8);
                if (withMinus)
                    start--;
                Error(Diagnostics.Octal_literals_are_not_allowed_Use_the_syntax_0, start, pos - start, literal);
                return SyntaxKind.NumericLiteral;
            }
        }
        else
        {
            mainFragment = ScanNumberFragment();
        }
        string decimalFragment = null;
        string scientificFragment = null;
        if (pos < end && text[pos] == CharacterCodes.Dot)
        {
            pos++;
            decimalFragment = ScanNumberFragment();
        }
        var endPos = pos;
        if (pos < end && (text[pos] == CharacterCodes.E || text[pos] == CharacterCodes.e))
        {
            pos++;
            tokenFlags |= TokenFlags.Scientific;
            if (text[pos] == CharacterCodes.Plus || text[pos] == CharacterCodes.Minus)
                pos++;
            var preNumericPart = pos;
            var finalFragment = ScanNumberFragment();
            if (string.IsNullOrEmpty(finalFragment))
            {
                Error(Diagnostics.Digit_expected);
            }
            else
            {
                scientificFragment = text[endPos..preNumericPart] + finalFragment;
                endPos = pos;
            }
        }
        string result;
        if ((tokenFlags & TokenFlags.ContainsSeparator) != 0)
        {
            result = mainFragment;
            if (!string.IsNullOrEmpty(decimalFragment))
            {
                result += "." + decimalFragment;
            }
            if (!string.IsNullOrEmpty(scientificFragment))
            {
                result += scientificFragment;
            }
        }
        else
        {
            result = text[start..endPos]; // No need to use all the fragments; no _ removal needed
        }

        if ((tokenFlags & TokenFlags.ContainsLeadingZero) != 0)
        {
            Error(Diagnostics.Decimals_with_leading_zeros_are_not_allowed, start, endPos - start);
            // if a literal has a leading zero, it must not be bigint
            tokenValue = "" + result;
            return SyntaxKind.NumericLiteral;
        }

        if (decimalFragment != null || (tokenFlags & TokenFlags.Scientific) != 0)
        {
            CheckForIdentifierStartAfterNumericLiteral(start, decimalFragment == null && (tokenFlags & TokenFlags.Scientific) != 0);
            // if value is not an integer, it can be safely coerced to a number
            tokenValue = double.Parse(result, CultureInfo.InvariantCulture).ToString().ToLowerInvariant();
            return SyntaxKind.NumericLiteral;
        }
        else
        {
            tokenValue = result;
            var type = CheckBigIntSuffix(); // if value is an integer, check whether it is a bigint
            CheckForIdentifierStartAfterNumericLiteral(start);
            return type;
        }
    }

    private void CheckForIdentifierStartAfterNumericLiteral(int numericStart, bool isScientific = false)
    {
        if (pos >= end || !IsIdentifierStart(text[pos], languageVersion))
        {
            return;
        }

        var identifierStart = pos;
        var length = ScanIdentifierParts().Length;

        if (length == 1 && text[identifierStart] == CharacterCodes.LineFeed)
        {
            if (isScientific)
            {
                Error(Diagnostics.A_bigint_literal_cannot_use_exponential_notation, numericStart, identifierStart - numericStart + 1);
            }
            else
            {
                Error(Diagnostics.A_bigint_literal_must_be_an_integer, numericStart, identifierStart - numericStart + 1);
            }
        }
        else
        {
            Error(Diagnostics.An_identifier_or_keyword_cannot_immediately_follow_a_numeric_literal, identifierStart, length);
            pos = identifierStart;
        }
    }

    private bool ScanDigits()
    {
        var start = pos;
        var isOctal = true;
        while (pos < end && IsDigit(text[pos]))
        {
            if (!IsOctalDigit(text[pos]))
            {
                isOctal = false;
            }
            pos++;
        }
        tokenValue = text[start..pos];
        return isOctal;
    }

    // Scans the given number of hexadecimal digits in the text,
    // returning -1 if the given number is unavailable.
    private long ScanExactNumberOfHexDigits(int count, bool canHaveSeparators)
    {
        var valueString = ScanHexDigits(minCount: count, scanAsManyAsPossible: false, canHaveSeparators);
        return !string.IsNullOrEmpty(valueString) ? Convert.ToInt64(valueString, 16) : -1;
    }

    // Scans as many hexadecimal digits as are available in the text,
    // returning "" if the given number of digits was unavailable.
    private string ScanMinimumNumberOfHexDigits(int count, bool canHaveSeparators)
    {
        return ScanHexDigits(minCount: count, scanAsManyAsPossible: true, canHaveSeparators);
    }

    private string ScanHexDigits(int minCount, bool scanAsManyAsPossible, bool canHaveSeparators)
    {
        var valueChars = new StringBuilder();
        var allowSeparator = false;
        var isPreviousTokenSeparator = false;
        while ((valueChars.Length < minCount || scanAsManyAsPossible) && pos < end)
        {
            var ch = text[pos];
            if (canHaveSeparators && ch == CharacterCodes._)
            {
                tokenFlags |= TokenFlags.ContainsSeparator;
                if (allowSeparator)
                {
                    allowSeparator = false;
                    isPreviousTokenSeparator = true;
                }
                else if (isPreviousTokenSeparator)
                {
                    Error(Diagnostics.Multiple_consecutive_numeric_separators_are_not_permitted, pos, 1);
                }
                else
                {
                    Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos, 1);
                }
                pos++;
                continue;
            }
            allowSeparator = canHaveSeparators;
            if (ch >= CharacterCodes.A && ch <= CharacterCodes.F)
            {
                ch = (char)(ch + CharacterCodes.a - CharacterCodes.A); // standardize hex literals to lowercase
            }
            else if (
                !((ch >= CharacterCodes._0 && ch <= CharacterCodes._9) ||
                    (ch >= CharacterCodes.a && ch <= CharacterCodes.f))
            )
            {
                break;
            }
            valueChars.Append(ch);
            pos++;
            isPreviousTokenSeparator = false;
        }
        if (valueChars.Length < minCount)
        {
            valueChars.Clear();
        }
        if (pos > 0 && text[pos - 1] == CharacterCodes._)
        {
            Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos - 1, 1);
        }
        return valueChars.ToString();
    }

    private string ScanString(bool jsxAttributeString = false)
    {
        var quote = text[pos];
        pos++;
        StringBuilder result = null;
        var start = pos;
        while (true)
        {
            if (pos >= end)
            {
                tokenFlags |= TokenFlags.Unterminated;
                Error(Diagnostics.Unterminated_string_literal);
                if (result is null)
                    return text[start..pos];
                result.Append(text, start, pos - start);
                break;
            }
            var ch = text[pos];
            if (ch == quote)
            {
                if (result is null)
                    return text[start..pos++];

                result.Append(text, start, pos - start);
                pos++;
                break;
            }
            if (ch == CharacterCodes.Backslash && !jsxAttributeString)
            {
                result ??= new();
                result.Append(text, start, pos - start);
                ScanEscapeSequence(result, shouldEmitInvalidEscapeError: true);
                start = pos;
                continue;
            }
            if ((ch == CharacterCodes.LineFeed || ch == CharacterCodes.CarriageReturn) &&
                !jsxAttributeString)
            {
                tokenFlags |= TokenFlags.Unterminated;
                Error(Diagnostics.Unterminated_string_literal);
                if (result is null)
                    return text[start..pos];

                result.Append(text, start, pos - start);
                break;
            }
            pos++;
        }
        return result.ToString();
    }

    // Sets the current 'tokenValue' and returns a NoSubstitutionTemplateLiteral or
    // a literal component of a TemplateExpression.
    private SyntaxKind ScanTemplateAndSetTokenValue(bool shouldEmitInvalidEscapeError)
    {
        var startedWithBacktick = text[pos] == CharacterCodes.Backtick;

        pos++;
        var start = pos;
        var contents = new StringBuilder();
        SyntaxKind resultingToken;

        while (true)
        {
            if (pos >= end)
            {
                contents.Append(text, start, pos - start);
                tokenFlags |= TokenFlags.Unterminated;
                Error(Diagnostics.Unterminated_template_literal);
                resultingToken = startedWithBacktick ? SyntaxKind.NoSubstitutionTemplateLiteral : SyntaxKind.TemplateTail;
                break;
            }

            var currChar = text[pos];

            // '`'
            if (currChar == CharacterCodes.Backtick)
            {
                contents.Append(text, start, pos - start);
                pos++;
                resultingToken = startedWithBacktick ? SyntaxKind.NoSubstitutionTemplateLiteral : SyntaxKind.TemplateTail;
                break;
            }

            // '${'
            if (currChar == CharacterCodes.Dollar && pos + 1 < end && text[pos + 1] == CharacterCodes.OpenBrace)
            {
                contents.Append(text, start, pos - start);
                pos += 2;
                resultingToken = startedWithBacktick ? SyntaxKind.TemplateHead : SyntaxKind.TemplateMiddle;
                break;
            }

            // Escape character
            if (currChar == CharacterCodes.Backslash)
            {
                contents.Append(text, start, pos - start);
                ScanEscapeSequence(contents, shouldEmitInvalidEscapeError);
                start = pos;
                continue;
            }

            // Speculated ECMAScript 6 Spec 11.8.6.1:
            // <CR><LF> and <CR> LineTerminatorSequences are normalized to <LF> for Template Values
            if (currChar == CharacterCodes.CarriageReturn)
            {
                contents.Append(text, start, pos - start);
                pos++;

                if (pos < end && text[pos] == CharacterCodes.LineFeed)
                {
                    pos++;
                }

                contents.Append('\n');
                start = pos;
                continue;
            }
            pos++;
        }

        Debug.Assert(resultingToken != SyntaxKind.Unknown);

        tokenValue = contents.ToString();
        return resultingToken;
    }

    // Extract from Section A.1
    // EscapeSequence ::
    //     | CharacterEscapeSequence
    //     | 0 (?![0-9])
    //     | LegacyOctalEscapeSequence
    //     | NonOctalDecimalEscapeSequence
    //     | HexEscapeSequence
    //     | UnicodeEscapeSequence
    // LegacyOctalEscapeSequence ::=
    //     | '0' (?=[89])
    //     | [1-7] (?![0-7])
    //     | [0-3] [0-7] (?![0-7])
    //     | [4-7] [0-7]
    //     | [0-3] [0-7] [0-7]
    // NonOctalDecimalEscapeSequence ::= [89]
    private void ScanEscapeSequence(StringBuilder sb, bool shouldEmitInvalidEscapeError)
    {
        var start = pos;
        pos++;
        if (pos >= end)
        {
            Error(Diagnostics.Unexpected_end_of_text);
            return;
        }

        var ch = text[pos];
        pos++;
        switch ((int)ch)
        {
            case CharacterCodes._0:
                // Although '0' preceding any digit is treated as LegacyOctalEscapeSequence,
                // '\08' should separately be interpreted as '\0' + '8'.
                if (pos >= end || !IsDigit(text[pos]))
                {
                    sb.Append('\0');
                    return;
                }
                goto label_characterCodes_8_9;

            // '\01', '\011'
            // falls through
            case CharacterCodes._1:
            case CharacterCodes._2:
            case CharacterCodes._3:
                // '\1', '\17', '\177'
                if (pos < end && IsOctalDigit(text[pos]))
                {
                    pos++;
                }
                goto label_characterCodes_8_9;

            // '\17', '\177'
            // falls through
            case CharacterCodes._4:
            case CharacterCodes._5:
            case CharacterCodes._6:
            case CharacterCodes._7:
                // '\4', '\47' but not '\477'
                if (pos < end && IsOctalDigit(text[pos]))
                {
                    pos++;
                }
                // '\47'
                tokenFlags |= TokenFlags.ContainsInvalidEscape;
                if (shouldEmitInvalidEscapeError)
                {
                    var code = Convert.ToUInt32(text[(start + 1)..pos], 8);
                    Error(Diagnostics.Octal_escape_sequences_are_not_allowed_Use_the_syntax_0, start, pos - start,
                        "\\x" + Convert.ToString(code, 16).PadLeft(2, '0'));
                    sb.Append((char)code);
                    return;
                }
                sb.Append(text[start..pos]);
                return;

            case CharacterCodes._8:
            case CharacterCodes._9:
            label_characterCodes_8_9:
                // the invalid '\8' and '\9'
                tokenFlags |= TokenFlags.ContainsInvalidEscape;
                if (shouldEmitInvalidEscapeError)
                {
                    Error(Diagnostics.Escape_sequence_0_is_not_allowed, start, pos - start, text[start..pos]);
                    sb.Append(ch);
                    return;
                }
                sb.Append(text[start..pos]);
                return;

            case CharacterCodes.b:
                sb.Append('\b');
                return;

            case CharacterCodes.t:
                sb.Append('\t');
                return;

            case CharacterCodes.n:
                sb.Append('\n');
                return;

            case CharacterCodes.v:
                sb.Append('\v');
                return;

            case CharacterCodes.f:
                sb.Append('\f');
                return;

            case CharacterCodes.r:
                sb.Append('\r');
                return;

            case CharacterCodes.SingleQuote:
                sb.Append('\'');
                return;

            case CharacterCodes.DoubleQuote:
                sb.Append('"');
                return;

            case CharacterCodes.u:
                if (pos < end && text[pos] == CharacterCodes.OpenBrace)
                {
                    // '\u{DDDDDDDD}'
                    pos++;
                    var escapedValueString = ScanMinimumNumberOfHexDigits(1, /*canHaveSeparators*/ false);
                    var escapedValue = !string.IsNullOrEmpty(escapedValueString) ? Convert.ToInt32(escapedValueString, 16) : -1;
                    // '\u{Not Code Point' or '\u{CodePoint'
                    if (escapedValue < 0)
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (shouldEmitInvalidEscapeError)
                        {
                            Error(Diagnostics.Hexadecimal_digit_expected);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                    if (!IsCodePoint(escapedValue))
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (shouldEmitInvalidEscapeError)
                        {
                            Error(Diagnostics.An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                    if (pos >= end)
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (shouldEmitInvalidEscapeError)
                        {
                            Error(Diagnostics.Unexpected_end_of_text);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                    if (text[pos] != CharacterCodes.CloseBrace)
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (shouldEmitInvalidEscapeError)
                        {
                            Error(Diagnostics.Unterminated_Unicode_escape_sequence);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                    pos++;
                    tokenFlags |= TokenFlags.ExtendedUnicodeEscape;
                    sb.Append(char.ConvertFromUtf32(escapedValue));
                    return;
                }
                // '\uDDDD'
                for (; pos < start + 6; pos++)
                {
                    if (!(pos < end && IsHexDigit(text[pos])))
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (shouldEmitInvalidEscapeError)
                        {
                            Error(Diagnostics.Hexadecimal_digit_expected);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                }
                tokenFlags |= TokenFlags.UnicodeEscape;
                sb.Append((char)Convert.ToUInt32(text[(start + 2)..pos], 16));
                return;

            case CharacterCodes.x:
                // '\xDD'
                for (; pos < start + 4; pos++)
                {
                    if (!(pos < end && IsHexDigit(text[pos])))
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (shouldEmitInvalidEscapeError)
                        {
                            Error(Diagnostics.Hexadecimal_digit_expected);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                }
                tokenFlags |= TokenFlags.HexEscape;
                sb.Append((char)Convert.ToUInt32(text[(start + 2)..pos], 16));
                return;

            // when encountering a LineContinuation (i.e. a backslash and a line terminator sequence),
            // the line terminator is interpreted to be "the empty code unit sequence".
            case CharacterCodes.CarriageReturn:
                if (pos < end && text[pos] == CharacterCodes.LineFeed)
                {
                    pos++;
                }
                return;
            // falls through
            case CharacterCodes.LineFeed:
            case CharacterCodes.LineSeparator:
            case CharacterCodes.ParagraphSeparator:
                return;
            default:
                sb.Append(ch);
                return;
        }
    }
    private string ScanExtendedUnicodeEscape()
    {
        var escapedValueString = ScanMinimumNumberOfHexDigits(1, /*canHaveSeparators*/ false);
        var escapedValue = !string.IsNullOrEmpty(escapedValueString) ? Convert.ToInt32(escapedValueString, 16) : -1;
        var isInvalidExtendedEscape = false;

        // Validate the value of the digit
        if (escapedValue < 0)
        {
            Error(Diagnostics.Hexadecimal_digit_expected);
            isInvalidExtendedEscape = true;
        }
        else if (escapedValue > 0x10FFFF)
        {
            Error(Diagnostics.An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive);
            isInvalidExtendedEscape = true;
        }

        if (pos >= end)
        {
            Error(Diagnostics.Unexpected_end_of_text);
            isInvalidExtendedEscape = true;
        }
        else if (text[pos] == CharacterCodes.CloseBrace)
        {
            // Only swallow the following character up if it's a '}'.
            pos++;
        }
        else
        {
            Error(Diagnostics.Unterminated_Unicode_escape_sequence);
            isInvalidExtendedEscape = true;
        }

        if (isInvalidExtendedEscape)
        {
            return "";
        }

        return char.ConvertFromUtf32(escapedValue);
    }

    // Current character is known to be a backslash. Check for Unicode escape of the form '\uXXXX'
    // and return code point value if valid Unicode escape is found. Otherwise return -1.
    private int PeekUnicodeEscape()
    {
        if (pos + 5 < end && text[pos + 1] == CharacterCodes.u)
        {
            var start = pos;
            pos += 2;
            var value = ScanExactNumberOfHexDigits(4, canHaveSeparators: false);
            pos = start;
            return (int)value;
        }
        return -1;
    }
    private int PeekExtendedUnicodeEscape()
    {
        if (pos + 2 < end && text[pos + 1] == CharacterCodes.u && text[pos + 2] == CharacterCodes.OpenBrace)
        {
            var start = pos;
            pos += 3;
            var escapedValueString = ScanMinimumNumberOfHexDigits(1, canHaveSeparators: false);
            var escapedValue = !string.IsNullOrEmpty(escapedValueString) ? Convert.ToInt32(escapedValueString, 16) : -1;
            pos = start;
            return escapedValue;
        }
        return -1;
    }

    private string ScanIdentifierParts()
    {
        var result = new StringBuilder();
        var start = pos;
        while (pos < end)
        {
            var ch = (int)text[pos];
            if (IsIdentifierPart(ch, languageVersion))
            {
                pos++;
            }
            else if (ch == CharacterCodes.Backslash)
            {
                ch = PeekExtendedUnicodeEscape();
                if (ch >= 0 && IsIdentifierPart(ch, languageVersion))
                {
                    pos += 3;
                    tokenFlags |= TokenFlags.ExtendedUnicodeEscape;
                    result.Append(ScanExtendedUnicodeEscape());
                    start = pos;
                    continue;
                }
                ch = PeekUnicodeEscape();
                if (!(ch >= 0 && IsIdentifierPart(ch, languageVersion)))
                {
                    break;
                }
                tokenFlags |= TokenFlags.UnicodeEscape;
                result.Append(text, start, pos - start);
                result.Append(char.ConvertFromUtf32(ch));
                // Valid Unicode escape is always six characters
                pos += 6;
                start = pos;
            }
            else
            {
                break;
            }
        }
        result.Append(text, start, pos - start);
        return result.ToString();
    }

    private SyntaxKind GetIdentifierToken()
    {
        var len = tokenValue.Length;
        if (len >= 2 && len <= 11)
        {
            var ch = tokenValue[0];
            if (ch >= CharacterCodes.a && ch <= CharacterCodes.z)
            {
                if (textToToken.TryGetValue(tokenValue, out SyntaxKind keyword))
                {
                    return token = keyword;
                }
            }
        }
        return token = SyntaxKind.Identifier;
    }

    private string ScanBinaryOrOctalDigits(int @base)
    {
        var value = new StringBuilder();
        // For counting number of digits; Valid binaryIntegerLiteral must have at least one binary digit following B or b.
        // Similarly valid octalIntegerLiteral must have at least one octal digit following o or O.
        var separatorAllowed = false;
        var isPreviousTokenSeparator = false;
        while (true)
        {
            var ch = text[pos];
            // Numeric separators are allowed anywhere within a numeric literal, except not at the beginning, or following another separator
            if (ch == CharacterCodes._)
            {
                tokenFlags |= TokenFlags.ContainsSeparator;
                if (separatorAllowed)
                {
                    separatorAllowed = false;
                    isPreviousTokenSeparator = true;
                }
                else if (isPreviousTokenSeparator)
                {
                    Error(Diagnostics.Multiple_consecutive_numeric_separators_are_not_permitted, pos, 1);
                }
                else
                {
                    Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos, 1);
                }
                pos++;
                continue;
            }
            separatorAllowed = true;
            if (!IsDigit(ch) || ch - CharacterCodes._0 >= @base)
            {
                break;
            }
            value.Append(text[pos]);
            pos++;
            isPreviousTokenSeparator = false;
        }
        if (pos > 0 && text[pos - 1] == CharacterCodes._)
        {
            // Literal ends with underscore - not allowed
            Error(Diagnostics.Numeric_separators_are_not_allowed_here, pos - 1, 1);
        }
        return value.ToString();
    }

    private SyntaxKind CheckBigIntSuffix()
    {
        if (pos < end && text[pos] == CharacterCodes.n)
        {
            tokenValue += "n";
            // Use base 10 instead of base 2 or base 8 for shorter literals
            if ((tokenFlags & TokenFlags.BinaryOrOctalSpecifier) != 0)
            {
                tokenValue = ParsePseudoBigInt(tokenValue) + "n";
            }
            pos++;
            return SyntaxKind.BigIntLiteral;
        }
        else
        {
            // not a bigint, so can convert to number in simplified form
            // Number() may not support 0b or 0o, so use parseInt() instead
            var numericValue = (tokenFlags & TokenFlags.BinarySpecifier) != 0
                ? Convert.ToInt64(tokenValue[2..], 2) // skip "0b"
                : (tokenFlags & TokenFlags.OctalSpecifier) != 0
                ? Convert.ToInt64(tokenValue[2..], 8) // skip "0o"
                : (tokenValue.Length > 1 && tokenValue[1] == 'x')
                ? Convert.ToInt64(tokenValue[2..], 16)
                : double.Parse(tokenValue);
            tokenValue = "" + numericValue.ToString(CultureInfo.InvariantCulture);
            return SyntaxKind.NumericLiteral;
        }
    }

    public SyntaxKind Scan()
    {
        fullStartPos = pos;
        tokenFlags = TokenFlags.None;
        var asteriskSeen = false;
        while (true)
        {
            tokenStart = pos;
            if (pos >= end)
            {
                return token = SyntaxKind.EndOfFileToken;
            }

            var ch = text[pos];
            if (pos == 0)
            {
                // If a file wasn't valid text at all, it will usually be apparent at
                // position 0 because UTF-8 decode will fail and produce U+FFFD.
                // If that happens, just issue one error and refuse to try to Scan further;
                // this is likely a binary file that cannot be parsed
                if (ch == CharacterCodes.ReplacementCharacter)
                {
                    // Jump to the end of the file and fail.
                    Error(Diagnostics.File_appears_to_be_binary);
                    pos = end;
                    return token = SyntaxKind.NonTextFileMarkerTrivia;
                }
                // Special handling for shebang
                if (ch == CharacterCodes.Hash && IsShebangTrivia(text, pos))
                {
                    pos = ScanShebangTrivia(text, pos);
                    if (skipTrivia)
                    {
                        continue;
                    }
                    else
                    {
                        return token = SyntaxKind.ShebangTrivia;
                    }
                }
            }

            switch ((int)ch)
            {
                case CharacterCodes.LineFeed:
                case CharacterCodes.CarriageReturn:
                    tokenFlags |= TokenFlags.PrecedingLineBreak;
                    if (skipTrivia)
                    {
                        pos++;
                        continue;
                    }
                    else
                    {
                        if (ch == CharacterCodes.CarriageReturn && pos + 1 < end && text[pos + 1] == CharacterCodes.LineFeed)
                        {
                            // consume both CR and LF
                            pos += 2;
                        }
                        else
                        {
                            pos++;
                        }
                        return token = SyntaxKind.NewLineTrivia;
                    }

                case CharacterCodes.Tab:
                case CharacterCodes.VerticalTab:
                case CharacterCodes.FormFeed:
                case CharacterCodes.Space:
                case CharacterCodes.NonBreakingSpace:
                case CharacterCodes.Ogham:
                case CharacterCodes.EnQuad:
                case CharacterCodes.EmQuad:
                case CharacterCodes.EnSpace:
                case CharacterCodes.EmSpace:
                case CharacterCodes.ThreePerEmSpace:
                case CharacterCodes.FourPerEmSpace:
                case CharacterCodes.SixPerEmSpace:
                case CharacterCodes.FigureSpace:
                case CharacterCodes.PunctuationSpace:
                case CharacterCodes.ThinSpace:
                case CharacterCodes.HairSpace:
                case CharacterCodes.ZeroWidthSpace:
                case CharacterCodes.NarrowNoBreakSpace:
                case CharacterCodes.MathematicalSpace:
                case CharacterCodes.IdeographicSpace:
                case CharacterCodes.ByteOrderMark:
                    if (skipTrivia)
                    {
                        pos++;
                        continue;
                    }
                    else
                    {
                        while (pos < end && IsWhiteSpaceSingleLine(text[pos]))
                        {
                            pos++;
                        }
                        return token = SyntaxKind.WhitespaceTrivia;
                    }

                case CharacterCodes.Exclamation:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        if (pos + 1 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.ExclamationEqualsEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.ExclamationEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.ExclamationToken;

                case CharacterCodes.DoubleQuote:
                case CharacterCodes.SingleQuote:
                    tokenValue = ScanString();
                    return token = SyntaxKind.StringLiteral;

                case CharacterCodes.Backtick:
                    return token = ScanTemplateAndSetTokenValue(/*shouldEmitInvalidEscapeError*/ false);

                case CharacterCodes.Percent:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.PercentEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.PercentToken;

                case CharacterCodes.Ampersand:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Ampersand)
                    {
                        if (pos + 2 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.AmpersandAmpersandEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.AmpersandAmpersandToken;
                    }
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.AmpersandEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.AmpersandToken;

                case CharacterCodes.OpenParen:
                    pos++;
                    return token = SyntaxKind.OpenParenToken;

                case CharacterCodes.CloseParen:
                    pos++;
                    return token = SyntaxKind.CloseParenToken;

                case CharacterCodes.Asterisk:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.AsteriskEqualsToken;
                    }
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Asterisk)
                    {
                        if (pos + 2 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.AsteriskAsteriskEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.AsteriskAsteriskToken;
                    }
                    pos++;
                    if (inJSDocType > 0 && !asteriskSeen && (tokenFlags & TokenFlags.PrecedingLineBreak) != 0)
                    {
                        // decoration at the start of a JSDoc comment line
                        asteriskSeen = true;
                        continue;
                    }
                    return token = SyntaxKind.AsteriskToken;

                case CharacterCodes.Plus:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Plus)
                    {
                        pos += 2;
                        return token = SyntaxKind.PlusPlusToken;
                    }
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.PlusEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.PlusToken;

                case CharacterCodes.Comma:
                    pos++;
                    return token = SyntaxKind.CommaToken;

                case CharacterCodes.Minus:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Minus)
                    {
                        pos += 2;
                        return token = SyntaxKind.MinusMinusToken;
                    }
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.MinusEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.MinusToken;

                case CharacterCodes.Dot:
                    if (pos + 1 < end && IsDigit(text[pos + 1]))
                    {
                        ScanNumber();
                        return token = SyntaxKind.NumericLiteral;
                    }
                    if (pos + 2 < end && text[pos + 1] == CharacterCodes.Dot && text[pos + 2] == CharacterCodes.Dot)
                    {
                        pos += 3;
                        return token = SyntaxKind.DotDotDotToken;
                    }
                    pos++;
                    return token = SyntaxKind.DotToken;

                case CharacterCodes.Slash:
                    // Single-line comment
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Slash)
                    {
                        pos += 2;

                        while (pos < end)
                        {
                            if (IsLineBreak(text[pos]))
                            {
                                break;
                            }
                            pos++;
                        }

                        commentDirectives = AppendIfCommentDirective(
                            commentDirectives,
                            text[tokenStart..pos],
                            commentDirectiveRegExSingleLine,
                            tokenStart
                        );

                        if (skipTrivia)
                        {
                            continue;
                        }
                        else
                        {
                            return token = SyntaxKind.SingleLineCommentTrivia;
                        }
                    }
                    // Multi-line comment
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Asterisk)
                    {
                        pos += 2;
                        var isJSDoc = pos < end && text[pos] == CharacterCodes.Asterisk && (pos + 1 >= end || text[pos + 1] != CharacterCodes.Slash);

                        var commentClosed = false;
                        var lastLineStart = tokenStart;
                        while (pos < end)
                        {
                            ch = text[pos];

                            if (ch == CharacterCodes.Asterisk && pos + 1 < end && text[pos + 1] == CharacterCodes.Slash)
                            {
                                pos += 2;
                                commentClosed = true;
                                break;
                            }

                            pos++;

                            if (IsLineBreak(ch))
                            {
                                lastLineStart = pos;
                                tokenFlags |= TokenFlags.PrecedingLineBreak;
                            }
                        }

                        if (isJSDoc && ShouldParseJSDoc())
                        {
                            tokenFlags |= TokenFlags.PrecedingJSDocComment;
                        }

                        commentDirectives = AppendIfCommentDirective(commentDirectives,
                            text[lastLineStart..pos],
                            commentDirectiveRegExMultiLine,
                            lastLineStart);

                        if (!commentClosed)
                        {
                            Error(Diagnostics.Asterisk_Slash_expected);
                        }

                        if (skipTrivia)
                        {
                            continue;
                        }
                        else
                        {
                            if (!commentClosed)
                            {
                                tokenFlags |= TokenFlags.Unterminated;
                            }
                            return token = SyntaxKind.MultiLineCommentTrivia;
                        }
                    }

                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.SlashEqualsToken;
                    }

                    pos++;
                    return token = SyntaxKind.SlashToken;

                case CharacterCodes._0:
                    if (pos + 2 < end && (text[pos + 1] == CharacterCodes.X || text[pos + 1] == CharacterCodes.x))
                    {
                        pos += 2;
                        tokenValue = ScanMinimumNumberOfHexDigits(1, canHaveSeparators: true);
                        if (string.IsNullOrEmpty(tokenValue))
                        {
                            Error(Diagnostics.Hexadecimal_digit_expected);
                            tokenValue = "0";
                        }
                        tokenValue = "0x" + tokenValue;
                        tokenFlags |= TokenFlags.HexSpecifier;
                        return token = CheckBigIntSuffix();
                    }
                    else if (pos + 2 < end && (text[pos + 1] == CharacterCodes.B || text[pos + 1] == CharacterCodes.b))
                    {
                        pos += 2;
                        tokenValue = ScanBinaryOrOctalDigits(@base: 2);
                        if (string.IsNullOrEmpty(tokenValue))
                        {
                            Error(Diagnostics.Binary_digit_expected);
                            tokenValue = "0";
                        }
                        tokenValue = "0b" + tokenValue;
                        tokenFlags |= TokenFlags.BinarySpecifier;
                        return token = CheckBigIntSuffix();
                    }
                    else if (pos + 2 < end && (text[pos + 1] == CharacterCodes.O || text[pos + 1] == CharacterCodes.o))
                    {
                        pos += 2;
                        tokenValue = ScanBinaryOrOctalDigits(@base: 8);
                        if (string.IsNullOrEmpty(tokenValue))
                        {
                            Error(Diagnostics.Octal_digit_expected);
                            tokenValue = "0";
                        }
                        tokenValue = "0o" + tokenValue;
                        tokenFlags |= TokenFlags.OctalSpecifier;
                        return token = CheckBigIntSuffix();
                    }
                    goto characterCodes1;

                // falls through
                case CharacterCodes._1:
                case CharacterCodes._2:
                case CharacterCodes._3:
                case CharacterCodes._4:
                case CharacterCodes._5:
                case CharacterCodes._6:
                case CharacterCodes._7:
                case CharacterCodes._8:
                case CharacterCodes._9:
                characterCodes1:
                    return token = ScanNumber();

                case CharacterCodes.Colon:
                    pos++;
                    return token = SyntaxKind.ColonToken;

                case CharacterCodes.Semicolon:
                    pos++;
                    return token = SyntaxKind.SemicolonToken;

                case CharacterCodes.LessThan:
                    if (IsConflictMarkerTrivia(text, pos))
                    {
                        pos = ScanConflictMarkerTrivia(text, pos, Error);
                        if (skipTrivia)
                        {
                            continue;
                        }
                        else
                        {
                            return token = SyntaxKind.ConflictMarkerTrivia;
                        }
                    }

                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.LessThan)
                    {
                        if (pos + 2 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.LessThanLessThanEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.LessThanLessThanToken;
                    }

                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.LessThanEqualsToken;
                    }

                    if (languageVariant == LanguageVariant.JSX &&
                        pos + 1 < end && text[pos + 1] == CharacterCodes.Slash &&
                        (pos + 2 >= end || text[pos + 2] != CharacterCodes.Asterisk))
                    {
                        pos += 2;
                        return token = SyntaxKind.LessThanSlashToken;
                    }
                    pos++;
                    return token = SyntaxKind.LessThanToken;

                case CharacterCodes.Equals:
                    if (IsConflictMarkerTrivia(text, pos))
                    {
                        pos = ScanConflictMarkerTrivia(text, pos, Error);
                        if (skipTrivia)
                        {
                            continue;
                        }
                        else
                        {
                            return token = SyntaxKind.ConflictMarkerTrivia;
                        }
                    }

                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        if (pos + 2 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.EqualsEqualsEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.EqualsEqualsToken;
                    }
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.GreaterThan)
                    {
                        pos += 2;
                        return token = SyntaxKind.EqualsGreaterThanToken;
                    }
                    pos++;
                    return token = SyntaxKind.EqualsToken;

                case CharacterCodes.GreaterThan:
                    if (IsConflictMarkerTrivia(text, pos))
                    {
                        pos = ScanConflictMarkerTrivia(text, pos, Error);
                        if (skipTrivia)
                        {
                            continue;
                        }
                        else
                        {
                            return token = SyntaxKind.ConflictMarkerTrivia;
                        }
                    }

                    pos++;
                    return token = SyntaxKind.GreaterThanToken;

                case CharacterCodes.Question:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Dot && (pos + 2 >= end || !IsDigit(text[pos + 2])))
                    {
                        pos += 2;
                        return token = SyntaxKind.QuestionDotToken;
                    }

                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Question)
                    {
                        if (pos + 2 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.QuestionQuestionEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.QuestionQuestionToken;
                    }

                    pos++;
                    return token = SyntaxKind.QuestionToken;

                case CharacterCodes.OpenBracket:
                    pos++;
                    return token = SyntaxKind.OpenBracketToken;

                case CharacterCodes.CloseBracket:
                    pos++;
                    return token = SyntaxKind.CloseBracketToken;

                case CharacterCodes.Caret:
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.CaretEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.CaretToken;

                case CharacterCodes.OpenBrace:
                    pos++;
                    return token = SyntaxKind.OpenBraceToken;

                case CharacterCodes.Bar:
                    if (IsConflictMarkerTrivia(text, pos))
                    {
                        pos = ScanConflictMarkerTrivia(text, pos, Error);
                        if (skipTrivia)
                        {
                            continue;
                        }
                        else
                        {
                            return token = SyntaxKind.ConflictMarkerTrivia;
                        }
                    }

                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Bar)
                    {
                        if (pos + 2 < end && text[pos + 2] == CharacterCodes.Equals)
                        {
                            pos += 3;
                            return token = SyntaxKind.BarBarEqualsToken;
                        }
                        pos += 2;
                        return token = SyntaxKind.BarBarToken;
                    }
                    if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                    {
                        pos += 2;
                        return token = SyntaxKind.BarEqualsToken;
                    }
                    pos++;
                    return token = SyntaxKind.BarToken;

                case CharacterCodes.CloseBrace:
                    pos++;
                    return token = SyntaxKind.CloseBraceToken;

                case CharacterCodes.Tilde:
                    pos++;
                    return token = SyntaxKind.TildeToken;

                case CharacterCodes.At:
                    pos++;
                    return token = SyntaxKind.AtToken;

                case CharacterCodes.Backslash:
                    var extendedCookedChar = PeekExtendedUnicodeEscape();
                    if (extendedCookedChar >= 0 && IsIdentifierStart(extendedCookedChar, languageVersion))
                    {
                        pos += 3;
                        tokenFlags |= TokenFlags.ExtendedUnicodeEscape;
                        tokenValue = ScanExtendedUnicodeEscape() + ScanIdentifierParts();
                        return token = GetIdentifierToken();
                    }

                    var cookedChar = PeekUnicodeEscape();
                    if (cookedChar >= 0 && IsIdentifierStart(cookedChar, languageVersion))
                    {
                        pos += 6;
                        tokenFlags |= TokenFlags.UnicodeEscape;
                        tokenValue = (char)cookedChar + ScanIdentifierParts();
                        return token = GetIdentifierToken();
                    }

                    Error(Diagnostics.Invalid_character);
                    pos++;
                    return token = SyntaxKind.Unknown;

                case CharacterCodes.Hash:
                    if (pos != 0 && pos + 1 < end && text[pos + 1] == CharacterCodes.Exclamation)
                    {
                        Error(Diagnostics.can_only_be_used_at_the_start_of_a_file);
                        pos++;
                        return token = SyntaxKind.Unknown;
                    }

                    var charAfterHash = pos + 1 < end ? text[pos + 1] : '\0';
                    if (charAfterHash == CharacterCodes.Backslash)
                    {
                        pos++;
                        extendedCookedChar = PeekExtendedUnicodeEscape();
                        if (extendedCookedChar >= 0 && IsIdentifierStart((char)extendedCookedChar, languageVersion))
                        {
                            pos += 3;
                            tokenFlags |= TokenFlags.ExtendedUnicodeEscape;
                            tokenValue = "#" + ScanExtendedUnicodeEscape() + ScanIdentifierParts();
                            return token = SyntaxKind.PrivateIdentifier;
                        }

                        cookedChar = PeekUnicodeEscape();
                        if (cookedChar >= 0 && IsIdentifierStart(cookedChar, languageVersion))
                        {
                            pos += 6;
                            tokenFlags |= TokenFlags.UnicodeEscape;
                            tokenValue = "#" + (char)cookedChar + ScanIdentifierParts();
                            return token = SyntaxKind.PrivateIdentifier;
                        }
                        pos--;
                    }

                    if (IsIdentifierStart(charAfterHash, languageVersion))
                    {
                        pos++;
                        // We're relying on ScanIdentifier's behavior and adjusting the token kind after the fact.
                        // Notably absent from this block is the fact that calling a function named "scanIdentifier",
                        // but identifiers don't include '#', and that function doesn't deal with it at all.
                        // This works because 'scanIdentifier' tries to reuse source characters and builds up substrings;
                        // however, it starts at the 'tokenPos' which includes the '#', and will "accidentally" prepend the '#' for us.
                        ScanIdentifier(charAfterHash, languageVersion);
                    }
                    else
                    {
                        tokenValue = "#";
                        Error(Diagnostics.Invalid_character, pos++, CharSize(ch));
                    }
                    return token = SyntaxKind.PrivateIdentifier;

                default:
                    var identifierKind = ScanIdentifier(ch, languageVersion);
                    if (identifierKind != SyntaxKind.Unknown)
                    {
                        return token = identifierKind;
                    }
                    else if (IsWhiteSpaceSingleLine(ch))
                    {
                        pos += CharSize(ch);
                        continue;
                    }
                    else if (IsLineBreak(ch))
                    {
                        tokenFlags |= TokenFlags.PrecedingLineBreak;
                        pos += CharSize(ch);
                        continue;
                    }
                    var size = CharSize(ch);
                    Error(Diagnostics.Invalid_character, pos, size);
                    pos += size;
                    return token = SyntaxKind.Unknown;
            }
        }
    }

    private bool ShouldParseJSDoc()
    {
        switch (jsDocParsingMode)
        {
            case JSDocParsingMode.ParseAll:
                return true;
            case JSDocParsingMode.ParseNone:
                return false;
        }

        if (scriptKind != ScriptKind.TS && scriptKind != ScriptKind.TSX)
        {
            // If outside of TS, we need JSDoc to get any type info.
            return true;
        }

        if (jsDocParsingMode == JSDocParsingMode.ParseForTypeInfo)
        {
            // If we're in TS, but we don't need to produce reliable errors,
            // we don't need to parse to find @see or @link.
            return false;
        }

        return jsDocSeeOrLink.IsMatch(text[fullStartPos..pos]);
    }

    public SyntaxKind ReScanInvalidIdentifier()
    {
        Debug.Assert(token == SyntaxKind.Unknown, "'reScanInvalidIdentifier' should only be called when the current token is 'SyntaxKind.Unknown'.");
        pos = tokenStart = fullStartPos;
        tokenFlags = 0;
        var ch = text[pos];
        var identifierKind = ScanIdentifier(ch, ScriptTarget.ESNext);
        if (identifierKind != SyntaxKind.Unknown)
        {
            return token = identifierKind;
        }
        pos += CharSize(ch);
        return token; // Still `SyntaxKind.Unknown`
    }

    private SyntaxKind ScanIdentifier(int startCharacter, ScriptTarget languageVersion)
    {
        var ch = startCharacter;
        if (IsIdentifierStart(ch, languageVersion))
        {
            pos += CharSize(ch);
            while (pos < end && IsIdentifierPart(ch = text[pos], languageVersion)) pos += CharSize(ch);
            tokenValue = text[tokenStart..pos];
            if (ch == CharacterCodes.Backslash)
            {
                tokenValue += ScanIdentifierParts();
            }
            return GetIdentifierToken();
        }
        return SyntaxKind.Unknown;
    }

    public SyntaxKind ReScanGreaterToken()
    {
        if (token == SyntaxKind.GreaterThanToken)
        {
            if (pos < end && text[pos] == CharacterCodes.GreaterThan)
            {
                if (pos + 1 < end && text[pos + 1] == CharacterCodes.GreaterThan)
                {
                    if (text[pos + 2] == CharacterCodes.Equals)
                    {
                        pos += 3;
                        return token = SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken;
                    }
                    pos += 2;
                    return token = SyntaxKind.GreaterThanGreaterThanGreaterThanToken;
                }
                if (pos + 1 < end && text[pos + 1] == CharacterCodes.Equals)
                {
                    pos += 2;
                    return token = SyntaxKind.GreaterThanGreaterThanEqualsToken;
                }
                pos++;
                return token = SyntaxKind.GreaterThanGreaterThanToken;
            }
            if (pos < end && text[pos] == CharacterCodes.Equals)
            {
                pos++;
                return token = SyntaxKind.GreaterThanEqualsToken;
            }
        }
        return token;
    }

    public SyntaxKind ReScanAsteriskEqualsToken()
    {
        Debug.Assert(token == SyntaxKind.AsteriskEqualsToken, "'reScanAsteriskEqualsToken' should only be called on a '*='");
        pos = tokenStart + 1;
        return token = SyntaxKind.EqualsToken;
    }

    public SyntaxKind ReScanSlashToken()
    {
        if (token == SyntaxKind.SlashToken || token == SyntaxKind.SlashEqualsToken)
        {
            var p = tokenStart + 1;
            var inEscape = false;
            var inCharacterClass = false;
            while (true)
            {
                // If we reach the end of a file, or hit a newline, then this is an unterminated
                // regex.  Report error and return what we have so far.
                if (p >= end)
                {
                    tokenFlags |= TokenFlags.Unterminated;
                    Error(Diagnostics.Unterminated_regular_expression_literal);
                    break;
                }

                var ch = text[p];
                if (IsLineBreak(ch))
                {
                    tokenFlags |= TokenFlags.Unterminated;
                    Error(Diagnostics.Unterminated_regular_expression_literal);
                    break;
                }

                if (inEscape)
                {
                    // Parsing an escape character;
                    // reset the flag and just advance to the next char.
                    inEscape = false;
                }
                else if (ch == CharacterCodes.Slash && !inCharacterClass)
                {
                    // A slash within a character class is permissible,
                    // but in general it signals the end of the regexp literal.
                    p++;
                    break;
                }
                else if (ch == CharacterCodes.OpenBracket)
                {
                    inCharacterClass = true;
                }
                else if (ch == CharacterCodes.Backslash)
                {
                    inEscape = true;
                }
                else if (ch == CharacterCodes.CloseBracket)
                {
                    inCharacterClass = false;
                }
                p++;
            }

            while (p < end && IsIdentifierPart(text[p], languageVersion))
            {
                p++;
            }
            pos = p;
            tokenValue = text[tokenStart..pos];
            token = SyntaxKind.RegularExpressionLiteral;
        }
        return token;
    }

    // Unconditionally back up and Scan a template expression portion.
    public SyntaxKind ReScanTemplateToken(bool isTaggedTemplate)
    {
        pos = tokenStart;
        token = ScanTemplateAndSetTokenValue(!!isTaggedTemplate);
        return token;
    }

    public SyntaxKind ReScanJsxToken(bool allowMultilineJsxText = true)
    {
        pos = tokenStart = fullStartPos;
        token = ScanJsxToken(allowMultilineJsxText);
        return token;
    }

    public SyntaxKind ReScanLessThanToken()
    {
        if (token == SyntaxKind.LessThanLessThanToken)
        {
            pos = tokenStart + 1;
            return token = SyntaxKind.LessThanToken;
        }
        return token;
    }

    public SyntaxKind ReScanHashToken()
    {
        if (token == SyntaxKind.PrivateIdentifier)
        {
            pos = tokenStart + 1;
            return token = SyntaxKind.HashToken;
        }
        return token;
    }

    public SyntaxKind ReScanQuestionToken()
    {
        Debug.Assert(token == SyntaxKind.QuestionQuestionToken, "'reScanQuestionToken' should only be called on a '??'");
        pos = tokenStart + 1;
        return token = SyntaxKind.QuestionToken;
    }

    public SyntaxKind ScanJsxToken(bool allowMultilineJsxText = true)
    {
        fullStartPos = tokenStart = pos;

        if (pos >= end)
        {
            return token = SyntaxKind.EndOfFileToken;
        }

        var ch = text[pos];
        if (ch == CharacterCodes.LessThan)
        {
            if (pos + 1 < end && text[pos + 1] == CharacterCodes.Slash)
            {
                pos += 2;
                return token = SyntaxKind.LessThanSlashToken;
            }
            pos++;
            return token = SyntaxKind.LessThanToken;
        }

        if (ch == CharacterCodes.OpenBrace)
        {
            pos++;
            return token = SyntaxKind.OpenBraceToken;
        }

        // First non-whitespace character on this line.
        var firstNonWhitespace = 0;

        // These initial values are special because the first line is:
        // firstNonWhitespace = 0 to indicate that we want leading whitespace,

        while (pos < end)
        {
            ch = text[pos];
            if (ch == CharacterCodes.OpenBrace)
            {
                break;
            }
            if (ch == CharacterCodes.LessThan)
            {
                if (IsConflictMarkerTrivia(text, pos))
                {
                    pos = ScanConflictMarkerTrivia(text, pos, Error);
                    return token = SyntaxKind.ConflictMarkerTrivia;
                }
                break;
            }
            if (ch == CharacterCodes.GreaterThan)
            {
                Error(Diagnostics.Unexpected_token_Did_you_mean_or_gt, pos, 1);
            }
            if (ch == CharacterCodes.CloseBrace)
            {
                Error(Diagnostics.Unexpected_token_Did_you_mean_or_rbrace, pos, 1);
            }

            // FirstNonWhitespace is 0, then we only see whitespaces so far. If we see a linebreak, we want to ignore that whitespaces.
            // i.e (- : whitespace)
            //      <div>----
            //      </div> becomes <div></div>
            //
            //      <div>----</div> becomes <div>----</div>
            if (IsLineBreak(ch) && firstNonWhitespace == 0)
            {
                firstNonWhitespace = -1;
            }
            else if (!allowMultilineJsxText && IsLineBreak(ch) && firstNonWhitespace > 0)
            {
                // Stop JsxText on each line during formatting. This allows the formatter to
                // indent each line correctly.
                break;
            }
            else if (!IsWhiteSpaceLike(ch))
            {
                firstNonWhitespace = pos;
            }

            pos++;
        }

        tokenValue = text[fullStartPos..pos];

        return firstNonWhitespace == -1 ? SyntaxKind.JsxTextAllWhiteSpaces : SyntaxKind.JsxText;
    }

    // Scans a JSX identifier; these differ from normal identifiers in that
    // they allow dashes
    internal SyntaxKind ScanJsxIdentifier()
    {
        if (TokenIsIdentifierOrKeyword(token))
        {
            // An identifier or keyword has already been parsed - Check for a `-` or a single instance of `:` and then append it and
            // everything after it to the token
            // Do note that this means that `scanJsxIdentifier` effectively _mutates_ the visible token without advancing to a new token
            // Any caller should be expecting this behavior and should only read the pos or token value after calling it.
            while (pos < end)
            {
                var ch = text[pos];
                if (ch == CharacterCodes.Minus)
                {
                    tokenValue += "-";
                    pos++;
                    continue;
                }
                var oldPos = pos;
                tokenValue += ScanIdentifierParts(); // reuse `scanIdentifierParts` so unicode escapes are handled
                if (pos == oldPos)
                {
                    break;
                }
            }
            return GetIdentifierToken();
        }
        return token;
    }

    public SyntaxKind ScanJsxAttributeValue()
    {
        fullStartPos = pos;

        switch ((int)text[pos])
        {
            case CharacterCodes.DoubleQuote:
            case CharacterCodes.SingleQuote:
                tokenValue = ScanString(/*jsxAttributeString*/ true);
                return token = SyntaxKind.StringLiteral;
            default:
                // If this Scans anything other than `{`, it's a parse error.
                return Scan();
        }
    }

    public SyntaxKind ReScanJsxAttributeValue()
    {
        pos = tokenStart = fullStartPos;
        return ScanJsxAttributeValue();
    }

    internal SyntaxKind ScanJSDocCommentTextToken(bool inBackticks)
    {
        fullStartPos = tokenStart = pos;
        tokenFlags = TokenFlags.None;
        if (pos >= end)
        {
            return token = SyntaxKind.EndOfFileToken;
        }
        for (var ch = text[pos]; pos < end && (!IsLineBreak(ch) && ch != CharacterCodes.Backtick); ch = ++pos < end ? text[pos] : '\0')
        {
            if (!inBackticks)
            {
                if (ch == CharacterCodes.OpenBrace)
                {
                    break;
                }
                else if (
                    ch == CharacterCodes.At
                    && pos - 1 >= 0 && IsWhiteSpaceSingleLine(text[pos - 1])
                    && !(pos + 1 < end && IsWhiteSpaceLike(text[pos + 1]))
                )
                {
                    // @ doesn't start a new tag inside ``, and elsewhere, only after whitespace and before non-whitespace
                    break;
                }
            }
        }
        if (pos == tokenStart)
        {
            return ScanJsDocToken();
        }
        tokenValue = text[tokenStart..pos];
        return token = SyntaxKind.JSDocCommentTextToken;
    }

    internal SyntaxKind ScanJsDocToken()
    {
        fullStartPos = tokenStart = pos;
        tokenFlags = TokenFlags.None;
        if (pos >= end)
        {
            return token = SyntaxKind.EndOfFileToken;
        }

        var ch = text[pos];
        pos += CharSize(ch);
        switch ((int)ch)
        {
            case CharacterCodes.Tab:
            case CharacterCodes.VerticalTab:
            case CharacterCodes.FormFeed:
            case CharacterCodes.Space:
                while (pos < end && IsWhiteSpaceSingleLine(text[pos]))
                {
                    pos++;
                }
                return token = SyntaxKind.WhitespaceTrivia;
            case CharacterCodes.At:
                return token = SyntaxKind.AtToken;
            case CharacterCodes.CarriageReturn:
                if (text[pos] == CharacterCodes.LineFeed)
                {
                    pos++;
                }
                tokenFlags |= TokenFlags.PrecedingLineBreak;
                return token = SyntaxKind.NewLineTrivia;

            case CharacterCodes.LineFeed:
                tokenFlags |= TokenFlags.PrecedingLineBreak;
                return token = SyntaxKind.NewLineTrivia;

            case CharacterCodes.Asterisk:
                return token = SyntaxKind.AsteriskToken;

            case CharacterCodes.OpenBrace:
                return token = SyntaxKind.OpenBraceToken;

            case CharacterCodes.CloseBrace:
                return token = SyntaxKind.CloseBraceToken;

            case CharacterCodes.OpenBracket:
                return token = SyntaxKind.OpenBracketToken;

            case CharacterCodes.CloseBracket:
                return token = SyntaxKind.CloseBracketToken;

            case CharacterCodes.LessThan:
                return token = SyntaxKind.LessThanToken;

            case CharacterCodes.GreaterThan:
                return token = SyntaxKind.GreaterThanToken;

            case CharacterCodes.Equals:
                return token = SyntaxKind.EqualsToken;

            case CharacterCodes.Comma:
                return token = SyntaxKind.CommaToken;

            case CharacterCodes.Dot:
                return token = SyntaxKind.DotToken;

            case CharacterCodes.Backtick:
                return token = SyntaxKind.BacktickToken;

            case CharacterCodes.Hash:
                return token = SyntaxKind.HashToken;

            case CharacterCodes.Backslash:
                pos--;
                var extendedCookedChar = PeekExtendedUnicodeEscape();
                if (extendedCookedChar >= 0 && IsIdentifierStart(extendedCookedChar, languageVersion))
                {
                    pos += 3;
                    tokenFlags |= TokenFlags.ExtendedUnicodeEscape;
                    tokenValue = ScanExtendedUnicodeEscape() + ScanIdentifierParts();
                    return token = GetIdentifierToken();
                }

                var cookedChar = PeekUnicodeEscape();
                if (cookedChar >= 0 && IsIdentifierStart(cookedChar, languageVersion))
                {
                    pos += 6;
                    tokenFlags |= TokenFlags.UnicodeEscape;
                    tokenValue = (char)cookedChar + ScanIdentifierParts();
                    return token = GetIdentifierToken();
                }
                pos++;
                return token = SyntaxKind.Unknown;
        }

        if (IsIdentifierStart(ch, languageVersion))
        {
            var ch2 = ch;
            while (pos < end && IsIdentifierPart(ch2 = text[pos], languageVersion) || text[pos] == CharacterCodes.Minus)
                pos += CharSize(ch2);
            tokenValue = text[tokenStart..pos];
            if (ch2 == CharacterCodes.Backslash)
            {
                tokenValue += ScanIdentifierParts();
            }
            return token = GetIdentifierToken();
        }
        else
        {
            return token = SyntaxKind.Unknown;
        }
    }

    internal static bool IsFalsy(object obj)
    {
        return obj is null or false or 0 or "" or SyntaxKind.Unknown ||
            (obj is not true and not string && int.TryParse(obj.ToString(), out int i) && i == 0);
    }

    private T SpeculationHelper<T>(Func<T> callback, bool isLookahead)
    {
        var savePos = pos;
        var saveStartPos = fullStartPos;
        var saveTokenPos = tokenStart;
        var saveToken = token;
        var saveTokenValue = tokenValue;
        var saveTokenFlags = tokenFlags;
        var result = callback();

        // If our callback returned something 'falsy' or we're just looking ahead,
        // then unconditionally restore us to where we were.
        if (isLookahead || IsFalsy(result))
        {
            pos = savePos;
            fullStartPos = saveStartPos;
            tokenStart = saveTokenPos;
            token = saveToken;
            tokenValue = saveTokenValue;
            tokenFlags = saveTokenFlags;
        }
        return result;
    }

    public T LookAhead<T>(Func<T> callback)
    {
        return SpeculationHelper<T>(callback, isLookahead: true);
    }

    public T TryScan<T>(Func<T> callback)
    {
        return SpeculationHelper<T>(callback, isLookahead: false);
    }

    public string GetText() => text;

    internal void ClearCommentDirectives()
    {
        commentDirectives = null;
    }

    public void SetOnError(ErrorCallback errorCallback)
    {
        onError = errorCallback;
    }

    public void SetText(string newText, int? start = null, int? length = null)
    {
        text = newText ?? "";
        end = length == null ? text.Length : (start ?? 0) + (length ?? 0);
        ResetTokenState(start ?? 0);
    }

    public void SetScriptTarget(ScriptTarget scriptTarget)
    {
        languageVersion = scriptTarget;
    }

    public void SetLanguageVariant(LanguageVariant variant)
    {
        languageVariant = variant;
    }

    public void SetScriptKind(ScriptKind kind)
    {
        scriptKind = kind;
    }

    public void SetJSDocParsingMode(JSDocParsingMode kind)
    {
        jsDocParsingMode = kind;
    }

    public void ResetTokenState(int position)
    {
        Debug.Assert(position >= 0);
        pos = position;
        fullStartPos = position;
        tokenStart = position;
        token = SyntaxKind.Unknown;
        tokenValue = null;
        tokenFlags = TokenFlags.None;
    }

    public void SetInJSDocType(bool inType)
    {
        inJSDocType += inType ? 1 : -1;
    }
}
