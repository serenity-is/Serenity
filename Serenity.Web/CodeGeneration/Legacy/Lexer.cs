#if !COREFX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

/*
The MIT License (MIT)

Copyright (c) 2013 Oleg Shevchenko

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace Serenity.CodeGeneration
{
    public class Lexer : IEnumerable<Token>, IEnumerator<Token>
    {
        private readonly LexerSettings settings;
        private LexerBehavior behavior;
        private TextReader reader;
        private string text;
        private int position;
        private int start;
        private int textLen;
        private int textPos;
        private int textBeg;
        private int bufBeg;
        private int maxSymLen;
        private int lineBegin;
        private int lineNumber;
        private int endLineBegin;
        private int endLineNumber;
        private StringBuilder buffer;
        private StringBuilder tokenBuffer;
        private Token current;
        private Token next;

        private Lexer(string text, TextReader reader, LexerBehavior behavior, LexerSettings settings)
        {
            if (settings == null)
            {
                settings = LexerSettings.Default;
            }
            else
            {
                settings = settings.Clone();
            }

            this.text = text;
            this.reader = reader;
            this.behavior = behavior;
            this.settings = settings;

            if (settings.Symbols != null)
            {
                foreach (KeyValuePair<string, int> entry in settings.Symbols)
                {
                    int len = entry.Key.Length;
                    if (len > maxSymLen)
                    {
                        maxSymLen = len;
                    }
                }
            }

            Reset();
        }

        public Lexer(string text, LexerBehavior behavior, LexerSettings settings)
            : this(text, null, behavior, settings)
        {
        }

        public Lexer(string text, LexerBehavior behavior)
            : this(text, null, behavior, null)
        {
        }

        public Lexer(string text, LexerSettings settings)
            : this(text, null, LexerBehavior.Default, settings)
        {
        }

        public Lexer(string text)
            : this(text, null, LexerBehavior.Default, null)
        {
        }

        public Lexer(TextReader reader, LexerBehavior behavior, LexerSettings settings)
            : this(null, reader, behavior, settings)
        {
        }

        public Lexer(TextReader reader, LexerBehavior behavior)
            : this(null, reader, behavior, null)
        {
        }

        public Lexer(TextReader reader, LexerSettings settings)
            : this(null, reader, LexerBehavior.Default, settings)
        {
        }

        public Lexer(TextReader reader)
            : this(null, reader, LexerBehavior.Default, null)
        {
        }

        private const int BufferCapacity = 8192;

        private const char EndOfTextChar = unchecked((char)-1);

        public Token Current
        {
            get
            {
                return current;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return text == null;
            }
        }

        public void Reset()
        {
            int readerPos = position - textPos;
            current = new Token(TokenType.Start, null, null, 0, 0, 0, 0, 0, 0, 0);
            next = null;
            textPos = 0;
            position = 0;
            textBeg = 0;
            tokenBuffer = null;
            buffer = null;
            bufBeg = -1;

            if (reader != null)
            {
                if (text != null && readerPos > 0)
                {
                    StreamReader streamReader = reader as StreamReader;
                    if (streamReader != null && streamReader.BaseStream.CanSeek)
                    {
                        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                        text = null;
                    }
                }

                if (text == null)
                {
                    textLen = 0;
                    ReadCharBuffer();
                }
            }
            else
            {
                textLen = (text == null ? 0 : text.Length);
            }
        }

        public Token GetNextToken(LexerBehavior behavior)
        {
            LexerBehavior saveBehavior = this.behavior;
            this.behavior = behavior;
            try
            {
                return GetNextToken();
            }
            finally
            {
                this.behavior = saveBehavior;
            }
        }

        public Token GetNextToken()
        {
            if (next != null)
            {
                current = next;
                next = null;
            }
            else
            {
                current = GetToken();
            }

            return current;
        }

        public Token PeekNextToken(LexerBehavior behavior)
        {
            LexerBehavior saveBehavior = this.behavior;
            this.behavior = behavior;
            try
            {
                return PeekNextToken();
            }
            finally
            {
                this.behavior = saveBehavior;
            }
        }

        public Token PeekNextToken()
        {
            if (next == null)
            {
                next = GetToken();
            }

            return next;
        }

        #region Private Implementation

        private Token GetToken()
        {
            if (text == null)
            {
                return new Token(TokenType.End, "", "", 0, 0, 0, 0, 0, 0, 0);
            }

            lineBegin = endLineBegin;
            lineNumber = endLineNumber;
            start = position;
            textBeg = textPos;
            bufBeg = -1;
            tokenBuffer = null;
            buffer = null;

            char currentChar = PeekChar();
            bool skip;
            do
            {
                skip = false;
                // end
                if (currentChar == EndOfTextChar && EndOfText())
                {
                    return GetEndToken();
                }

                // separator
                if (currentChar <= ' ')
                {
                    bool skipWhiteSpaces = (behavior & LexerBehavior.SkipWhiteSpaces) != 0;
                    do
                    {
                        ReadNext();
                        if (skipWhiteSpaces)
                        {
                            textBeg = textPos;
                        }

                        if (EndOfLine(currentChar))
                        {
                            if (skipWhiteSpaces)
                            {
                                textBeg = textPos;
                            }
                            else if ((settings.Options & LexerOptions.EndOfLineAsToken) != 0)
                            {
                                return new Token(TokenType.EndOfLine, "", GetTokenText(), 0, start, position, lineBegin, lineNumber, endLineBegin, endLineNumber);
                            }
                        }

                        currentChar = PeekChar();
                        if (currentChar == EndOfTextChar && EndOfText())
                        {
                            break;
                        }

                    } while (currentChar <= ' ');

                    if (!skipWhiteSpaces)
                    {
                        return new Token(TokenType.WhiteSpace, "", GetTokenText(), 0, start, position, lineBegin, lineNumber, endLineBegin, endLineNumber);
                    }

                    textBeg = textPos;
                    skip = true;
                    start = position;
                }

                // inline comment
                string[] inlineComments = settings.InlineComments;
                if (inlineComments != null)
                {
                    for (int inlineCommentIndex = 0; inlineCommentIndex < inlineComments.Length; inlineCommentIndex++)
                    {
                        string inlineComment = inlineComments[inlineCommentIndex];
                        if (NextSymbolIs(inlineComment))
                        {
                            bool skipComments = ((behavior & LexerBehavior.SkipComments) != 0);
                            skip = true;
                            if (skipComments)
                            {
                                textBeg = textPos;
                            }

                            currentChar = PeekChar();
                            while (true)
                            {
                                if (currentChar == '\r' || currentChar == '\n')
                                {
                                    break;
                                }

                                currentChar = NextChar();
                                if (currentChar == EndOfTextChar && EndOfText())
                                {
                                    break;
                                }

                                if (skipComments)
                                {
                                    textBeg = textPos;
                                }
                            }

                            if (skipComments)
                            {
                                start = position;
                            }
                            else
                            {
                                return new Token(TokenType.Comment, "", GetTokenText(), 0, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
                            }

                            break;
                        }
                    }
                }

                // comment
                if (!string.IsNullOrEmpty(settings.CommentBegin) && NextSymbolIs(settings.CommentBegin))
                {
                    bool skipComments = ((behavior & LexerBehavior.SkipComments) != 0);
                    skip = true;
                    if (skipComments)
                    {
                        textBeg = textPos;
                    }

                    while (true)
                    {
                        if (NextSymbolIs(settings.CommentEnd))
                        {
                            currentChar = PeekChar();
                            if (skipComments)
                            {
                                textBeg = textPos;
                            }

                            break;
                        }

                        currentChar = NextChar();
                        if (currentChar == EndOfTextChar && EndOfText())
                        {
                            break;
                        }
                        else
                        {
                            EndOfLine(currentChar);
                        }

                        if (skipComments)
                        {
                            textBeg = textPos;
                        }
                    }

                    if (skipComments)
                    {
                        start = position;
                    }
                    else
                    {
                        return new Token(TokenType.Comment, "", GetTokenText(), 0, start, position, lineBegin, lineNumber, endLineBegin, endLineNumber);
                    }
                }

                lineNumber = endLineNumber;
                lineBegin = endLineBegin;

            } while (skip);

            // quoted string
            char[] stringQuotes = settings.StringQuotes;
            if (stringQuotes != null)
            {
                for (int i = 0; i < stringQuotes.Length; i++)
                {
                    char stringQuoteChar = stringQuotes[i];
                    if (currentChar == stringQuoteChar || i == 0 && currentChar == settings.StringPrefix && PeekChar(1) == stringQuoteChar)
                    {
                        return GetQuotedStringToken(currentChar != stringQuoteChar, stringQuoteChar);
                    }
                }
            }

            // quoted identifier
            bool isIdentQuote = currentChar == settings.IdentQuote;
            bool quote = isIdentQuote || currentChar == settings.IdentQuoteBegin;
            char nextChar;
            if (quote || currentChar == settings.IdentPrefix && (isIdentQuote = (nextChar = PeekChar(1)) == settings.IdentQuote || nextChar == settings.IdentQuoteBegin))
            {
                return GetQuotedIdentifierToken(!quote, isIdentQuote);
            }

            // prefix identifier
            if (currentChar == settings.IdentPrefix)
            {
                return GetPrefixedIdentifierToken();
            }

            // number
            if (currentChar >= '0' && currentChar <= '9')
            {
                return GetNumberToken(currentChar);
            }

            // keyword / identifier
            if (Char.IsLetter(currentChar) || currentChar == '_' || IsIdentChar(currentChar))
            {
                return GetKeywordOrIdentifierToken(currentChar);
            }

            // predefined symbol
            if (settings.Symbols != null)
            {
                string symbol = PeekSubstring(maxSymLen);
                for (int i = symbol.Length; i > 0; i--, symbol = symbol.Substring(0, i))
                {
                    int symbolId;
                    if (settings.Symbols.TryGetValue(symbol, out symbolId))
                    {
                        Skip(i);
                        string symbolText = (behavior & LexerBehavior.PersistTokenText) != 0 ? symbol : null;
                        return new Token(TokenType.Symbol, symbol, symbolText, (int)symbolId, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
                    }
                }
            }

            // just a char
            currentChar = NextChar();
            string charText = (behavior & LexerBehavior.PersistTokenText) != 0 ? currentChar.ToString() : null;
            return new Token(TokenType.Char, currentChar, charText, 0, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
        }

        private Token GetEndToken()
        {
            if (reader != null)
            {
                reader.Close();
            }

            return new Token(TokenType.End, "", "", 0, start, start, lineBegin, lineNumber, lineBegin, lineNumber);
        }

        private Token GetQuotedIdentifierToken(bool prefix, bool isIdentQuote)
        {
            if (prefix)
            {
                ReadNext();
            }

            char quoteEnd;
            bool doubleQuote;
            if (isIdentQuote)
            {
                quoteEnd = settings.IdentQuote;
                doubleQuote = (settings.Options & LexerOptions.IdentDoubleQuote) != 0;
            }
            else
            {
                quoteEnd = settings.IdentQuoteEnd;
                doubleQuote = false;
            }

            ReadNext();
            bufBeg = textPos;

            while (true)
            {
                char currentChar = NextChar();
                BufferAdd(currentChar);

                if (currentChar == quoteEnd)
                {
                    if (doubleQuote && PeekChar() == quoteEnd)
                    {
                        EnsureBuffer(1);
                        currentChar = NextChar();
                        BufferAdd(currentChar);
                    }
                    else
                    {
                        break;
                    }
                }

                if (currentChar == EndOfTextChar && EndOfText())
                {
                    break;
                }
                else
                {
                    EndOfLine(currentChar);
                }
            }

            string val = GetBufferValue(-1);
            return new Token(TokenType.Identifier, val, GetTokenText(), 0, start, position, lineBegin, lineNumber, endLineBegin, endLineNumber);
        }

        private Token GetQuotedStringToken(bool prefix, char stringQuoteChar)
        {
            char escapeChar;
            bool escaping;
            bool doubleQuote;
            if (prefix)
            {
                escapeChar = '\0';
                escaping = false;
                doubleQuote = true;
                ReadNext();
            }
            else
            {
                escapeChar = settings.StringEscapeChar;
                escaping = (settings.Options & LexerOptions.StringEscaping) != 0;
                doubleQuote = (settings.Options & LexerOptions.StringDoubleQuote) != 0;
            }

            ReadNext();
            bufBeg = textPos;

            while (true)
            {
                char currentChar = NextChar();
                BufferAdd(currentChar);

                if (currentChar == escapeChar && escaping)
                {
                    EnsureBuffer(1);
                    currentChar = NextChar();
                    BufferAdd(currentChar);
                }
                else if (currentChar == stringQuoteChar)
                {
                    if (doubleQuote && PeekChar() == stringQuoteChar)
                    {
                        EnsureBuffer(1);
                        currentChar = NextChar();
                        BufferAdd(currentChar);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (currentChar == EndOfTextChar && EndOfText())
                {
                    break;
                }
                else
                {
                    EndOfLine(currentChar);
                }
            }

            string val = GetBufferValue(-1);
            return new Token(TokenType.QuotedString, val, GetTokenText(), 0, start, position, lineBegin, lineNumber, endLineBegin, endLineNumber);
        }

        private Token GetKeywordOrIdentifierToken(char currentChar)
        {
            bufBeg = textPos;
            do
            {
                ReadNext();
                BufferAdd(currentChar);
                currentChar = PeekChar();
            } while (Char.IsLetterOrDigit(currentChar) || currentChar == '_' || IsIdentChar(currentChar));

            string val = GetBufferValue(0);

            int id = 0;
            TokenType tokenType = TokenType.Identifier;
            if ((settings.Options & LexerOptions.IdentToUpper) != 0)
            {
                val = val.ToUpper(settings.CultureInfo);
                if (settings.Keywords != null && settings.Keywords.TryGetValue(val, out id))
                {
                    tokenType = TokenType.Keyword;
                }
            }
            else
            {
                if (settings.Keywords != null && settings.Keywords.TryGetValue(val.ToUpper(settings.CultureInfo), out id))
                {
                    tokenType = TokenType.Keyword;
                }

                if ((settings.Options & LexerOptions.IdentToLower) != 0)
                {
                    val = val.ToLower();
                }
            }

            return new Token(tokenType, val, GetTokenText(), (int)id, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
        }

        private Token GetNumberToken(char currentChar)
        {
            bufBeg = textPos;
            do
            {
                ReadNext();
                BufferAdd(currentChar);
                currentChar = PeekChar();
            }
            while (currentChar >= '0' && currentChar <= '9');

            string decimalSeparator = settings.DecimalSeparator;
            if (SymbolIs(decimalSeparator))
            {
                int ln = decimalSeparator.Length;
                char ch = PeekChar(ln);
                if (ch >= '0' && ch <= '9')
                {
                    Skip(ln);
                    BufferAdd(decimalSeparator);
                    currentChar = ch;
                    do
                    {
                        ReadNext();
                        BufferAdd(currentChar);
                        currentChar = PeekChar();
                    } while (currentChar >= '0' && currentChar <= '9');
                }
            }

            if (char.IsLetter(currentChar))
            {
                do
                {
                    ReadNext();
                    BufferAdd(currentChar);
                    currentChar = PeekChar();

                } while ((currentChar >= '0' && currentChar <= '9') || currentChar == '-' || currentChar == '+' || Char.IsLetter(currentChar));

                string val = GetBufferValue(0);
                return new Token(TokenType.Number, val, GetTokenText(), 0, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
            }
            else
            {
                string val = GetBufferValue(0);
                decimal decimalVal;
                if (decimal.TryParse(val, out decimalVal))
                {
                    return new Token(TokenType.Decimal, decimalVal, GetTokenText(), 0, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
                }
                else
                {
                    return new Token(TokenType.Number, val, GetTokenText(), 0, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
                }
            }
        }

        private Token GetPrefixedIdentifierToken()
        {
            ReadNext();
            bufBeg = textPos;

            char currentChar = PeekChar();
            if (Char.IsLetterOrDigit(currentChar) || currentChar == '_' || IsIdentChar(currentChar))
            {
                do
                {
                    ReadNext();
                    BufferAdd(currentChar);
                    currentChar = PeekChar();
                }
                while (Char.IsLetterOrDigit(currentChar) || currentChar == '_' || IsIdentChar(currentChar));
            }

            string val = GetBufferValue(0);
            if ((settings.Options & LexerOptions.IdentToUpper) != 0)
            {
                val = val.ToUpper(settings.CultureInfo);
            }
            else if ((settings.Options & LexerOptions.IdentToLower) != 0)
            {
                val = val.ToLower(settings.CultureInfo);
            }

            return new Token(TokenType.Identifier, val, GetTokenText(), 0, start, position, lineBegin, lineNumber, lineBegin, lineNumber);
        }

        private bool IsIdentChar(char currentChar)
        {
            char[] identChars = settings.IdentChars;
            if (identChars != null)
            {
                int len = identChars.Length;
                for (int i = 0; i < len; i++)
                {
                    char ch = identChars[i];
                    if (currentChar == ch)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private char PeekChar()
        {
            if (textPos < textLen)
            {
                return text[textPos];
            }

            if (textLen == BufferCapacity)
            {
                ReadCharBuffer();
                if (textPos < textLen)
                {
                    return text[textPos];
                }
            }

            return EndOfTextChar;
        }

        private char PeekChar(int ofs)
        {
            int i = textPos + ofs;
            if (i < textLen)
            {
                return text[i];
            }

            if (textLen == BufferCapacity)
            {
                ReadCharBuffer();
                ofs += textPos;
                if (ofs < textLen)
                {
                    return text[ofs];
                }
            }

            return EndOfTextChar;
        }

        private string PeekSubstring(int count)
        {
            if (textPos + count <= textLen)
            {
                return text.Substring(textPos, count);
            }

            if (textLen == BufferCapacity)
            {
                ReadCharBuffer();
            }

            int i = textLen - textPos;
            if (count <= i)
            {
                return text.Substring(textPos, count);
            }
            else
            {
                return text.Substring(textPos, i);
            }
        }

        private char NextChar()
        {
            if (textPos < textLen)
            {
                position++;
                return text[textPos++];
            }

            if (textLen == BufferCapacity)
            {
                ReadCharBuffer();
                if (textPos < textLen)
                {
                    position++;
                    return text[textPos++];
                }
            }

            return EndOfTextChar;
        }

        private void ReadNext()
        {
            if (textPos < textLen)
            {
                position++;
                textPos++;
            }
            else
            {
                if (textLen == BufferCapacity)
                {
                    ReadCharBuffer();
                    position++;
                    textPos++;
                }
            }
        }

        private bool NextSymbolIs(string s)
        {
            int ln = s.Length;
            if (textLen - textPos < ln && textLen == BufferCapacity)
            {
                ReadCharBuffer();
            }

            if (textLen - textPos < ln || text[textPos] != s[0])
            {
                return false;
            }

            if (settings.CompareInfo.Compare(text, textPos, ln, s, 0, ln, CompareOptions.None) == 0)
            {
                position += ln;
                textPos += ln;
                return true;
            }

            return false;
        }

        private bool SymbolIs(string s)
        {
            int ln = s.Length;
            if (textLen - textPos < ln && textLen == BufferCapacity)
            {
                ReadCharBuffer();
            }

            if (textLen - textPos < ln || text[textPos] != s[0])
            {
                return false;
            }

            return (settings.CompareInfo.Compare(text, textPos, ln, s, 0, ln, CompareOptions.None) == 0);
        }

        private void Skip(int ofs)
        {
            if (textLen - textPos < ofs && textLen == BufferCapacity)
            {
                ReadCharBuffer();
            }

            int i = Math.Min(textLen - textPos, ofs);
            position += i;
            textPos += i;
        }

        private bool EndOfLine(char currentChar)
        {
            if (currentChar == '\r')
            {
                endLineNumber++;
                endLineBegin = position;
                currentChar = PeekChar();
                if (currentChar == '\n')
                {
                    ReadNext();
                    BufferAdd(currentChar);
                    endLineBegin = position;
                }

                return true;
            }
            else if (currentChar == '\n')
            {
                endLineNumber++;
                endLineBegin = position;

                return true;
            }

            return false;
        }

        private bool EndOfText()
        {
            if (textPos < textLen)
            {
                return false;
            }

            if (textLen == BufferCapacity)
            {
                ReadCharBuffer();
                return textPos >= textLen;
            }

            return true;
        }

        private void BufferAdd(char currentChar)
        {
            if (buffer != null)
            {
                buffer.Append(currentChar);
            }
            else if (bufBeg >= 0 && textPos >= textLen)
            {
                buffer = new StringBuilder(text, bufBeg, textPos - bufBeg, BufferCapacity);
            }
        }

        private void BufferAdd(string str)
        {
            if (buffer != null)
            {
                buffer.Append(str);
            }
            else if (bufBeg >= 0 && textPos >= textLen)
            {
                buffer = new StringBuilder(text, bufBeg, textPos - bufBeg, BufferCapacity);
            }
        }

        private void EnsureBuffer(int ofs)
        {
            if (buffer == null)
            {
                buffer = new StringBuilder(text, bufBeg, textPos - bufBeg - ofs, BufferCapacity);
            }
            else
            {
                buffer.Remove(buffer.Length - ofs, ofs);
            }
        }

        private string GetBufferValue(int ofs)
        {
            if (buffer != null)
            {
                return buffer.ToString(0, buffer.Length + ofs);
            }
            else
            {
                return text.Substring(bufBeg, textPos - bufBeg + ofs);
            }
        }

        private void ReadCharBuffer()
        {
            if (reader == null)
            {
                return;
            }

            if (tokenBuffer != null)
            {
                tokenBuffer.Append(text, 0, textPos);
            }
            else if (textBeg < textPos && (behavior & LexerBehavior.PersistTokenText) != 0)
            {
                tokenBuffer = new StringBuilder(text, textBeg, textPos - textBeg, BufferCapacity);
            }
            else
            {
                textBeg = 0;
            }

            char[] charBuffer = new char[BufferCapacity];
            if (textPos < textLen)
            {
                if (textPos == 0)
                {
                    throw new ArgumentException("'BufferCapacity' too small.");
                }
                textLen -= textPos;
                text.CopyTo(textPos, charBuffer, 0, textLen);
            }
            else
            {
                textLen = 0;
            }

            textLen += reader.Read(charBuffer, textLen, BufferCapacity - textLen);
            text = new string(charBuffer, 0, textLen);
            textPos = 0;
        }

        private string GetTokenText()
        {
            if (tokenBuffer != null)
            {
                tokenBuffer.Append(text, 0, textPos);
                return tokenBuffer.ToString(0, tokenBuffer.Length);
            }

            if ((behavior & LexerBehavior.PersistTokenText) == 0)
            {
                return null;
            }
            else
            {
                return text.Substring(textBeg, textPos - textBeg);
            }
        }

        #endregion

        #region IEnumerable<Token> Members

        IEnumerator<Token> IEnumerable<Token>.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get
            {
                return current;
            }
        }

        bool IEnumerator.MoveNext()
        {
            return GetNextToken().Type != TokenType.End;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Dispose();
            }
        }

        #endregion
    }

    public enum TokenType
    {
        Char,
        Symbol,
        Number,
        Decimal,
        Identifier,
        Keyword,
        QuotedString,
        WhiteSpace,
        EndOfLine,
        Comment,
        Start,
        End
    }

    [Flags]
    public enum LexerBehavior
    {
        SkipWhiteSpaces = 1,
        SkipComments = 2,
        PersistTokenText = 4,
        Default = PersistTokenText
    }

    [Flags]
    public enum LexerOptions
    {
        IdentIgnoreCase = 1,
        IdentToLower = 3,
        IdentToUpper = 5,
        IdentDoubleQuote = 8,
        StringEscaping = 16,
        StringDoubleQuote = 32,
        EndOfLineAsToken = 64
    }

    public sealed class Token
    {
        public readonly TokenType Type;
        public readonly object Value;
        public readonly string Text;
        public readonly int Id;
        public readonly int StartPosition;
        public readonly int EndPosition;
        public readonly int LineBegin;
        public readonly int LineNumber;
        public readonly int EndLineBegin;
        public readonly int EndLineNumber;

        public Token(TokenType type, object value, string text, int id, int startPosition, int endPosition, int lineBegin, int lineNumber, int endLineBegin, int endLineNumber)
        {
            Type = type;
            Value = value;
            Text = text;
            Id = id;
            StartPosition = startPosition;
            EndPosition = endPosition;
            LineBegin = lineBegin;
            LineNumber = lineNumber;
            EndLineBegin = endLineBegin;
            EndLineNumber = endLineNumber;
        }

        public int LinePosition
        {
            get
            {
                return StartPosition - LineBegin;
            }
        }

        public int EndLinePosition
        {
            get
            {
                return EndPosition - EndLineBegin;
            }
        }
    }

    public sealed class LexerSettings : ICloneable
    {
        public LexerOptions Options;
        public IDictionary<string, int> Symbols;
        public IDictionary<string, int> Keywords;
        public CultureInfo CultureInfo;
        public CompareInfo CompareInfo;
        public char[] StringQuotes;
        public char StringEscapeChar;
        public char StringPrefix;
        public char IdentQuote;
        public char IdentQuoteBegin;
        public char IdentQuoteEnd;
        public char IdentPrefix;
        public char[] IdentChars;
        public string[] InlineComments;
        public string CommentBegin;
        public string CommentEnd;
        public string DecimalSeparator;

        public static LexerSettings Default
        {
            get
            {
                LexerSettings settings = new LexerSettings();
                settings.CultureInfo = CultureInfo.InvariantCulture;
                settings.CompareInfo = CultureInfo.InvariantCulture.CompareInfo;
                settings.DecimalSeparator = ".";
                settings.Options = LexerOptions.IdentIgnoreCase | LexerOptions.StringDoubleQuote;
                settings.StringQuotes = new char[] { '\"', '\'' };
                settings.InlineComments = new string[] { "--" };
                settings.CommentBegin = "/*";
                settings.CommentEnd = "*/";

                return settings;
            }
        }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        public LexerSettings Clone()
        {
            LexerSettings settings = (LexerSettings)MemberwiseClone();

            if (settings.CultureInfo == null)
            {
                settings.CultureInfo = CultureInfo.InvariantCulture;
            }

            if (settings.CompareInfo == null)
            {
                settings.CompareInfo = settings.CultureInfo.CompareInfo;
            }

            if (string.IsNullOrEmpty(settings.DecimalSeparator))
            {
                settings.DecimalSeparator = settings.CultureInfo.NumberFormat.NumberDecimalSeparator;
            }

            if (settings.Symbols != null && settings.Symbols.Count > 0)
            {
                settings.Symbols = new Dictionary<string, int>(settings.Symbols);
            }
            else
            {
                settings.Symbols = null;
            }

            if (settings.Keywords != null && settings.Keywords.Count > 0)
            {
                bool ignoreCase = (settings.Options & LexerOptions.IdentIgnoreCase) != 0;
                settings.Keywords = new Dictionary<string, int>(settings.Keywords, StringComparer.Create(settings.CultureInfo, ignoreCase));
            }
            else
            {
                settings.Keywords = null;
            }

            if (settings.StringQuotes != null)
            {
                settings.StringQuotes = (char[])settings.StringQuotes.Clone();
            }

            if (settings.IdentChars != null)
            {
                settings.IdentChars = (char[])settings.IdentChars.Clone();
            }

            string[] inlineComments = settings.InlineComments;
            if (inlineComments != null)
            {
                int length = inlineComments.Length;
                int count = 0;
                for (int i = 0; i < length; i++)
                {
                    string inlineComment = inlineComments[i];
                    if (inlineComment == null)
                    {
                        continue;
                    }

                    if (i != count)
                    {
                        inlineComments[count] = inlineComment;
                    }

                    count++;
                }

                if (count == 0)
                {
                    settings.InlineComments = null;
                }
                else
                {
                    string[] arr = new string[count];
                    Array.Copy(inlineComments, 0, arr, 0, count);
                }
            }

            if (!string.IsNullOrEmpty(settings.CommentBegin) && string.IsNullOrEmpty(settings.CommentEnd))
            {
                settings.CommentEnd = settings.CommentBegin;
            }

            return settings;
        }

        #endregion
    }
}
#endif