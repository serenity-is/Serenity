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
    private int skipJsDocLeadingAsterisks;

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
    internal bool HasPrecedingJSDocLeadingAsterisks() => (tokenFlags & TokenFlags.PrecedingJSDocLeadingAsterisks) != 0;
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
                ScanEscapeSequence(result, EscapeSequenceScanningFlags.String | EscapeSequenceScanningFlags.ReportErrors);
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
                ScanEscapeSequence(contents, EscapeSequenceScanningFlags.String | (shouldEmitInvalidEscapeError ? EscapeSequenceScanningFlags.ReportErrors : 0));
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
    private void ScanEscapeSequence(StringBuilder sb, EscapeSequenceScanningFlags flags)
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
                if (flags.HasFlag(EscapeSequenceScanningFlags.ReportInvalidEscapeErrors))
                {
                    var code = Convert.ToUInt32(text[(start + 1)..pos], 8);
                    if (flags.HasFlag(EscapeSequenceScanningFlags.RegularExpression) &&
                        !flags.HasFlag(EscapeSequenceScanningFlags.AtomEscape) &&
                        ch != CharacterCodes._0)
                    {
                        Error(Diagnostics.Octal_escape_sequences_and_backreferences_are_not_allowed_in_a_character_class_If_this_was_intended_as_an_escape_sequence_use_the_syntax_0_instead,
                            start, pos - start, "\\x" + Convert.ToString(code, 16).PadLeft(2, '0'));
                    }
                    else
                    {
                        Error(Diagnostics.Octal_escape_sequences_are_not_allowed_Use_the_syntax_0, start, pos - start,
                            "\\x" + Convert.ToString(code, 16).PadLeft(2, '0'));
                    }
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
                if (flags.HasFlag(EscapeSequenceScanningFlags.ReportInvalidEscapeErrors))
                {
                    if (flags.HasFlag(EscapeSequenceScanningFlags.RegularExpression) &&
                        !flags.HasFlag(EscapeSequenceScanningFlags.AtomEscape))
                    {
                        Error(Diagnostics.Decimal_escape_sequences_and_backreferences_are_not_allowed_in_a_character_class, start, pos - start);
                    }
                    else
                    {
                        Error(Diagnostics.Escape_sequence_0_is_not_allowed, start, pos - start, text[start..pos]);
                    }
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
                    // '\u{DDDDDD}'
                    pos -= 2;
                    var result = ScanExtendedUnicodeEscape(flags.HasFlag(EscapeSequenceScanningFlags.ReportInvalidEscapeErrors));
                    if (!flags.HasFlag(EscapeSequenceScanningFlags.AllowExtendedUnicodeEscape))
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (flags.HasFlag(EscapeSequenceScanningFlags.ReportInvalidEscapeErrors))
                        {
                            Error(Diagnostics.Unicode_escape_sequences_are_only_available_when_the_Unicode_u_flag_or_the_Unicode_Sets_v_flag_is_set, start, pos - start);
                        }
                    }
                    sb.Append(result);
                    return;
                }
                // '\uDDDD'
                for (; pos < start + 6; pos++)
                {
                    if (!(pos < end && IsHexDigit(text[pos])))
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (flags.HasFlag(EscapeSequenceScanningFlags.ReportInvalidEscapeErrors))
                        {
                            Error(Diagnostics.Hexadecimal_digit_expected);
                        }
                        sb.Append(text[start..pos]);
                        return;
                    }
                }
                tokenFlags |= TokenFlags.UnicodeEscape;
                var escapedValue = Convert.ToUInt32(text[(start + 2)..pos], 16);
                var escapedValueString = (char)escapedValue;
                if (flags.HasFlag(EscapeSequenceScanningFlags.AnyUnicodeMode) &&
                    escapedValue >= 0xD800 && escapedValue <= 0xDBFF &&
                    pos + 6 < end && text[pos..(pos + 2)] == "\\u" &&
                    text[pos + 2] != CharacterCodes.OpenBrace)
                {
                    // For regular expressions in any Unicode mode, \u HexLeadSurrogate \u HexTrailSurrogate is treated as a single character
                    // for the purpose of determining whether a character class range is out of order
                    // https://tc39.es/ecma262/#prod-RegExpUnicodeEscapeSequence
                    var nextStart = pos;
                    var nextPos = pos + 2;
                    for (; nextPos < nextStart + 6; nextPos++)
                    {
                        if (!IsHexDigit(text[nextPos]))
                        {
                            // leave the error to the next call
                            sb.Append(escapedValueString);
                            return;
                        }
                    }
                    var nextEscapedValue = Convert.ToUInt32(text[(nextStart + 2)..nextPos], 16);
                    if (nextEscapedValue >= 0xDC00 && nextEscapedValue <= 0xDFFF)
                    {
                        pos = nextPos;
                        sb.Append(escapedValueString + (char)nextEscapedValue);
                        return;
                    }
                }
                sb.Append(escapedValueString);
                return;

            case CharacterCodes.x:
                // '\xDD'
                for (; pos < start + 4; pos++)
                {
                    if (!(pos < end && IsHexDigit(text[pos])))
                    {
                        tokenFlags |= TokenFlags.ContainsInvalidEscape;
                        if (flags.HasFlag(EscapeSequenceScanningFlags.ReportInvalidEscapeErrors))
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
                if (flags.HasFlag(EscapeSequenceScanningFlags.AnyUnicodeMode)
                    || flags.HasFlag(EscapeSequenceScanningFlags.RegularExpression)
                        && !(flags.HasFlag(EscapeSequenceScanningFlags.AnnexB))
                        && IsIdentifierPart(ch, languageVersion))
                {
                    Error(Diagnostics.This_character_cannot_be_escaped_in_a_regular_expression, pos - 2, 2);
                }
                sb.Append(ch);
                return;
        }
    }
    private string ScanExtendedUnicodeEscape(bool shouldEmitInvalidEscapeError)
    {
        var start = pos;
        pos += 3;
        var escapedStart = pos;
        var escapedValueString = ScanMinimumNumberOfHexDigits(1, /*canHaveSeparators*/ false);
        var escapedValue = !string.IsNullOrEmpty(escapedValueString) ? Convert.ToInt32(escapedValueString, 16) : -1;
        var isInvalidExtendedEscape = false;

        // Validate the value of the digit
        if (escapedValue < 0)
        {
            if (shouldEmitInvalidEscapeError)
                Error(Diagnostics.Hexadecimal_digit_expected);
            isInvalidExtendedEscape = true;
        }
        else if (escapedValue > 0x10FFFF)
        {
            if (shouldEmitInvalidEscapeError)
                Error(Diagnostics.An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive, escapedStart, pos - escapedStart);
            isInvalidExtendedEscape = true;
        }

        if (pos >= end)
        {
            if (shouldEmitInvalidEscapeError)
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
            if (shouldEmitInvalidEscapeError)
                Error(Diagnostics.Unterminated_Unicode_escape_sequence);
            isInvalidExtendedEscape = true;
        }

        if (isInvalidExtendedEscape)
        {
            tokenFlags |= TokenFlags.ContainsInvalidEscape;
            return text[start..pos];
        }

        tokenFlags |= TokenFlags.ExtendedUnicodeEscape;
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
        if (CodePointUnchecked(pos + 1) == CharacterCodes.u && 
            CodePointUnchecked(pos + 2) == CharacterCodes.OpenBrace)
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
            var ch = CodePointUnchecked(pos);
            if (IsIdentifierPart(ch, languageVersion))
            {
                pos++;
            }
            else if (ch == CharacterCodes.Backslash)
            {
                ch = PeekExtendedUnicodeEscape();
                if (ch >= 0 && IsIdentifierPart(ch, languageVersion))
                {
                    result.Append(ScanExtendedUnicodeEscape(shouldEmitInvalidEscapeError: true));
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
        while (true)
        {
            tokenStart = pos;
            if (pos >= end)
            {
                return token = SyntaxKind.EndOfFileToken;
            }

            var ch = CodePointUnchecked(pos);
            if (pos == 0)
            {
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
                    if (skipJsDocLeadingAsterisks > 0 &&
                        !tokenFlags.HasFlag(TokenFlags.PrecedingJSDocLeadingAsterisks) &&
                        tokenFlags.HasFlag(TokenFlags.PrecedingLineBreak))
                    {
                        // decoration at the start of a JSDoc comment line
                        tokenFlags |= TokenFlags.PrecedingJSDocLeadingAsterisks;
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
                        tokenValue = ScanExtendedUnicodeEscape(shouldEmitInvalidEscapeError: true) + ScanIdentifierParts();
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
                        Error(Diagnostics.can_only_be_used_at_the_start_of_a_file, pos, 2);
                        pos++;
                        return token = SyntaxKind.Unknown;
                    }

                    var charAfterHash = CodePointUnchecked(pos + 1);
                    if (charAfterHash == CharacterCodes.Backslash)
                    {
                        pos++;
                        extendedCookedChar = PeekExtendedUnicodeEscape();
                        if (extendedCookedChar >= 0 && IsIdentifierStart((char)extendedCookedChar, languageVersion))
                        {
                            tokenValue = "#" + ScanExtendedUnicodeEscape(shouldEmitInvalidEscapeError: true) + ScanIdentifierParts();
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

                case CharacterCodes.ReplacementCharacter:
                    Error(Diagnostics.File_appears_to_be_binary, 0, 0);
                    pos = end;
                    return token = SyntaxKind.NonTextFileMarkerTrivia;

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
        var ch = CodePointUnchecked(pos);
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
            while (pos < end && IsIdentifierPart(ch = CodePointUnchecked(pos), languageVersion)) pos += CharSize(ch);
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

    /**
    * Returns the code point for the character at the given position within `text`. This
    * should only be used when pos is guaranteed to be within the bounds of `text` as this
    * function does not perform bounds checks.
    */
    int CodePointUnchecked(int pos)
    {
        return CodePointAt(text, pos);
    }

    /**
     * Returns the code point for the character at the given position within `text`. If
     * `pos` is outside the bounds set for `text`, `CharacterCodes.EOF` is returned instead.
     */
    int CodePointChecked(int pos)
    {
        return pos >= 0 && pos < end ? CodePointAt(text, pos) : CharacterCodes.EOF;
    }

    /**
     * Returns the char code for the character at the given position within `text`. This
     * should only be used when pos is guaranteed to be within the bounds of `text` as this
     * function does not perform bounds checks.
     */
    char CharCodeUnchecked(int pos)
    {
        return text[pos];
    }

    /**
     * Returns the char code for the character at the given position within `text`. If
     * `pos` is outside the bounds set for `text`, `CharacterCodes.EOF` is returned instead.
     */
    char CharCodeChecked(int pos)
    {
        if (pos >= 0 && pos < end)
            return text[pos];
        unchecked
        {
            return (char)(short)-1;
        }
    }

    public SyntaxKind ReScanSlashToken(bool reportErrors)
    {
        if (token == SyntaxKind.SlashToken || token == SyntaxKind.SlashEqualsToken)
        {
            // Quickly get to the end of regex such that we know the flags
            var startOfRegExpBody = tokenStart + 1;
            pos = startOfRegExpBody;
            var inEscape = false;
            var namedCaptureGroups = false;
            // Although nested character classes are allowed in Unicode Sets mode,
            // an unescaped slash is nevertheless invalid even in a character class in any Unicode mode.
            // This is indicated by Section 12.9.5 Regular Expression Literals of the specification,
            // where nested character classes are not considered at all. (A `[` RegularExpressionClassChar
            // does nothing in a RegularExpressionClass, and a `]` always closes the class.)
            // Additionally, parsing nested character classes will misinterpret regexes like `/[[]/`
            // as unterminated, consuming characters beyond the slash. (This even applies to `/[[]/v`,
            // which should be parsed as a well-terminated regex with an incomplete character class.)
            // Thus we must not handle nested character classes in the first pass.
            var inCharacterClass = false;
            while (true)
            {
                // If we reach the end of a file, or hit a newline, then this is an unterminated
                // regex.  Report error and return what we have so far.
                char ch;
                if (pos >= text.Length || IsLineBreak(ch = text[pos]))
                {
                    tokenFlags |= TokenFlags.Unterminated;
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
                else if (!inCharacterClass
                   && ch == CharacterCodes.OpenParen
                   && pos < text.Length - 2
                   && text[pos + 1] == CharacterCodes.Question
                   && text[pos + 2] == CharacterCodes.LessThan
                   && (pos >= text.Length - 3 ||
                       (text[pos + 3] != CharacterCodes.Equals
                        && text[pos + 3] != CharacterCodes.Exclamation))
                )
                {
                    namedCaptureGroups = true;
                }
                pos++;
            }

            var endOfRegExpBody = pos;
            if (tokenFlags.HasFlag(TokenFlags.Unterminated))
            {
                // Search for the nearest unbalanced bracket for better recovery. Since the expression is
                // invalid anyways, we take nested square brackets into consideration for the best guess.
                pos = startOfRegExpBody;
                inEscape = false;
                var characterClassDepth = 0;
                var inDecimalQuantifier = false;
                var groupDepth = 0;
                while (pos < endOfRegExpBody)
                {
                    var ch = text[pos];
                    if (inEscape)
                    {
                        inEscape = false;
                    }
                    else if (ch == CharacterCodes.Backslash)
                    {
                        inEscape = true;
                    }
                    else if (ch == CharacterCodes.OpenBracket)
                    {
                        characterClassDepth++;
                    }
                    else if (ch == CharacterCodes.CloseBracket && characterClassDepth != 0)
                    {
                        characterClassDepth--;
                    }
                    else if (characterClassDepth == 0)
                    {
                        if (ch == CharacterCodes.OpenBrace)
                        {
                            inDecimalQuantifier = true;
                        }
                        else if (ch == CharacterCodes.CloseBrace && inDecimalQuantifier)
                        {
                            inDecimalQuantifier = false;
                        }
                        else if (!inDecimalQuantifier)
                        {
                            if (ch == CharacterCodes.OpenParen)
                            {
                                groupDepth++;
                            }
                            else if (ch == CharacterCodes.CloseParen && groupDepth != 0)
                            {
                                groupDepth--;
                            }
                            else if (ch == CharacterCodes.CloseParen || ch == CharacterCodes.CloseBracket || ch == CharacterCodes.CloseBrace)
                            {
                                // We encountered an unbalanced bracket outside a character class. Treat this position as the end of regex.
                                break;
                            }
                        }
                    }
                    pos++;
                }
                // Whitespaces and semicolons at the end are not likely to be part of the regex
                while (IsWhiteSpaceLike(CharCodeChecked(pos - 1)) || CharCodeChecked(pos - 1) == CharacterCodes.Semicolon) pos--;
                Error(Diagnostics.Unterminated_regular_expression_literal, tokenStart, pos - tokenStart);
            }
            else
            {
                // Consume the slash character
                pos++;
                var regExpFlags = RegularExpressionFlags.None;
                while (true)
                {
                    var ch = CodePointChecked(pos);
                    if (ch == CharacterCodes.EOF || !IsIdentifierPart(ch, languageVersion))
                    {
                        break;
                    }
                    var size = CharSize(ch);
                    if (reportErrors)
                    {
                        var flag = CharacterCodeToRegularExpressionFlag(ch);
                        if (flag == null)
                        {
                            Error(Diagnostics.Unknown_regular_expression_flag, pos, size);
                        }
                        else if (regExpFlags.HasFlag(flag.Value))
                        {
                            Error(Diagnostics.Duplicate_regular_expression_flag, pos, size);
                        }
                        else if (((regExpFlags | flag) & RegularExpressionFlags.AnyUnicodeMode) == RegularExpressionFlags.AnyUnicodeMode)
                        {
                            Error(Diagnostics.The_Unicode_u_flag_and_the_Unicode_Sets_v_flag_cannot_be_set_simultaneously, pos, size);
                        }
                        else
                        {
                            regExpFlags |= flag.Value;
                            CheckRegularExpressionFlagAvailability(flag.Value, size);
                        }
                    }
                    pos += size;
                }
                if (reportErrors)
                {
                    ScanRange(startOfRegExpBody, endOfRegExpBody - startOfRegExpBody, () =>
                    {
                        ScanRegularExpressionWorker(regExpFlags, annexB: true, namedCaptureGroups);
                        return 0;
                    });
                }
            }
            tokenValue = text[tokenStart..pos];
            token = SyntaxKind.RegularExpressionLiteral;
        }
        return token;
    }


    void ScanRegularExpressionWorker(RegularExpressionFlags regExpFlags, bool annexB, bool namedCaptureGroups)
    {
        /** Grammar parameter */
        var unicodeSetsMode = regExpFlags.HasFlag(RegularExpressionFlags.UnicodeSets);
        /** Grammar parameter */
        var anyUnicodeMode = regExpFlags.HasFlag(RegularExpressionFlags.AnyUnicodeMode);

        // Regular expressions are checked more strictly when either in 'u' or 'v' mode, or
        // when not using the looser interpretation of the syntax from ECMA-262 Annex B.
        var anyUnicodeModeOrNonAnnexB = anyUnicodeMode || !annexB;

        /** @see {scanClassSetExpression} */
        var mayContainStrings = false;

        /** The number of all (named and unnamed) capturing groups defined in the regex. */
        var numberOfCapturingGroups = 0;
        /** All named capturing groups defined in the regex. */
        HashSet<string> groupSpecifiers = null;
        /** All references to named capturing groups in the regex. */
        List<RegexGroupNameRef> groupNameReferences = null;
        /** All numeric backreferences within the regex. */
        List<RegexDecimalEscape> decimalEscapes = null;
        /** A stack of scopes for named capturing groups. @see {scanGroupName} */
        Stack<HashSet<string>> namedCapturingGroupsScopeStack = [];
        HashSet<string> topNamedCapturingGroupsScope = null;

        // Disjunction ::= Alternative ('|' Alternative)*
        void scanDisjunction(bool isInGroup)
        {
            while (true)
            {
                namedCapturingGroupsScopeStack.Push(topNamedCapturingGroupsScope);
                topNamedCapturingGroupsScope = null;
                scanAlternative(isInGroup);
                topNamedCapturingGroupsScope = namedCapturingGroupsScopeStack.Pop();
                if (CharCodeChecked(pos) != CharacterCodes.Bar)
                {
                    return;
                }
                pos++;
            }
        }

        // Alternative ::= Term*
        // Term ::=
        //     | Assertion
        //     | Atom Quantifier?
        // Assertion ::=
        //     | '^'
        //     | '$'
        //     | '\b'
        //     | '\B'
        //     | '(?=' Disjunction ')'
        //     | '(?!' Disjunction ')'
        //     | '(?<=' Disjunction ')'
        //     | '(?<!' Disjunction ')'
        // Quantifier ::= QuantifierPrefix '?'?
        // QuantifierPrefix ::=
        //     | '*'
        //     | '+'
        //     | '?'
        //     | '{' DecimalDigits (',' DecimalDigits?)? '}'
        // Atom ::=
        //     | PatternCharacter
        //     | '.'
        //     | '\' AtomEscape
        //     | CharacterClass
        //     | '(?<' RegExpIdentifierName '>' Disjunction ')'
        //     | '(?' RegularExpressionFlags ('-' RegularExpressionFlags)? ':' Disjunction ')'
        // CharacterClass ::= unicodeMode
        //     ? '[' ClassRanges ']'
        //     : '[' ClassSetExpression ']'
        void scanAlternative(bool isInGroup)
        {
            var isPreviousTermQuantifiable = false;
            while (true)
            {
                var start = pos;
                var ch = CharCodeChecked(pos);
                switch ((int)ch)
                {
                    case CharacterCodes.EOF:
                        return;
                    case CharacterCodes.Caret:
                    case CharacterCodes.Dollar:
                        pos++;
                        isPreviousTermQuantifiable = false;
                        break;
                    case CharacterCodes.Backslash:
                        pos++;
                        switch ((int)CharCodeChecked(pos))
                        {
                            case CharacterCodes.b:
                            case CharacterCodes.B:
                                pos++;
                                isPreviousTermQuantifiable = false;
                                break;
                            default:
                                scanAtomEscape();
                                isPreviousTermQuantifiable = true;
                                break;
                        }
                        break;
                    case CharacterCodes.OpenParen:
                        pos++;
                        if (CharCodeChecked(pos) == CharacterCodes.Question)
                        {
                            pos++;
                            switch ((int)CharCodeChecked(pos))
                            {
                                case CharacterCodes.Equals:
                                case CharacterCodes.Exclamation:
                                    pos++;
                                    // In Annex B, `(?=Disjunction)` and `(?!Disjunction)` are quantifiable
                                    isPreviousTermQuantifiable = !anyUnicodeModeOrNonAnnexB;
                                    break;
                                case CharacterCodes.LessThan:
                                    var groupNameStart = pos;
                                    pos++;
                                    switch ((int)CharCodeChecked(pos))
                                    {
                                        case CharacterCodes.Equals:
                                        case CharacterCodes.Exclamation:
                                            pos++;
                                            isPreviousTermQuantifiable = false;
                                            break;
                                        default:
                                            scanGroupName(isReference: false);
                                            scanExpectedChar(CharacterCodes.GreaterThan);
                                            if (languageVersion < ScriptTarget.ES2018)
                                            {
                                                Error(Diagnostics.Named_capturing_groups_are_only_available_when_targeting_ES2018_or_later, groupNameStart, pos - groupNameStart);
                                            }
                                            numberOfCapturingGroups++;
                                            isPreviousTermQuantifiable = true;
                                            break;
                                    }
                                    break;
                                default:
                                    start = pos;
                                    var setFlags = scanPatternModifiers(RegularExpressionFlags.None);
                                    if (CharCodeChecked(pos) == CharacterCodes.Minus)
                                    {
                                        pos++;
                                        scanPatternModifiers(setFlags);
                                        if (pos == start + 1)
                                        {
                                            Error(Diagnostics.Subpattern_flags_must_be_present_when_there_is_a_minus_sign, start, pos - start);
                                        }
                                    }
                                    scanExpectedChar(CharacterCodes.Colon);
                                    isPreviousTermQuantifiable = true;
                                    break;
                            }
                        }
                        else
                        {
                            numberOfCapturingGroups++;
                            isPreviousTermQuantifiable = true;
                        }
                        scanDisjunction(/*isInGroup*/ true);
                        scanExpectedChar(CharacterCodes.CloseParen);
                        break;
                    case CharacterCodes.OpenBrace:
                        pos++;
                        var digitsStart = pos;
                        ScanDigits();
                        var min = tokenValue;
                        if (!anyUnicodeModeOrNonAnnexB && string.IsNullOrEmpty(min))
                        {
                            isPreviousTermQuantifiable = true;
                            break;
                        }
                        if (CharCodeChecked(pos) == CharacterCodes.Comma)
                        {
                            pos++;
                            ScanDigits();
                            var max = tokenValue;
                            if (string.IsNullOrEmpty(min))
                            {
                                if (!string.IsNullOrEmpty(max) || CharCodeChecked(pos) == CharacterCodes.CloseBrace)
                                {
                                    Error(Diagnostics.Incomplete_quantifier_Digit_expected, digitsStart, 0);
                                }
                                else
                                {
                                    Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, start, 1, ch);
                                    isPreviousTermQuantifiable = true;
                                    break;
                                }
                            }
                            else if (!string.IsNullOrEmpty(max) && int.Parse(min, CultureInfo.InvariantCulture) >
                                int.Parse(max, CultureInfo.InvariantCulture) && (anyUnicodeModeOrNonAnnexB || CharCodeChecked(pos) == CharacterCodes.CloseBrace))
                            {
                                Error(Diagnostics.Numbers_out_of_order_in_quantifier, digitsStart, pos - digitsStart);
                            }
                        }
                        else if (string.IsNullOrEmpty(min))
                        {
                            if (anyUnicodeModeOrNonAnnexB)
                            {
                                Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, start, 1, ch);
                            }
                            isPreviousTermQuantifiable = true;
                            break;
                        }
                        if (CharCodeChecked(pos) != CharacterCodes.CloseBrace)
                        {
                            if (anyUnicodeModeOrNonAnnexB)
                            {
                                Error(Diagnostics._0_expected, pos, 0, (char)CharacterCodes.CloseBrace);
                                pos--;
                            }
                            else
                            {
                                isPreviousTermQuantifiable = true;
                                break;
                            }
                        }
                        goto asteriskPlusQuestion;
                    // falls through
                    case CharacterCodes.Asterisk:
                    case CharacterCodes.Plus:
                    case CharacterCodes.Question:
                    asteriskPlusQuestion:
                        pos++;
                        if (CharCodeChecked(pos) == CharacterCodes.Question)
                        {
                            // Non-greedy
                            pos++;
                        }
                        if (!isPreviousTermQuantifiable)
                        {
                            Error(Diagnostics.There_is_nothing_available_for_repetition, start, pos - start);
                        }
                        isPreviousTermQuantifiable = false;
                        break;
                    case CharacterCodes.Dot:
                        pos++;
                        isPreviousTermQuantifiable = true;
                        break;
                    case CharacterCodes.OpenBracket:
                        pos++;
                        if (unicodeSetsMode)
                        {
                            scanClassSetExpression();
                        }
                        else
                        {
                            scanClassRanges();
                        }
                        scanExpectedChar(CharacterCodes.CloseBracket);
                        isPreviousTermQuantifiable = true;
                        break;
                    case CharacterCodes.CloseParen:
                        if (isInGroup)
                        {
                            return;
                        }
                        goto closeBracketBrace;
                    // falls through
                    case CharacterCodes.CloseBracket:
                    case CharacterCodes.CloseBrace:
                    closeBracketBrace:
                        if (anyUnicodeModeOrNonAnnexB || ch == CharacterCodes.CloseParen)
                        {
                            Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos, 1, ch);
                        }
                        pos++;
                        isPreviousTermQuantifiable = true;
                        break;
                    case CharacterCodes.Slash:
                    case CharacterCodes.Bar:
                        return;
                    default:
                        scanSourceCharacter();
                        isPreviousTermQuantifiable = true;
                        break;
                }
            }
        }

        RegularExpressionFlags scanPatternModifiers(RegularExpressionFlags currFlags)
        {
            while (true)
            {
                var ch = CodePointChecked(pos);
                if (ch == CharacterCodes.EOF ||
                    !IsIdentifierPart(ch, languageVersion))
                {
                    break;
                }
                var size = CharSize(ch);
                var flag = CharacterCodeToRegularExpressionFlag(ch);
                if (flag == null)
                {
                    Error(Diagnostics.Unknown_regular_expression_flag, pos, size);
                }
                else if (currFlags.HasFlag(flag.Value))
                {
                    Error(Diagnostics.Duplicate_regular_expression_flag, pos, size);
                }
                else if (flag.Value.HasFlag(RegularExpressionFlags.Modifiers))
                {
                    Error(Diagnostics.This_regular_expression_flag_cannot_be_toggled_within_a_subpattern, pos, size);
                }
                else
                {
                    currFlags |= flag.Value;
                    CheckRegularExpressionFlagAvailability(flag.Value, size);
                }
                pos += size;
            }
            return currFlags;
        }

        // AtomEscape ::=
        //     | DecimalEscape
        //     | CharacterClassEscape
        //     | CharacterEscape
        //     | 'k<' RegExpIdentifierName '>'
        void scanAtomEscape()
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.Backslash);
            switch ((int)CharCodeChecked(pos))
            {
                case CharacterCodes.k:
                    pos++;
                    if (CharCodeChecked(pos) == CharacterCodes.LessThan)
                    {
                        pos++;
                        scanGroupName(isReference: true);
                        scanExpectedChar(CharacterCodes.GreaterThan);
                    }
                    else if (anyUnicodeModeOrNonAnnexB || namedCaptureGroups)
                    {
                        Error(Diagnostics.k_must_be_followed_by_a_capturing_group_name_enclosed_in_angle_brackets, pos - 2, 2);
                    }
                    break;
                case CharacterCodes.q:
                    if (unicodeSetsMode)
                    {
                        pos++;
                        Error(Diagnostics.q_is_only_available_inside_character_class, pos - 2, 2);
                        break;
                    }
                    goto default;
                // falls through
                default:
                    // The scanEscapeSequence call in scanCharacterEscape must return non-empty strings
                    // since there must not be line breaks in a regex literal
                    Debug.Assert(scanCharacterClassEscape() || scanDecimalEscape() || !string.IsNullOrEmpty(scanCharacterEscape(atomEscape: true)));
                    break;
            }
        }

        // DecimalEscape ::= [1-9] [0-9]*
        bool scanDecimalEscape()
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.Backslash);
            var ch = CharCodeChecked(pos);
            if (ch >= CharacterCodes._1 && ch <= CharacterCodes._9)
            {
                var start = pos;
                ScanDigits();
                (decimalEscapes ??= []).Append(new() { Pos = start, End = pos, Value = decimal.Parse(tokenValue, CultureInfo.InvariantCulture) });
                return true;
            }
            return false;
        }

        // CharacterEscape ::=
        //     | `c` ControlLetter
        //     | IdentityEscape
        //     | (Other sequences handled by `scanEscapeSequence`)
        // IdentityEscape ::=
        //     | '^' | '$' | '/' | '\' | '.' | '*' | '+' | '?' | '(' | ')' | '[' | ']' | '{' | '}' | '|'
        //     | [~UnicodeMode] (any other non-identifier characters)
        string scanCharacterEscape(bool atomEscape)
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.Backslash);
            var ch = (int)CharCodeChecked(pos);
            switch (ch)
            {
                case CharacterCodes.EOF:
                    Error(Diagnostics.Undetermined_character_escape, pos - 1, 1);
                    return "\\";
                case CharacterCodes.c:
                    pos++;
                    ch = CharCodeChecked(pos);
                    if (IsASCIILetter(ch))
                    {
                        pos++;
                        return ((char)(ch & 0x1f)).ToString();
                    }
                    if (anyUnicodeModeOrNonAnnexB)
                    {
                        Error(Diagnostics.c_must_be_followed_by_an_ASCII_letter, pos - 2, 2);
                    }
                    else if (atomEscape)
                    {
                        // Annex B treats
                        //
                        //  ExtendedAtom : `\` [lookahead = `c`]
                        //
                        // as the single character `\` when `c` isn't followed by a valid control character
                        pos--;
                        return "\\";
                    }
                    return ((char)ch).ToString();
                case CharacterCodes.Caret:
                case CharacterCodes.Dollar:
                case CharacterCodes.Slash:
                case CharacterCodes.Backslash:
                case CharacterCodes.Dot:
                case CharacterCodes.Asterisk:
                case CharacterCodes.Plus:
                case CharacterCodes.Question:
                case CharacterCodes.OpenParen:
                case CharacterCodes.CloseParen:
                case CharacterCodes.OpenBracket:
                case CharacterCodes.CloseBracket:
                case CharacterCodes.OpenBrace:
                case CharacterCodes.CloseBrace:
                case CharacterCodes.Bar:
                    pos++;
                    return ((char)ch).ToString();
                default:
                    pos--;
                    var sb = new StringBuilder();
                    ScanEscapeSequence(sb,
                        EscapeSequenceScanningFlags.RegularExpression
                            | (annexB ? EscapeSequenceScanningFlags.AnnexB : 0)
                            | (anyUnicodeMode ? EscapeSequenceScanningFlags.AnyUnicodeMode : 0)
                            | (atomEscape ? EscapeSequenceScanningFlags.AtomEscape : 0)
                    );
                    return sb.ToString();
            }
        }

        void scanGroupName(bool isReference)
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.LessThan);
            tokenStart = pos;
            ScanIdentifier(CodePointChecked(pos), languageVersion);
            if (pos == tokenStart)
            {
                Error(Diagnostics.Expected_a_capturing_group_name);
            }
            else if (isReference)
            {
                (groupNameReferences ??= []).Add(new() { Pos = tokenStart, End = pos, Name = tokenValue });
            }
            else if (topNamedCapturingGroupsScope?.Contains(tokenValue) == true ||
                namedCapturingGroupsScopeStack.Any(group => group?.Contains(tokenValue) == true))
            {
                Error(Diagnostics.Named_capturing_groups_with_the_same_name_must_be_mutually_exclusive_to_each_other, tokenStart, pos - tokenStart);
            }
            else
            {
                (topNamedCapturingGroupsScope ??= []).Add(tokenValue);
                (groupSpecifiers ??= []).Add(tokenValue);
            }
        }

        bool isClassContentExit(int ch)
        {
            return ch == CharacterCodes.CloseBracket || ch == CharacterCodes.EOF || pos >= end;
        }

        // ClassRanges ::= '^'? (ClassAtom ('-' ClassAtom)?)*
        void scanClassRanges()
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.OpenBracket);
            if (CharCodeChecked(pos) == CharacterCodes.Caret)
            {
                // character complement
                pos++;
            }
            while (true)
            {
                var ch = CharCodeChecked(pos);
                if (isClassContentExit(ch))
                {
                    return;
                }
                var minStart = pos;
                var minCharacter = scanClassAtom();
                if (CharCodeChecked(pos) == CharacterCodes.Minus)
                {
                    pos++;
                    ch = CharCodeChecked(pos);
                    if (isClassContentExit(ch))
                    {
                        return;
                    }
                    if (string.IsNullOrEmpty(minCharacter) && anyUnicodeModeOrNonAnnexB)
                    {
                        Error(Diagnostics.A_character_class_range_must_not_be_bounded_by_another_character_class, minStart, pos - 1 - minStart);
                    }
                    var maxStart = pos;
                    var maxCharacter = scanClassAtom();
                    if (string.IsNullOrEmpty(maxCharacter) && anyUnicodeModeOrNonAnnexB)
                    {
                        Error(Diagnostics.A_character_class_range_must_not_be_bounded_by_another_character_class, maxStart, pos - maxStart);
                        continue;
                    }
                    if (string.IsNullOrEmpty(minCharacter))
                    {
                        continue;
                    }
                    var minCharacterValue = CodePointAt(minCharacter, 0);
                    var maxCharacterValue = CodePointAt(maxCharacter, 0);
                    if (
                        minCharacter.Length == CharSize(minCharacterValue) &&
                        maxCharacter.Length == CharSize(maxCharacterValue) &&
                        minCharacterValue > maxCharacterValue)
                    {
                        Error(Diagnostics.Range_out_of_order_in_character_class, minStart, pos - minStart);
                    }
                }
            }
        }

        // Static Semantics: MayContainStrings
        //     ClassUnion: ClassSetOperands.some(ClassSetOperand => ClassSetOperand.MayContainStrings)
        //     ClassIntersection: ClassSetOperands.every(ClassSetOperand => ClassSetOperand.MayContainStrings)
        //     ClassSubtraction: ClassSetOperands[0].MayContainStrings
        //     ClassSetOperand:
        //         || ClassStringDisjunctionContents.MayContainStrings
        //         || CharacterClassEscape.UnicodePropertyValueExpression.LoneUnicodePropertyNameOrValue.MayContainStrings
        //     ClassStringDisjunctionContents: ClassStrings.some(ClassString => ClassString.ClassSetCharacters.length !== 1)
        //     LoneUnicodePropertyNameOrValue: isBinaryUnicodePropertyOfStrings(LoneUnicodePropertyNameOrValue)

        // ClassSetExpression ::= '^'? (ClassUnion | ClassIntersection | ClassSubtraction)
        // ClassUnion ::= (ClassSetRange | ClassSetOperand)*
        // ClassIntersection ::= ClassSetOperand ('&&' ClassSetOperand)+
        // ClassSubtraction ::= ClassSetOperand ('--' ClassSetOperand)+
        // ClassSetRange ::= ClassSetCharacter '-' ClassSetCharacter
        void scanClassSetExpression()
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.OpenBracket);
            var isCharacterComplement = false;
            if (CharCodeChecked(pos) == CharacterCodes.Caret)
            {
                pos++;
                isCharacterComplement = true;
            }
            var expressionMayContainStrings = false;
            var ch = (int)CharCodeChecked(pos);
            if (isClassContentExit(ch))
            {
                return;
            }
            var start = pos;
            string operand = null;
            switch (text[pos..(pos + 2)])
            {
                case "--":
                case "&&":
                    Error(Diagnostics.Expected_a_class_set_operand);
                    mayContainStrings = false;
                    break;
                default:
                    operand = scanClassSetOperand();
                    break;
            }
            switch ((int)CharCodeChecked(pos))
            {
                case CharacterCodes.Minus:
                    if (CharCodeChecked(pos + 1) == CharacterCodes.Minus)
                    {
                        if (isCharacterComplement && mayContainStrings)
                        {
                            Error(Diagnostics.Anything_that_would_possibly_match_more_than_a_single_character_is_invalid_inside_a_negated_character_class, start, pos - start);
                        }
                        expressionMayContainStrings = mayContainStrings;
                        scanClassSetSubExpression(ClassSetExpressionType.ClassSubtraction);
                        mayContainStrings = !isCharacterComplement && expressionMayContainStrings;
                        return;
                    }
                    break;
                case CharacterCodes.Ampersand:
                    if (CharCodeChecked(pos + 1) == CharacterCodes.Ampersand)
                    {
                        scanClassSetSubExpression(ClassSetExpressionType.ClassIntersection);
                        if (isCharacterComplement && mayContainStrings)
                        {
                            Error(Diagnostics.Anything_that_would_possibly_match_more_than_a_single_character_is_invalid_inside_a_negated_character_class, start, pos - start);
                        }
                        expressionMayContainStrings = mayContainStrings;
                        mayContainStrings = !isCharacterComplement && expressionMayContainStrings;
                        return;
                    }
                    else
                    {
                        Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos, 1, (char)ch);
                    }
                    break;
                default:
                    if (isCharacterComplement && mayContainStrings)
                    {
                        Error(Diagnostics.Anything_that_would_possibly_match_more_than_a_single_character_is_invalid_inside_a_negated_character_class, start, pos - start);
                    }
                    expressionMayContainStrings = mayContainStrings;
                    break;
            }
            while (true)
            {
                ch = CharCodeChecked(pos);
                if (ch == CharacterCodes.EOF)
                {
                    break;
                }
                switch (ch)
                {
                    case CharacterCodes.Minus:
                        pos++;
                        ch = CharCodeChecked(pos);
                        if (isClassContentExit(ch))
                        {
                            mayContainStrings = !isCharacterComplement && expressionMayContainStrings;
                            return;
                        }
                        if (ch == CharacterCodes.Minus)
                        {
                            pos++;
                            Error(Diagnostics.Operators_must_not_be_mixed_within_a_character_class_Wrap_it_in_a_nested_class_instead, pos - 2, 2);
                            start = pos - 2;
                            operand = text[start..pos];
                            continue;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(operand))
                            {
                                Error(Diagnostics.A_character_class_range_must_not_be_bounded_by_another_character_class, start, pos - 1 - start);
                            }
                            var secondStart = pos;
                            var secondOperand = scanClassSetOperand();
                            if (isCharacterComplement && mayContainStrings)
                            {
                                Error(Diagnostics.Anything_that_would_possibly_match_more_than_a_single_character_is_invalid_inside_a_negated_character_class, secondStart, pos - secondStart);
                            }
                            expressionMayContainStrings |= mayContainStrings;
                            if (string.IsNullOrEmpty(secondOperand))
                            {
                                Error(Diagnostics.A_character_class_range_must_not_be_bounded_by_another_character_class, secondStart, pos - secondStart);
                                break;
                            }
                            if (string.IsNullOrEmpty(operand))
                            {
                                break;
                            }
                            var minCharacterValue = CodePointAt(operand, 0);
                            var maxCharacterValue = CodePointAt(secondOperand, 0);
                            if (
                                operand.Length == CharSize(minCharacterValue) &&
                                secondOperand.Length == CharSize(maxCharacterValue) &&
                                minCharacterValue > maxCharacterValue)
                            {
                                Error(Diagnostics.Range_out_of_order_in_character_class, start, pos - start);
                            }
                        }
                        break;
                    case CharacterCodes.Ampersand:
                        start = pos;
                        pos++;
                        if (CharCodeChecked(pos) == CharacterCodes.Ampersand)
                        {
                            pos++;
                            Error(Diagnostics.Operators_must_not_be_mixed_within_a_character_class_Wrap_it_in_a_nested_class_instead, pos - 2, 2);
                            if (CharCodeChecked(pos) == CharacterCodes.Ampersand)
                            {
                                Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos, 1, (char)ch);
                                pos++;
                            }
                        }
                        else
                        {
                            Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos - 1, 1, (char)ch);
                        }
                        operand = text[start..pos];
                        continue;
                }
                if (isClassContentExit(CharCodeChecked(pos)))
                {
                    break;
                }
                start = pos;
                switch (text[pos..(pos + 2)])
                { // TODO: don't use slice
                    case "--":
                    case "&&":
                        Error(Diagnostics.Operators_must_not_be_mixed_within_a_character_class_Wrap_it_in_a_nested_class_instead, pos, 2);
                        pos += 2;
                        operand = text[start..pos];
                        break;
                    default:
                        operand = scanClassSetOperand();
                        break;
                }
            }
            mayContainStrings = !isCharacterComplement && expressionMayContainStrings;
        }

        void scanClassSetSubExpression(ClassSetExpressionType expressionType)
        {
            var expressionMayContainStrings = mayContainStrings;
            while (true)
            {
                var ch = CharCodeChecked(pos);
                if (isClassContentExit(ch))
                {
                    break;
                }
                // Provide user-friendly diagnostic messages
                switch ((int)ch)
                {
                    case CharacterCodes.Minus:
                        pos++;
                        if (CharCodeChecked(pos) == CharacterCodes.Minus)
                        {
                            pos++;
                            if (expressionType != ClassSetExpressionType.ClassSubtraction)
                            {
                                Error(Diagnostics.Operators_must_not_be_mixed_within_a_character_class_Wrap_it_in_a_nested_class_instead, pos - 2, 2);
                            }
                        }
                        else
                        {
                            Error(Diagnostics.Operators_must_not_be_mixed_within_a_character_class_Wrap_it_in_a_nested_class_instead, pos - 1, 1);
                        }
                        break;
                    case CharacterCodes.Ampersand:
                        pos++;
                        if (CharCodeChecked(pos) == CharacterCodes.Ampersand)
                        {
                            pos++;
                            if (expressionType != ClassSetExpressionType.ClassIntersection)
                            {
                                Error(Diagnostics.Operators_must_not_be_mixed_within_a_character_class_Wrap_it_in_a_nested_class_instead, pos - 2, 2);
                            }
                            if (CharCodeChecked(pos) == CharacterCodes.Ampersand)
                            {
                                Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos, 1, ch);
                                pos++;
                            }
                        }
                        else
                        {
                            Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos - 1, 1, ch);
                        }
                        break;
                    default:
                        switch (expressionType)
                        {
                            case ClassSetExpressionType.ClassSubtraction:
                                Error(Diagnostics._0_expected, pos, 0, "--");
                                break;
                            case ClassSetExpressionType.ClassIntersection:
                                Error(Diagnostics._0_expected, pos, 0, "&&");
                                break;
                            default:
                                break;
                        }
                        break;
                }
                ch = CharCodeChecked(pos);
                if (isClassContentExit(ch))
                {
                    Error(Diagnostics.Expected_a_class_set_operand);
                    break;
                }
                scanClassSetOperand();
                // Used only if expressionType is Intersection
                expressionMayContainStrings &= mayContainStrings;
            }
            mayContainStrings = expressionMayContainStrings;
        }

        // ClassSetOperand ::=
        //     | '[' ClassSetExpression ']'
        //     | '\' CharacterClassEscape
        //     | '\q{' ClassStringDisjunctionContents '}'
        //     | ClassSetCharacter
        string scanClassSetOperand()
        {
            mayContainStrings = false;
            switch ((int)CharCodeChecked(pos))
            {
                case CharacterCodes.EOF:
                    return "";
                case CharacterCodes.OpenBracket:
                    pos++;
                    scanClassSetExpression();
                    scanExpectedChar(CharacterCodes.CloseBracket);
                    return "";
                case CharacterCodes.Backslash:
                    pos++;
                    if (scanCharacterClassEscape())
                    {
                        return "";
                    }
                    else if (CharCodeChecked(pos) == CharacterCodes.Q)
                    {
                        pos++;
                        if (CharCodeChecked(pos) == CharacterCodes.OpenBrace)
                        {
                            pos++;
                            scanClassStringDisjunctionContents();
                            scanExpectedChar(CharacterCodes.CloseBrace);
                            return "";
                        }
                        else
                        {
                            Error(Diagnostics.q_must_be_followed_by_string_alternatives_enclosed_in_braces, pos - 2, 2);
                            return "q";
                        }
                    }
                    pos--;
                    goto default;
                // falls through
                default:
                    return scanClassSetCharacter();
            }
        }

        // ClassStringDisjunctionContents ::= ClassSetCharacter* ('|' ClassSetCharacter*)*
        void scanClassStringDisjunctionContents()
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.OpenBrace);
            var characterCount = 0;
            while (true)
            {
                var ch = CharCodeChecked(pos);
                switch ((int)ch)
                {
                    case CharacterCodes.EOF:
                        return;
                    case CharacterCodes.CloseBrace:
                        if (characterCount != 1)
                        {
                            mayContainStrings = true;
                        }
                        return;
                    case CharacterCodes.Bar:
                        if (characterCount != 1)
                        {
                            mayContainStrings = true;
                        }
                        pos++;
                        characterCount = 0;
                        break;
                    default:
                        scanClassSetCharacter();
                        characterCount++;
                        break;
                }
            }
        }

        // ClassSetCharacter ::=
        //     | SourceCharacter -- ClassSetSyntaxCharacter -- ClassSetReservedDoublePunctuator
        //     | '\' (CharacterEscape | ClassSetReservedPunctuator | 'b')
        string scanClassSetCharacter()
        {
            var ch = CharCodeChecked(pos);
            if (ch == CharacterCodes.EOF)
            {
                // no need to report an error, the initial scan will already have reported that the RegExp is unterminated.
                return "";
            }
            if (ch == CharacterCodes.Backslash)
            {
                pos++;
                ch = CharCodeChecked(pos);
                switch ((int)ch)
                {
                    case CharacterCodes.b:
                        pos++;
                        return "\b";
                    case CharacterCodes.Ampersand:
                    case CharacterCodes.Minus:
                    case CharacterCodes.Exclamation:
                    case CharacterCodes.Hash:
                    case CharacterCodes.Percent:
                    case CharacterCodes.Comma:
                    case CharacterCodes.Colon:
                    case CharacterCodes.Semicolon:
                    case CharacterCodes.LessThan:
                    case CharacterCodes.Equals:
                    case CharacterCodes.GreaterThan:
                    case CharacterCodes.At:
                    case CharacterCodes.Backtick:
                    case CharacterCodes.Tilde:
                        pos++;
                        return ch.ToString();
                    default:
                        return scanCharacterEscape(atomEscape: false).ToString();
                }
            }
            else if (ch == CharCodeChecked(pos + 1))
            {
                switch ((int)ch)
                {
                    case CharacterCodes.Ampersand:
                    case CharacterCodes.Exclamation:
                    case CharacterCodes.Hash:
                    case CharacterCodes.Percent:
                    case CharacterCodes.Asterisk:
                    case CharacterCodes.Plus:
                    case CharacterCodes.Comma:
                    case CharacterCodes.Dot:
                    case CharacterCodes.Colon:
                    case CharacterCodes.Semicolon:
                    case CharacterCodes.LessThan:
                    case CharacterCodes.Equals:
                    case CharacterCodes.GreaterThan:
                    case CharacterCodes.Question:
                    case CharacterCodes.At:
                    case CharacterCodes.Backtick:
                    case CharacterCodes.Tilde:
                        Error(Diagnostics.A_character_class_must_not_contain_a_reserved_double_punctuator_Did_you_mean_to_escape_it_with_backslash, pos, 2);
                        pos += 2;
                        return text[(pos - 2)..pos];
                }
            }
            switch ((int)ch)
            {
                case CharacterCodes.Slash:
                case CharacterCodes.OpenParen:
                case CharacterCodes.CloseParen:
                case CharacterCodes.OpenBracket:
                case CharacterCodes.CloseBracket:
                case CharacterCodes.OpenBrace:
                case CharacterCodes.CloseBrace:
                case CharacterCodes.Minus:
                case CharacterCodes.Bar:
                    Error(Diagnostics.Unexpected_0_Did_you_mean_to_escape_it_with_backslash, pos, 1, ch);
                    pos++;
                    return ch.ToString();
            }
            return scanSourceCharacter();
        }

        // ClassAtom ::=
        //     | SourceCharacter but not one of '\' or ']'
        //     | '\' ClassEscape
        // ClassEscape ::=
        //     | 'b'
        //     | '-'
        //     | CharacterClassEscape
        //     | CharacterEscape
        string scanClassAtom()
        {
            if (CharCodeChecked(pos) == CharacterCodes.Backslash)
            {
                pos++;
                var ch = CharCodeChecked(pos);
                switch ((int)ch)
                {
                    case CharacterCodes.b:
                        pos++;
                        return "\b";
                    case CharacterCodes.Minus:
                        pos++;
                        return ch.ToString();
                    default:
                        if (scanCharacterClassEscape())
                        {
                            return "";
                        }
                        return scanCharacterEscape(atomEscape: false).ToString();
                }
            }
            else
            {
                return scanSourceCharacter();
            }
        }

        // CharacterClassEscape ::=
        //     | 'd' | 'D' | 's' | 'S' | 'w' | 'W'
        //     | [+UnicodeMode] ('P' | 'p') '{' UnicodePropertyValueExpression '}'
        bool scanCharacterClassEscape()
        {
            Debug.Assert(CharCodeUnchecked(pos - 1) == CharacterCodes.Backslash);
            var isCharacterComplement = false;
            var start = pos - 1;
            var ch = CharCodeChecked(pos);
            switch ((int)ch)
            {
                case CharacterCodes.d:
                case CharacterCodes.D:
                case CharacterCodes.s:
                case CharacterCodes.S:
                case CharacterCodes.w:
                case CharacterCodes.W:
                    pos++;
                    return true;
                case CharacterCodes.P:
                    isCharacterComplement = true;
                    goto charactercodesP;
                // falls through
                case CharacterCodes.p:
                charactercodesP:
                    pos++;
                    if (CharCodeChecked(pos) == CharacterCodes.OpenBrace)
                    {
                        pos++;
                        var propertyNameOrValueStart = pos;
                        var propertyNameOrValue = scanWordCharacters();
                        if (CharCodeChecked(pos) == CharacterCodes.Equals)
                        {
                            var propertyName = NonBinaryUnicodeProperties.TryGetValue(propertyNameOrValue, out string value) ? value : null;
                            if (pos == propertyNameOrValueStart)
                            {
                                Error(Diagnostics.Expected_a_Unicode_property_name);
                            }
                            else if (propertyName == null)
                            {
                                Error(Diagnostics.Unknown_Unicode_property_name, propertyNameOrValueStart, pos - propertyNameOrValueStart);
                                //var suggestion = getSpellingSuggestion(propertyNameOrValue, nonBinaryUnicodeProperties.keys(), identity);
                                //if (suggestion)
                                //{
                                //    Error(Diagnostics.Did_you_mean_0, propertyNameOrValueStart, pos - propertyNameOrValueStart, suggestion);
                                //}
                            }
                            pos++;
                            var propertyValueStart = pos;
                            var propertyValue = scanWordCharacters();
                            if (pos == propertyValueStart)
                            {
                                Error(Diagnostics.Expected_a_Unicode_property_value);
                            }
                            else if (propertyName != null &&
                                (!ValuesOfNonBinaryUnicodeProperties.TryGetValue(propertyName, out var x) ||
                                 x?.Contains(propertyValue) != true))
                            {
                                Error(Diagnostics.Unknown_Unicode_property_value, propertyValueStart, pos - propertyValueStart);
                                //const suggestion = getSpellingSuggestion(propertyValue, valuesOfNonBinaryUnicodeProperties[propertyName], identity);
                                //if (suggestion)
                                //{
                                //    Error(Diagnostics.Did_you_mean_0, propertyValueStart, pos - propertyValueStart, suggestion);
                                //}
                            }
                        }
                        else
                        {
                            if (pos == propertyNameOrValueStart)
                            {
                                Error(Diagnostics.Expected_a_Unicode_property_name_or_value);
                            }
                            else if (BinaryUnicodePropertiesOfStrings.Contains(propertyNameOrValue))
                            {
                                if (!unicodeSetsMode)
                                {
                                    Error(Diagnostics.Any_Unicode_property_that_would_possibly_match_more_than_a_single_character_is_only_available_when_the_Unicode_Sets_v_flag_is_set, propertyNameOrValueStart, pos - propertyNameOrValueStart);
                                }
                                else if (isCharacterComplement)
                                {
                                    Error(Diagnostics.Anything_that_would_possibly_match_more_than_a_single_character_is_invalid_inside_a_negated_character_class, propertyNameOrValueStart, pos - propertyNameOrValueStart);
                                }
                                else
                                {
                                    mayContainStrings = true;
                                }
                            }
                            else if (!ValuesOfNonBinaryUnicodeProperties["General_Category"].Contains(propertyNameOrValue) &&
                                !BinaryUnicodeProperties.Contains(propertyNameOrValue))
                            {
                                Error(Diagnostics.Unknown_Unicode_property_name_or_value, propertyNameOrValueStart, pos - propertyNameOrValueStart);
                                //const suggestion = getSpellingSuggestion(propertyNameOrValue, [...valuesOfNonBinaryUnicodeProperties.General_Category, ...binaryUnicodeProperties, ...binaryUnicodePropertiesOfStrings], identity);
                                //if (suggestion)
                                //{
                                //    Error(Diagnostics.Did_you_mean_0, propertyNameOrValueStart, pos - propertyNameOrValueStart, suggestion);
                                //}
                            }
                        }
                        scanExpectedChar(CharacterCodes.CloseBrace);
                        if (!anyUnicodeMode)
                        {
                            Error(Diagnostics.Unicode_property_value_expressions_are_only_available_when_the_Unicode_u_flag_or_the_Unicode_Sets_v_flag_is_set, start, pos - start);
                        }
                    }
                    else if (anyUnicodeModeOrNonAnnexB)
                    {
                        Error(Diagnostics._0_must_be_followed_by_a_Unicode_property_value_expression_enclosed_in_braces, pos - 2, 2, ch);
                    }
                    else
                    {
                        pos--;
                        return false;
                    }
                    return true;
            }
            return false;
        }

        string scanWordCharacters()
        {
            var value = "";
            while (true)
            {
                var ch = CharCodeChecked(pos);
                if (ch == CharacterCodes.EOF || !IsWordCharacter(ch))
                {
                    break;
                }
                value += ch;
                pos++;
            }
            return value;
        }

        string scanSourceCharacter()
        {
            var size = anyUnicodeMode ? CharSize(CodePointChecked(pos)) : 1;
            pos += size;
            return size > 0 ? text[(pos - size)..pos] : "";
        }

        void scanExpectedChar(int ch)
        {
            if (CharCodeChecked(pos) == ch)
            {
                pos++;
            }
            else
            {
                Error(Diagnostics._0_expected, pos, 0, (char)ch);
            }
        }

        scanDisjunction(isInGroup: false);

        groupNameReferences?.ForEach(reference =>
        {
            if (!groupSpecifiers?.Contains(reference.Name) != true)
            {
                Error(Diagnostics.There_is_no_capturing_group_named_0_in_this_regular_expression, reference.Pos, (reference.End ?? 0) - (reference.Pos ?? 0), reference.Name);
                if (groupSpecifiers != null)
                {
                    //const suggestion = getSpellingSuggestion(reference.name, groupSpecifiers, identity);
                    //if (suggestion)
                    //{
                    //    error(Diagnostics.Did_you_mean_0, reference.pos, reference.end - reference.pos, suggestion);
                    //}
                }
            }
        });

        decimalEscapes?.ForEach(escape =>
        {
            // Although a DecimalEscape with a value greater than the number of capturing groups
            // is treated as either a LegacyOctalEscapeSequence or an IdentityEscape in Annex B,
            // an error is nevertheless reported since it's most likely a mistake.
            if (escape.Value > numberOfCapturingGroups)
            {
                if (numberOfCapturingGroups != 0)
                {
                    Error(Diagnostics.This_backreference_refers_to_a_group_that_does_not_exist_There_are_only_0_capturing_groups_in_this_regular_expression, escape.Pos, (escape.End ?? 0) - (escape.Pos ?? 0), numberOfCapturingGroups);
                }
                else
                {
                    Error(Diagnostics.This_backreference_refers_to_a_group_that_does_not_exist_There_are_no_capturing_groups_in_this_regular_expression, escape.Pos, (escape.End ?? 0) - (escape.Pos ?? 0));
                }
            }
        });
    }

    void CheckRegularExpressionFlagAvailability(RegularExpressionFlags flag, int size)
    {
        var availableFrom = RegExpFlagToFirstAvailableLanguageVersion.TryGetValue(flag, out var v) ? (ScriptTarget)v : (ScriptTarget?)null;
        if (availableFrom != null && languageVersion < availableFrom)
        {
            Error(Diagnostics.This_regular_expression_flag_is_only_available_when_targeting_0_or_later, pos, size, Enum.GetName(typeof(ScriptTarget), availableFrom.Value));
        }
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
        for (var ch = text[pos]; pos < end && (!IsLineBreak(ch) && ch != CharacterCodes.Backtick); ch = CharCodeUnchecked(++pos))
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

            case CharacterCodes.OpenParen:
                return token = SyntaxKind.OpenParenToken;

            case CharacterCodes.CloseParen:
                return token = SyntaxKind.CloseParenToken;

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
                    tokenValue = ScanExtendedUnicodeEscape(shouldEmitInvalidEscapeError: true) + ScanIdentifierParts();
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

    T ScanRange<T>(int start, int length, Func<T> callback)
    {
        var saveEnd = end;
        var savePos = pos;
        var saveStartPos = fullStartPos;
        var saveTokenPos = tokenStart;
        var saveToken = token;
        var saveTokenValue = tokenValue;
        var saveTokenFlags = tokenFlags;
        var saveErrorExpectations = commentDirectives;

        SetText(text, start, length);
        var result = callback();

        end = saveEnd;
        pos = savePos;
        fullStartPos = saveStartPos;
        tokenStart = saveTokenPos;
        token = saveToken;
        tokenValue = saveTokenValue;
        tokenFlags = saveTokenFlags;
        commentDirectives = saveErrorExpectations;

        return result;
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

    public void SetSkipJsDocLeadingAsterisks(bool skip)
    {
        skipJsDocLeadingAsterisks += skip ? 1 : -1;
    }
}