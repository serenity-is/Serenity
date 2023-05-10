using Serenity.TypeScript.TsTypes;
using static Serenity.TypeScript.TsParser.Scanner;

namespace Serenity.TypeScript.TsParser;

public class JsDocParser
{
    public Parser Parser { get; set; }
    public JsDocParser(Parser parser)
    {
        this.Parser = parser;
    }

    private Scanner Scanner => Parser.Scanner;
    private string SourceText => Parser.SourceText;
    private SyntaxKind CurrentToken { get => Parser.CurrentToken; set => Parser.CurrentToken = value; }
    private bool ParseErrorBeforeNextFinishedNode { get => Parser.ParseErrorBeforeNextFinishedNode; set => Parser.ParseErrorBeforeNextFinishedNode = value; }
    private List<Diagnostic> ParseDiagnostics { get => Parser.ParseDiagnostics; set => Parser.ParseDiagnostics = value; }
    #region parserMethods

    private void ClearState() => Parser.ClearState();
    private void FixupParentReferences(INode rootNode) => Parser.FixupParentReferences(rootNode);
    private void ParseErrorAtCurrentToken(DiagnosticMessage message, object arg0 = null) => Parser.ParseErrorAtCurrentToken(message, arg0);
    private void ParseErrorAtPosition(int start, int length, DiagnosticMessage message, object arg0 = null) => Parser.ParseErrorAtPosition(start, length, message, arg0);
    private SyntaxKind Token() => Parser.Token();
    private SyntaxKind NextToken() => Parser.NextToken();
    private T TryParse<T>(Func<T> callback) => Parser.TryParse<T>(callback);
    private bool ParseExpected(SyntaxKind kind, DiagnosticMessage diagnosticMessage = null, bool shouldAdvance = true) => Parser.ParseExpected(kind, diagnosticMessage, shouldAdvance);
    private bool ParseOptional(SyntaxKind t) => Parser.ParseOptional(t);
    private INode ParseOptionalToken<T>(SyntaxKind t) where T : Node, new() => Parser.ParseOptionalToken<T>(t);
    private T ParseTokenNode<T>() where T : Node, new() => Parser.ParseTokenNode<T>(Token());
    private NodeArray<T> CreateList<T>(T[] elements = null, int? pos = null)      => Parser.CreateList<T>(elements, pos);
    private T FinishNode<T>(T node, int? end = null) where T : Node => Parser.FinishNode<T>(node, end);
    private Identifier ParseIdentifierName() => Parser.ParseIdentifierName();
    private NodeArray<T> ParseDelimitedList<T>(ParsingContext kind, Func<T> parseElement, bool? considerSemicolonAsDelimiter = null) where T : INode => Parser.ParseDelimitedList<T>(kind, parseElement, considerSemicolonAsDelimiter);
    private TypeLiteralNode ParseTypeLiteral() => Parser.ParseTypeLiteral();
    private IExpression ParseExpression() => Parser.ParseExpression();
    #endregion

    public bool IsJsDocType()
    {
        return Token() switch
        {
            SyntaxKind.AsteriskToken or SyntaxKind.QuestionToken or SyntaxKind.OpenParenToken or SyntaxKind.OpenBracketToken or SyntaxKind.ExclamationToken or SyntaxKind.OpenBraceToken or SyntaxKind.FunctionKeyword or SyntaxKind.DotDotDotToken or SyntaxKind.NewKeyword or SyntaxKind.ThisKeyword => true,
            _ => TokenIsIdentifierOrKeyword(Token()),
        };
    }


    public static (JsDocTypeExpression res, List<Diagnostic> diagnostics) ParseJsDocTypeExpressionForTests(string content, int? start, int? length)
    {
        var dp = new JsDocParser(new Parser());
        dp.Parser.InitializeState(content, null, ScriptKind.Js);

        var sourceFile = dp.Parser.CreateSourceFile("file.js", ScriptKind.Js);

        dp.Parser.Scanner.SetText(content, start, length);

        var currentToken = dp.Parser.Scanner.Scan();
        var jsDocTypeExpression = dp.ParseJsDocTypeExpression();
        var diagnostics = dp.Parser.ParseDiagnostics;

        dp.Parser.ClearState();


        return (jsDocTypeExpression, diagnostics);         
    }


    public JsDocTypeExpression ParseJsDocTypeExpression()
    {
        var result = new JsDocTypeExpression();


        ParseExpected(SyntaxKind.OpenBraceToken);

        result.Type = ParseJsDocTopLevelType();

        ParseExpected(SyntaxKind.CloseBraceToken);


        FixupParentReferences(result);

        return FinishNode(result);
    }


    public IJsDocType ParseJsDocTopLevelType()
    {
        var type = ParseJsDocType();
        if (Token() == SyntaxKind.BarToken)
        {
            var unionType = new JsDocUnionType {Types = ParseJsDocTypeList(type)};


            type = FinishNode(unionType);
        }
        if (Token() == SyntaxKind.EqualsToken)
        {
            var optionalType = new JsDocOptionalType();

            NextToken();

            optionalType.Type = type;

            type = FinishNode(optionalType);
        }


        return type;
    }


    public IJsDocType ParseJsDocType()
    {
        var type = ParseBasicTypeExpression();
        while (true)
        {
            if (Token() == SyntaxKind.OpenBracketToken)
            {
                var arrayType = new JsDocArrayType {ElementType = type};



                NextToken();

                ParseExpected(SyntaxKind.CloseBracketToken);


                type = FinishNode(arrayType);
            }
            else
        if (Token() == SyntaxKind.QuestionToken)
            {
                var nullableType = new JsDocNullableType {Type = type};



                NextToken();

                type = FinishNode(nullableType);
            }
            else
        if (Token() == SyntaxKind.ExclamationToken)
            {
                var nonNullableType = new JsDocNonNullableType {Type = type};



                NextToken();

                type = FinishNode(nonNullableType);
            }
            else
            {

                break;
            }
        }


        return type;
    }


    public IJsDocType ParseBasicTypeExpression()
    {
        return Token() switch
        {
            SyntaxKind.AsteriskToken => ParseJsDocAllType(),
            SyntaxKind.QuestionToken => ParseJsDocUnknownOrNullableType(),
            SyntaxKind.OpenParenToken => ParseJsDocUnionType(),
            SyntaxKind.OpenBracketToken => ParseJsDocTupleType(),
            SyntaxKind.ExclamationToken => ParseJsDocNonNullableType(),
            SyntaxKind.OpenBraceToken => ParseJsDocRecordType(),
            SyntaxKind.FunctionKeyword => ParseJsDocFunctionType(),
            SyntaxKind.DotDotDotToken => ParseJsDocVariadicType(),
            SyntaxKind.NewKeyword => ParseJsDocConstructorType(),
            SyntaxKind.ThisKeyword => ParseJsDocThisType(),
            SyntaxKind.AnyKeyword or SyntaxKind.StringKeyword or SyntaxKind.NumberKeyword or SyntaxKind.BooleanKeyword or SyntaxKind.SymbolKeyword or SyntaxKind.VoidKeyword or SyntaxKind.NullKeyword or SyntaxKind.UndefinedKeyword or SyntaxKind.NeverKeyword or SyntaxKind.ObjectKeyword => ParseTokenNode<JsDocType>(),
            SyntaxKind.StringLiteral or SyntaxKind.NumericLiteral or SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword => ParseJsDocLiteralType(),
            _ => ParseJsDocTypeReference(),
        };
    }


    public JsDocThisType ParseJsDocThisType()
    {
        var result = new JsDocThisType();

        NextToken();

        ParseExpected(SyntaxKind.ColonToken);

        result.Type = ParseJsDocType();

        return FinishNode(result);
    }


    public JsDocConstructorType ParseJsDocConstructorType()
    {
        var result = new JsDocConstructorType();

        NextToken();

        ParseExpected(SyntaxKind.ColonToken);

        result.Type = ParseJsDocType();

        return FinishNode(result);
    }


    public JsDocVariadicType ParseJsDocVariadicType()
    {
        var result = new JsDocVariadicType();

        NextToken();

        result.Type = ParseJsDocType();

        return FinishNode(result);
    }


    public JsDocFunctionType ParseJsDocFunctionType()
    {
        var result = new JsDocFunctionType();

        NextToken();


        ParseExpected(SyntaxKind.OpenParenToken);

        result.Parameters = Parser.ParseDelimitedList(ParsingContext.JSDocFunctionParameters, ParseJsDocParameter);

        CheckForTrailingComma(result.Parameters);

        ParseExpected(SyntaxKind.CloseParenToken);
        if (Token() == SyntaxKind.ColonToken)
        {

            NextToken();

            result.Type = ParseJsDocType();
        }


        return FinishNode(result);
    }


    public ParameterDeclaration ParseJsDocParameter()
    {
        var parameter = new ParameterDeclaration {Type = ParseJsDocType()};

        if (ParseOptional(SyntaxKind.EqualsToken))
        {

            parameter.QuestionToken = new QuestionToken { }; 
        }

        return FinishNode(parameter);
    }


    public JsDocTypeReference ParseJsDocTypeReference()
    {
        var result = new JsDocTypeReference {Name = Parser.ParseSimplePropertyName() as Identifier};

        if (Token() == SyntaxKind.LessThanToken)
        {

            result.TypeArguments = ParseTypeArguments();
        }
        else
        {
            while (ParseOptional(SyntaxKind.DotToken))
            {
                if (Token() == SyntaxKind.LessThanToken)
                {

                    result.TypeArguments = ParseTypeArguments();

                    break;
                }
                else
                {

                    result.Name = ParseQualifiedName(result.Name);
                }
            }
        }



        return FinishNode(result);
    }


    public NodeArray<IJsDocType> ParseTypeArguments()
    {

        NextToken();
        var typeArguments = ParseDelimitedList(ParsingContext.JSDocTypeArguments, ParseJsDocType);

        CheckForTrailingComma(typeArguments);

        CheckForEmptyTypeArgumentList(typeArguments);

        ParseExpected(SyntaxKind.GreaterThanToken);


        return typeArguments;
    }


    public void CheckForEmptyTypeArgumentList<T>(NodeArray<T> typeArguments)
    {
        if (!ParseDiagnostics.Any() && typeArguments != null && !typeArguments.Any())
        {
            var start = (typeArguments.Pos ?? 0) - "<".Length;
            var end = SkipTriviaM(SourceText, (int)typeArguments.End) + ">".Length;

            ParseErrorAtPosition(start, end - start, Diagnostics.Type_argument_list_cannot_be_empty);
        }
    }


    public QualifiedName ParseQualifiedName(IEntityName left)
    {
        var result = new QualifiedName
        {
            Left = left,
            Right = ParseIdentifierName()
        };




        return FinishNode(result);
    }


    public JsDocRecordType ParseJsDocRecordType()
    {
        var result = new JsDocRecordType {Literal = ParseTypeLiteral()};


        return FinishNode(result);
    }


    public JsDocNonNullableType ParseJsDocNonNullableType()
    {
        var result = new JsDocNonNullableType();

        NextToken();

        result.Type = ParseJsDocType();

        return FinishNode(result);
    }


    public JsDocTupleType ParseJsDocTupleType()
    {
        var result = new JsDocTupleType();

        NextToken();

        result.Types = ParseDelimitedList(ParsingContext.JSDocTupleTypes, ParseJsDocType);

        CheckForTrailingComma(result.Types);

        ParseExpected(SyntaxKind.CloseBracketToken);


        return FinishNode(result);
    }


    public void CheckForTrailingComma<T>(NodeArray<T> list)
    {
        if (Parser.ParseDiagnostics.Count == 0 && list.HasTrailingComma)
        {
            var start = list.End - ",".Length;

            Parser.ParseErrorAtPosition((int)start, ",".Length, Diagnostics.Trailing_comma_not_allowed);
        }
    }


    public JsDocUnionType ParseJsDocUnionType()
    {
        var result = new JsDocUnionType();

        NextToken();

        result.Types = ParseJsDocTypeList(ParseJsDocType());


        ParseExpected(SyntaxKind.CloseParenToken);


        return FinishNode(result);
    }


    public NodeArray<IJsDocType> ParseJsDocTypeList(IJsDocType firstType)
    {

        var types = Parser.CreateList<IJsDocType>();  
        types.Add(firstType);
        types.Pos = firstType.Pos;
        while (ParseOptional(SyntaxKind.BarToken))
        {

            types.Add(ParseJsDocType());
        }


        types.End = Scanner.StartPos;

        return types;
    }


    public JsDocAllType ParseJsDocAllType()
    {
        var result = new JsDocAllType();

        NextToken();

        return FinishNode(result);
    }


    public JsDocLiteralType ParseJsDocLiteralType()
    {
        var result = new JsDocLiteralType {Literal = Parser.ParseLiteralTypeNode()};


        return FinishNode(result);
    }


    public   JsDocType ParseJsDocUnknownOrNullableType()
    {
        var pos = Scanner.StartPos;

        NextToken();
        if (Token() == SyntaxKind.CommaToken ||
                            Token() == SyntaxKind.CloseBraceToken ||
                            Token() == SyntaxKind.CloseParenToken ||
                            Token() == SyntaxKind.GreaterThanToken ||
                            Token() == SyntaxKind.EqualsToken ||
                            Token() == SyntaxKind.BarToken)
        {
            var result = new JsDocUnknownType();

            return FinishNode(result);
        }
        else
        {
            var result = new JsDocNullableType {Type = ParseJsDocType()};


            return FinishNode(result);
        }
    }


    public Tuple<JsDoc, List<Diagnostic>> ParseIsolatedJsDocComment(string content, int start, int length)
    {
        Parser ??= new Parser();
        Parser.InitializeState(content, null, ScriptKind.Js);

        Parser.SourceFile = new SourceFile { LanguageVariant = LanguageVariant.Standard, Text = content };
        var jsDoc = ParseJsDocCommentWorker(start, length);
        var diagnostics = ParseDiagnostics;

        ClearState();


        return jsDoc != null ? Tuple.Create(jsDoc, diagnostics) : null;
    }


    public JsDoc ParseJsDocComment(INode parent, int? start, int? length)
    {
        var saveToken = CurrentToken;
        var saveParseDiagnosticsLength = Parser.ParseDiagnostics.Count;
        var saveParseErrorBeforeNextFinishedNode = ParseErrorBeforeNextFinishedNode;
        var comment = ParseJsDocCommentWorker(start, length);
        if (comment != null)
        {

            comment.Parent = parent;
        }


        CurrentToken = saveToken;

        ParseDiagnostics = ParseDiagnostics.Take(saveParseDiagnosticsLength).ToList();

        ParseErrorBeforeNextFinishedNode = saveParseErrorBeforeNextFinishedNode;


        return comment;
    }


    public JsDoc ParseJsDocCommentWorker(int? start = null, int? length = null)
    {
        var content = Parser.SourceText;

        start = start ?? 0;
        var end = length == null ? content.Length : start + length;

        length = end - start;


        Debug.Assert(start >= 0);

        Debug.Assert(start <= end);

        Debug.Assert(end <= content.Length);
        NodeArray<IJsDocTag> tags = new NodeArray<IJsDocTag>();
        List<string> comments = new List<string>();
        JsDoc result = null;
        if (!IsJsDocStart(content, (int)start))
        {

            return result;
        }


        Scanner.ScanRange<JsDoc>(start + 3, (length ?? 0) - 5, () =>
       {
           var advanceToken = true;
           var state = JSDocState.SawAsterisk;
           int? margin = null;
           var indent = start - Math.Max(content.LastIndexOf('\n', (int)start), 0) + 4;


           NextJsDocToken();
           while (Token() == SyntaxKind.WhitespaceTrivia)
           {

               NextJsDocToken();
           }
           if (Token() == SyntaxKind.NewLineTrivia)
           {

               state = JSDocState.BeginningOfLine;

               indent = 0;

               NextJsDocToken();
           }
           while (Token() != SyntaxKind.EndOfFileToken)
           {
               switch (Token())
               {
                   case SyntaxKind.AtToken:
                       if (state == JSDocState.BeginningOfLine || state == JSDocState.SawAsterisk)
                       {

                           RemoveTrailingNewlines(comments);

                           ParseTag((int)indent);

                           state = JSDocState.BeginningOfLine;

                           advanceToken = false;

                           margin = null;

                           indent++;
                       }
                       else
                       {

                           PushComment(Scanner.TokenText);
                       }

                       break;
                   case SyntaxKind.NewLineTrivia:

                       comments.Add(Scanner.TokenText);

                       state = JSDocState.BeginningOfLine;

                       indent = 0;

                       break;
                   case SyntaxKind.AsteriskToken:
                       var asterisk = Scanner.TokenText;
                       if (state == JSDocState.SawAsterisk || state == JSDocState.SavingComments)
                       {

                           state = JSDocState.SavingComments;

                           PushComment(asterisk);
                       }
                       else
                       {

                           state = JSDocState.SawAsterisk;

                           indent += asterisk.Length;
                       }

                       break;
                   case SyntaxKind.Identifier:

                       PushComment(Scanner.TokenText);

                       state = JSDocState.SavingComments;

                       break;
                   case SyntaxKind.WhitespaceTrivia:
                       var whitespace = Scanner.TokenText;
                       if (state == JSDocState.SavingComments)
                       {

                           comments.Add(whitespace);
                       }
                       else
                       if (margin != null && (indent ?? 0) + whitespace.Length > margin)
                       {

                           comments.Add(whitespace.Slice((int)margin - (indent ?? 0) - 1));
                       }

                       indent += whitespace.Length;

                       break;
                   case SyntaxKind.EndOfFileToken:

                       break;
                   default:

                       state = JSDocState.SavingComments;

                       PushComment(Scanner.TokenText);

                       break;
               }
               if (advanceToken)
               {

                   NextJsDocToken();
               }
               else
               {

                   advanceToken = true;
               }
           }

           RemoveLeadingNewlines(comments);

           RemoveTrailingNewlines(comments);

           result = CreateJsDocComment();
           return result;
           void PushComment(string text)
           {
               margin ??= indent;

               comments.Add(text);

               indent += text.Length;
           }
       }
);

        return result;






        void RemoveLeadingNewlines(List<string> comments3)
        {
            while (comments3.Any() && (comments3[0] == "\n" || comments3[0] == "\r"))
            {

                comments3 = comments3.Skip(1).ToList();  
            }
        }


        void RemoveTrailingNewlines(List<string> comments2)
        {
            while (comments2.Any() && (comments2[comments2.Count() - 1] == "\n" || comments2[comments2.Count() - 1] == "\r"))
            {

                comments2.Pop();
            }
        }


        bool IsJsDocStart(string content2, int start2)
        {

            return content2[start2] == '/' &&
                content2[start2 + 1] == '*' &&
                content2[start2 + 2] == '*' &&
                content2[start2 + 3] != '*';
        }


        JsDoc CreateJsDocComment()
        {
            var result2 = new JsDoc
            {
                Tags = tags,
                Comment = comments.Any() ? string.Join("", comments) : null
            };



            return FinishNode(result2, end);
        }


        void SkipWhitespace()
        {
            while (Token() == SyntaxKind.WhitespaceTrivia || Token() == SyntaxKind.NewLineTrivia)
            {

                NextJsDocToken();
            }
        }


        void ParseTag(int indent)
        {

            Debug.Assert(Token() == SyntaxKind.AtToken);
            var atToken = new AtToken {End = Scanner.TextPos};


            NextJsDocToken();
            var tagName = ParseJsDocIdentifierName();

            SkipWhitespace();
            if (tagName == null)
            {

                return;
            }
            IJsDocTag tag = null;
            if (tagName != null)
            {
                tag = tagName.Text switch
                {
                    "augments" => ParseAugmentsTag(atToken, tagName),
                    "param" => ParseParamTag(atToken, tagName),
                    "return" or "returns" => ParseReturnTag(atToken, tagName),
                    "template" => ParseTemplateTag(atToken, tagName),
                    "type" => ParseTypeTag(atToken, tagName),
                    "typedef" => ParseTypedefTag(atToken, tagName),
                    _ => ParseUnknownTag(atToken, tagName),
                };
            }
            else
            {

                tag = ParseUnknownTag(atToken, tagName);
            }
            if (tag == null)
            {

                return;
            }

            AddTag(tag, ParseTagComments(indent + (tag.End ?? 0) - (tag.Pos ?? 0)));
        }


        List<string> ParseTagComments(int indent)
        {
            List<string> comments2 = new List<string>();
            var state = JSDocState.SawAsterisk;
            int? margin = null;
            while (Token() != SyntaxKind.AtToken && Token() != SyntaxKind.EndOfFileToken)
            {
                switch (Token())
                {
                    case SyntaxKind.NewLineTrivia:
                        if (state >= JSDocState.SawAsterisk)
                        {

                            state = JSDocState.BeginningOfLine;

                            comments2.Add(Scanner.TokenText);
                        }

                        indent = 0;

                        break;
                    case SyntaxKind.AtToken:

                        break;
                    case SyntaxKind.WhitespaceTrivia:
                        if (state == JSDocState.SavingComments)
                        {

                            PushComment(Scanner.TokenText);
                        }
                        else
                        {
                            var whitespace = Scanner.TokenText;
                            if (margin != null && indent + whitespace.Length > margin)
                            {

                                comments2.Add(whitespace.Slice((int)margin - indent - 1));
                            }

                            indent += whitespace.Length;
                        }

                        break;
                    case SyntaxKind.AsteriskToken:
                        if (state == JSDocState.BeginningOfLine)
                        {

                            state = JSDocState.SawAsterisk;

                            indent += Scanner.TokenText.Length;

                            break;
                        }
                        goto caseLabel5;
                    default:

                        caseLabel5: state = JSDocState.SavingComments;
                        PushComment(Scanner.TokenText);

                        break;
                }
                if (Token() == SyntaxKind.AtToken)
                {

                    break;
                }

                NextJsDocToken();
            }


            RemoveLeadingNewlines(comments2);

            RemoveTrailingNewlines(comments2);

            return comments2;
            void PushComment(string text)
            {
                margin ??= indent;

                comments2.Add(text);

                indent += text.Length;
            }
        }





        JsDocTag ParseUnknownTag(AtToken atToken, Identifier tagName)
        {
            var result2 = new JsDocTag
            {
                AtToken = atToken,
                TagName = tagName
            };



            return FinishNode(result2);
        }


        void AddTag(IJsDocTag tag, List<string> comments2)
        {

            tag.Comment = string.Join("", comments2);
            if (tags == null)
            {

                tags = Parser.CreateList<IJsDocTag>();
                tags.Pos = tag.Pos;

            }
            else
            {

                tags.Add(tag);
            }

            tags.End = tag.End;
        }


        JsDocTypeExpression TryParseTypeExpression()
        {

            return TryParse(() =>
            {
                SkipWhitespace();
                if (Token() != SyntaxKind.OpenBraceToken)
                {
                    return null;
                }

                return ParseJsDocTypeExpression();
            });
        }


        JsDocParameterTag ParseParamTag(AtToken atToken, Identifier tagName)
        {
            var typeExpression = TryParseTypeExpression();

            SkipWhitespace();
            Identifier name = null;
            bool isBracketed = false;
            if ((OpenBracketToken)ParseOptionalToken<OpenBracketToken>(SyntaxKind.OpenBracketToken) != null)
            {

                name = ParseJsDocIdentifierName();

                SkipWhitespace();

                isBracketed = true;
                if ((EqualsToken)ParseOptionalToken<EqualsToken>(SyntaxKind.EqualsToken) != null)
                {

                    ParseExpression();
                }


                ParseExpected(SyntaxKind.CloseBracketToken);
            }
            else
        if (TokenIsIdentifierOrKeyword(Token()))
            {

                name = ParseJsDocIdentifierName();
            }
            if (name == null)
            {

                ParseErrorAtPosition(Scanner.StartPos, 0, Diagnostics.Identifier_expected);

                return null;
            }
            Identifier preName = null;
            Identifier postName = null;
            if (typeExpression != null)
            {

                postName = name;
            }
            else
            {

                preName = name;
            }
            typeExpression ??= TryParseTypeExpression();
            var result4 = new JsDocParameterTag
            {
                AtToken = atToken,
                TagName = tagName,
                PreParameterName = preName,
                TypeExpression = typeExpression,
                PostParameterName = postName,
                ParameterName = postName ?? preName,
                IsBracketed = isBracketed
            };








            return FinishNode(result4);
        }


        JsDocReturnTag ParseReturnTag(AtToken atToken, Identifier tagName)
        {
            if (tags.Any(t => t.Kind == SyntaxKind.JsDocReturnTag))
            {

                ParseErrorAtPosition(tagName.Pos ?? 0, Scanner.TokenPos - (tagName.Pos ?? 0), Diagnostics._0_tag_already_specified, tagName.Text);
            }
            var result5 = new JsDocReturnTag
            {
                AtToken = atToken,
                TagName = tagName,
                TypeExpression = TryParseTypeExpression()
            };




            return FinishNode(result5);
        }


        JsDocTypeTag ParseTypeTag(AtToken atToken, Identifier tagName)
        {
            if (tags.Any(t => t.Kind == SyntaxKind.JsDocTypeTag))
            {

                ParseErrorAtPosition(tagName.Pos ?? 0, Scanner.TokenPos - (tagName.Pos ?? 0), Diagnostics._0_tag_already_specified, tagName.Text);
            }
            var result6 = new JsDocTypeTag
            {
                AtToken = atToken,
                TagName = tagName,
                TypeExpression = TryParseTypeExpression()
            };




            return FinishNode(result6);
        }


        JsDocPropertyTag ParsePropertyTag(AtToken atToken, Identifier tagName)
        {
            var typeExpression = TryParseTypeExpression();

            SkipWhitespace();
            var name = ParseJsDocIdentifierName();

            SkipWhitespace();
            if (name == null)
            {

                ParseErrorAtPosition(Scanner.StartPos,  0, Diagnostics.Identifier_expected);

                return null;
            }
            var result7 = new JsDocPropertyTag
            {
                AtToken = atToken,
                TagName = tagName,
                Name = name,
                TypeExpression = typeExpression
            };





            return FinishNode(result7);
        }


        JsDocAugmentsTag ParseAugmentsTag(AtToken atToken, Identifier tagName)
        {
            var typeExpression = TryParseTypeExpression();
            var result8 = new JsDocAugmentsTag
            {
                AtToken = atToken,
                TagName = tagName,
                TypeExpression = typeExpression
            };




            return FinishNode(result8);
        }


        IJsDocTag ParseTypedefTag(AtToken atToken, Identifier tagName)
        {
            var typeExpression = TryParseTypeExpression();

            SkipWhitespace();
            var typedefTag = new JsDocTypedefTag
            {
                AtToken = atToken,
                TagName = tagName,
                FullName = ParseJsDocTypeNameWithNamespace(0)
            };



            if (typedefTag.FullName != null)
            {
                var rightNode = typedefTag.FullName;
                while (true)
                {
                    if ((SyntaxKind)rightNode.Kind == SyntaxKind.Identifier || (rightNode as JsDocNamespaceDeclaration)?.Body == null)
                    {

                        typedefTag.Name = (SyntaxKind)rightNode.Kind == SyntaxKind.Identifier ? rightNode : (rightNode as JsDocTypedefTag)?.Name;

                        break;
                    }

                    rightNode = (rightNode as JsDocNamespaceDeclaration)?.Body;
                }
            }

            typedefTag.TypeExpression = typeExpression;

            SkipWhitespace();
            if (typeExpression != null)
            {
                if ((SyntaxKind)typeExpression.Type.Kind == SyntaxKind.JsDocTypeReference)
                {
                    var jsDocTypeReference = (JsDocTypeReference)typeExpression.Type;
                    if ((SyntaxKind)jsDocTypeReference.Name.Kind == SyntaxKind.Identifier)
                    {
                        Identifier name = jsDocTypeReference.Name as Identifier;
                        if (name?.Text == "Object")
                        {

                            typedefTag.JsDocTypeLiteral = ScanChildTags();
                        }
                    }
                }
                typedefTag.JsDocTypeLiteral ??= (JsDocTypeLiteral)typeExpression.Type;
            }
            else
            {

                typedefTag.JsDocTypeLiteral = ScanChildTags();
            }


            return FinishNode(typedefTag);
        }


        JsDocTypeLiteral ScanChildTags()
        {
            var jsDocTypeLiteral = new JsDocTypeLiteral();
            var resumePos = Scanner.StartPos;
            var canParseTag = true;
            var seenAsterisk = false;
            var parentTagTerminated = false;
            while (Token() != SyntaxKind.EndOfFileToken && !parentTagTerminated)
            {

                NextJsDocToken();
                switch (Token())
                {
                    case SyntaxKind.AtToken:
                        if (canParseTag)
                        {

                            parentTagTerminated = !TryParseChildTag(jsDocTypeLiteral);
                            if (!parentTagTerminated)
                            {

                                resumePos = Scanner.StartPos;
                            }
                        }

                        seenAsterisk = false;

                        break;
                    case SyntaxKind.NewLineTrivia:

                        resumePos = Scanner.StartPos - 1;

                        canParseTag = true;

                        seenAsterisk = false;

                        break;
                    case SyntaxKind.AsteriskToken:
                        if (seenAsterisk)
                        {

                            canParseTag = false;
                        }

                        seenAsterisk = true;

                        break;
                    case SyntaxKind.Identifier:

                        canParseTag = false;
                        goto caseLabel5;
                    case SyntaxKind.EndOfFileToken:
                        caseLabel5:
                        break;
                }
            }

            Scanner.TextPos = resumePos;

            return FinishNode(jsDocTypeLiteral);
        }


        INode ParseJsDocTypeNameWithNamespace(NodeFlags flags)
        {
            var pos = Scanner.TokenPos;
            var typeNameOrNamespaceName = ParseJsDocIdentifierName();
            if (typeNameOrNamespaceName != null && ParseOptional(SyntaxKind.DotToken))
            {
                var jsDocNamespaceNode = new JsDocNamespaceDeclaration();

                jsDocNamespaceNode.Flags |= flags;

                jsDocNamespaceNode.Name = typeNameOrNamespaceName;

                jsDocNamespaceNode.Body = ParseJsDocTypeNameWithNamespace(NodeFlags.NestedNamespace);

                return jsDocNamespaceNode;
            }
            if (typeNameOrNamespaceName != null && (flags & NodeFlags.NestedNamespace) != 0)
            {

                typeNameOrNamespaceName.IsInJsDocNamespace = true;
            }

            return typeNameOrNamespaceName;
        }


        bool TryParseChildTag(JsDocTypeLiteral parentTag)
        {

            Debug.Assert(Token() == SyntaxKind.AtToken);
            var atToken = new AtToken {End = Scanner.TextPos};


            NextJsDocToken();
            var tagName = ParseJsDocIdentifierName();

            SkipWhitespace();
            if (tagName == null)
            {

                return false;
            }
            switch (tagName.Text)
            {
                case "type":
                    if (parentTag.JsDocTypeTag != null)
                    {

                        return false;
                    }

                    parentTag.JsDocTypeTag = ParseTypeTag(atToken, tagName);

                    return true;
                case "prop":
                case "property":
                    var propertyTag = ParsePropertyTag(atToken, tagName);
                    if (propertyTag != null)
                    {
                        parentTag.JsDocPropertyTags ??= new NodeArray<JsDocPropertyTag>();

                        parentTag.JsDocPropertyTags.Add(propertyTag);

                        return true;
                    }

                    return false;
            }

            return false;
        }


        JsDocTemplateTag ParseTemplateTag(AtToken atToken, Identifier tagName)
        {
            if (tags.Any(t => t.Kind == SyntaxKind.JsDocTemplateTag))
            {

                ParseErrorAtPosition(tagName.Pos ?? 0, Scanner.TokenPos - (tagName.Pos ?? 0), Diagnostics._0_tag_already_specified, tagName.Text);
            }
            var typeParameters = CreateList<TypeParameterDeclaration>();
            while (true)
            {
                var name = ParseJsDocIdentifierName();

                SkipWhitespace();
                if (name == null)
                {

                    ParseErrorAtPosition(Scanner.StartPos, 0, Diagnostics.Identifier_expected);

                    return null;
                }
                var typeParameter = new TypeParameterDeclaration {Name = name};


                FinishNode(typeParameter);


                typeParameters.Add(typeParameter);
                if (Token() == SyntaxKind.CommaToken)
                {

                    NextJsDocToken();

                    SkipWhitespace();
                }
                else
                {

                    break;
                }
            }
            var result3 = new JsDocTemplateTag
            {
                AtToken = atToken,
                TagName = tagName,
                TypeParameters = typeParameters
            };




            FinishNode(result3);

            typeParameters.End = result3.End;

            return result3;
        }


        SyntaxKind NextJsDocToken()
        {
            CurrentToken = Scanner.ScanJsDocToken();
            return CurrentToken;
        }


        Identifier ParseJsDocIdentifierName()
        {

            return CreateJsDocIdentifier(TokenIsIdentifierOrKeyword(Token()));
        }


        Identifier CreateJsDocIdentifier(bool isIdentifier)
        {
            if (!isIdentifier)
            {

                ParseErrorAtCurrentToken(Diagnostics.Identifier_expected);

                return null;
            }
            var pos = Scanner.TokenPos;
            var end2 = Scanner.TextPos;
            var result9 = new Identifier { Text = content[pos..end2] };


            FinishNode(result9, end2);


            NextJsDocToken();

            return result9;
        }
    }
}
