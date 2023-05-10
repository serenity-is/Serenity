using Serenity.TypeScript.TsTypes;
using static Serenity.TypeScript.TsParser.Core;
using static Serenity.TypeScript.TsParser.Scanner;
using static Serenity.TypeScript.TsParser.Ts;
using static Serenity.TypeScript.TsParser.Utilities;

namespace Serenity.TypeScript.TsParser;

public class Parser
{

    public Scanner Scanner = new(LanguageVariant.Standard, null);
    public NodeFlags DisallowInAndDecoratorContext = NodeFlags.DisallowInContext | NodeFlags.DecoratorContext;

    public NodeFlags ContextFlags;
    public bool ParseErrorBeforeNextFinishedNode = false;
    public SourceFile SourceFile;
    public List<Diagnostic> ParseDiagnostics;
    public /*IncrementalParser.SyntaxCursor*/object SyntaxCursor;

    public SyntaxKind CurrentToken;
    public string SourceText;
    public int NodeCount;

    public int ParsingContext; //ParsingContext
    public JsDocParser JsDocParser;
    public Parser()
    {
        JsDocParser = new JsDocParser(this);
    }
    //public (JSDocTypeExpression res, List<Diagnostic> diagnostics) parseJSDocTypeExpressionForTests(string content, int? start = null, int? length = null)
    //{

    //    return JSDocParser.parseJSDocTypeExpressionForTests(content, start, length);
    //}

    public bool Optimized { get; set; }


    public SourceFile ParseSourceFile(string fileName, string sourceText, /*IncrementalParser.SyntaxCursor*/object syntaxCursor, bool setParentNodes, ScriptKind scriptKind)
    {

        scriptKind = EnsureScriptKind(fileName, scriptKind);


        InitializeState(sourceText, syntaxCursor, scriptKind);
        var result = ParseSourceFileWorker(fileName, setParentNodes, scriptKind);


        ClearState();


        return result;
    }


    public IEntityName ParseIsolatedEntityName(string content)
    {

        InitializeState(content, /*syntaxCursor*/ null, ScriptKind.Js);

        // Prime the scanner.
        NextToken();
        var entityName = ParseEntityName(/*allowReservedWords*/ true);
        var isInvalid = Token() == SyntaxKind.EndOfFileToken && !ParseDiagnostics.Any();

        ClearState();

        return isInvalid ? entityName : null;
    }


    public LanguageVariant GetLanguageVariant(ScriptKind scriptKind)
    {

        // .tsx and .jsx files are treated as jsx language variant.
        return scriptKind == ScriptKind.Tsx || scriptKind == ScriptKind.Jsx || scriptKind == ScriptKind.Js ? LanguageVariant.Jsx : LanguageVariant.Standard;
    }


    public void InitializeState(string _sourceText, /*IncrementalParser.SyntaxCursor*/object _syntaxCursor, ScriptKind scriptKind)
    {

        //NodeConstructor = objectAllocator.getNodeConstructor();

        //TokenConstructor = objectAllocator.getTokenConstructor();

        //IdentifierConstructor = objectAllocator.getIdentifierConstructor();

        //SourceFileConstructor = objectAllocator.getSourceFileConstructor();


        SourceText = _sourceText;

        SyntaxCursor = _syntaxCursor;


        ParseDiagnostics = new List<Diagnostic>(); // [];

        ParsingContext = 0;

        ContextFlags = scriptKind == ScriptKind.Js || scriptKind == ScriptKind.Jsx ? NodeFlags.JavaScriptFile : NodeFlags.None;

        ParseErrorBeforeNextFinishedNode = false;


        // Initialize and prime the scanner before parsing the source elements.
        Scanner.SetText(SourceText);

        Scanner.OnError += ScanError; //.setOnError(scanError);

        Scanner.SetLanguageVariant(GetLanguageVariant(scriptKind));
    }


    public void ClearState()
    {

        // Clear out the text the scanner is pointing at, so it doesn't keep anything alive unnecessarily.
        Scanner.SetText("");

        Scanner.SetOnError(null);


        // Clear any data.  We don't want to accidentally hold onto it for too long.
        ParseDiagnostics = null;

        SourceFile = null;

        SyntaxCursor = null;

        SourceText = null;
    }


    public SourceFile ParseSourceFileWorker(string fileName, bool setParentNodes, ScriptKind scriptKind)
    {

        SourceFile = CreateSourceFile(fileName, scriptKind);

        SourceFile.Flags = ContextFlags;

        // Prime the scanner.
        NextToken();

        SourceFile.Statements = ParseList2(TsTypes.ParsingContext.SourceElements, ParseStatement);

        Debug.Assert(Token() == SyntaxKind.EndOfFileToken);

        SourceFile.EndOfFileToken = ParseTokenNode<EndOfFileToken>(Token());

        SetExternalModuleIndicator(SourceFile);

        SourceFile.ParseDiagnostics = ParseDiagnostics;

        if (setParentNodes)
            FixupParentReferences(SourceFile);

        return SourceFile;
    }

    public T AddJsDocComment<T>(T node) where T : INode
    {
        if (Optimized)
            return node;

        var comments = GetJsDocCommentRanges(node, SourceFile.Text);
        if (comments.Any())
        {
            foreach (var comment in comments)
            {
                var jsDoc = JsDocParser.ParseJsDocComment(node, comment.Pos, comment.End - comment.Pos);
                if (jsDoc == null)
                {

                    continue;
                }
                node.JsDoc ??= new List<JsDoc>();

                node.JsDoc.Add(jsDoc);
            }
        }

        return node;
    }


    public void FixupParentReferences(INode rootNode)
    {
        INode parent = rootNode;

        ForEachChild(rootNode, visitNode);

        return;
        INode visitNode(INode n)
        {
            if (n.Parent != parent)
            {

                n.Parent = parent;
                var saveParent = parent;

                parent = n;

                ForEachChild(n, visitNode);
                if (n.JsDoc != null)
                {
                    foreach (var jsDoc in n.JsDoc)
                    {

                        jsDoc.Parent = n;

                        parent = jsDoc;

                        ForEachChild(jsDoc, visitNode);
                    }
                }

                parent = saveParent;
            }
            return null;
        }

    }



    public SourceFile CreateSourceFile(string fileName, ScriptKind scriptKind)
    {
        var sourceFile = new SourceFile
        {
            Pos = 0,
            End = SourceText.Length,
            Text = SourceText,

            BindDiagnostics = new List<Diagnostic>(),

            FileName = Optimized ? fileName : NormalizePath(fileName),

            LanguageVariant = GetLanguageVariant(scriptKind)
        };

        sourceFile.IsDeclarationFile = FileExtensionIs(sourceFile.FileName, ".d.ts");

        sourceFile.ScriptKind = scriptKind;


        return sourceFile;
    }


    public void SetContextFlag(bool val, NodeFlags flag)
    {
        if (val)
        {

            ContextFlags |= flag;
        }
        else
        {

            ContextFlags &= ~flag;
        }
    }


    public void SetDisallowInContext(bool val)
    {

        SetContextFlag(val, NodeFlags.DisallowInContext);
    }


    public void SetYieldContext(bool val)
    {

        SetContextFlag(val, NodeFlags.YieldContext);
    }


    public void SetDecoratorContext(bool val)
    {

        SetContextFlag(val, NodeFlags.DecoratorContext);
    }


    public void SetAwaitContext(bool val)
    {

        SetContextFlag(val, NodeFlags.AwaitContext);
    }


    public T DoOutsideOfContext<T>(NodeFlags context, Func<T> func)
    {
        var contextFlagsToClear = context & ContextFlags;
        if (contextFlagsToClear != 0)
        {

            // clear the requested context flags
            SetContextFlag(/*val*/ false, contextFlagsToClear);
            var result = func();

            // restore the context flags we just cleared
            SetContextFlag(/*val*/ true, contextFlagsToClear);

            return result;
        }


        // no need to do anything special as we are not in any of the requested contexts
        return func();
    }


    public T DoInsideOfContext<T>(NodeFlags context, Func<T> func)
    {
        var contextFlagsToSet = context & ~ContextFlags;
        if (contextFlagsToSet != 0)
        {

            // set the requested context flags
            SetContextFlag(/*val*/ true, contextFlagsToSet);
            var result = func();

            // reset the context flags we just set
            SetContextFlag(/*val*/ false, contextFlagsToSet);

            return result;
        }


        // no need to do anything special as we are already in all of the requested contexts
        return func();
    }


    public T AllowInAnd<T>(Func<T> func)
    {

        return DoOutsideOfContext(NodeFlags.DisallowInContext, func);
    }


    public T DisallowInAnd<T>(Func<T> func)
    {

        return DoInsideOfContext(NodeFlags.DisallowInContext, func);
    }


    public T DoInYieldContext<T>(Func<T> func)
    {

        return DoInsideOfContext(NodeFlags.YieldContext, func);
    }


    public T DoInDecoratorContext<T>(Func<T> func)
    {

        return DoInsideOfContext(NodeFlags.DecoratorContext, func);
    }


    public T DoInAwaitContext<T>(Func<T> func)
    {

        return DoInsideOfContext(NodeFlags.AwaitContext, func);
    }


    public T DoOutsideOfAwaitContext<T>(Func<T> func)
    {

        return DoOutsideOfContext(NodeFlags.AwaitContext, func);
    }


    public T DoInYieldAndAwaitContext<T>(Func<T> func)
    {

        return DoInsideOfContext(NodeFlags.YieldContext | NodeFlags.AwaitContext, func);
    }


    public bool InContext(NodeFlags flags)
    {

        return (ContextFlags & flags) != 0;
    }


    public bool InYieldContext()
    {

        return InContext(NodeFlags.YieldContext);
    }


    public bool InDisallowInContext()
    {

        return InContext(NodeFlags.DisallowInContext);
    }


    public bool InDecoratorContext()
    {

        return InContext(NodeFlags.DecoratorContext);
    }


    public bool InAwaitContext()
    {

        return InContext(NodeFlags.AwaitContext);
    }


    public void ParseErrorAtCurrentToken(DiagnosticMessage message, object arg0 = null)
    {
        var start = Scanner.TokenPos;
        var length = Scanner.TextPos - start;


        ParseErrorAtPosition(start, length, message, arg0);
    }


    public void ParseErrorAtPosition(int start, int length, DiagnosticMessage message, object _ = null)
    {
        var lastError = LastOrUndefined(ParseDiagnostics);
        if (lastError == null || start != lastError.Start)
        {

            ParseDiagnostics.Add(CreateFileDiagnostic(SourceFile, start, length, message));
        }


        // Mark that we've encountered an error.  We'll set an appropriate bit on the next
        // node we finish so that it can't be reused incrementally.
        ParseErrorBeforeNextFinishedNode = true;
    }


    public void ScanError(DiagnosticMessage message, int? length = null)
    {
        var pos = Scanner.TextPos;

        ParseErrorAtPosition(pos, length ?? 0, message);
    }


    public int GetNodePos()
    {

        return Scanner.StartPos;
    }


    public int GetNodeEnd()
    {

        return Scanner.StartPos;
    }


    public SyntaxKind Token()
    {

        return CurrentToken;
    }


    public SyntaxKind NextToken()
    {
        CurrentToken = Scanner.Scan();
        return CurrentToken;
    }


    public SyntaxKind ReScanGreaterToken()
    {
        CurrentToken = Scanner.ReScanGreaterToken();
        return CurrentToken;
    }


    public SyntaxKind ReScanSlashToken()
    {
        CurrentToken = Scanner.ReScanSlashToken();
        return CurrentToken;
    }


    public SyntaxKind ReScanTemplateToken()
    {
        CurrentToken = Scanner.ReScanTemplateToken();
        return CurrentToken;
    }


    public SyntaxKind ScanJsxIdentifier()
    {
        CurrentToken = Scanner.ScanJsxIdentifier();
        return CurrentToken;
    }


    public SyntaxKind ScanJsxText()
    {
        CurrentToken = Scanner.ScanJsxToken();
        return CurrentToken;
    }


    public SyntaxKind ScanJsxAttributeValue()
    {
        CurrentToken = Scanner.ScanJsxAttributeValue();
        return CurrentToken;
    }


    public T SpeculationHelper<T>(Func<T> callback, bool isLookAhead)
    {
        var saveToken = CurrentToken;
        var saveParseDiagnosticsLength = ParseDiagnostics.Count;
        var saveParseErrorBeforeNextFinishedNode = ParseErrorBeforeNextFinishedNode;
        var saveContextFlags = ContextFlags;
        var result = isLookAhead
                        ? Scanner.LookAhead(callback)
                        : Scanner.TryScan(callback);


        Debug.Assert(saveContextFlags == ContextFlags);
        if (result == null || ((result is bool) && Convert.ToBoolean(result) == false) || isLookAhead)
        {

            CurrentToken = saveToken;

            //parseDiagnostics.Count = saveParseDiagnosticsLength;
            ParseDiagnostics = ParseDiagnostics.Take(saveParseDiagnosticsLength).ToList();

            ParseErrorBeforeNextFinishedNode = saveParseErrorBeforeNextFinishedNode;
        }


        return result;
    }


    public T LookAhead<T>(Func<T> callback)
    {

        return SpeculationHelper(callback, /*isLookAhead*/ true);
    }


    public T TryParse<T>(Func<T> callback)
    {

        return SpeculationHelper(callback, /*isLookAhead*/ false);
    }


    public bool IsIdentifier()
    {
        if (Token() == SyntaxKind.Identifier)
        {

            return true;
        }
        if (Token() == SyntaxKind.YieldKeyword && InYieldContext())
        {

            return false;
        }
        if (Token() == SyntaxKind.AwaitKeyword && InAwaitContext())
        {

            return false;
        }


        return Token() > SyntaxKind.LastReservedWord;
    }


    public bool ParseExpected(SyntaxKind kind, DiagnosticMessage diagnosticMessage = null, bool shouldAdvance = true)
    {
        if (Token() == kind)
        {
            if (shouldAdvance)
            {

                NextToken();
            }

            return true;
        }
        if (diagnosticMessage != null)
        {

            ParseErrorAtCurrentToken(diagnosticMessage);
        }
        else
        {

            ParseErrorAtCurrentToken(Diagnostics._0_expected, TokenToString(kind));
        }

        return false;
    }


    public bool ParseOptional(SyntaxKind t)
    {
        if (Token() == t)
        {

            NextToken();

            return true;
        }

        return false;
    }


    //public Token<TKind> parseOptionalToken<TKind>(TKind t) where TKind : SyntaxKind
    //{
    //}


    public Node ParseOptionalToken<T>(SyntaxKind t) where T : Node, new()
    {
        if (Token() == t)
        {

            return ParseTokenNode<T>(Token());
        }

        return null;
    }


    //public Token<TKind> parseExpectedToken<TKind>(TKind t, bool reportAtCurrentPosition, DiagnosticMessage diagnosticMessage, object arg0 = null) where TKind : SyntaxKind
    //{
    //}


    public Node ParseExpectedToken<T>(SyntaxKind t, bool reportAtCurrentPosition, DiagnosticMessage diagnosticMessage, object arg0 = null) where T : Node, new()
    {

        return ParseOptionalToken<T>(t) ??
            CreateMissingNode<T>(t, reportAtCurrentPosition, diagnosticMessage, arg0);
    }


    public T ParseTokenNode<T>(SyntaxKind sk)
        where T : Node, new()
    {
        var node = new T
        {
            Pos = Scanner.StartPos,
            Kind = sk
        };

        NextToken();

        return FinishNode(node);
    }


    public bool CanParseSemicolon()
    {
        if (Token() == SyntaxKind.SemicolonToken)
        {

            return true;
        }


        // We can parse out an optional semicolon in ASI cases in the following cases.
        return Token() == SyntaxKind.CloseBraceToken || Token() == SyntaxKind.EndOfFileToken || Scanner.HasPrecedingLineBreak;
    }


    public bool ParseSemicolon()
    {
        if (CanParseSemicolon())
        {
            if (Token() == SyntaxKind.SemicolonToken)
            {

                // consume the semicolon if it was explicitly provided.
                NextToken();
            }


            return true;
        }
        else
        {

            return ParseExpected(SyntaxKind.SemicolonToken);
        }
    }


    public NodeArray<T> CreateList<T>(T[] elements = null, int? pos = null) /*where T : Node*/
    {
        var array = elements == null ? new NodeArray<T>() : new NodeArray<T>(elements); // (List<T>)(elements || []);
        if (!(pos >= 0))
        {

            pos = GetNodePos();
        }

        array.Pos = pos;

        array.End = pos;

        return array;
    }



    public T FinishNode<T>(T node, int? end = null) where T : INode
    {
        node.End = end == null ? Scanner.StartPos : (int)end;
        if (ContextFlags != NodeFlags.None)
        {
            node.Flags |= ContextFlags;
        }
        if (ParseErrorBeforeNextFinishedNode)
        {
            ParseErrorBeforeNextFinishedNode = false;
            node.Flags |= NodeFlags.ThisNodeHasError;
        }
        return node;
    }

    public Node CreateMissingNode<T>(SyntaxKind _, bool reportAtCurrentPosition, DiagnosticMessage diagnosticMessage = null, object arg0 = null) where T : Node
    {
        if (reportAtCurrentPosition)
        {
            ParseErrorAtPosition(Scanner.StartPos, 0, diagnosticMessage, arg0);
        }
        else
        {
            ParseErrorAtCurrentToken(diagnosticMessage, arg0);
        }
        var result = (T)Activator.CreateInstance(typeof(T));
        result.Kind = SyntaxKind.MissingDeclaration;
        result.Pos = Scanner.StartPos;
        //var result = new MissingNode { kind = kind, pos = scanner.getStartPos(), text = "" }; // createNode(kind, scanner.getStartPos());
        //(< Identifier > result).text = ";
        return FinishNode(result);
    }

    public Identifier CreateIdentifier(bool isIdentifier, DiagnosticMessage diagnosticMessage = null)
    {

        if (isIdentifier)
        {
            var node = new Identifier { Pos = Scanner.StartPos };
            if (Token() != SyntaxKind.Identifier)
            {

                node.OriginalKeywordKind = Token();
            }

            node.Text = Scanner.TokenValue;

            NextToken();

            return FinishNode(node);
        }


        return (Identifier)CreateMissingNode<Identifier>(SyntaxKind.Identifier, /*reportAtCurrentPosition*/ false, diagnosticMessage ?? Diagnostics.Identifier_expected);
    }


    public Identifier ParseIdentifier(DiagnosticMessage diagnosticMessage = null)
    {

        return CreateIdentifier(IsIdentifier(), diagnosticMessage);
    }


    public Identifier ParseIdentifierName()
    {

        return CreateIdentifier(TokenIsIdentifierOrKeyword(Token()));
    }


    public bool IsLiteralPropertyName()
    {

        return TokenIsIdentifierOrKeyword(Token()) ||
            Token() == SyntaxKind.StringLiteral ||
            Token() == SyntaxKind.NumericLiteral;
    }


    public IPropertyName ParsePropertyNameWorker(bool allowComputedPropertyNames)
    {
        if (Token() == SyntaxKind.StringLiteral || Token() == SyntaxKind.NumericLiteral)
        {

            var le = ParseLiteralNode(/*internName*/ true);
            if (le is StringLiteral literal) return literal;
            else if (le is NumericLiteral numeric) return numeric;
            return null; // /*(StringLiteral | NumericLiteral)*/le;
        }
        if (allowComputedPropertyNames && Token() == SyntaxKind.OpenBracketToken)
        {

            return ParseComputedPropertyName();
        }

        return ParseIdentifierName();
    }


    public IPropertyName ParsePropertyName()
    {

        return ParsePropertyNameWorker(/*allowComputedPropertyNames*/ true);
    }


    public /*Identifier | LiteralExpression*/IPropertyName ParseSimplePropertyName()
    {

        return ParsePropertyNameWorker(/*allowComputedPropertyNames*/ false);
    }


    public bool IsSimplePropertyName()
    {

        return Token() == SyntaxKind.StringLiteral || Token() == SyntaxKind.NumericLiteral || TokenIsIdentifierOrKeyword(Token());
    }


    public ComputedPropertyName ParseComputedPropertyName()
    {
        var node = new ComputedPropertyName() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBracketToken);


        // We parse any expression (including a comma expression). But the grammar
        // says that only an assignment expression is allowed, so the grammar checker
        // will error if it sees a comma expression.
        node.Expression = AllowInAnd(ParseExpression);


        ParseExpected(SyntaxKind.CloseBracketToken);

        return FinishNode(node);
    }


    public bool ParseContextualModifier(SyntaxKind t)
    {

        return Token() == t && TryParse(NextTokenCanFollowModifier);
    }


    public bool NextTokenIsOnSameLineAndCanFollowModifier()
    {

        NextToken();
        if (Scanner.HasPrecedingLineBreak)
        {

            return false;
        }

        return CanFollowModifier();
    }


    public bool NextTokenCanFollowModifier()
    {
        if (Token() == SyntaxKind.ConstKeyword)
        {

            // 'const' is only a modifier if followed by 'enum'.
            return NextToken() == SyntaxKind.EnumKeyword;
        }
        if (Token() == SyntaxKind.ExportKeyword)
        {

            NextToken();
            if (Token() == SyntaxKind.DefaultKeyword)
            {

                return LookAhead(NextTokenIsClassOrFunctionOrAsync);
            }

            return Token() != SyntaxKind.AsteriskToken && Token() != SyntaxKind.AsKeyword && Token() != SyntaxKind.OpenBraceToken && CanFollowModifier();
        }
        if (Token() == SyntaxKind.DefaultKeyword)
        {

            return NextTokenIsClassOrFunctionOrAsync();
        }
        if (Token() == SyntaxKind.StaticKeyword)
        {

            NextToken();

            return CanFollowModifier();
        }


        return NextTokenIsOnSameLineAndCanFollowModifier();
    }


    public bool ParseAnyContextualModifier()
    {

        return IsModifierKind(Token()) && TryParse(NextTokenCanFollowModifier);
    }


    public bool CanFollowModifier()
    {

        return Token() == SyntaxKind.OpenBracketToken
            || Token() == SyntaxKind.OpenBraceToken
            || Token() == SyntaxKind.AsteriskToken
            || Token() == SyntaxKind.DotDotDotToken
            || IsLiteralPropertyName();
    }


    public bool NextTokenIsClassOrFunctionOrAsync()
    {

        NextToken();

        return Token() == SyntaxKind.ClassKeyword || Token() == SyntaxKind.FunctionKeyword ||
            (Token() == SyntaxKind.AsyncKeyword && LookAhead(NextTokenIsFunctionKeywordOnSameLine));
    }


    public bool IsListElement(ParsingContext parsingContext, bool inErrorRecovery)
    {
        switch (parsingContext)
        {
            case TsTypes.ParsingContext.SourceElements:
            case TsTypes.ParsingContext.BlockStatements:
            case TsTypes.ParsingContext.SwitchClauseStatements:

                // If we're in error recovery, then we don't want to treat ';' as an empty statement.
                // The problem is that ';' can show up in far too many contexts, and if we see one
                // and assume it's a statement, then we may bail out inappropriately from whatever
                // we're parsing.  For example, if we have a semicolon in the middle of a class, then
                // we really don't want to assume the class is over and we're on a statement in the
                // outer module.  We just want to consume and move on.
                return !(Token() == SyntaxKind.SemicolonToken && inErrorRecovery) && IsStartOfStatement();
            case TsTypes.ParsingContext.SwitchClauses:

                return Token() == SyntaxKind.CaseKeyword || Token() == SyntaxKind.DefaultKeyword;
            case TsTypes.ParsingContext.TypeMembers:

                return LookAhead(IsTypeMemberStart);
            case TsTypes.ParsingContext.ClassMembers:

                // We allow semicolons as class elements (as specified by ES6) as long as we're
                // not in error recovery.  If we're in error recovery, we don't want an errant
                // semicolon to be treated as a class member (since they're almost always used
                // for statements.
                return LookAhead(IsClassMemberStart) || (Token() == SyntaxKind.SemicolonToken && !inErrorRecovery);
            case TsTypes.ParsingContext.EnumMembers:

                // Include open bracket computed properties. This technically also lets in indexers,
                // which would be a candidate for improved error reporting.
                return Token() == SyntaxKind.OpenBracketToken || IsLiteralPropertyName();
            case TsTypes.ParsingContext.ObjectLiteralMembers:

                return Token() == SyntaxKind.OpenBracketToken || Token() == SyntaxKind.AsteriskToken || Token() == SyntaxKind.DotDotDotToken || IsLiteralPropertyName();
            case TsTypes.ParsingContext.RestProperties:

                return IsLiteralPropertyName();
            case TsTypes.ParsingContext.ObjectBindingElements:

                return Token() == SyntaxKind.OpenBracketToken || Token() == SyntaxKind.DotDotDotToken || IsLiteralPropertyName();
            case TsTypes.ParsingContext.HeritageClauseElement:
                if (Token() == SyntaxKind.OpenBraceToken)
                {

                    return LookAhead(IsValidHeritageClauseObjectLiteral);
                }
                if (!inErrorRecovery)
                {

                    return IsStartOfLeftHandSideExpression() && !IsHeritageClauseExtendsOrImplementsKeyword();
                }
                else
                {

                    // If we're in error recovery we tighten up what we're willing to match.
                    // That way we don't treat something like "this" as a valid heritage clause
                    // element during recovery.
                    return IsIdentifier() && !IsHeritageClauseExtendsOrImplementsKeyword();
                }
            //goto caseLabel12;
            case TsTypes.ParsingContext.VariableDeclarations:
                //caseLabel12:
                return IsIdentifierOrPattern();
            case TsTypes.ParsingContext.ArrayBindingElements:

                return Token() == SyntaxKind.CommaToken || Token() == SyntaxKind.DotDotDotToken || IsIdentifierOrPattern();
            case TsTypes.ParsingContext.TypeParameters:

                return IsIdentifier();
            case TsTypes.ParsingContext.ArgumentExpressions:
            case TsTypes.ParsingContext.ArrayLiteralMembers:

                return Token() == SyntaxKind.CommaToken || Token() == SyntaxKind.DotDotDotToken || IsStartOfExpression();
            case TsTypes.ParsingContext.Parameters:

                return IsStartOfParameter();
            case TsTypes.ParsingContext.TypeArguments:
            case TsTypes.ParsingContext.TupleElementTypes:

                return Token() == SyntaxKind.CommaToken || IsStartOfType();
            case TsTypes.ParsingContext.HeritageClauses:

                return IsHeritageClause();
            case TsTypes.ParsingContext.ImportOrExportSpecifiers:

                return TokenIsIdentifierOrKeyword(Token());
            case TsTypes.ParsingContext.JsxAttributes:

                return TokenIsIdentifierOrKeyword(Token()) || Token() == SyntaxKind.OpenBraceToken;
            case TsTypes.ParsingContext.JsxChildren:

                return true;
            case TsTypes.ParsingContext.JSDocFunctionParameters:
            case TsTypes.ParsingContext.JSDocTypeArguments:
            case TsTypes.ParsingContext.JSDocTupleTypes:

                return JsDocParser.IsJsDocType();
            case TsTypes.ParsingContext.JSDocRecordMembers:

                return IsSimplePropertyName();
        }


        Debug.Fail("Non-exhaustive case in 'isListElement'.");
        return false;
    }


    public bool IsValidHeritageClauseObjectLiteral()
    {

        Debug.Assert(Token() == SyntaxKind.OpenBraceToken);
        if (NextToken() == SyntaxKind.CloseBraceToken)
        {
            var next = NextToken();

            return next == SyntaxKind.CommaToken || next == SyntaxKind.OpenBraceToken || next == SyntaxKind.ExtendsKeyword || next == SyntaxKind.ImplementsKeyword;
        }


        return true;
    }


    public bool NextTokenIsIdentifier()
    {

        NextToken();

        return IsIdentifier();
    }


    public bool NextTokenIsIdentifierOrKeyword()
    {

        NextToken();

        return TokenIsIdentifierOrKeyword(Token());
    }


    public bool IsHeritageClauseExtendsOrImplementsKeyword()
    {
        if (Token() == SyntaxKind.ImplementsKeyword ||
                        Token() == SyntaxKind.ExtendsKeyword)
        {


            return LookAhead(NextTokenIsStartOfExpression);
        }


        return false;
    }


    public bool NextTokenIsStartOfExpression()
    {

        NextToken();

        return IsStartOfExpression();
    }


    public bool IsListTerminator(ParsingContext kind)
    {
        if (Token() == SyntaxKind.EndOfFileToken)
        {

            // Being at the end of the file ends all lists.
            return true;
        }
        return kind switch
        {
            TsTypes.ParsingContext.BlockStatements or TsTypes.ParsingContext.SwitchClauses or TsTypes.ParsingContext.TypeMembers or TsTypes.ParsingContext.ClassMembers or TsTypes.ParsingContext.EnumMembers or TsTypes.ParsingContext.ObjectLiteralMembers or TsTypes.ParsingContext.ObjectBindingElements or TsTypes.ParsingContext.ImportOrExportSpecifiers => Token() == SyntaxKind.CloseBraceToken,
            TsTypes.ParsingContext.SwitchClauseStatements => Token() == SyntaxKind.CloseBraceToken || Token() == SyntaxKind.CaseKeyword || Token() == SyntaxKind.DefaultKeyword,
            TsTypes.ParsingContext.HeritageClauseElement => Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.ExtendsKeyword || Token() == SyntaxKind.ImplementsKeyword,
            TsTypes.ParsingContext.VariableDeclarations => IsVariableDeclaratorListTerminator(),
            TsTypes.ParsingContext.TypeParameters => Token() == SyntaxKind.GreaterThanToken || Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.ExtendsKeyword || Token() == SyntaxKind.ImplementsKeyword,// Tokens other than '>' are here for better error recovery
            TsTypes.ParsingContext.ArgumentExpressions => Token() == SyntaxKind.CloseParenToken || Token() == SyntaxKind.SemicolonToken,// Tokens other than ')' are here for better error recovery
            TsTypes.ParsingContext.ArrayLiteralMembers or TsTypes.ParsingContext.TupleElementTypes or TsTypes.ParsingContext.ArrayBindingElements => Token() == SyntaxKind.CloseBracketToken,
            TsTypes.ParsingContext.Parameters or TsTypes.ParsingContext.RestProperties => Token() == SyntaxKind.CloseParenToken || Token() == SyntaxKind.CloseBracketToken /*|| token == SyntaxKind.OpenBraceToken*/,// Tokens other than ')' and ']' (the latter for index signatures) are here for better error recovery
            TsTypes.ParsingContext.TypeArguments => Token() != SyntaxKind.CommaToken,// All other tokens should cause the type-argument to terminate except comma token
            TsTypes.ParsingContext.HeritageClauses => Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.CloseBraceToken,
            TsTypes.ParsingContext.JsxAttributes => Token() == SyntaxKind.GreaterThanToken || Token() == SyntaxKind.SlashToken,
            TsTypes.ParsingContext.JsxChildren => Token() == SyntaxKind.LessThanToken && LookAhead(NextTokenIsSlash),
            TsTypes.ParsingContext.JSDocFunctionParameters => Token() == SyntaxKind.CloseParenToken || Token() == SyntaxKind.ColonToken || Token() == SyntaxKind.CloseBraceToken,
            TsTypes.ParsingContext.JSDocTypeArguments => Token() == SyntaxKind.GreaterThanToken || Token() == SyntaxKind.CloseBraceToken,
            TsTypes.ParsingContext.JSDocTupleTypes => Token() == SyntaxKind.CloseBracketToken || Token() == SyntaxKind.CloseBraceToken,
            TsTypes.ParsingContext.JSDocRecordMembers => Token() == SyntaxKind.CloseBraceToken,
            _ => false,// ?
        };
    }


    public bool IsVariableDeclaratorListTerminator()
    {
        if (CanParseSemicolon())
        {

            return true;
        }
        if (IsInOrOfKeyword(Token()))
        {

            return true;
        }
        if (Token() == SyntaxKind.EqualsGreaterThanToken)
        {

            return true;
        }


        // Keep trying to parse out variable declarators.
        return false;
    }


    public bool IsInSomeParsingContext()
    {
        //throw new NotImplementedException();
        //for (var kind = 0; kind < Enum.GetNames(typeof(ParsingContext)).Count(); kind++)
        foreach (ParsingContext kind in Enum.GetValues(typeof(ParsingContext)))
        {
            if ((ParsingContext & (1 << (int)kind)) != 0)
            {
                if (IsListElement(kind, /*inErrorRecovery*/ true) || IsListTerminator(kind))
                {
                    return true;
                }
            }
        }


        return false;
    }


    public NodeArray<T> ParseList<T>(ParsingContext kind, Func<T> parseElement) where T : INode
    {
        var saveParsingContext = ParsingContext;

        ParsingContext |= 1 << (int)kind;
        var result = CreateList<T>();
        while (!IsListTerminator(kind))
        {
            if (IsListElement(kind, /*inErrorRecovery*/ false))
            {
                var element = ParseListElement(kind, parseElement);

                result.Add(element);


                continue;
            }
            if (AbortParsingListOrMoveToNextToken(kind))
            {

                break;
            }
        }


        result.End = GetNodeEnd();

        ParsingContext = saveParsingContext;

        return result;
    }

    public NodeArray<T> ParseList2<T>(ParsingContext kind, Func<T> parseElement) where T : INode
    {
        var saveParsingContext = ParsingContext;

        ParsingContext |= 1 << (int)kind;
        var result = CreateList<T>();
        while (!IsListTerminator(kind))
        {
            if (IsListElement(kind, /*inErrorRecovery*/ false))
            {
                var element = ParseListElement2(kind, parseElement);

                result.Add(element);


                continue;
            }
            if (AbortParsingListOrMoveToNextToken(kind))
            {

                break;
            }
        }


        result.End = GetNodeEnd();

        ParsingContext = saveParsingContext;

        return result;
    }
    public T ParseListElement<T>(ParsingContext _, Func<T> parseElement) where T : INode
    {
        return parseElement();
    }
    public T ParseListElement2<T>(ParsingContext _, Func<T> parseElement) where T : INode
    {
        return parseElement();
    }

    public INode ConsumeNode(INode node)
    {

        // Move the scanner so it is after the node we just consumed.
        Scanner.TextPos = node.End ?? 0;

        NextToken();

        return node;
    }

    public bool CanReuseNode(Node node, ParsingContext parsingContext)
    {
        switch (parsingContext)
        {
            case TsTypes.ParsingContext.ClassMembers:

                return IsReusableClassMember(node);
            case TsTypes.ParsingContext.SwitchClauses:

                return IsReusableSwitchClause(node);
            case TsTypes.ParsingContext.SourceElements:
            case TsTypes.ParsingContext.BlockStatements:
            case TsTypes.ParsingContext.SwitchClauseStatements:

                return IsReusableStatement(node);
            case TsTypes.ParsingContext.EnumMembers:

                return IsReusableEnumMember(node);
            case TsTypes.ParsingContext.TypeMembers:

                return IsReusableTypeMember(node);
            case TsTypes.ParsingContext.VariableDeclarations:

                return IsReusableVariableDeclaration(node);
            case TsTypes.ParsingContext.Parameters:

                return IsReusableParameter(node);
            case TsTypes.ParsingContext.RestProperties:

                return false;
            case TsTypes.ParsingContext.HeritageClauses:
            case TsTypes.ParsingContext.TypeParameters:
            case TsTypes.ParsingContext.TupleElementTypes:
            case TsTypes.ParsingContext.TypeArguments:
            case TsTypes.ParsingContext.ArgumentExpressions:
            case TsTypes.ParsingContext.ObjectLiteralMembers:
            case TsTypes.ParsingContext.HeritageClauseElement:
            case TsTypes.ParsingContext.JsxAttributes:
            case TsTypes.ParsingContext.JsxChildren:
                break;
        }


        return false;
    }


    public bool IsReusableClassMember(Node node)
    {
        if (node != null)
        {
            switch (node.Kind)
            {
                case SyntaxKind.Constructor:
                case SyntaxKind.IndexSignature:
                case SyntaxKind.GetAccessor:
                case SyntaxKind.SetAccessor:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.SemicolonClassElement:

                    return true;
                case SyntaxKind.MethodDeclaration:
                    var methodDeclaration = (MethodDeclaration)node;
                    var nameIsConstructor = methodDeclaration.Name.Kind == SyntaxKind.Identifier &&
                                                ((Identifier)methodDeclaration.Name).OriginalKeywordKind == SyntaxKind.ConstructorKeyword;


                    return !nameIsConstructor;
            }
        }


        return false;
    }


    public bool IsReusableSwitchClause(Node node)
    {
        if (node != null)
        {
            switch (node.Kind)
            {
                case SyntaxKind.CaseClause:
                case SyntaxKind.DefaultClause:

                    return true;
            }
        }


        return false;
    }


    public bool IsReusableStatement(Node node)
    {
        if (node != null)
        {
            switch (node.Kind)
            {
                case SyntaxKind.FunctionDeclaration:
                case SyntaxKind.VariableStatement:
                case SyntaxKind.Block:
                case SyntaxKind.IfStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.ThrowStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.BreakStatement:
                case SyntaxKind.ContinueStatement:
                case SyntaxKind.ForInStatement:
                case SyntaxKind.ForOfStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.WithStatement:
                case SyntaxKind.EmptyStatement:
                case SyntaxKind.TryStatement:
                case SyntaxKind.LabeledStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.DebuggerStatement:
                case SyntaxKind.ImportDeclaration:
                case SyntaxKind.ImportEqualsDeclaration:
                case SyntaxKind.ExportDeclaration:
                case SyntaxKind.ExportAssignment:
                case SyntaxKind.ModuleDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.TypeAliasDeclaration:

                    return true;
            }
        }


        return false;
    }


    public bool IsReusableEnumMember(Node node)
    {

        return node.Kind == SyntaxKind.EnumMember;
    }


    public bool IsReusableTypeMember(Node node)
    {
        if (node != null)
        {
            switch (node.Kind)
            {
                case SyntaxKind.ConstructSignature:
                case SyntaxKind.MethodSignature:
                case SyntaxKind.IndexSignature:
                case SyntaxKind.PropertySignature:
                case SyntaxKind.CallSignature:

                    return true;
            }
        }


        return false;
    }


    public bool IsReusableVariableDeclaration(Node node)
    {
        if (node.Kind != SyntaxKind.VariableDeclaration)
        {

            return false;
        }
        var variableDeclarator = (VariableDeclaration)node;

        return variableDeclarator.Initializer == null;
    }


    public bool IsReusableParameter(Node node)
    {
        if (node.Kind != SyntaxKind.Parameter)
        {

            return false;
        }
        var parameter = (ParameterDeclaration)node;

        return parameter.Initializer == null;
    }


    public bool AbortParsingListOrMoveToNextToken(ParsingContext kind)
    {

        ParseErrorAtCurrentToken(ParsingContextErrors(kind));
        if (IsInSomeParsingContext())
        {

            return true;
        }


        NextToken();

        return false;
    }


    public DiagnosticMessage ParsingContextErrors(ParsingContext context)
    {
        return context switch
        {
            TsTypes.ParsingContext.SourceElements => Diagnostics.Declaration_or_statement_expected,
            TsTypes.ParsingContext.BlockStatements => Diagnostics.Declaration_or_statement_expected,
            TsTypes.ParsingContext.SwitchClauses => Diagnostics.case_or_default_expected,
            TsTypes.ParsingContext.SwitchClauseStatements => Diagnostics.Statement_expected,
            TsTypes.ParsingContext.RestProperties or TsTypes.ParsingContext.TypeMembers => Diagnostics.Property_or_signature_expected,
            TsTypes.ParsingContext.ClassMembers => Diagnostics.Unexpected_token_A_constructor_method_accessor_or_property_was_expected,
            TsTypes.ParsingContext.EnumMembers => Diagnostics.Enum_member_expected,
            TsTypes.ParsingContext.HeritageClauseElement => Diagnostics.Expression_expected,
            TsTypes.ParsingContext.VariableDeclarations => Diagnostics.Variable_declaration_expected,
            TsTypes.ParsingContext.ObjectBindingElements => Diagnostics.Property_destructuring_pattern_expected,
            TsTypes.ParsingContext.ArrayBindingElements => Diagnostics.Array_element_destructuring_pattern_expected,
            TsTypes.ParsingContext.ArgumentExpressions => Diagnostics.Argument_expression_expected,
            TsTypes.ParsingContext.ObjectLiteralMembers => Diagnostics.Property_assignment_expected,
            TsTypes.ParsingContext.ArrayLiteralMembers => Diagnostics.Expression_or_comma_expected,
            TsTypes.ParsingContext.Parameters => Diagnostics.Parameter_declaration_expected,
            TsTypes.ParsingContext.TypeParameters => Diagnostics.Type_parameter_declaration_expected,
            TsTypes.ParsingContext.TypeArguments => Diagnostics.Type_argument_expected,
            TsTypes.ParsingContext.TupleElementTypes => Diagnostics.Type_expected,
            TsTypes.ParsingContext.HeritageClauses => Diagnostics.Unexpected_token_expected,
            TsTypes.ParsingContext.ImportOrExportSpecifiers => Diagnostics.Identifier_expected,
            TsTypes.ParsingContext.JsxAttributes => Diagnostics.Identifier_expected,
            TsTypes.ParsingContext.JsxChildren => Diagnostics.Identifier_expected,
            TsTypes.ParsingContext.JSDocFunctionParameters => Diagnostics.Parameter_declaration_expected,
            TsTypes.ParsingContext.JSDocTypeArguments => Diagnostics.Type_argument_expected,
            TsTypes.ParsingContext.JSDocTupleTypes => Diagnostics.Type_expected,
            TsTypes.ParsingContext.JSDocRecordMembers => Diagnostics.Property_assignment_expected,
            _ => null,
        };
    }


    public NodeArray<T> ParseDelimitedList<T>(ParsingContext kind, Func<T> parseElement, bool? considerSemicolonAsDelimiter = null) where T : INode
    {
        var saveParsingContext = ParsingContext;

        ParsingContext |= 1 << (int)kind;
        var result = CreateList<T>();
        var commaStart = -1;
        while (true)
        {
            if (IsListElement(kind, /*inErrorRecovery*/ false))
            {

                result.Add(ParseListElement(kind, parseElement));

                commaStart = Scanner.TokenPos;
                if (ParseOptional(SyntaxKind.CommaToken))
                {

                    continue;
                }


                commaStart = -1;
                if (IsListTerminator(kind))
                {

                    break;
                }


                // We didn't get a comma, and the list wasn't terminated, explicitly parse
                // out a comma so we give a good error message.
                ParseExpected(SyntaxKind.CommaToken);
                if (considerSemicolonAsDelimiter == true && Token() == SyntaxKind.SemicolonToken && !Scanner.HasPrecedingLineBreak)
                {

                    NextToken();
                }

                continue;
            }
            if (IsListTerminator(kind))
            {

                break;
            }
            if (AbortParsingListOrMoveToNextToken(kind))
            {

                break;
            }
        }
        if (commaStart >= 0)
        {

            // Always preserve a trailing comma by marking it on the NodeArray
            result.HasTrailingComma = true;
        }


        result.End = GetNodeEnd();

        ParsingContext = saveParsingContext;

        return result;
    }


    public NodeArray<T> CreateMissingList<T>() where T : INode
    {

        return CreateList<T>();
    }




    public NodeArray<T> ParseBracketedList<T>(ParsingContext kind, Func<T> parseElement, SyntaxKind open, SyntaxKind close) where T : INode
    {
        if (ParseExpected(open))
        {
            var result = ParseDelimitedList(kind, parseElement);

            ParseExpected(close);

            return result;
        }


        return CreateMissingList<T>();
    }


    public IEntityName ParseEntityName(bool allowReservedWords, DiagnosticMessage diagnosticMessage = null)
    {
        IEntityName entity = ParseIdentifier(diagnosticMessage);
        while (ParseOptional(SyntaxKind.DotToken))
        {
            QualifiedName node = new()
            {
                Pos = entity.Pos,                 //(QualifiedName)createNode(SyntaxKind.QualifiedName, entity.pos);
                Left = entity,
                Right = ParseRightSideOfDot(allowReservedWords)
            };

            entity = FinishNode(node);
        }

        return entity;
    }


    public Identifier ParseRightSideOfDot(bool allowIdentifierNames)
    {
        if (Scanner.HasPrecedingLineBreak && TokenIsIdentifierOrKeyword(Token()))
        {
            var matchesPattern = LookAhead(NextTokenIsIdentifierOrKeywordOnSameLine);
            if (matchesPattern)
            {

                // Report that we need an identifier.  However, report it right after the dot,
                // and not on the next token.  This is because the next token might actually
                // be an identifier and the error would be quite confusing.
                return (Identifier)CreateMissingNode<Identifier>(SyntaxKind.Identifier, /*reportAtCurrentPosition*/ true, Diagnostics.Identifier_expected);
            }
        }


        return allowIdentifierNames ? ParseIdentifierName() : ParseIdentifier();
    }


    public TemplateExpression ParseTemplateExpression()
    {
        var template = new TemplateExpression
        {
            Pos = Scanner.StartPos,
            Head = ParseTemplateHead()
        };

        Debug.Assert(template.Head.Kind == SyntaxKind.TemplateHead, "Template head has wrong token kind");
        var templateSpans = CreateList<TemplateSpan>();


        do
        {
            templateSpans.Add(ParseTemplateSpan());
        }
        while (LastOrUndefined(templateSpans).Literal.Kind == SyntaxKind.TemplateMiddle);


        templateSpans.End = GetNodeEnd();

        template.TemplateSpans = templateSpans;


        return FinishNode(template);
    }


    public TemplateSpan ParseTemplateSpan()
    {
        var span = new TemplateSpan
        {
            Pos = Scanner.StartPos,
            Expression = AllowInAnd(ParseExpression)
        };
        //var literal = TemplateMiddle | TemplateTail;
        if (Token() == SyntaxKind.CloseBraceToken)
        {

            ReScanTemplateToken();

            span.Literal = ParseTemplateMiddleOrTemplateTail();
        }
        else
        {

            span.Literal = (TemplateTail)ParseExpectedToken<TemplateTail>(SyntaxKind.TemplateTail, /*reportAtCurrentPosition*/ false, Diagnostics._0_expected, TokenToString(SyntaxKind.CloseBraceToken));
        }


        //span.literal = literal;

        return FinishNode(span);
    }


    public ILiteralExpression ParseLiteralNode(bool? internName = null)
    {
        var t = Token();
        if (t == SyntaxKind.StringLiteral) return (ILiteralExpression)ParseLiteralLikeNode(new StringLiteral(), internName == true);
        else if (t == SyntaxKind.RegularExpressionLiteral) return (ILiteralExpression)ParseLiteralLikeNode(new RegularExpressionLiteral(), internName == true);
        else if (t == SyntaxKind.NoSubstitutionTemplateLiteral) return (ILiteralExpression)ParseLiteralLikeNode(new NoSubstitutionTemplateLiteral(), internName == true);
        else if (t == SyntaxKind.NumericLiteral) return (ILiteralExpression)ParseLiteralLikeNode(new NumericLiteral(), internName == true);
        else
        {
            throw new NotSupportedException("parseLiteralNode");
        }
        //return parseLiteralLikeNode(token(), internName == true);
    }


    public TemplateHead ParseTemplateHead()
    {
        Token();
        var fragment = new TemplateHead();
        ParseLiteralLikeNode(fragment, /*internName*/ false);

        Debug.Assert(fragment.Kind == SyntaxKind.TemplateHead, "Template head has wrong token kind");

        return fragment;
    }


    public /*TemplateMiddle | TemplateTail*/ILiteralLikeNode ParseTemplateMiddleOrTemplateTail()
    {
        var t = Token();
        ILiteralLikeNode fragment = null;
        if (t == SyntaxKind.TemplateMiddle)
        {
            fragment = ParseLiteralLikeNode(new TemplateMiddle(), /*internName*/ false);
        }
        else if (t == SyntaxKind.TemplateTail)
        {
            fragment = ParseLiteralLikeNode(new TemplateTail(), /*internName*/ false);
        }
        //var fragment = parseLiteralLikeNode(token(), /*internName*/ false);

        Debug.Assert(fragment.Kind == SyntaxKind.TemplateMiddle || fragment.Kind == SyntaxKind.TemplateTail, "Template fragment has wrong token kind");

        return /*(TemplateMiddle | TemplateTail)*/fragment;
    }


    public ILiteralLikeNode ParseLiteralLikeNode(/*SyntaxKind kind*/ILiteralLikeNode node, bool _)
    {
        node.Pos = Scanner.StartPos;
        //var node = new LiteralLikeNode { pos = scanner.getStartPos() }; // LiteralExpression();
        var text = Scanner.TokenValue;

        node.Text = text;
        if (Scanner.IsUnterminated)
        {

            node.IsUnterminated = true;
        }
        var tokenPos = Scanner.TokenPos;

        NextToken();

        FinishNode(node);
        if (node.Kind == SyntaxKind.NumericLiteral
                        && SourceText[tokenPos] == '0'
                        && IsOctalDigit(SourceText[tokenPos + 1]))
        {


            node.IsOctalLiteral = true;
        }


        return node;
    }


    public TypeReferenceNode ParseTypeReference()
    {
        var typeName = ParseEntityName(/*allowReservedWords*/ false, Diagnostics.Type_expected);
        var node = new TypeReferenceNode
        {
            Pos = typeName.Pos,
            TypeName = typeName
        };
        if (!Scanner.HasPrecedingLineBreak && Token() == SyntaxKind.LessThanToken)
        {

            node.TypeArguments = ParseBracketedList(TsTypes.ParsingContext.TypeArguments, ParseType, SyntaxKind.LessThanToken, SyntaxKind.GreaterThanToken);
        }

        return FinishNode(node);
    }


    public TypePredicateNode ParseThisTypePredicate(ThisTypeNode lhs)
    {

        NextToken();
        var node = new TypePredicateNode
        {
            Pos = lhs.Pos,
            ParameterName = lhs,

            Type = ParseType()
        };

        return FinishNode(node);
    }


    public ThisTypeNode ParseThisTypeNode()
    {
        var node = new ThisTypeNode { Pos = Scanner.StartPos };

        NextToken();

        return FinishNode(node);
    }


    public TypeQueryNode ParseTypeQuery()
    {
        var node = new TypeQueryNode() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.TypeOfKeyword);

        node.ExprName = ParseEntityName(/*allowReservedWords*/ true);

        return FinishNode(node);
    }


    public TypeParameterDeclaration ParseTypeParameter()
    {
        var node = new TypeParameterDeclaration
        {
            Pos = Scanner.StartPos,
            Name = ParseIdentifier()
        };
        if (ParseOptional(SyntaxKind.ExtendsKeyword))
        {
            if (IsStartOfType() || !IsStartOfExpression())
            {

                node.Constraint = ParseType();
            }
            else
            {

                // It was not a type, and it looked like an expression.  Parse out an expression
                // here so we recover well.  Note: it is important that we call parseUnaryExpression
                // and not parseExpression here.  If the user has:
                //
                //      <T extends "">
                //
                // We do *not* want to consume the  >  as we're consuming the expression for "".
                node.Expression = (Expression)ParseUnaryExpressionOrHigher();
            }
        }
        if (ParseOptional(SyntaxKind.EqualsToken))
        {

            node.Default = ParseType();
        }


        return FinishNode(node);
    }


    public NodeArray<TypeParameterDeclaration> ParseTypeParameters()
    {
        if (Token() == SyntaxKind.LessThanToken)
        {

            return ParseBracketedList(TsTypes.ParsingContext.TypeParameters, ParseTypeParameter, SyntaxKind.LessThanToken, SyntaxKind.GreaterThanToken);
        }
        return null;
    }


    public ITypeNode ParseParameterType()
    {
        if (ParseOptional(SyntaxKind.ColonToken))
        {

            return ParseType();
        }


        return null;
    }


    public bool IsStartOfParameter()
    {

        return Token() == SyntaxKind.DotDotDotToken || IsIdentifierOrPattern() || IsModifierKind(Token()) || Token() == SyntaxKind.AtToken || Token() == SyntaxKind.ThisKeyword;
    }


    public ParameterDeclaration ParseParameter()
    {
        var node = new ParameterDeclaration() { Pos = Scanner.StartPos };
        if (Token() == SyntaxKind.ThisKeyword)
        {

            node.Name = CreateIdentifier(/*isIdentifier*/true, null);

            node.Type = ParseParameterType();

            return FinishNode(node);
        }


        node.Decorators = ParseDecorators();

        node.Modifiers = ParseModifiers();

        node.DotDotDotToken = (DotDotDotToken)ParseOptionalToken<DotDotDotToken>(SyntaxKind.DotDotDotToken);


        // FormalParameter [Yield,Await]:
        //      BindingElement[?Yield,?Await]
        node.Name = ParseIdentifierOrPattern();
        if (GetFullWidth(node.Name) == 0 && !HasModifiers(node) && IsModifierKind(Token()))
        {

            // in cases like
            // 'use strict'
            // function foo(static)
            // isParameter('static') == true, because of isModifier('static')
            // however 'static' is not a legal identifier in a strict mode.
            // so result of this function will be ParameterDeclaration (flags = 0, name = missing, type = null, initializer = null)
            // and current token will not change => parsing of the enclosing parameter list will last till the end of time (or OOM)
            // to avoid this we'll advance cursor to the next token.
            NextToken();
        }


        node.QuestionToken = (QuestionToken)ParseOptionalToken<QuestionToken>(SyntaxKind.QuestionToken);

        node.Type = ParseParameterType();

        node.Initializer = ParseBindingElementInitializer(/*inParameter*/ true);


        // Do not check for initializers in an ambient context for parameters. This is not
        // a grammar error because the grammar allows arbitrary call signatures in
        // an ambient context.
        // It is actually not necessary for this to be an error at all. The reason is that
        // function/constructor implementations are syntactically disallowed in ambient
        // contexts. In addition, parameter initializers are semantically disallowed in
        // overload signatures. So parameter initializers are transitively disallowed in
        // ambient contexts.

        return AddJsDocComment(FinishNode(node));
    }


    public IExpression ParseBindingElementInitializer(bool inParameter)
    {

        return inParameter ? ParseParameterInitializer() : ParseNonParameterInitializer();
    }


    public IExpression ParseParameterInitializer()
    {

        return ParseInitializer(/*inParameter*/ true);
    }


    public void FillSignature(SyntaxKind returnToken, bool
                yieldContext, bool
                awaitContext, bool
                requireCompleteParameterList, ISignatureDeclaration
                signature)
    {
        var returnTokenRequired = returnToken == SyntaxKind.EqualsGreaterThanToken;

        signature.TypeParameters = ParseTypeParameters();

        signature.Parameters = ParseParameterList(yieldContext, awaitContext, requireCompleteParameterList);
        if (returnTokenRequired)
        {

            ParseExpected(returnToken);

            signature.Type = ParseTypeOrTypePredicate();
        }
        else
        if (ParseOptional(returnToken))
        {

            signature.Type = ParseTypeOrTypePredicate();
        }
    }
    public void FillSignatureEqualsGreaterThanToken(SyntaxKind returnToken, bool
                yieldContext, bool
                awaitContext, bool
                requireCompleteParameterList, SignatureDeclaration
                signature)
    {
        var returnTokenRequired = returnToken == SyntaxKind.EqualsGreaterThanToken;

        signature.TypeParameters = ParseTypeParameters();

        signature.Parameters = ParseParameterList(yieldContext, awaitContext, requireCompleteParameterList);
        if (returnTokenRequired)
        {

            ParseExpected(returnToken);

            signature.Type = ParseTypeOrTypePredicate();
        }
        else
        if (ParseOptional(returnToken))
        {

            signature.Type = ParseTypeOrTypePredicate();
        }
    }
    public void FillSignatureColonToken(SyntaxKind
                returnToken, bool
                yieldContext, bool
                awaitContext, bool
                requireCompleteParameterList, SignatureDeclaration
                signature)
    {
        var returnTokenRequired = returnToken == SyntaxKind.EqualsGreaterThanToken;

        signature.TypeParameters = ParseTypeParameters();

        signature.Parameters = ParseParameterList(yieldContext, awaitContext, requireCompleteParameterList);
        if (returnTokenRequired)
        {

            ParseExpected(returnToken);

            signature.Type = ParseTypeOrTypePredicate();
        }
        else
        if (ParseOptional(returnToken))
        {

            signature.Type = ParseTypeOrTypePredicate();
        }
    }

    public NodeArray<ParameterDeclaration> ParseParameterList(bool yieldContext, bool awaitContext, bool requireCompleteParameterList)
    {
        if (ParseExpected(SyntaxKind.OpenParenToken))
        {
            var savedYieldContext = InYieldContext();
            var savedAwaitContext = InAwaitContext();


            SetYieldContext(yieldContext);

            SetAwaitContext(awaitContext);
            var result = ParseDelimitedList(TsTypes.ParsingContext.Parameters, ParseParameter);


            SetYieldContext(savedYieldContext);

            SetAwaitContext(savedAwaitContext);
            if (!ParseExpected(SyntaxKind.CloseParenToken) && requireCompleteParameterList)
            {

                // Caller insisted that we had to end with a )   We didn't.  So just return
                // null here.
                return null;
            }


            return result;
        }


        // We didn't even have an open paren.  If the caller requires a complete parameter list,
        // we definitely can't provide that.  However, if they're ok with an incomplete one,
        // then just return an empty set of parameters.
        return requireCompleteParameterList ? null : CreateMissingList<ParameterDeclaration>();
    }


    public void ParseTypeMemberSemicolon()
    {
        if (ParseOptional(SyntaxKind.CommaToken))
        {

            return;
        }


        // Didn't have a comma.  We must have a (possible ASI) semicolon.
        ParseSemicolon();
    }


    public /*CallSignatureDeclaration | ConstructSignatureDeclaration*/ITypeElement ParseSignatureMember(SyntaxKind kind)
    {
        //var node = new CallSignatureDeclaration | ConstructSignatureDeclaration();
        if (kind == SyntaxKind.ConstructSignature)
        {

            var node = new ConstructSignatureDeclaration { Pos = Scanner.StartPos };
            ParseExpected(SyntaxKind.NewKeyword);
            FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, node);

            ParseTypeMemberSemicolon();

            return AddJsDocComment(FinishNode(node));
        }
        else
        {
            var node = new CallSignatureDeclaration { Pos = Scanner.StartPos };
            FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, node);

            ParseTypeMemberSemicolon();

            return AddJsDocComment(FinishNode(node));
        }

        //fillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, node);

        //parseTypeMemberSemicolon();

        //return addJSDocComment(finishNode(node));
    }


    public bool IsIndexSignature()
    {
        if (Token() != SyntaxKind.OpenBracketToken)
        {

            return false;
        }


        return LookAhead(IsUnambiguouslyIndexSignature);
    }


    public bool IsUnambiguouslyIndexSignature()
    {

        // The only allowed sequence is:
        //
        //   [id:
        //
        // However, for error recovery, we also check the following cases:
        //
        //   [...
        //   [id,
        //   [id?,
        //   [id?:
        //   [id?]
        //   [public id
        //   [private id
        //   [protected id
        //   []
        //
        NextToken();
        if (Token() == SyntaxKind.DotDotDotToken || Token() == SyntaxKind.CloseBracketToken)
        {

            return true;
        }
        if (IsModifierKind(Token()))
        {

            NextToken();
            if (IsIdentifier())
            {

                return true;
            }
        }
        else
        if (!IsIdentifier())
        {

            return false;
        }
        else
        {

            // Skip the identifier
            NextToken();
        }
        if (Token() == SyntaxKind.ColonToken || Token() == SyntaxKind.CommaToken)
        {

            return true;
        }
        if (Token() != SyntaxKind.QuestionToken)
        {

            return false;
        }


        // If any of the following tokens are after the question mark, it cannot
        // be a conditional expression, so treat it as an indexer.
        NextToken();

        return Token() == SyntaxKind.ColonToken || Token() == SyntaxKind.CommaToken || Token() == SyntaxKind.CloseBracketToken;
    }


    public IndexSignatureDeclaration ParseIndexSignatureDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new IndexSignatureDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers,

            Parameters = ParseBracketedList(TsTypes.ParsingContext.Parameters, ParseParameter, SyntaxKind.OpenBracketToken, SyntaxKind.CloseBracketToken),

            Type = ParseTypeAnnotation()
        };

        ParseTypeMemberSemicolon();

        return FinishNode(node);
    }


    public /*PropertySignature | MethodSignature*/ITypeElement ParsePropertyOrMethodSignature(int fullStart, NodeArray<Modifier> modifiers)
    {
        var name = ParsePropertyName();
        var questionToken = (QuestionToken)ParseOptionalToken<QuestionToken>(SyntaxKind.QuestionToken);
        if (Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken)
        {
            var method = new MethodSignature
            {
                Pos = fullStart,
                Modifiers = modifiers,

                Name = name,

                QuestionToken = questionToken
            };


            // Method signatures don't exist in expression contexts.  So they have neither
            // [Yield] nor [Await]
            FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, method);

            ParseTypeMemberSemicolon();

            return AddJsDocComment(FinishNode(method));
        }
        else
        {
            var property = new PropertySignature
            {
                Pos = fullStart,
                Modifiers = modifiers,

                Name = name,

                QuestionToken = questionToken,

                Type = ParseTypeAnnotation()
            };
            if (Token() == SyntaxKind.EqualsToken)
            {

                // Although type literal properties cannot not have initializers, we attempt
                // to parse an initializer so we can report in the checker that an interface
                // property or type literal property cannot have an initializer.
                property.Initializer = ParseNonParameterInitializer();
            }


            ParseTypeMemberSemicolon();

            return AddJsDocComment(FinishNode(property));
        }
    }


    public bool IsTypeMemberStart()
    {
        if (Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken)
        {

            return true;
        }
        bool idToken = false;
        while (IsModifierKind(Token()))
        {

            idToken = true;

            NextToken();
        }
        if (Token() == SyntaxKind.OpenBracketToken)
        {

            return true;
        }
        if (IsLiteralPropertyName())
        {

            idToken = true;

            NextToken();
        }
        if (idToken)
        {

            return Token() == SyntaxKind.OpenParenToken ||
                Token() == SyntaxKind.LessThanToken ||
                Token() == SyntaxKind.QuestionToken ||
                Token() == SyntaxKind.ColonToken ||
                Token() == SyntaxKind.CommaToken ||
                CanParseSemicolon();
        }

        return false;
    }


    public ITypeElement ParseTypeMember()
    {
        if (Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken)
        {

            return ParseSignatureMember(SyntaxKind.CallSignature);
        }
        if (Token() == SyntaxKind.NewKeyword && LookAhead(IsStartOfConstructSignature))
        {

            return ParseSignatureMember(SyntaxKind.ConstructSignature);
        }
        var fullStart = GetNodePos();
        var modifiers = ParseModifiers();
        if (IsIndexSignature())
        {

            return ParseIndexSignatureDeclaration(fullStart, /*decorators*/ null, modifiers);
        }

        return ParsePropertyOrMethodSignature(fullStart, modifiers);
    }


    public bool IsStartOfConstructSignature()
    {

        NextToken();

        return Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken;
    }


    public TypeLiteralNode ParseTypeLiteral()
    {
        var node = new TypeLiteralNode
        {
            Pos = Scanner.StartPos,
            Members = ParseObjectTypeMembers()
        };

        return FinishNode(node);
    }


    public NodeArray<ITypeElement> ParseObjectTypeMembers()
    {
        NodeArray<ITypeElement> members = null;
        if (ParseExpected(SyntaxKind.OpenBraceToken))
        {

            members = ParseList(TsTypes.ParsingContext.TypeMembers, ParseTypeMember);

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            members = CreateMissingList<ITypeElement>();
        }


        return members;
    }


    public bool IsStartOfMappedType()
    {

        NextToken();
        if (Token() == SyntaxKind.ReadonlyKeyword)
        {

            NextToken();
        }

        return Token() == SyntaxKind.OpenBracketToken && NextTokenIsIdentifier() && NextToken() == SyntaxKind.InKeyword;
    }


    public TypeParameterDeclaration ParseMappedTypeParameter()
    {
        var node = new TypeParameterDeclaration
        {
            Pos = Scanner.StartPos,
            Name = ParseIdentifier()
        };

        ParseExpected(SyntaxKind.InKeyword);

        node.Constraint = ParseType();

        return FinishNode(node);
    }


    public MappedTypeNode ParseMappedType()
    {
        var node = new MappedTypeNode() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBraceToken);

        node.ReadonlyToken = (ReadonlyToken)ParseOptionalToken<ReadonlyToken>(SyntaxKind.ReadonlyKeyword);

        ParseExpected(SyntaxKind.OpenBracketToken);

        node.TypeParameter = ParseMappedTypeParameter();

        ParseExpected(SyntaxKind.CloseBracketToken);

        node.QuestionToken = (QuestionToken)ParseOptionalToken<QuestionToken>(SyntaxKind.QuestionToken);

        node.Type = ParseTypeAnnotation();

        ParseSemicolon();

        ParseExpected(SyntaxKind.CloseBraceToken);

        return FinishNode(node);
    }


    public TupleTypeNode ParseTupleType()
    {
        var node = new TupleTypeNode
        {
            Pos = Scanner.StartPos,
            ElementTypes = ParseBracketedList(TsTypes.ParsingContext.TupleElementTypes, ParseType, SyntaxKind.OpenBracketToken, SyntaxKind.CloseBracketToken)
        };

        return FinishNode(node);
    }


    public ParenthesizedTypeNode ParseParenthesizedType()
    {
        var node = new ParenthesizedTypeNode() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Type = ParseType();

        ParseExpected(SyntaxKind.CloseParenToken);

        return FinishNode(node);
    }


    public IFunctionOrConstructorTypeNode ParseFunctionOrConstructorType(SyntaxKind kind)
    {

        var node = kind == SyntaxKind.FunctionType ?
            (IFunctionOrConstructorTypeNode)new FunctionTypeNode { Kind = SyntaxKind.FunctionType } :
            kind == SyntaxKind.ConstructorType ?
            new ConstructorTypeNode { Kind = SyntaxKind.ConstructorType } :
            throw new NotSupportedException("parseFunctionOrConstructorType");
        node.Pos = Scanner.StartPos;
        //new FunctionOrConstructorTypeNode { kind = kind, pos = scanner.getStartPos() };
        if (kind == SyntaxKind.ConstructorType)
        {

            ParseExpected(SyntaxKind.NewKeyword);
        }

        FillSignature(SyntaxKind.EqualsGreaterThanToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, node);

        return FinishNode(node);
    }


    public TypeNode ParseKeywordAndNoDot()
    {
        var node = ParseTokenNode<TypeNode>(Token());

        return Token() == SyntaxKind.DotToken ? null : node;
    }


    public LiteralTypeNode ParseLiteralTypeNode()
    {
        var node = new LiteralTypeNode
        {
            Pos = Scanner.StartPos,
            Literal = ParseSimpleUnaryExpression()
        };

        FinishNode(node);

        return node;
    }


    public bool NextTokenIsNumericLiteral()
    {

        return NextToken() == SyntaxKind.NumericLiteral;
    }


    public ITypeNode ParseNonArrayType()
    {
        switch (Token())
        {
            case SyntaxKind.AnyKeyword:
            case SyntaxKind.StringKeyword:
            case SyntaxKind.NumberKeyword:
            case SyntaxKind.BooleanKeyword:
            case SyntaxKind.SymbolKeyword:
            case SyntaxKind.UndefinedKeyword:
            case SyntaxKind.NeverKeyword:
            case SyntaxKind.ObjectKeyword:
                var node = TryParse(ParseKeywordAndNoDot);

                return node ?? ParseTypeReference();
            case SyntaxKind.StringLiteral:
            case SyntaxKind.NumericLiteral:
            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:

                return ParseLiteralTypeNode();
            case SyntaxKind.MinusToken:

                return LookAhead(NextTokenIsNumericLiteral) ? ParseLiteralTypeNode() : ParseTypeReference();
            case SyntaxKind.VoidKeyword:
            case SyntaxKind.NullKeyword:

                return ParseTokenNode<TypeNode>(Token());
            case SyntaxKind.ThisKeyword:
                {
                    var thisKeyword = ParseThisTypeNode();
                    if (Token() == SyntaxKind.IsKeyword && !Scanner.HasPrecedingLineBreak)
                    {
                        return ParseThisTypePredicate(thisKeyword);
                    }
                    else
                    {
                        return thisKeyword;
                    }
                }
            //goto caseLabel17;
            case SyntaxKind.TypeOfKeyword:
                //caseLabel17:
                return ParseTypeQuery();
            case SyntaxKind.OpenBraceToken:

                return LookAhead(IsStartOfMappedType) ? ParseMappedType() : ParseTypeLiteral();
            case SyntaxKind.OpenBracketToken:

                return ParseTupleType();
            case SyntaxKind.OpenParenToken:

                return ParseParenthesizedType();
            default:

                return ParseTypeReference();
        }
    }


    public bool IsStartOfType()
    {
        return Token() switch
        {
            SyntaxKind.AnyKeyword or SyntaxKind.StringKeyword or SyntaxKind.NumberKeyword or SyntaxKind.BooleanKeyword or SyntaxKind.SymbolKeyword or SyntaxKind.VoidKeyword or SyntaxKind.UndefinedKeyword or SyntaxKind.NullKeyword or SyntaxKind.ThisKeyword or SyntaxKind.TypeOfKeyword or SyntaxKind.NeverKeyword or SyntaxKind.OpenBraceToken or SyntaxKind.OpenBracketToken or SyntaxKind.LessThanToken or SyntaxKind.BarToken or SyntaxKind.AmpersandToken or SyntaxKind.NewKeyword or SyntaxKind.StringLiteral or SyntaxKind.NumericLiteral or SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword or SyntaxKind.ObjectKeyword => true,
            SyntaxKind.MinusToken => LookAhead(NextTokenIsNumericLiteral),
            SyntaxKind.OpenParenToken => LookAhead(IsStartOfParenthesizedOrFunctionType),// Only consider '(' the start of a type if followed by ')', '...', an identifier, a modifier,
                                                                                         // or something that starts a type. We don't want to consider things like '(1)' a type.
            _ => IsIdentifier(),
        };
    }


    public bool IsStartOfParenthesizedOrFunctionType()
    {

        NextToken();

        return Token() == SyntaxKind.CloseParenToken || IsStartOfParameter() || IsStartOfType();
    }


    public ITypeNode ParseArrayTypeOrHigher()
    {
        var type = ParseNonArrayType();
        while (!Scanner.HasPrecedingLineBreak && ParseOptional(SyntaxKind.OpenBracketToken))
        {
            if (IsStartOfType())
            {
                var node = new IndexedAccessTypeNode
                {
                    Pos = type.Pos,
                    ObjectType = type,

                    IndexType = ParseType()
                };

                ParseExpected(SyntaxKind.CloseBracketToken);

                type = FinishNode(node);
            }
            else
            {
                var node = new ArrayTypeNode
                {
                    Pos = type.Pos,
                    ElementType = type
                };

                ParseExpected(SyntaxKind.CloseBracketToken);

                type = FinishNode(node);
            }
        }

        return type;
    }


    public /*MappedTypeNode*/TypeOperatorNode ParseTypeOperator(SyntaxKind/*.KeyOfKeyword*/ @operator)
    {
        var node = new TypeOperatorNode() { Pos = Scanner.StartPos };

        ParseExpected(@operator);

        node.Operator = @operator;

        node.Type = ParseTypeOperatorOrHigher();

        return FinishNode(node);
    }


    public ITypeNode ParseTypeOperatorOrHigher()
    {
        return Token() switch
        {
            SyntaxKind.KeyOfKeyword => ParseTypeOperator(SyntaxKind.KeyOfKeyword),
            _ => ParseArrayTypeOrHigher(),
        };
    }


    public ITypeNode ParseUnionOrIntersectionType(SyntaxKind/*.UnionType | SyntaxKind.IntersectionType*/ kind, Func<ITypeNode> parseConstituentType, SyntaxKind/*.BarToken | SyntaxKind.AmpersandToken*/ @operator)
    {

        ParseOptional(@operator);
        var type = parseConstituentType();
        if (Token() == @operator)
        {
            var types = CreateList<ITypeNode>(); //[type], type.pos);
            types.Pos = type.Pos;
            types.Add(type);


            while (ParseOptional(@operator))
            {

                types.Add(parseConstituentType());
            }

            types.End = GetNodeEnd();
            var node = kind == SyntaxKind.UnionType ?
                (IUnionOrIntersectionTypeNode)new UnionTypeNode { Kind = kind, Pos = type.Pos } :
                kind == SyntaxKind.IntersectionType ? new IntersectionTypeNode { Kind = kind, Pos = type.Pos }
                : throw new NotSupportedException("parseUnionOrIntersectionType");

            node.Types = types;

            type = FinishNode(node);
        }

        return type;
    }


    public ITypeNode ParseIntersectionTypeOrHigher()
    {

        return ParseUnionOrIntersectionType(SyntaxKind.IntersectionType, ParseTypeOperatorOrHigher, SyntaxKind.AmpersandToken);
    }


    public ITypeNode ParseUnionTypeOrHigher()
    {

        return ParseUnionOrIntersectionType(SyntaxKind.UnionType, ParseIntersectionTypeOrHigher, SyntaxKind.BarToken);
    }


    public bool IsStartOfFunctionType()
    {
        if (Token() == SyntaxKind.LessThanToken)
        {

            return true;
        }

        return Token() == SyntaxKind.OpenParenToken && LookAhead(IsUnambiguouslyStartOfFunctionType);
    }


    public bool SkipParameterStart()
    {
        if (IsModifierKind(Token()))
        {

            // Skip modifiers
            ParseModifiers();
        }
        if (IsIdentifier() || Token() == SyntaxKind.ThisKeyword)
        {

            NextToken();

            return true;
        }
        if (Token() == SyntaxKind.OpenBracketToken || Token() == SyntaxKind.OpenBraceToken)
        {
            var previousErrorCount = ParseDiagnostics.Count;

            ParseIdentifierOrPattern();

            return previousErrorCount == ParseDiagnostics.Count;
        }

        return false;
    }


    public bool IsUnambiguouslyStartOfFunctionType()
    {

        NextToken();
        if (Token() == SyntaxKind.CloseParenToken || Token() == SyntaxKind.DotDotDotToken)
        {

            // ( )
            // ( ...
            return true;
        }
        if (SkipParameterStart())
        {
            if (Token() == SyntaxKind.ColonToken || Token() == SyntaxKind.CommaToken ||
                                Token() == SyntaxKind.QuestionToken || Token() == SyntaxKind.EqualsToken)
            {

                // ( xxx :
                // ( xxx ,
                // ( xxx ?
                // ( xxx =
                return true;
            }
            if (Token() == SyntaxKind.CloseParenToken)
            {

                NextToken();
                if (Token() == SyntaxKind.EqualsGreaterThanToken)
                {

                    // ( xxx ) =>
                    return true;
                }
            }
        }

        return false;
    }


    public ITypeNode ParseTypeOrTypePredicate()
    {
        var typePredicateVariable = IsIdentifier() ? TryParse(ParseTypePredicatePrefix) : null;
        var type = ParseType();
        if (typePredicateVariable != null)
        {
            var node = new TypePredicateNode
            {
                Pos = typePredicateVariable.Pos,
                ParameterName = typePredicateVariable,

                Type = type
            };

            return FinishNode(node);
        }
        else
        {

            return type;
        }
    }


    public Identifier ParseTypePredicatePrefix()
    {
        var id = ParseIdentifier();
        if (Token() == SyntaxKind.IsKeyword && !Scanner.HasPrecedingLineBreak)
        {

            NextToken();

            return id;
        }
        return null;
    }


    public ITypeNode ParseType()
    {

        // The rules about 'yield' only apply to actual code/expression contexts.  They don't
        // apply to 'type' contexts.  So we disable these parameters here before moving on.
        return DoOutsideOfContext(NodeFlags.TypeExcludesFlags, ParseTypeWorker);
    }


    public ITypeNode ParseTypeWorker()
    {
        if (IsStartOfFunctionType())
        {

            return ParseFunctionOrConstructorType(SyntaxKind.FunctionType);
        }
        if (Token() == SyntaxKind.NewKeyword)
        {

            return ParseFunctionOrConstructorType(SyntaxKind.ConstructorType);
        }

        return ParseUnionTypeOrHigher();
    }


    public ITypeNode ParseTypeAnnotation()
    {

        return ParseOptional(SyntaxKind.ColonToken) ? ParseType() : null;
    }


    public bool IsStartOfLeftHandSideExpression()
    {
        return Token() switch
        {
            SyntaxKind.ThisKeyword or SyntaxKind.SuperKeyword or SyntaxKind.NullKeyword or SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword or SyntaxKind.NumericLiteral or SyntaxKind.StringLiteral or SyntaxKind.NoSubstitutionTemplateLiteral or SyntaxKind.TemplateHead or SyntaxKind.OpenParenToken or SyntaxKind.OpenBracketToken or SyntaxKind.OpenBraceToken or SyntaxKind.FunctionKeyword or SyntaxKind.ClassKeyword or SyntaxKind.NewKeyword or SyntaxKind.SlashToken or SyntaxKind.SlashEqualsToken or SyntaxKind.Identifier => true,
            _ => IsIdentifier(),
        };
    }


    public bool IsStartOfExpression()
    {
        if (IsStartOfLeftHandSideExpression())
        {

            return true;
        }
        switch (Token())
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.TildeToken:
            case SyntaxKind.ExclamationToken:
            case SyntaxKind.DeleteKeyword:
            case SyntaxKind.TypeOfKeyword:
            case SyntaxKind.VoidKeyword:
            case SyntaxKind.PlusPlusToken:
            case SyntaxKind.MinusMinusToken:
            case SyntaxKind.LessThanToken:
            case SyntaxKind.AwaitKeyword:
            case SyntaxKind.YieldKeyword:

                // Yield/await always starts an expression.  Either it is an identifier (in which case
                // it is definitely an expression).  Or it's a keyword (either because we're in
                // a generator or async function, or in strict mode (or both)) and it started a yield or await expression.
                return true;
            default:

                // Error tolerance.  If we see the start of some binary operator, we consider
                // that the start of an expression.  That way we'll parse out a missing identifier,
                // give a good message about an identifier being missing, and then consume the
                // rest of the binary expression.
                if (IsBinaryOperator())
                {
                    return true;
                }


                return IsIdentifier();
        }
    }


    public bool IsStartOfExpressionStatement()
    {

        // As per the grammar, none of '{' or 'function' or 'class' can start an expression statement.
        return Token() != SyntaxKind.OpenBraceToken &&
            Token() != SyntaxKind.FunctionKeyword &&
            Token() != SyntaxKind.ClassKeyword &&
            Token() != SyntaxKind.AtToken &&
            IsStartOfExpression();
    }


    public IExpression ParseExpression()
    {
        var saveDecoratorContext = InDecoratorContext();
        if (saveDecoratorContext)
        {

            SetDecoratorContext(/*val*/ false);
        }
        var expr = ParseAssignmentExpressionOrHigher();
        /*BinaryOperator*/
        Token operatorToken;
        while ((operatorToken = (/*BinaryOperator*/Token)ParseOptionalToken</*BinaryOperator*/Token>(SyntaxKind.CommaToken)) != null)
        {

            expr = MakeBinaryExpression(expr, operatorToken, ParseAssignmentExpressionOrHigher());
        }
        if (saveDecoratorContext)
        {

            SetDecoratorContext(/*val*/ true);
        }

        return expr;
    }


    public IExpression ParseInitializer(bool inParameter)
    {
        if (Token() != SyntaxKind.EqualsToken)
        {
            if (Scanner.HasPrecedingLineBreak || (inParameter && Token() == SyntaxKind.OpenBraceToken) || !IsStartOfExpression())
            {

                // preceding line break, open brace in a parameter (likely a function body) or current token is not an expression -
                // do not try to parse initializer
                return null;
            }
        }


        // Initializer[In, Yield] :
        //     = AssignmentExpression[?In, ?Yield]

        ParseExpected(SyntaxKind.EqualsToken);

        return ParseAssignmentExpressionOrHigher();
    }


    public IExpression ParseAssignmentExpressionOrHigher()
    {
        if (IsYieldExpression())
        {

            return ParseYieldExpression();
        }
        var arrowExpression = TryParseParenthesizedArrowFunctionExpression() ?? TryParseAsyncSimpleArrowFunctionExpression();
        if (arrowExpression != null)
        {

            return arrowExpression;
        }
        var expr = ParseBinaryExpressionOrHigher(/*precedence*/ 0);
        if (expr.Kind == SyntaxKind.Identifier && Token() == SyntaxKind.EqualsGreaterThanToken)
        {

            return ParseSimpleArrowFunctionExpression((Identifier)expr);
        }
        if (IsLeftHandSideExpression(expr) && IsAssignmentOperator(ReScanGreaterToken()))
        {

            return MakeBinaryExpression(expr, ParseTokenNode</*BinaryOperator*/Token>(Token()), ParseAssignmentExpressionOrHigher());
        }


        // It wasn't an assignment or a lambda.  This is a conditional expression:
        return ParseConditionalExpressionRest(expr);
    }


    public bool IsYieldExpression()
    {
        if (Token() == SyntaxKind.YieldKeyword)
        {
            if (InYieldContext())
            {

                return true;
            }


            // We're in a context where 'yield expr' is not allowed.  However, if we can
            // definitely tell that the user was trying to parse a 'yield expr' and not
            // just a normal expr that start with a 'yield' identifier, then parse out
            // a 'yield expr'.  We can then report an error later that they are only
            // allowed in generator expressions.
            //
            // for example, if we see 'yield(foo)', then we'll have to treat that as an
            // invocation expression of something called 'yield'.  However, if we have
            // 'yield foo' then that is not legal as a normal expression, so we can
            // definitely recognize this as a yield expression.
            //
            // for now we just check if the next token is an identifier.  More heuristics
            // can be added here later as necessary.  We just need to make sure that we
            // don't accidentally consume something legal.
            return LookAhead(NextTokenIsIdentifierOrKeywordOrNumberOnSameLine);
        }


        return false;
    }


    public bool NextTokenIsIdentifierOnSameLine()
    {

        NextToken();

        return !Scanner.HasPrecedingLineBreak && IsIdentifier();
    }


    public YieldExpression ParseYieldExpression()
    {
        var node = new YieldExpression() { Pos = Scanner.StartPos };


        // YieldExpression[In] :
        //      yield
        //      yield [no LineTerminator here] [Lexical goal InputElementRegExp]AssignmentExpression[?In, Yield]
        //      yield [no LineTerminator here] * [Lexical goal InputElementRegExp]AssignmentExpression[?In, Yield]
        NextToken();
        if (!Scanner.HasPrecedingLineBreak &&
                        (Token() == SyntaxKind.AsteriskToken || IsStartOfExpression()))
        {

            node.AsteriskToken = (AsteriskToken)ParseOptionalToken<AsteriskToken>(SyntaxKind.AsteriskToken);

            node.Expression = ParseAssignmentExpressionOrHigher();

            return FinishNode(node);
        }
        else
        {

            // if the next token is not on the same line as yield.  or we don't have an '*' or
            // the start of an expression, then this is just a simple "yield" expression.
            return FinishNode(node);
        }
    }


    public ArrowFunction ParseSimpleArrowFunctionExpression(Identifier identifier, NodeArray<Modifier> asyncModifier = null)
    {

        Debug.Assert(Token() == SyntaxKind.EqualsGreaterThanToken, "parseSimpleArrowFunctionExpression should only have been called if we had a =>");
        ArrowFunction node;
        if (asyncModifier != null)
        {
            node = new ArrowFunction
            {
                Pos = (int)asyncModifier.Pos,
                Modifiers = asyncModifier
            }; // (ArrowFunction)createNode(SyntaxKind.ArrowFunction, asyncModifier.pos);
        }
        else
        {

            node = new ArrowFunction { Pos = identifier.Pos }; // (ArrowFunction)createNode(SyntaxKind.ArrowFunction, identifier.pos);
        }
        var parameter = new ParameterDeclaration
        {
            Pos = identifier.Pos,
            Name = identifier
        };

        FinishNode(parameter);


        node.Parameters = CreateList<ParameterDeclaration>(); // ([parameter], parameter.pos);
        node.Parameters.Pos = parameter.Pos;
        node.Parameters.Add(parameter);


        node.Parameters.End = parameter.End;


        node.EqualsGreaterThanToken = (EqualsGreaterThanToken)ParseExpectedToken<EqualsGreaterThanToken>(SyntaxKind.EqualsGreaterThanToken, /*reportAtCurrentPosition*/ false, Diagnostics._0_expected, "=>");

        node.Body = ParseArrowFunctionExpressionBody(/*isAsync*/ /*!!*/asyncModifier?.Any() == true);


        return AddJsDocComment(FinishNode(node));
    }


    public ArrowFunction TryParseParenthesizedArrowFunctionExpression()
    {
        var triState = IsParenthesizedArrowFunctionExpression();
        if (triState == Tristate.False)
        {

            // It's definitely not a parenthesized arrow function expression.
            return null;
        }
        var arrowFunction = triState == Tristate.True
                        ? ParseParenthesizedArrowFunctionExpressionHead(/*allowAmbiguity*/ true)
                        : TryParse(ParsePossibleParenthesizedArrowFunctionExpressionHead);
        if (arrowFunction == null)
        {

            // Didn't appear to actually be a parenthesized arrow function.  Just bail out.
            return null;
        }
        var isAsync = /*!!*/(GetModifierFlags(arrowFunction) & ModifierFlags.Async) != 0;
        var lastToken = Token();

        arrowFunction.EqualsGreaterThanToken = (EqualsGreaterThanToken)ParseExpectedToken<EqualsGreaterThanToken>(SyntaxKind.EqualsGreaterThanToken, /*reportAtCurrentPosition*/false, Diagnostics._0_expected, "=>");

        arrowFunction.Body = (lastToken == SyntaxKind.EqualsGreaterThanToken || lastToken == SyntaxKind.OpenBraceToken)
            ? ParseArrowFunctionExpressionBody(isAsync)
            : ParseIdentifier();


        return AddJsDocComment(FinishNode(arrowFunction));
    }


    public Tristate IsParenthesizedArrowFunctionExpression()
    {
        if (Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken || Token() == SyntaxKind.AsyncKeyword)
        {

            return LookAhead(IsParenthesizedArrowFunctionExpressionWorker);
        }
        if (Token() == SyntaxKind.EqualsGreaterThanToken)
        {

            // ERROR RECOVERY TWEAK:
            // If we see a standalone => try to parse it as an arrow function expression as that's
            // likely what the user intended to write.
            return Tristate.True;
        }

        // Definitely not a parenthesized arrow function.
        return Tristate.False;
    }


    public Tristate IsParenthesizedArrowFunctionExpressionWorker()
    {
        if (Token() == SyntaxKind.AsyncKeyword)
        {

            NextToken();
            if (Scanner.HasPrecedingLineBreak)
            {

                return Tristate.False;
            }
            if (Token() != SyntaxKind.OpenParenToken && Token() != SyntaxKind.LessThanToken)
            {

                return Tristate.False;
            }
        }
        var first = Token();
        var second = NextToken();
        if (first == SyntaxKind.OpenParenToken)
        {
            if (second == SyntaxKind.CloseParenToken)
            {
                var third = NextToken();
                return third switch
                {
                    SyntaxKind.EqualsGreaterThanToken or SyntaxKind.ColonToken or SyntaxKind.OpenBraceToken => Tristate.True,
                    _ => Tristate.False,
                };
            }
            if (second == SyntaxKind.OpenBracketToken || second == SyntaxKind.OpenBraceToken)
            {

                return Tristate.Unknown;
            }
            if (second == SyntaxKind.DotDotDotToken)
            {

                return Tristate.True;
            }
            if (!IsIdentifier())
            {

                return Tristate.False;
            }
            if (NextToken() == SyntaxKind.ColonToken)
            {

                return Tristate.True;
            }


            // This *could* be a parenthesized arrow function.
            // Return Unknown to let the caller know.
            return Tristate.Unknown;
        }
        else
        {

            Debug.Assert(first == SyntaxKind.LessThanToken);
            if (!IsIdentifier())
            {

                return Tristate.False;
            }
            if (SourceFile.LanguageVariant == LanguageVariant.Jsx)
            {
                var isArrowFunctionInJsx = LookAhead(() =>
                {
                    var third = NextToken();
                    if (third == SyntaxKind.ExtendsKeyword)
                    {
                        var fourth = NextToken();
                        return fourth switch
                        {
                            SyntaxKind.EqualsToken or SyntaxKind.GreaterThanToken => false,
                            _ => true,
                        };
                    }
                    else if (third == SyntaxKind.CommaToken)
                    {
                        return true;
                    }
                    return false;
                });
                if (isArrowFunctionInJsx)
                {

                    return Tristate.True;
                }


                return Tristate.False;
            }


            // This *could* be a parenthesized arrow function.
            return Tristate.Unknown;
        }
    }


    public ArrowFunction ParsePossibleParenthesizedArrowFunctionExpressionHead()
    {

        return ParseParenthesizedArrowFunctionExpressionHead(/*allowAmbiguity*/ false);
    }


    public ArrowFunction TryParseAsyncSimpleArrowFunctionExpression()
    {
        if (Token() == SyntaxKind.AsyncKeyword)
        {
            var isUnParenthesizedAsyncArrowFunction = LookAhead(IsUnParenthesizedAsyncArrowFunctionWorker);
            if (isUnParenthesizedAsyncArrowFunction == Tristate.True)
            {
                var asyncModifier = ParseModifiersForArrowFunction();
                var expr = ParseBinaryExpressionOrHigher(/*precedence*/ 0);

                return ParseSimpleArrowFunctionExpression((Identifier)expr, asyncModifier);
            }
        }

        return null;
    }


    public Tristate IsUnParenthesizedAsyncArrowFunctionWorker()
    {
        if (Token() == SyntaxKind.AsyncKeyword)
        {

            NextToken();
            if (Scanner.HasPrecedingLineBreak || Token() == SyntaxKind.EqualsGreaterThanToken)
            {

                return Tristate.False;
            }
            var expr = ParseBinaryExpressionOrHigher(/*precedence*/ 0);
            if (!Scanner.HasPrecedingLineBreak && expr.Kind == SyntaxKind.Identifier && Token() == SyntaxKind.EqualsGreaterThanToken)
            {

                return Tristate.True;
            }
        }


        return Tristate.False;
    }


    public ArrowFunction ParseParenthesizedArrowFunctionExpressionHead(bool allowAmbiguity)
    {
        var node = new ArrowFunction
        {
            Pos = Scanner.StartPos,
            Modifiers = ParseModifiersForArrowFunction()
        };
        var isAsync = /*!!*/(GetModifierFlags(node) & ModifierFlags.Async) != 0;


        // Arrow functions are never generators.
        //
        // If we're speculatively parsing a signature for a parenthesized arrow function, then
        // we have to have a complete parameter list.  Otherwise we might see something like
        // a => (b => c)
        // And think that "(b =>" was actually a parenthesized arrow function with a missing
        // close paren.
        FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ isAsync, /*requireCompleteParameterList*/ !allowAmbiguity, node);
        if (node.Parameters == null)
        {

            return null;
        }
        if (!allowAmbiguity && Token() != SyntaxKind.EqualsGreaterThanToken && Token() != SyntaxKind.OpenBraceToken)
        {

            // Returning null here will cause our caller to rewind to where we started from.
            return null;
        }


        return node;
    }


    public /*Block | Expression*/IBlockOrExpression ParseArrowFunctionExpressionBody(bool isAsync)
    {
        if (Token() == SyntaxKind.OpenBraceToken)
        {

            return ParseFunctionBlock(/*allowYield*/ false, /*allowAwait*/ isAsync, /*ignoreMissingOpenBrace*/ false);
        }
        if (Token() != SyntaxKind.SemicolonToken &&
                        Token() != SyntaxKind.FunctionKeyword &&
                        Token() != SyntaxKind.ClassKeyword &&
                        IsStartOfStatement() &&
                        !IsStartOfExpressionStatement())
        {

            // Check if we got a plain statement (i.e. no expression-statements, no function/class expressions/declarations)
            //
            // Here we try to recover from a potential error situation in the case where the
            // user meant to supply a block. For example, if the user wrote:
            //
            //  a =>
            //      let v = 0;
            //  }
            //
            // they may be missing an open brace.  Check to see if that's the case so we can
            // try to recover better.  If we don't do this, then the next close curly we see may end
            // up preemptively closing the containing construct.
            //
            // Note: even when 'ignoreMissingOpenBrace' is passed as true, parseBody will still error.
            return ParseFunctionBlock(/*allowYield*/ false, /*allowAwait*/ isAsync, /*ignoreMissingOpenBrace*/ true);
        }


        return isAsync
            ? DoInAwaitContext(ParseAssignmentExpressionOrHigher)
            : DoOutsideOfAwaitContext(ParseAssignmentExpressionOrHigher);
    }


    public IExpression ParseConditionalExpressionRest(IExpression leftOperand)
    {
        var questionToken = (QuestionToken)ParseOptionalToken<QuestionToken>(SyntaxKind.QuestionToken);
        if (questionToken == null)
        {

            return leftOperand;
        }
        var node = new ConditionalExpression
        {
            Pos = leftOperand.Pos,
            Condition = leftOperand,

            QuestionToken = questionToken,

            WhenTrue = DoOutsideOfContext(DisallowInAndDecoratorContext, ParseAssignmentExpressionOrHigher),

            ColonToken = (ColonToken)ParseExpectedToken<ColonToken>(SyntaxKind.ColonToken, /*reportAtCurrentPosition*/ false,
            Diagnostics._0_expected, TokenToString(SyntaxKind.ColonToken)),

            WhenFalse = ParseAssignmentExpressionOrHigher()
        };

        return FinishNode(node);
    }


    public IExpression ParseBinaryExpressionOrHigher(int precedence)
    {
        var leftOperand = ParseUnaryExpressionOrHigher();

        if (leftOperand == null) throw new NullReferenceException();
        return ParseBinaryExpressionRest(precedence, leftOperand);
    }


    public bool IsInOrOfKeyword(SyntaxKind t)
    {

        return t == SyntaxKind.InKeyword || t == SyntaxKind.OfKeyword;
    }


    public IExpression ParseBinaryExpressionRest(int precedence, IExpression leftOperand)
    {
        while (true)
        {

            // We either have a binary operator here, or we're finished.  We call
            // reScanGreaterToken so that we merge token sequences like > and = into >=

            ReScanGreaterToken();
            var newPrecedence = GetBinaryOperatorPrecedence();
            var consumeCurrentOperator = Token() == SyntaxKind.AsteriskAsteriskToken ?
                                newPrecedence >= precedence :
                                newPrecedence > precedence;
            if (!consumeCurrentOperator)
            {

                break;
            }
            if (Token() == SyntaxKind.InKeyword && InDisallowInContext())
            {

                break;
            }
            if (Token() == SyntaxKind.AsKeyword)
            {
                if (Scanner.HasPrecedingLineBreak)
                {

                    break;
                }
                else
                {

                    NextToken();

                    leftOperand = MakeAsExpression(leftOperand, ParseType());
                }
            }
            else
            {

                leftOperand = MakeBinaryExpression(leftOperand, ParseTokenNode</*BinaryOperator*/Token>(Token()), ParseBinaryExpressionOrHigher(newPrecedence));
            }
        }


        return leftOperand;
    }


    public bool IsBinaryOperator()
    {
        if (InDisallowInContext() && Token() == SyntaxKind.InKeyword)
        {

            return false;
        }


        return GetBinaryOperatorPrecedence() > 0;
    }


    public int GetBinaryOperatorPrecedence()
    {
        return Token() switch
        {
            SyntaxKind.BarBarToken => 1,
            SyntaxKind.AmpersandAmpersandToken => 2,
            SyntaxKind.BarToken => 3,
            SyntaxKind.CaretToken => 4,
            SyntaxKind.AmpersandToken => 5,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.ExclamationEqualsToken or SyntaxKind.EqualsEqualsEqualsToken or SyntaxKind.ExclamationEqualsEqualsToken => 6,
            SyntaxKind.LessThanToken or SyntaxKind.GreaterThanToken or SyntaxKind.LessThanEqualsToken or SyntaxKind.GreaterThanEqualsToken or SyntaxKind.InstanceOfKeyword or SyntaxKind.InKeyword or SyntaxKind.AsKeyword => 7,
            SyntaxKind.LessThanLessThanToken or SyntaxKind.GreaterThanGreaterThanToken or SyntaxKind.GreaterThanGreaterThanGreaterThanToken => 8,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 9,
            SyntaxKind.AsteriskToken or SyntaxKind.SlashToken or SyntaxKind.PercentToken => 10,
            SyntaxKind.AsteriskAsteriskToken => 11,
            // -1 is lower than all other precedences.  Returning it will cause binary expression
            // parsing to stop.
            _ => -1,
        };
    }


    public BinaryExpression MakeBinaryExpression(IExpression left, /*BinaryOperator*/Token operatorToken, IExpression right)
    {
        var node = new BinaryExpression
        {
            Pos = left.Pos,
            Left = left,

            OperatorToken = operatorToken,

            Right = right
        };

        return FinishNode(node);
    }


    public AsExpression MakeAsExpression(IExpression left, ITypeNode right)
    {
        var node = new AsExpression
        {
            Pos = left.Pos,
            Expression = left,

            Type = right
        };

        return FinishNode(node);
    }


    public PrefixUnaryExpression ParsePrefixUnaryExpression()
    {
        var node = new PrefixUnaryExpression
        {
            Pos = Scanner.StartPos,
            Operator = /*(PrefixUnaryOperator)*/Token()
        };

        NextToken();

        node.Operand = ParseSimpleUnaryExpression();


        return FinishNode(node);
    }


    public DeleteExpression ParseDeleteExpression()
    {
        var node = new DeleteExpression() { Pos = Scanner.StartPos };

        NextToken();

        node.Expression = ParseSimpleUnaryExpression(); // as UnaryExpression;

        return FinishNode(node);
    }


    public TypeOfExpression ParseTypeOfExpression()
    {
        var node = new TypeOfExpression() { Pos = Scanner.StartPos };

        NextToken();

        node.Expression = ParseSimpleUnaryExpression(); //  as UnaryExpression;

        return FinishNode(node);
    }


    public VoidExpression ParseVoidExpression()
    {
        var node = new VoidExpression() { Pos = Scanner.StartPos };

        NextToken();

        node.Expression = ParseSimpleUnaryExpression(); //  as UnaryExpression;

        return FinishNode(node);
    }


    public bool IsAwaitExpression()
    {
        if (Token() == SyntaxKind.AwaitKeyword)
        {
            if (InAwaitContext())
            {

                return true;
            }


            // here we are using similar heuristics as 'isYieldExpression'
            return LookAhead(NextTokenIsIdentifierOnSameLine);
        }


        return false;
    }


    public AwaitExpression ParseAwaitExpression()
    {
        var node = new AwaitExpression() { Pos = Scanner.StartPos };

        NextToken();

        node.Expression = ParseSimpleUnaryExpression(); // as UnaryExpression;

        return FinishNode(node);
    }

    //UnaryExpression | BinaryExpression
    public IExpression ParseUnaryExpressionOrHigher()
    {
        if (IsUpdateExpression())
        {
            var incrementExpression = ParseIncrementExpression();

            return Token() == SyntaxKind.AsteriskAsteriskToken ?
                ParseBinaryExpressionRest(GetBinaryOperatorPrecedence(), incrementExpression) :
                incrementExpression;
        }
        var unaryOperator = Token();
        var simpleUnaryExpression = ParseSimpleUnaryExpression();
        if (Token() == SyntaxKind.AsteriskAsteriskToken)
        {
            var start = Scanner.SkipTriviaM(SourceText, simpleUnaryExpression.Pos ?? 0);
            if (simpleUnaryExpression.Kind == SyntaxKind.TypeAssertionExpression)
            {

                ParseErrorAtPosition(start, (simpleUnaryExpression.End ?? 0) - start, Diagnostics.A_type_assertion_expression_is_not_allowed_in_the_left_hand_side_of_an_exponentiation_expression_Consider_enclosing_the_expression_in_parentheses);
            }
            else
            {

                ParseErrorAtPosition(start, (simpleUnaryExpression.End ?? 0) - start, Diagnostics.An_unary_expression_with_the_0_operator_is_not_allowed_in_the_left_hand_side_of_an_exponentiation_expression_Consider_enclosing_the_expression_in_parentheses, TokenToString(unaryOperator));
            }
        }

        return simpleUnaryExpression;
    }


    public /*Unary*/IExpression ParseSimpleUnaryExpression()
    {
        switch (Token())
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.TildeToken:
            case SyntaxKind.ExclamationToken:

                return ParsePrefixUnaryExpression();
            case SyntaxKind.DeleteKeyword:

                return ParseDeleteExpression();
            case SyntaxKind.TypeOfKeyword:

                return ParseTypeOfExpression();
            case SyntaxKind.VoidKeyword:

                return ParseVoidExpression();
            case SyntaxKind.LessThanToken:

                // This is modified UnaryExpression grammar in TypeScript
                //  UnaryExpression (modified):
                //      < type > UnaryExpression
                return ParseTypeAssertion();
            case SyntaxKind.AwaitKeyword:
                if (IsAwaitExpression())
                {

                    return ParseAwaitExpression();
                }
                break;
            default:

                return ParseIncrementExpression();
        }
        return null;
    }


    public bool IsUpdateExpression()
    {
        switch (Token())
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.TildeToken:
            case SyntaxKind.ExclamationToken:
            case SyntaxKind.DeleteKeyword:
            case SyntaxKind.TypeOfKeyword:
            case SyntaxKind.VoidKeyword:
            case SyntaxKind.AwaitKeyword:

                return false;
            case SyntaxKind.LessThanToken:
                if (SourceFile.LanguageVariant != LanguageVariant.Jsx)
                {

                    return false;
                }
                break;
            default:

                return true;
        }
        return true;
    }


    public /*Increment*/IExpression ParseIncrementExpression()
    {
        if (Token() == SyntaxKind.PlusPlusToken || Token() == SyntaxKind.MinusMinusToken)
        {
            var node = new PrefixUnaryExpression
            {
                Pos = Scanner.StartPos,
                Operator = /*(PrefixUnaryOperator)*/Token()
            };

            NextToken();

            node.Operand = ParseLeftHandSideExpressionOrHigher();

            return FinishNode(node);
        }
        else
        if (SourceFile.LanguageVariant == LanguageVariant.Jsx && Token() == SyntaxKind.LessThanToken && LookAhead(NextTokenIsIdentifierOrKeyword))
        {

            // JSXElement is part of primaryExpression
            return ParseJsxElementOrSelfClosingElement(/*inExpressionContext*/ true);
        }
        var expression = ParseLeftHandSideExpressionOrHigher();


        //Debug.assert(isLeftHandSideExpression(expression));
        if ((Token() == SyntaxKind.PlusPlusToken || Token() == SyntaxKind.MinusMinusToken) && !Scanner.HasPrecedingLineBreak)
        {
            var node = new PostfixUnaryExpression
            {
                Pos = expression.Pos,
                Operand = expression,

                Operator = /*(PostfixUnaryOperator)*/Token()
            };

            NextToken();

            return FinishNode(node);
        }


        return expression;
    }


    public /*LeftHandSideExpression*/IExpression ParseLeftHandSideExpressionOrHigher()
    {
        var expression = Token() == SyntaxKind.SuperKeyword
                        ? ParseSuperExpression()
                        : ParseMemberExpressionOrHigher();


        // Now, we *may* be complete.  However, we might have consumed the start of a
        // CallExpression.  As such, we need to consume the rest of it here to be complete.
        return ParseCallExpressionRest(expression);
    }


    public IMemberExpression ParseMemberExpressionOrHigher()
    {
        var expression = ParsePrimaryExpression();

        return ParseMemberExpressionRest(expression);
    }


    public IMemberExpression ParseSuperExpression()
    {
        var expression = ParseTokenNode<PrimaryExpression>(Token());
        if (Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.DotToken || Token() == SyntaxKind.OpenBracketToken)
        {

            return expression;
        }
        var node = new PropertyAccessExpression
        {
            Pos = expression.Pos,
            Expression = expression
        };

        ParseExpectedToken<DotToken>(SyntaxKind.DotToken, /*reportAtCurrentPosition*/ false, Diagnostics.super_must_be_followed_by_an_argument_list_or_member_access);

        node.Name = ParseRightSideOfDot(/*allowIdentifierNames*/ true);

        return FinishNode(node);
    }


    public bool TagNamesAreEquivalent(IJsxTagNameExpression lhs, IJsxTagNameExpression rhs)
    {
        if (lhs.Kind != rhs.Kind)
        {

            return false;
        }
        if (lhs.Kind == SyntaxKind.Identifier)
        {

            return (lhs as Identifier).Text == (rhs as Identifier).Text;
        }
        if (lhs.Kind == SyntaxKind.ThisKeyword)
        {

            return true;
        }


        // If we are at this statement then we must have PropertyAccessExpression and because tag name in Jsx element can only
        // take forms of JsxTagNameExpression which includes an identifier, "this" expression, or another propertyAccessExpression
        // it is safe to case the expression property as such. See parseJsxElementName for how we parse tag name in Jsx element
        return true;
        //todo
        //((PropertyAccessExpression)lhs).name.text == ((PropertyAccessExpression)rhs).name.text &&
        //tagNamesAreEquivalent(((PropertyAccessExpression)lhs).expression as JsxTagNameExpression, ((PropertyAccessExpression)rhs).expression as JsxTagNameExpression);
    }


    public /*JsxElement | JsxSelfClosingElement*/PrimaryExpression ParseJsxElementOrSelfClosingElement(bool inExpressionContext)
    {
        var opening = ParseJsxOpeningOrSelfClosingElement(inExpressionContext);
        //var result = JsxElement | JsxSelfClosingElement;
        if (opening.Kind == SyntaxKind.JsxOpeningElement)
        {
            var node = new JsxElement
            {
                Pos = opening.Pos,
                OpeningElement = opening
            };

            var tn = (opening as JsxOpeningElement)?.TagName;
            tn ??= (opening as JsxSelfClosingElement)?.TagName;

            node.JsxChildren = ParseJsxChildren(tn); // IJsxTagNameExpression);

            node.ClosingElement = ParseJsxClosingElement(inExpressionContext);
            // todo check     node.closingElement.tagName as JsxTagNameExpression
            if (!TagNamesAreEquivalent(tn, node.ClosingElement.TagName))
            {

                ParseErrorAtPosition(node.ClosingElement.Pos ?? 0, (node.ClosingElement.End ?? 0) - (node.ClosingElement.Pos ?? 0), Diagnostics.Expected_corresponding_JSX_closing_tag_for_0, GetTextOfNodeFromSourceText(SourceText, tn));
            }


            var result = FinishNode(node);

            if (inExpressionContext && Token() == SyntaxKind.LessThanToken)
            {
                var invalidElement = TryParse(() => ParseJsxElementOrSelfClosingElement(/*inExpressionContext*/true));
                if (invalidElement != null)
                {

                    ParseErrorAtCurrentToken(Diagnostics.JSX_expressions_must_have_one_parent_element);
                    var badNode = new BinaryExpression
                    {
                        Pos = result.Pos,
                        End = invalidElement.End,

                        Left = result,

                        Right = invalidElement,

                        OperatorToken = (/*BinaryOperator*/Token)CreateMissingNode<Token>(SyntaxKind.CommaToken, /*reportAtCurrentPosition*/ false, /*diagnosticMessage*/ null)
                    };

                    badNode.OperatorToken.Pos = badNode.OperatorToken.End = badNode.Right.Pos;

                    return (JsxElement)(Node)badNode;
                }
            }



            return result;
        }
        else
        {

            Debug.Assert(opening.Kind == SyntaxKind.JsxSelfClosingElement);

            // Nothing else to do for self-closing elements
            var result = (JsxSelfClosingElement)opening;
            if (inExpressionContext && Token() == SyntaxKind.LessThanToken)
            {
                var invalidElement = TryParse(() => ParseJsxElementOrSelfClosingElement(/*inExpressionContext*/true));
                if (invalidElement != null)
                {

                    ParseErrorAtCurrentToken(Diagnostics.JSX_expressions_must_have_one_parent_element);
                    var badNode = new BinaryExpression
                    {
                        Pos = result.Pos,
                        End = invalidElement.End,

                        Left = result,

                        Right = invalidElement,

                        OperatorToken = (/*BinaryOperator*/Token)CreateMissingNode<Token>(SyntaxKind.CommaToken, /*reportAtCurrentPosition*/ false, /*diagnosticMessage*/ null)
                    };

                    badNode.OperatorToken.Pos = badNode.OperatorToken.End = badNode.Right.Pos;

                    return (JsxElement)(Node)badNode;
                }
            }

            return result;
        }

    }


    public JsxText ParseJsxText()
    {
        var node = new JsxText() { Pos = Scanner.StartPos };

        CurrentToken = Scanner.ScanJsxToken();

        return FinishNode(node);
    }


    public /*JsxChild*/Node ParseJsxChild()
    {
        switch (Token())
        {
            case SyntaxKind.JsxText:

                return ParseJsxText();
            case SyntaxKind.OpenBraceToken:

                return ParseJsxExpression(/*inExpressionContext*/ false);
            case SyntaxKind.LessThanToken:

                return ParseJsxElementOrSelfClosingElement(/*inExpressionContext*/ false);
        }

        Debug.Fail("Unknown JSX child kind " + Token());
        return null;
    }


    public NodeArray<IJsxChild> ParseJsxChildren(/*LeftHandSide*/IExpression openingTagName)
    {
        var result = CreateList<IJsxChild>(); //List<IJsxChild>(); // 
        var saveParsingContext = ParsingContext;

        ParsingContext |= 1 << (int)TsTypes.ParsingContext.JsxChildren;
        while (true)
        {

            CurrentToken = Scanner.ReScanJsxToken();
            if (Token() == SyntaxKind.LessThanSlashToken)
            {

                // Closing tag
                break;
            }
            else
            if (Token() == SyntaxKind.EndOfFileToken)
            {

                // If we hit EOF, issue the error at the tag that lacks the closing element
                // rather than at the end of the file (which is useless)
                ParseErrorAtPosition(openingTagName.Pos ?? 0, (openingTagName.End ?? 0) - (openingTagName.Pos ?? 0), Diagnostics.JSX_element_0_has_no_corresponding_closing_tag, GetTextOfNodeFromSourceText(SourceText, openingTagName));

                break;
            }
            else
            if (Token() == SyntaxKind.ConflictMarkerTrivia)
            {

                break;
            }

            result.Add(ParseJsxChild() as IJsxChild);
        }


        result.End = Scanner.TokenPos;


        ParsingContext = saveParsingContext;


        return result;
    }


    public JsxAttributes ParseJsxAttributes()
    {
        var jsxAttributes = new JsxAttributes
        {
            Pos = Scanner.StartPos,
            Properties = ParseList(TsTypes.ParsingContext.JsxAttributes, ParseJsxAttribute)
        };

        return FinishNode(jsxAttributes);
    }

    //JsxOpeningElement | JsxSelfClosingElement
    public Expression ParseJsxOpeningOrSelfClosingElement(bool inExpressionContext)
    {
        var fullStart = Scanner.StartPos;


        ParseExpected(SyntaxKind.LessThanToken);
        var tagName = ParseJsxElementName();
        var attributes = ParseJsxAttributes();
        //JsxOpeningLikeElement node = null;
        if (Token() == SyntaxKind.GreaterThanToken)
        {

            // Closing tag, so scan the immediately-following text with the JSX scanning instead
            // of regular scanning to avoid treating illegal characters (e.g. '#') as immediate
            // scanning errors
            var node = new JsxOpeningElement
            {
                Pos = fullStart,
                TagName = tagName,

                Attributes = attributes
            }; //(JsxOpeningElement)createNode(SyntaxKind.JsxOpeningElement, fullStart);

            ScanJsxText();
            return FinishNode(node);
        }
        else
        {

            ParseExpected(SyntaxKind.SlashToken);
            if (inExpressionContext)
            {

                ParseExpected(SyntaxKind.GreaterThanToken);
            }
            else
            {

                ParseExpected(SyntaxKind.GreaterThanToken, /*diagnostic*/ null, /*shouldAdvance*/ false);

                ScanJsxText();
            }

            var node = new JsxSelfClosingElement
            {
                Pos = fullStart,
                TagName = tagName,

                Attributes = attributes
            }; //(JsxSelfClosingElement)createNode(SyntaxKind.JsxSelfClosingElement, fullStart);
            return FinishNode(node);
        }


        //node.tagName = tagName;

        //node.attributes = attributes;


        //return finishNode(node);
    }


    public IJsxTagNameExpression ParseJsxElementName()
    {

        ScanJsxIdentifier();
        IJsxTagNameExpression _ = Token() == SyntaxKind.ThisKeyword ?
            ParseTokenNode<PrimaryExpression>(Token()) : ParseIdentifierName();
        if (Token() == SyntaxKind.ThisKeyword)
        {
            IJsxTagNameExpression expression2 = ParseTokenNode<PrimaryExpression>(Token());
            while (ParseOptional(SyntaxKind.DotToken))
            {
                PropertyAccessExpression propertyAccess = new()
                {
                    Pos = expression2.Pos,
                    Expression = expression2,

                    Name = ParseRightSideOfDot(/*allowIdentifierNames*/ true)
                }; //(PropertyAccessExpression)createNode(SyntaxKind.PropertyAccessExpression, expression.pos);

                expression2 = FinishNode(propertyAccess);
            }

            return expression2;
        }
        else
        {
            IJsxTagNameExpression expression2 = ParseIdentifierName();
            while (ParseOptional(SyntaxKind.DotToken))
            {
                PropertyAccessExpression propertyAccess = new()
                {
                    Pos = expression2.Pos,
                    Expression = expression2,

                    Name = ParseRightSideOfDot(/*allowIdentifierNames*/ true)
                }; //(PropertyAccessExpression)createNode(SyntaxKind.PropertyAccessExpression, expression.pos);

                expression2 = FinishNode(propertyAccess);
            }

            return expression2;
        }
    }


    public JsxExpression ParseJsxExpression(bool inExpressionContext)
    {
        var node = new JsxExpression() { Pos = Scanner.StartPos };


        ParseExpected(SyntaxKind.OpenBraceToken);
        if (Token() != SyntaxKind.CloseBraceToken)
        {

            node.DotDotDotToken = (DotDotDotToken)ParseOptionalToken<DotDotDotToken>(SyntaxKind.DotDotDotToken);

            node.Expression = ParseAssignmentExpressionOrHigher();
        }
        if (inExpressionContext)
        {

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            ParseExpected(SyntaxKind.CloseBraceToken, /*message*/ null, /*shouldAdvance*/ false);

            ScanJsxText();
        }


        return FinishNode(node);
    }

    //JsxAttribute | JsxSpreadAttribute
    public ObjectLiteralElement ParseJsxAttribute()
    {
        if (Token() == SyntaxKind.OpenBraceToken)
        {

            return ParseJsxSpreadAttribute();
        }


        ScanJsxIdentifier();
        var node = new JsxAttribute
        {
            Pos = Scanner.StartPos,
            Name = ParseIdentifierName()
        };
        if (Token() == SyntaxKind.EqualsToken)
        {
            node.Initializer = ScanJsxAttributeValue() switch
            {
                SyntaxKind.StringLiteral => (StringLiteral)ParseLiteralNode(),
                _ => ParseJsxExpression(/*inExpressionContext*/ true),
            };
        }

        return FinishNode(node);
    }


    public JsxSpreadAttribute ParseJsxSpreadAttribute()
    {
        var node = new JsxSpreadAttribute() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBraceToken);

        ParseExpected(SyntaxKind.DotDotDotToken);

        node.Expression = ParseExpression();

        ParseExpected(SyntaxKind.CloseBraceToken);

        return FinishNode(node);
    }


    public JsxClosingElement ParseJsxClosingElement(bool inExpressionContext)
    {
        var node = new JsxClosingElement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.LessThanSlashToken);

        node.TagName = ParseJsxElementName();
        if (inExpressionContext)
        {

            ParseExpected(SyntaxKind.GreaterThanToken);
        }
        else
        {

            ParseExpected(SyntaxKind.GreaterThanToken, /*diagnostic*/ null, /*shouldAdvance*/ false);

            ScanJsxText();
        }

        return FinishNode(node);
    }


    public TypeAssertion ParseTypeAssertion()
    {
        var node = new TypeAssertion() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.LessThanToken);

        node.Type = ParseType();

        ParseExpected(SyntaxKind.GreaterThanToken);

        node.Expression = ParseSimpleUnaryExpression(); // as UnaryExpression;

        return FinishNode(node);
    }


    public IMemberExpression ParseMemberExpressionRest(/*LeftHandSideExpression*/IMemberExpression expression)
    {
        while (true)
        {
            var dotToken = (DotToken)ParseOptionalToken<DotToken>(SyntaxKind.DotToken);
            if (dotToken != null)
            {
                var propertyAccess = new PropertyAccessExpression
                {
                    Pos = expression.Pos,
                    Expression = expression,

                    Name = ParseRightSideOfDot(/*allowIdentifierNames*/ true)
                };

                expression = FinishNode(propertyAccess);

                continue;
            }
            if (Token() == SyntaxKind.ExclamationToken && !Scanner.HasPrecedingLineBreak)
            {

                NextToken();
                var nonNullExpression = new NonNullExpression
                {
                    Pos = expression.Pos,
                    Expression = expression
                };

                expression = FinishNode(nonNullExpression);

                continue;
            }
            if (!InDecoratorContext() && ParseOptional(SyntaxKind.OpenBracketToken))
            {
                var indexedAccess = new ElementAccessExpression
                {
                    Pos = expression.Pos,
                    Expression = expression
                };
                if (Token() != SyntaxKind.CloseBracketToken)
                {

                    indexedAccess.ArgumentExpression = AllowInAnd(ParseExpression);
                    if (indexedAccess.ArgumentExpression.Kind == SyntaxKind.StringLiteral ||
                        indexedAccess.ArgumentExpression.Kind == SyntaxKind.NumericLiteral)
                    {
                        var literal = (LiteralExpression)indexedAccess.ArgumentExpression;//(LiteralExpression)
                    }
                }


                ParseExpected(SyntaxKind.CloseBracketToken);

                expression = FinishNode(indexedAccess);

                continue;
            }
            if (Token() == SyntaxKind.NoSubstitutionTemplateLiteral || Token() == SyntaxKind.TemplateHead)
            {
                var tagExpression = new TaggedTemplateExpression
                {
                    Pos = expression.Pos,
                    Tag = expression,

                    Template = Token() == SyntaxKind.NoSubstitutionTemplateLiteral
                    ? (Node)/*(NoSubstitutionTemplateLiteral)*/ParseLiteralNode()
                    : ParseTemplateExpression()
                };

                expression = FinishNode(tagExpression);

                continue;
            }


            return expression;
        }
    }


    public /*LeftHandSideExpression*/IMemberExpression ParseCallExpressionRest(/*LeftHandSideExpression*/IMemberExpression expression)
    {
        while (true)
        {

            expression = ParseMemberExpressionRest(expression);
            if (Token() == SyntaxKind.LessThanToken)
            {
                var typeArguments = TryParse(ParseTypeArgumentsInExpression);
                if (typeArguments == null)
                {

                    return expression;
                }
                var callExpr = new CallExpression
                {
                    Pos = expression.Pos,
                    Expression = expression,

                    TypeArguments = typeArguments,

                    Arguments = ParseArgumentList()
                };

                expression = FinishNode(callExpr);

                continue;
            }
            else
            if (Token() == SyntaxKind.OpenParenToken)
            {
                var callExpr = new CallExpression
                {
                    Pos = expression.Pos,
                    Expression = expression,

                    Arguments = ParseArgumentList()
                };

                expression = FinishNode(callExpr);

                continue;
            }


            return expression;
        }
    }


    public NodeArray<IExpression> ParseArgumentList()
    {

        ParseExpected(SyntaxKind.OpenParenToken);
        var result = ParseDelimitedList(TsTypes.ParsingContext.ArgumentExpressions, ParseArgumentExpression);

        ParseExpected(SyntaxKind.CloseParenToken);

        return result;
    }


    public NodeArray<ITypeNode> ParseTypeArgumentsInExpression()
    {
        if (!ParseOptional(SyntaxKind.LessThanToken))
        {

            return null;
        }
        var typeArguments = ParseDelimitedList(TsTypes.ParsingContext.TypeArguments, ParseType);
        if (!ParseExpected(SyntaxKind.GreaterThanToken))
        {

            // If it doesn't have the closing >  then it's definitely not an type argument list.
            return null;
        }


        // If we have a '<', then only parse this as a argument list if the type arguments
        // are complete and we have an open paren.  if we don't, rewind and return nothing.
        return typeArguments != null && CanFollowTypeArgumentsInExpression()
            ? typeArguments
            : null;
    }


    public bool CanFollowTypeArgumentsInExpression()
    {
        return Token() switch
        {
            SyntaxKind.OpenParenToken or SyntaxKind.DotToken or SyntaxKind.CloseParenToken or SyntaxKind.CloseBracketToken or SyntaxKind.ColonToken or SyntaxKind.SemicolonToken or SyntaxKind.QuestionToken or SyntaxKind.EqualsEqualsToken or SyntaxKind.EqualsEqualsEqualsToken or SyntaxKind.ExclamationEqualsToken or SyntaxKind.ExclamationEqualsEqualsToken or SyntaxKind.AmpersandAmpersandToken or SyntaxKind.BarBarToken or SyntaxKind.CaretToken or SyntaxKind.AmpersandToken or SyntaxKind.BarToken or SyntaxKind.CloseBraceToken or SyntaxKind.EndOfFileToken => true,// foo<x>
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   // these cases can't legally follow a type arg list.  However, they're not legal
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   // expressions either.  The user is probably in the middle of a generic type. So
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   // treat it as such.
            _ => false,// Anything else treat as an expression.
        };
    }


    public IPrimaryExpression ParsePrimaryExpression()
    {
        switch (Token())
        {
            case SyntaxKind.NumericLiteral:
            case SyntaxKind.StringLiteral:
            case SyntaxKind.NoSubstitutionTemplateLiteral:

                return ParseLiteralNode();
            case SyntaxKind.ThisKeyword:
            case SyntaxKind.SuperKeyword:
            case SyntaxKind.NullKeyword:
            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:

                return ParseTokenNode<PrimaryExpression>(Token());
            case SyntaxKind.OpenParenToken:

                return ParseParenthesizedExpression();
            case SyntaxKind.OpenBracketToken:

                return ParseArrayLiteralExpression();
            case SyntaxKind.OpenBraceToken:

                return ParseObjectLiteralExpression();
            case SyntaxKind.AsyncKeyword:
                if (!LookAhead(NextTokenIsFunctionKeywordOnSameLine))
                {

                    break;
                }


                return ParseFunctionExpression();
            case SyntaxKind.ClassKeyword:

                return ParseClassExpression();
            case SyntaxKind.FunctionKeyword:

                return ParseFunctionExpression();
            case SyntaxKind.NewKeyword:

                return ParseNewExpression();
            case SyntaxKind.SlashToken:
            case SyntaxKind.SlashEqualsToken:
                if (ReScanSlashToken() == SyntaxKind.RegularExpressionLiteral)
                {

                    return ParseLiteralNode();
                }

                break;
            case SyntaxKind.TemplateHead:

                return ParseTemplateExpression();
        }


        return ParseIdentifier(Diagnostics.Expression_expected);
    }


    public ParenthesizedExpression ParseParenthesizedExpression()
    {
        var node = new ParenthesizedExpression() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.CloseParenToken);

        return FinishNode(node);
    }


    public Expression ParseSpreadElement()
    {
        var node = new SpreadElement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.DotDotDotToken);

        node.Expression = ParseAssignmentExpressionOrHigher();

        return FinishNode(node);
    }


    public IExpression ParseArgumentOrArrayLiteralElement()
    {

        return Token() == SyntaxKind.DotDotDotToken ? ParseSpreadElement() :
            Token() == SyntaxKind.CommaToken ? (new OmittedExpression() { Pos = Scanner.StartPos }) /*createNode(SyntaxKind.OmittedExpression)*/ :
                ParseAssignmentExpressionOrHigher();
    }


    public IExpression ParseArgumentExpression()
    {

        return DoOutsideOfContext(DisallowInAndDecoratorContext, ParseArgumentOrArrayLiteralElement);
    }


    public ArrayLiteralExpression ParseArrayLiteralExpression()
    {
        var node = new ArrayLiteralExpression() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBracketToken);
        if (Scanner.HasPrecedingLineBreak)
        {

            node.MultiLine = true;
        }

        node.Elements = ParseDelimitedList(TsTypes.ParsingContext.ArrayLiteralMembers, ParseArgumentOrArrayLiteralElement);

        ParseExpected(SyntaxKind.CloseBracketToken);

        return FinishNode(node);
    }


    public IAccessorDeclaration TryParseAccessorDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        if (ParseContextualModifier(SyntaxKind.GetKeyword))
        {

            return ParseAccessorDeclaration(SyntaxKind.GetAccessor, fullStart, decorators, modifiers);
        }
        else
        if (ParseContextualModifier(SyntaxKind.SetKeyword))
        {

            return ParseAccessorDeclaration(SyntaxKind.SetAccessor, fullStart, decorators, modifiers);
        }


        return null;
    }


    public IObjectLiteralElementLike ParseObjectLiteralElement()
    {
        var fullStart = Scanner.StartPos;
        var dotDotDotToken = (DotDotDotToken)ParseOptionalToken<DotDotDotToken>(SyntaxKind.DotDotDotToken);
        if (dotDotDotToken != null)
        {
            var spreadElement = new SpreadAssignment
            {
                Pos = fullStart,
                Expression = ParseAssignmentExpressionOrHigher()
            };

            return AddJsDocComment(FinishNode(spreadElement));
        }
        var decorators = ParseDecorators();
        var modifiers = ParseModifiers();
        var accessor = TryParseAccessorDeclaration(fullStart, decorators, modifiers);
        if (accessor != null)
        {

            return accessor;
        }
        var asteriskToken = (AsteriskToken)ParseOptionalToken<AsteriskToken>(SyntaxKind.AsteriskToken);
        var tokenIsIdentifier = IsIdentifier();
        var propertyName = ParsePropertyName(); // parseIdentifierName(); // 
        var questionToken = (QuestionToken)ParseOptionalToken<QuestionToken>(SyntaxKind.QuestionToken);
        if (asteriskToken != null || Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken)
        {

            return ParseMethodDeclaration(fullStart, decorators, modifiers, asteriskToken, propertyName, questionToken);
        }
        var isShorthandPropertyAssignment =
                        tokenIsIdentifier && (Token() == SyntaxKind.CommaToken || Token() == SyntaxKind.CloseBraceToken || Token() == SyntaxKind.EqualsToken);
        if (isShorthandPropertyAssignment)
        {
            var shorthandDeclaration = new ShorthandPropertyAssignment
            {
                Pos = fullStart,
                Name = (Identifier)propertyName,

                QuestionToken = questionToken
            };
            var equalsToken = (EqualsToken)ParseOptionalToken<EqualsToken>(SyntaxKind.EqualsToken);
            if (equalsToken != null)
            {

                shorthandDeclaration.EqualsToken = equalsToken;

                shorthandDeclaration.ObjectAssignmentInitializer = AllowInAnd(ParseAssignmentExpressionOrHigher);
            }

            return AddJsDocComment(FinishNode(shorthandDeclaration));
        }
        else
        {
            var propertyAssignment = new PropertyAssignment
            {
                Pos = fullStart,
                Modifiers = modifiers,

                Name = propertyName,

                QuestionToken = questionToken
            };

            ParseExpected(SyntaxKind.ColonToken);

            propertyAssignment.Initializer = AllowInAnd(ParseAssignmentExpressionOrHigher);

            return AddJsDocComment(FinishNode(propertyAssignment));
        }
    }


    public ObjectLiteralExpression ParseObjectLiteralExpression()
    {
        var node = new ObjectLiteralExpression() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBraceToken);
        if (Scanner.HasPrecedingLineBreak)
        {

            node.MultiLine = true;
        }


        node.Properties = ParseDelimitedList(TsTypes.ParsingContext.ObjectLiteralMembers, ParseObjectLiteralElement, /*considerSemicolonAsDelimiter*/ true);

        ParseExpected(SyntaxKind.CloseBraceToken);

        return FinishNode(node);
    }


    public FunctionExpression ParseFunctionExpression()
    {
        var saveDecoratorContext = InDecoratorContext();
        if (saveDecoratorContext)
        {

            SetDecoratorContext(/*val*/ false);
        }
        var node = new FunctionExpression
        {
            Pos = Scanner.StartPos,
            Modifiers = ParseModifiers()
        };

        ParseExpected(SyntaxKind.FunctionKeyword);

        node.AsteriskToken = (AsteriskToken)ParseOptionalToken<AsteriskToken>(SyntaxKind.AsteriskToken);
        var isGenerator = /*!!*/node.AsteriskToken != null;
        var isAsync = /*!!*/(GetModifierFlags(node) & ModifierFlags.Async) != 0;

        node.Name =
            isGenerator && isAsync ? DoInYieldAndAwaitContext(ParseOptionalIdentifier) :
                isGenerator ? DoInYieldContext(ParseOptionalIdentifier) :
                    isAsync ? DoInAwaitContext(ParseOptionalIdentifier) :
                        ParseOptionalIdentifier();


        FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ isGenerator, /*awaitContext*/ isAsync, /*requireCompleteParameterList*/ false, node);

        node.Body = ParseFunctionBlock(/*allowYield*/ isGenerator, /*allowAwait*/ isAsync, /*ignoreMissingOpenBrace*/ false);
        if (saveDecoratorContext)
        {

            SetDecoratorContext(/*val*/ true);
        }


        return AddJsDocComment(FinishNode(node));
    }


    public Identifier ParseOptionalIdentifier()
    {

        return IsIdentifier() ? ParseIdentifier() : null;
    }


    public /*NewExpression | MetaProperty*/IPrimaryExpression ParseNewExpression()
    {
        var fullStart = Scanner.StartPos;

        ParseExpected(SyntaxKind.NewKeyword);
        if (ParseOptional(SyntaxKind.DotToken))
        {
            var node = new MetaProperty
            {
                Pos = fullStart,
                KeywordToken = SyntaxKind.NewKeyword,

                Name = ParseIdentifierName()
            };

            return FinishNode(node);
        }
        else
        {
            var node = new NewExpression
            {
                Pos = fullStart,
                Expression = ParseMemberExpressionOrHigher(),

                TypeArguments = TryParse(ParseTypeArgumentsInExpression)
            };
            if (node.TypeArguments != null || Token() == SyntaxKind.OpenParenToken)
            {

                node.Arguments = ParseArgumentList();
            }

            return FinishNode(node);
        }
    }


    public Block ParseBlock(bool ignoreMissingOpenBrace, DiagnosticMessage diagnosticMessage = null)
    {
        var node = new Block() { Pos = Scanner.StartPos };
        if (ParseExpected(SyntaxKind.OpenBraceToken, diagnosticMessage) || ignoreMissingOpenBrace)
        {
            if (Scanner.HasPrecedingLineBreak)
            {

                node.MultiLine = true;
            }


            node.Statements = ParseList2(TsTypes.ParsingContext.BlockStatements, ParseStatement);

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            node.Statements = new NodeArray<IStatement>(); //.Cast<Node>().ToList(); createMissingList
        }

        return FinishNode(node);
    }

    public Block ParseFunctionBlock(bool allowYield, bool allowAwait, bool ignoreMissingOpenBrace, DiagnosticMessage diagnosticMessage = null)
    {

        var savedYieldContext = InYieldContext();

        SetYieldContext(allowYield);
        var savedAwaitContext = InAwaitContext();

        SetAwaitContext(allowAwait);
        var saveDecoratorContext = InDecoratorContext();
        if (saveDecoratorContext)
        {

            SetDecoratorContext(/*val*/ false);
        }

        Block block = null;
        if (Optimized && Token() == SyntaxKind.OpenBraceToken)
        {
            var node = new Block() { Pos = Scanner.StartPos, Statements = new NodeArray<IStatement>() };

            SyntaxKind token;
            int openBraces = 1;
            do
            {
                token = NextToken();

                if (token == SyntaxKind.NoSubstitutionTemplateLiteral ||
                    token == SyntaxKind.TemplateHead)
                {
                    if (token == SyntaxKind.NoSubstitutionTemplateLiteral)
                        ParseLiteralNode();
                    else
                        ParseTemplateExpression();

                    continue;
                }

                if (token == SyntaxKind.OpenBraceToken)
                    openBraces++;
                else if (token == SyntaxKind.CloseBraceToken)
                {
                    openBraces--;
                    if (openBraces == 0)
                    {
                        ParseExpected(SyntaxKind.CloseBraceToken);
                        block = FinishNode(node);
                        break;
                    }
                }
            }
            while (token != SyntaxKind.EndOfFileToken);

            if (block == null)
            {
                ParseExpected(SyntaxKind.CloseBraceToken);
                block = FinishNode(node);
            }
        }
        else
            block = ParseBlock(ignoreMissingOpenBrace, diagnosticMessage);

        if (saveDecoratorContext)
        {

            SetDecoratorContext(/*val*/ true);
        }


        SetYieldContext(savedYieldContext);

        SetAwaitContext(savedAwaitContext);


        return block;
    }


    public EmptyStatement ParseEmptyStatement()
    {
        var node = new EmptyStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.SemicolonToken);

        return FinishNode(node);
    }


    public IfStatement ParseIfStatement()
    {
        var node = new IfStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.IfKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.CloseParenToken);

        node.ThenStatement = ParseStatement();

        node.ElseStatement = ParseOptional(SyntaxKind.ElseKeyword) ? ParseStatement() : null;

        return FinishNode(node);
    }


    public DoStatement ParseDoStatement()
    {
        var node = new DoStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.DoKeyword);

        node.Statement = ParseStatement();

        ParseExpected(SyntaxKind.WhileKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.CloseParenToken);


        // From: https://mail.mozilla.org/pipermail/es-discuss/2011-August/016188.html
        // 157 min --- All allen at wirfs-brock.com CONF --- "do{;}while(false)false" prohibited in
        // spec but allowed in consensus reality. Approved -- this is the de-facto standard whereby
        //  do;while(0)x will have a semicolon inserted before x.
        ParseOptional(SyntaxKind.SemicolonToken);

        return FinishNode(node);
    }


    public WhileStatement ParseWhileStatement()
    {
        var node = new WhileStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.WhileKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.CloseParenToken);

        node.Statement = ParseStatement();

        return FinishNode(node);
    }


    public Statement ParseForOrForInOrForOfStatement()
    {
        var pos = GetNodePos();

        ParseExpected(SyntaxKind.ForKeyword);
        var awaitToken = (AwaitKeywordToken)ParseOptionalToken<AwaitKeywordToken>(SyntaxKind.AwaitKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);
        IVariableDeclarationListOrExpression initializer = null;
        //Node initializer = null;
        if (Token() != SyntaxKind.SemicolonToken)
        {
            if (Token() == SyntaxKind.VarKeyword || Token() == SyntaxKind.LetKeyword || Token() == SyntaxKind.ConstKeyword)
            {

                initializer = ParseVariableDeclarationList(/*inForStatementInitializer*/ true);
            }
            else
            {

                initializer = DisallowInAnd(ParseExpression);
            }
        }
        IterationStatement forOrForInOrForOfStatement = null;
        if (awaitToken != null ? ParseExpected(SyntaxKind.OfKeyword) : ParseOptional(SyntaxKind.OfKeyword))
        {
            var forOfStatement = new ForOfStatement
            {
                Pos = pos,
                AwaitModifier = awaitToken,

                Initializer = initializer,

                Expression = AllowInAnd(ParseAssignmentExpressionOrHigher)
            };

            ParseExpected(SyntaxKind.CloseParenToken);

            forOrForInOrForOfStatement = forOfStatement;
        }
        else
        if (ParseOptional(SyntaxKind.InKeyword))
        {
            var forInStatement = new ForInStatement
            {
                Pos = pos,
                Initializer = initializer,

                Expression = AllowInAnd(ParseExpression)
            };

            ParseExpected(SyntaxKind.CloseParenToken);

            forOrForInOrForOfStatement = forInStatement;
        }
        else
        {
            var forStatement = new ForStatement
            {
                Pos = pos,
                Initializer = initializer
            };

            ParseExpected(SyntaxKind.SemicolonToken);
            if (Token() != SyntaxKind.SemicolonToken && Token() != SyntaxKind.CloseParenToken)
            {

                forStatement.Condition = AllowInAnd(ParseExpression);
            }

            ParseExpected(SyntaxKind.SemicolonToken);
            if (Token() != SyntaxKind.CloseParenToken)
            {

                forStatement.Incrementor = AllowInAnd(ParseExpression);
            }

            ParseExpected(SyntaxKind.CloseParenToken);

            forOrForInOrForOfStatement = forStatement;
        }


        forOrForInOrForOfStatement.Statement = ParseStatement();


        return FinishNode(forOrForInOrForOfStatement);
    }


    public IBreakOrContinueStatement ParseBreakOrContinueStatement(SyntaxKind kind)
    {
        var node = kind == SyntaxKind.ContinueStatement ? (IBreakOrContinueStatement)new ContinueStatement { Pos = Scanner.StartPos } : kind == SyntaxKind.BreakStatement ? new BreakStatement { Pos = Scanner.StartPos } : throw new NotSupportedException("parseBreakOrContinueStatement");


        ParseExpected(kind == SyntaxKind.BreakStatement ? SyntaxKind.BreakKeyword : SyntaxKind.ContinueKeyword);
        if (!CanParseSemicolon())
        {

            node.Label = ParseIdentifier();
        }


        ParseSemicolon();

        return FinishNode(node);
    }


    public ReturnStatement ParseReturnStatement()
    {
        var node = new ReturnStatement() { Pos = Scanner.StartPos };


        ParseExpected(SyntaxKind.ReturnKeyword);
        if (!CanParseSemicolon())
        {

            node.Expression = AllowInAnd(ParseExpression);
        }


        ParseSemicolon();

        return FinishNode(node);
    }


    public WithStatement ParseWithStatement()
    {
        var node = new WithStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.WithKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.CloseParenToken);

        node.Statement = ParseStatement();

        return FinishNode(node);
    }


    public CaseClause ParseCaseClause()
    {
        var node = new CaseClause() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.CaseKeyword);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.ColonToken);

        node.Statements = ParseList2(TsTypes.ParsingContext.SwitchClauseStatements, ParseStatement);

        return FinishNode(node);
    }


    public DefaultClause ParseDefaultClause()
    {
        var node = new DefaultClause() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.DefaultKeyword);

        ParseExpected(SyntaxKind.ColonToken);

        node.Statements = ParseList2(TsTypes.ParsingContext.SwitchClauseStatements, ParseStatement);

        return FinishNode(node);
    }


    public ICaseOrDefaultClause ParseCaseOrDefaultClause()
    {

        return Token() == SyntaxKind.CaseKeyword ? ParseCaseClause() : ParseDefaultClause();
    }


    public SwitchStatement ParseSwitchStatement()
    {
        var node = new SwitchStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.SwitchKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = AllowInAnd(ParseExpression);

        ParseExpected(SyntaxKind.CloseParenToken);
        var caseBlock = new CaseBlock() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBraceToken);

        caseBlock.Clauses = ParseList(TsTypes.ParsingContext.SwitchClauses, ParseCaseOrDefaultClause);

        ParseExpected(SyntaxKind.CloseBraceToken);

        node.CaseBlock = FinishNode(caseBlock);

        return FinishNode(node);
    }


    public ThrowStatement ParseThrowStatement()
    {
        var node = new ThrowStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.ThrowKeyword);

        node.Expression = Scanner.HasPrecedingLineBreak ? null : AllowInAnd(ParseExpression);

        ParseSemicolon();

        return FinishNode(node);
    }


    public TryStatement ParseTryStatement()
    {
        var node = new TryStatement() { Pos = Scanner.StartPos };


        ParseExpected(SyntaxKind.TryKeyword);

        node.TryBlock = ParseBlock(/*ignoreMissingOpenBrace*/ false);

        node.CatchClause = Token() == SyntaxKind.CatchKeyword ? ParseCatchClause() : null;
        if (node.CatchClause == null || Token() == SyntaxKind.FinallyKeyword)
        {

            ParseExpected(SyntaxKind.FinallyKeyword);

            node.FinallyBlock = ParseBlock(/*ignoreMissingOpenBrace*/ false);
        }


        return FinishNode(node);
    }


    public CatchClause ParseCatchClause()
    {
        var result = new CatchClause() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.CatchKeyword);
        if (ParseExpected(SyntaxKind.OpenParenToken))
        {

            result.VariableDeclaration = ParseVariableDeclaration();
        }


        ParseExpected(SyntaxKind.CloseParenToken);

        result.Block = ParseBlock(/*ignoreMissingOpenBrace*/ false);

        return FinishNode(result);
    }


    public DebuggerStatement ParseDebuggerStatement()
    {
        var node = new DebuggerStatement() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.DebuggerKeyword);

        ParseSemicolon();

        return FinishNode(node);
    }


    public /*ExpressionStatement | LabeledStatement*/Statement ParseExpressionOrLabeledStatement()
    {
        var fullStart = Scanner.StartPos;
        var expression = AllowInAnd(ParseExpression);
        if (expression.Kind == SyntaxKind.Identifier && ParseOptional(SyntaxKind.ColonToken))
        {
            var labeledStatement = new LabeledStatement
            {
                Pos = fullStart,
                Label = (Identifier)expression,

                Statement = ParseStatement()
            };

            return AddJsDocComment(FinishNode(labeledStatement));
        }
        else
        {
            var expressionStatement = new ExpressionStatement
            {
                Pos = fullStart,
                Expression = expression
            };

            ParseSemicolon();

            return AddJsDocComment(FinishNode(expressionStatement));
        }
    }


    public bool NextTokenIsIdentifierOrKeywordOnSameLine()
    {

        NextToken();

        return TokenIsIdentifierOrKeyword(Token()) && !Scanner.HasPrecedingLineBreak;
    }


    public bool NextTokenIsFunctionKeywordOnSameLine()
    {

        NextToken();

        return Token() == SyntaxKind.FunctionKeyword && !Scanner.HasPrecedingLineBreak;
    }


    public bool NextTokenIsIdentifierOrKeywordOrNumberOnSameLine()
    {

        NextToken();

        return (TokenIsIdentifierOrKeyword(Token()) || Token() == SyntaxKind.NumericLiteral) && !Scanner.HasPrecedingLineBreak;
    }


    public bool IsDeclaration()
    {
        while (true)
        {
            switch (Token())
            {
                case SyntaxKind.VarKeyword:
                case SyntaxKind.LetKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.FunctionKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.EnumKeyword:

                    return true;
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.TypeKeyword:

                    return NextTokenIsIdentifierOnSameLine();
                case SyntaxKind.ModuleKeyword:
                case SyntaxKind.NamespaceKeyword:

                    return NextTokenIsIdentifierOrStringLiteralOnSameLine();
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.AsyncKeyword:
                case SyntaxKind.DeclareKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.ReadonlyKeyword:

                    NextToken();
                    if (Scanner.HasPrecedingLineBreak)
                    {

                        return false;
                    }

                    continue;
                case SyntaxKind.GlobalKeyword:

                    NextToken();

                    return Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.Identifier || Token() == SyntaxKind.ExportKeyword;
                case SyntaxKind.ImportKeyword:

                    NextToken();

                    return Token() == SyntaxKind.StringLiteral || Token() == SyntaxKind.AsteriskToken ||
                        Token() == SyntaxKind.OpenBraceToken || TokenIsIdentifierOrKeyword(Token());
                case SyntaxKind.ExportKeyword:

                    NextToken();
                    if (Token() == SyntaxKind.EqualsToken || Token() == SyntaxKind.AsteriskToken ||
                                                Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.DefaultKeyword ||
                                                Token() == SyntaxKind.AsKeyword)
                    {

                        return true;
                    }

                    continue;
                case SyntaxKind.StaticKeyword:

                    NextToken();

                    continue;
                default:

                    return false;
            }
        }
    }


    public bool IsStartOfDeclaration()
    {

        return LookAhead(IsDeclaration);
    }


    public bool IsStartOfStatement()
    {
        return Token() switch
        {
            SyntaxKind.AtToken or SyntaxKind.SemicolonToken or SyntaxKind.OpenBraceToken or SyntaxKind.VarKeyword or SyntaxKind.LetKeyword or SyntaxKind.FunctionKeyword or SyntaxKind.ClassKeyword or SyntaxKind.EnumKeyword or SyntaxKind.IfKeyword or SyntaxKind.DoKeyword or SyntaxKind.WhileKeyword or SyntaxKind.ForKeyword or SyntaxKind.ContinueKeyword or SyntaxKind.BreakKeyword or SyntaxKind.ReturnKeyword or SyntaxKind.WithKeyword or SyntaxKind.SwitchKeyword or SyntaxKind.ThrowKeyword or SyntaxKind.TryKeyword or SyntaxKind.DebuggerKeyword or SyntaxKind.CatchKeyword or SyntaxKind.FinallyKeyword => true,
            SyntaxKind.ConstKeyword or SyntaxKind.ExportKeyword or SyntaxKind.ImportKeyword => IsStartOfDeclaration(),
            SyntaxKind.AsyncKeyword or SyntaxKind.DeclareKeyword or SyntaxKind.InterfaceKeyword or SyntaxKind.ModuleKeyword or SyntaxKind.NamespaceKeyword or SyntaxKind.TypeKeyword or SyntaxKind.GlobalKeyword => true,// When these don't start a declaration, they're an identifier in an expression statement
            SyntaxKind.PublicKeyword or SyntaxKind.PrivateKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.StaticKeyword or SyntaxKind.ReadonlyKeyword => IsStartOfDeclaration() || !LookAhead(NextTokenIsIdentifierOrKeywordOnSameLine),// When these don't start a declaration, they may be the start of a class member if an identifier
                                                                                                                                                                                                                                             // immediately follows. Otherwise they're an identifier in an expression statement.
            _ => IsStartOfExpression(),
        };
    }


    public bool NextTokenIsIdentifierOrStartOfDestructuring()
    {

        NextToken();

        return IsIdentifier() || Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.OpenBracketToken;
    }


    public bool IsLetDeclaration()
    {

        // In ES6 'let' always starts a lexical declaration if followed by an identifier or {
        // or [.
        return LookAhead(NextTokenIsIdentifierOrStartOfDestructuring);
    }


    public IStatement ParseStatement()
    {
        switch (Token())
        {
            case SyntaxKind.SemicolonToken:

                return ParseEmptyStatement();
            case SyntaxKind.OpenBraceToken:

                return ParseBlock(/*ignoreMissingOpenBrace*/ false);
            case SyntaxKind.VarKeyword:

                return ParseVariableStatement(Scanner.StartPos, /*decorators*/ null, /*modifiers*/ null);
            case SyntaxKind.LetKeyword:
                if (IsLetDeclaration())
                {

                    return ParseVariableStatement(Scanner.StartPos, /*decorators*/ null, /*modifiers*/ null);
                }

                break;
            case SyntaxKind.FunctionKeyword:

                return ParseFunctionDeclaration(Scanner.StartPos, /*decorators*/ null, /*modifiers*/ null);
            case SyntaxKind.ClassKeyword:

                return ParseClassDeclaration(Scanner.StartPos, /*decorators*/ null, /*modifiers*/ null);
            case SyntaxKind.IfKeyword:

                return ParseIfStatement();
            case SyntaxKind.DoKeyword:

                return ParseDoStatement();
            case SyntaxKind.WhileKeyword:

                return ParseWhileStatement();
            case SyntaxKind.ForKeyword:

                return ParseForOrForInOrForOfStatement();
            case SyntaxKind.ContinueKeyword:

                return ParseBreakOrContinueStatement(SyntaxKind.ContinueStatement);
            case SyntaxKind.BreakKeyword:

                return ParseBreakOrContinueStatement(SyntaxKind.BreakStatement);
            case SyntaxKind.ReturnKeyword:

                return ParseReturnStatement();
            case SyntaxKind.WithKeyword:

                return ParseWithStatement();
            case SyntaxKind.SwitchKeyword:

                return ParseSwitchStatement();
            case SyntaxKind.ThrowKeyword:

                return ParseThrowStatement();
            case SyntaxKind.TryKeyword:
            case SyntaxKind.CatchKeyword:
            case SyntaxKind.FinallyKeyword:

                return ParseTryStatement();
            case SyntaxKind.DebuggerKeyword:

                return ParseDebuggerStatement();
            case SyntaxKind.AtToken:

                return ParseDeclaration();
            case SyntaxKind.AsyncKeyword:
            case SyntaxKind.InterfaceKeyword:
            case SyntaxKind.TypeKeyword:
            case SyntaxKind.ModuleKeyword:
            case SyntaxKind.NamespaceKeyword:
            case SyntaxKind.DeclareKeyword:
            case SyntaxKind.ConstKeyword:
            case SyntaxKind.EnumKeyword:
            case SyntaxKind.ExportKeyword:
            case SyntaxKind.ImportKeyword:
            case SyntaxKind.PrivateKeyword:
            case SyntaxKind.ProtectedKeyword:
            case SyntaxKind.PublicKeyword:
            case SyntaxKind.AbstractKeyword:
            case SyntaxKind.StaticKeyword:
            case SyntaxKind.ReadonlyKeyword:
            case SyntaxKind.GlobalKeyword:
                if (IsStartOfDeclaration())
                {

                    return ParseDeclaration();
                }

                break;
        }

        return ParseExpressionOrLabeledStatement();
    }


    public IStatement ParseDeclaration()
    {
        var fullStart = GetNodePos();
        var decorators = ParseDecorators();
        var modifiers = ParseModifiers();
        switch (Token())
        {
            case SyntaxKind.VarKeyword:
            case SyntaxKind.LetKeyword:
            case SyntaxKind.ConstKeyword:

                return ParseVariableStatement(fullStart, decorators, modifiers);
            case SyntaxKind.FunctionKeyword:

                return ParseFunctionDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.ClassKeyword:

                return ParseClassDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.InterfaceKeyword:

                return ParseInterfaceDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.TypeKeyword:

                return ParseTypeAliasDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.EnumKeyword:

                return ParseEnumDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.GlobalKeyword:
            case SyntaxKind.ModuleKeyword:
            case SyntaxKind.NamespaceKeyword:

                return ParseModuleDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.ImportKeyword:

                return ParseImportDeclarationOrImportEqualsDeclaration(fullStart, decorators, modifiers);
            case SyntaxKind.ExportKeyword:

                NextToken();
                return Token() switch
                {
                    SyntaxKind.DefaultKeyword or SyntaxKind.EqualsToken => ParseExportAssignment(fullStart, decorators, modifiers),
                    SyntaxKind.AsKeyword => ParseNamespaceExportDeclaration(fullStart, decorators, modifiers),
                    _ => ParseExportDeclaration(fullStart, decorators, modifiers),
                };
            default:

                if (decorators?.Any() == true || modifiers?.Any() == true)
                {
                    // We reached this point because we encountered decorators and/or modifiers and assumed a declaration
                    // would follow. For recovery and error reporting purposes, return an incomplete declaration.
                    var node = (Statement)CreateMissingNode<Statement>(SyntaxKind.MissingDeclaration, /*reportAtCurrentPosition*/ true, Diagnostics.Declaration_expected);
                    node.Pos = fullStart;
                    node.Decorators = decorators;
                    node.Modifiers = modifiers;
                    return FinishNode(node);
                }
                break;
        }
        return null;
    }


    public bool NextTokenIsIdentifierOrStringLiteralOnSameLine()
    {

        NextToken();

        return !Scanner.HasPrecedingLineBreak && (IsIdentifier() || Token() == SyntaxKind.StringLiteral);
    }


    public Block ParseFunctionBlockOrSemicolon(bool isGenerator, bool isAsync, DiagnosticMessage diagnosticMessage = null)
    {
        if (Token() != SyntaxKind.OpenBraceToken && CanParseSemicolon())
        {

            ParseSemicolon();

            return null;
        }


        return ParseFunctionBlock(isGenerator, isAsync, /*ignoreMissingOpenBrace*/ false, diagnosticMessage);
    }


    public IArrayBindingElement ParseArrayBindingElement()
    {
        if (Token() == SyntaxKind.CommaToken)
        {

            return new OmittedExpression { Pos = Scanner.StartPos }; //(OmittedExpression)createNode(SyntaxKind.OmittedExpression);
        }
        var node = new BindingElement
        {
            Pos = Scanner.StartPos,
            DotDotDotToken = (DotDotDotToken)ParseOptionalToken<DotDotDotToken>(SyntaxKind.DotDotDotToken),

            Name = ParseIdentifierOrPattern(),

            Initializer = ParseBindingElementInitializer(/*inParameter*/ false)
        };

        return FinishNode(node);
    }


    public IArrayBindingElement ParseObjectBindingElement()
    {
        var node = new BindingElement
        {
            Pos = Scanner.StartPos,
            DotDotDotToken = (DotDotDotToken)ParseOptionalToken<DotDotDotToken>(SyntaxKind.DotDotDotToken)
        };
        var tokenIsIdentifier = IsIdentifier();
        var propertyName = ParsePropertyName();
        if (tokenIsIdentifier && Token() != SyntaxKind.ColonToken)
        {

            node.Name = (Identifier)propertyName;
        }
        else
        {

            ParseExpected(SyntaxKind.ColonToken);

            node.PropertyName = propertyName;

            node.Name = ParseIdentifierOrPattern();
        }

        node.Initializer = ParseBindingElementInitializer(/*inParameter*/ false);

        return FinishNode(node);
    }


    public ObjectBindingPattern ParseObjectBindingPattern()
    {
        var node = new ObjectBindingPattern() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBraceToken);

        node.Elements = ParseDelimitedList(TsTypes.ParsingContext.ObjectBindingElements, ParseObjectBindingElement);

        ParseExpected(SyntaxKind.CloseBraceToken);

        return FinishNode(node);
    }


    public ArrayBindingPattern ParseArrayBindingPattern()
    {
        var node = new ArrayBindingPattern() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.OpenBracketToken);

        node.Elements = ParseDelimitedList(TsTypes.ParsingContext.ArrayBindingElements, ParseArrayBindingElement);

        ParseExpected(SyntaxKind.CloseBracketToken);

        return FinishNode(node);
    }


    public bool IsIdentifierOrPattern()
    {

        return Token() == SyntaxKind.OpenBraceToken || Token() == SyntaxKind.OpenBracketToken || IsIdentifier();
    }


    public /*Identifier | BindingPattern*/Node ParseIdentifierOrPattern()
    {
        if (Token() == SyntaxKind.OpenBracketToken)
        {

            return ParseArrayBindingPattern();
        }
        if (Token() == SyntaxKind.OpenBraceToken)
        {

            return ParseObjectBindingPattern();
        }

        return ParseIdentifier();
    }


    public VariableDeclaration ParseVariableDeclaration()
    {
        var node = new VariableDeclaration
        {
            Pos = Scanner.StartPos,
            Name = ParseIdentifierOrPattern(),

            Type = ParseTypeAnnotation()
        };
        if (!IsInOrOfKeyword(Token()))
        {

            node.Initializer = ParseInitializer(/*inParameter*/ false);
        }

        return FinishNode(node);
    }


    public IVariableDeclarationList ParseVariableDeclarationList(bool inForStatementInitializer)
    {
        var node = new VariableDeclarationList() { Pos = Scanner.StartPos };
        switch (Token())
        {
            case SyntaxKind.VarKeyword:

                break;
            case SyntaxKind.LetKeyword:

                node.Flags |= NodeFlags.Let;

                break;
            case SyntaxKind.ConstKeyword:

                node.Flags |= NodeFlags.Const;

                break;
            default:

                Debug.Fail();
                break;
        }


        NextToken();
        if (Token() == SyntaxKind.OfKeyword && LookAhead(CanFollowContextualOfKeyword))
        {

            node.Declarations = CreateMissingList<VariableDeclaration>();
        }
        else
        {
            var savedDisallowIn = InDisallowInContext();

            SetDisallowInContext(inForStatementInitializer);


            node.Declarations = ParseDelimitedList(TsTypes.ParsingContext.VariableDeclarations, ParseVariableDeclaration);


            SetDisallowInContext(savedDisallowIn);
        }


        return FinishNode(node);
    }


    public bool CanFollowContextualOfKeyword()
    {

        return NextTokenIsIdentifier() && NextToken() == SyntaxKind.CloseParenToken;
    }


    public VariableStatement ParseVariableStatement(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new VariableStatement
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers,

            DeclarationList = ParseVariableDeclarationList(/*inForStatementInitializer*/ false)
        };

        ParseSemicolon();

        return AddJsDocComment(FinishNode(node));
    }


    public FunctionDeclaration ParseFunctionDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new FunctionDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };

        ParseExpected(SyntaxKind.FunctionKeyword);

        node.AsteriskToken = (AsteriskToken)ParseOptionalToken<AsteriskToken>(SyntaxKind.AsteriskToken);

        node.Name = HasModifier(node, ModifierFlags.Default) ? ParseOptionalIdentifier() : ParseIdentifier();
        var isGenerator = /*!!*/node.AsteriskToken != null;
        var isAsync = HasModifier(node, ModifierFlags.Async);

        FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ isGenerator, /*awaitContext*/ isAsync, /*requireCompleteParameterList*/ false, node);

        node.Body = ParseFunctionBlockOrSemicolon(isGenerator, isAsync, Diagnostics.or_expected);

        return AddJsDocComment(FinishNode(node));
    }


    public ConstructorDeclaration ParseConstructorDeclaration(int pos, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new ConstructorDeclaration
        {
            Pos = pos,
            Decorators = decorators,

            Modifiers = modifiers
        };

        ParseExpected(SyntaxKind.ConstructorKeyword);

        FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, node);

        node.Body = ParseFunctionBlockOrSemicolon(/*isGenerator*/ false, /*isAsync*/ false, Diagnostics.or_expected);

        return AddJsDocComment(FinishNode(node));
    }


    public MethodDeclaration ParseMethodDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers, AsteriskToken asteriskToken, IPropertyName name, QuestionToken questionToken, DiagnosticMessage diagnosticMessage = null)
    {
        var method = new MethodDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers,

            AsteriskToken = asteriskToken,

            Name = name,

            QuestionToken = questionToken
        };
        var isGenerator = /*!!*/asteriskToken != null;
        var isAsync = HasModifier(method, ModifierFlags.Async);

        FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ isGenerator, /*awaitContext*/ isAsync, /*requireCompleteParameterList*/ false, method);

        method.Body = ParseFunctionBlockOrSemicolon(isGenerator, isAsync, diagnosticMessage);

        return AddJsDocComment(FinishNode(method));
    }


    public ClassElement ParsePropertyDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers, IPropertyName name, QuestionToken questionToken)
    {
        var property = new PropertyDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers,

            Name = name,

            QuestionToken = questionToken,

            Type = ParseTypeAnnotation()
        };


        // For instance properties specifically, since they are evaluated inside the constructor,
        // we do *not * want to parse yield expressions, so we specifically turn the yield context
        // off. The grammar would look something like this:
        //
        //    MemberVariableDeclaration[Yield]:
        //        AccessibilityModifier_opt   PropertyName   TypeAnnotation_opt   Initializer_opt[In];
        //        AccessibilityModifier_opt  static_opt  PropertyName   TypeAnnotation_opt   Initializer_opt[In, ?Yield];
        //
        // The checker may still error in the static case to explicitly disallow the yield expression.
        property.Initializer = HasModifier(property, ModifierFlags.Static)
            ? AllowInAnd(ParseNonParameterInitializer)
            : DoOutsideOfContext(NodeFlags.YieldContext | NodeFlags.DisallowInContext, ParseNonParameterInitializer);


        ParseSemicolon();

        return AddJsDocComment(FinishNode(property));
    }


    public IClassElement ParsePropertyOrMethodDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var asteriskToken = (AsteriskToken)ParseOptionalToken<AsteriskToken>(SyntaxKind.AsteriskToken);
        var name = ParsePropertyName();
        var questionToken = (QuestionToken)ParseOptionalToken<QuestionToken>(SyntaxKind.QuestionToken);
        if (asteriskToken != null || Token() == SyntaxKind.OpenParenToken || Token() == SyntaxKind.LessThanToken)
        {

            return ParseMethodDeclaration(fullStart, decorators, modifiers, asteriskToken, name, questionToken, Diagnostics.or_expected);
        }
        else
        {

            return ParsePropertyDeclaration(fullStart, decorators, modifiers, name, questionToken);
        }
    }


    public IExpression ParseNonParameterInitializer()
    {

        return ParseInitializer(/*inParameter*/ false);
    }


    public IAccessorDeclaration ParseAccessorDeclaration(SyntaxKind kind, int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        IAccessorDeclaration node = kind == SyntaxKind.GetAccessor ? new GetAccessorDeclaration() { Kind = kind, Pos = fullStart } : kind == SyntaxKind.SetAccessor ? new SetAccessorDeclaration() { Kind = kind, Pos = fullStart } : throw new NotSupportedException("parseAccessorDeclaration");

        node.Decorators = decorators;

        node.Modifiers = modifiers;

        node.Name = ParsePropertyName();

        FillSignature(SyntaxKind.ColonToken, /*yieldContext*/ false, /*awaitContext*/ false, /*requireCompleteParameterList*/ false, node);

        node.Body = ParseFunctionBlockOrSemicolon(/*isGenerator*/ false, /*isAsync*/ false);

        return AddJsDocComment(FinishNode(node));
    }


    public bool IsClassMemberModifier(SyntaxKind idToken)
    {
        return idToken switch
        {
            SyntaxKind.PublicKeyword or SyntaxKind.PrivateKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.StaticKeyword or SyntaxKind.ReadonlyKeyword => true,
            _ => false,
        };
    }


    public bool IsClassMemberStart()
    {
        SyntaxKind idToken = SyntaxKind.Unknown; // null;
        if (Token() == SyntaxKind.AtToken)
        {

            return true;
        }
        while (IsModifierKind(Token()))
        {

            idToken = Token();
            if (IsClassMemberModifier(idToken))
            {

                return true;
            }


            NextToken();
        }
        if (Token() == SyntaxKind.AsteriskToken)
        {

            return true;
        }
        if (IsLiteralPropertyName())
        {

            idToken = Token();

            NextToken();
        }
        if (Token() == SyntaxKind.OpenBracketToken)
        {

            return true;
        }
        if (idToken != SyntaxKind.Unknown)  // null)
        {
            if (!IsKeyword(idToken) || idToken == SyntaxKind.SetKeyword || idToken == SyntaxKind.GetKeyword)
            {

                return true;
            }
            return Token() switch
            {
                SyntaxKind.OpenParenToken or SyntaxKind.LessThanToken or SyntaxKind.ColonToken or SyntaxKind.EqualsToken or SyntaxKind.QuestionToken => true,// Not valid, but permitted so that it gets caught later on.
                _ => CanParseSemicolon(),// Covers
                                         //  - Semicolons     (declaration termination)
                                         //  - Closing braces (end-of-class, must be declaration)
                                         //  - End-of-files   (not valid, but permitted so that it gets caught later on)
                                         //  - Line-breaks    (enabling *automatic semicolon insertion*)
            };
        }


        return false;
    }


    public NodeArray<Decorator> ParseDecorators()
    {
        NodeArray<Decorator> decorators = null;
        while (true)
        {
            var decoratorStart = GetNodePos();
            if (!ParseOptional(SyntaxKind.AtToken))
            {

                break;
            }
            var decorator = new Decorator
            {
                Pos = decoratorStart,
                Expression = DoInDecoratorContext(ParseLeftHandSideExpressionOrHigher)
            };

            FinishNode(decorator);
            if (decorators == null)
            {

                decorators = CreateList<Decorator>();
                decorators.Pos = decoratorStart;
                decorators.Add(decorator);
            }
            else
            {

                decorators.Add(decorator);
            }
        }
        if (decorators != null)
        {

            decorators.End = GetNodeEnd();
        }

        return decorators;
    }


    public NodeArray<Modifier> ParseModifiers(bool? permitInvalidConstAsModifier = null)
    {
        var modifiers = CreateList<Modifier>();
        while (true)
        {
            var modifierStart = Scanner.StartPos;
            var modifierKind = Token();
            if (Token() == SyntaxKind.ConstKeyword && permitInvalidConstAsModifier == true)
            {
                if (!TryParse(NextTokenIsOnSameLineAndCanFollowModifier))
                {

                    break;
                }
            }
            else
            {
                if (!ParseAnyContextualModifier())
                {

                    break;
                }
            }
            var modifier = FinishNode(new Modifier { Kind = modifierKind, Pos = modifierStart }); //(Modifier)createNode(modifierKind, modifierStart)
            if (modifiers == null)
            {

                modifiers = CreateList<Modifier>();
                modifiers.Pos = modifierStart;
                modifiers.Add(modifier);
            }
            else
            {

                modifiers.Add(modifier);
            }
        }
        if (modifiers != null)
        {

            modifiers.End = Scanner.StartPos;
        }

        return modifiers;
    }


    public NodeArray<Modifier> ParseModifiersForArrowFunction()
    {
        NodeArray<Modifier> modifiers = null;
        if (Token() == SyntaxKind.AsyncKeyword)
        {
            var modifierStart = Scanner.StartPos;
            var modifierKind = Token();

            NextToken();
            var modifier = FinishNode(new Modifier { Kind = modifierKind, Pos = modifierStart });
            //finishNode((Modifier)createNode(modifierKind, modifierStart));

            modifiers = CreateList<Modifier>();
            modifiers.Pos = modifierStart;
            modifiers.Add(modifier);

            modifiers.End = Scanner.StartPos;
        }


        return modifiers;
    }


    public IClassElement ParseClassElement()
    {
        if (Token() == SyntaxKind.SemicolonToken)
        {
            var result = new SemicolonClassElement() { Pos = Scanner.StartPos };

            NextToken();

            return FinishNode(result);
        }
        var fullStart = GetNodePos();
        var decorators = ParseDecorators();
        var modifiers = ParseModifiers(/*permitInvalidConstAsModifier*/ true);
        var accessor = TryParseAccessorDeclaration(fullStart, decorators, modifiers);
        if (accessor != null)
        {

            return accessor;
        }
        if (Token() == SyntaxKind.ConstructorKeyword)
        {

            return ParseConstructorDeclaration(fullStart, decorators, modifiers);
        }
        if (IsIndexSignature())
        {

            return ParseIndexSignatureDeclaration(fullStart, decorators, modifiers);
        }
        if (TokenIsIdentifierOrKeyword(Token()) ||
                        Token() == SyntaxKind.StringLiteral ||
                        Token() == SyntaxKind.NumericLiteral ||
                        Token() == SyntaxKind.AsteriskToken ||
                        Token() == SyntaxKind.OpenBracketToken)
        {


            return ParsePropertyOrMethodDeclaration(fullStart, decorators, modifiers);
        }
        if (decorators?.Any() == true || modifiers?.Any() == true)
        {
            var name = (Identifier)CreateMissingNode<Identifier>(SyntaxKind.Identifier, /*reportAtCurrentPosition*/ true, Diagnostics.Declaration_expected);

            return ParsePropertyDeclaration(fullStart, decorators, modifiers, name, /*questionToken*/ null);
        }


        // 'isClassMemberStart' should have hinted not to attempt parsing.
        Debug.Fail("Should not have attempted to parse class member declaration.");
        return null;
    }


    public ClassExpression ParseClassExpression()
    {
        var node = new ClassExpression { Pos = Scanner.StartPos };
        node.Pos = Scanner.StartPos;
        //node.decorators = decorators;

        //node.modifiers = modifiers;

        ParseExpected(SyntaxKind.ClassKeyword);

        node.Name = ParseNameOfClassDeclarationOrExpression();

        node.TypeParameters = ParseTypeParameters();

        node.HeritageClauses = ParseHeritageClauses();
        if (ParseExpected(SyntaxKind.OpenBraceToken))
        {

            // ClassTail[Yield,Await] : (Modified) See 14.5
            //      ClassHeritage[?Yield,?Await]opt { ClassBody[?Yield,?Await]opt }
            node.Members = ParseClassMembers();

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            node.Members = new NodeArray<IClassElement>(); // createMissingList<ClassElement>();
        }


        return AddJsDocComment(FinishNode(node));
        //return (ClassExpression)parseClassDeclarationOrExpression(
        //    /*fullStart*/ scanner.getStartPos(),
        //    /*decorators*/ null,
        //    /*modifiers*/ null,
        //    SyntaxKind.ClassExpression);
    }


    public ClassDeclaration ParseClassDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new ClassDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,
            Modifiers = modifiers
        };



        ParseExpected(SyntaxKind.ClassKeyword);

        node.Name = ParseNameOfClassDeclarationOrExpression();

        node.TypeParameters = ParseTypeParameters();

        node.HeritageClauses = ParseHeritageClauses();
        if (ParseExpected(SyntaxKind.OpenBraceToken))
        {

            // ClassTail[Yield,Await] : (Modified) See 14.5
            //      ClassHeritage[?Yield,?Await]opt { ClassBody[?Yield,?Await]opt }
            node.Members = ParseClassMembers();

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            node.Members = new NodeArray<IClassElement>(); // createMissingList<ClassElement>();
        }


        return AddJsDocComment(FinishNode(node));

        //return (ClassDeclaration)parseClassDeclarationOrExpression(fullStart, decorators, modifiers, SyntaxKind.ClassDeclaration);
    }


    //public ClassLikeDeclaration parseClassDeclarationOrExpression(int fullStart, ListWithPos<Decorator> decorators, ListWithPos<Modifier> modifiers, SyntaxKind kind)
    //{
    //    var node = new ClassLikeDeclaration() { pos = fullStart };

    //    node.decorators = decorators;

    //    node.modifiers = modifiers;

    //    parseExpected(SyntaxKind.ClassKeyword);

    //    node.name = parseNameOfClassDeclarationOrExpression();

    //    node.typeParameters = parseTypeParameters();

    //    node.heritageClauses = parseHeritageClauses();
    //    if (parseExpected(SyntaxKind.OpenBraceToken))
    //    {

    //        // ClassTail[Yield,Await] : (Modified) See 14.5
    //        //      ClassHeritage[?Yield,?Await]opt { ClassBody[?Yield,?Await]opt }
    //        node.members = parseClassMembers();

    //        parseExpected(SyntaxKind.CloseBraceToken);
    //    }
    //    else
    //    {

    //        node.members = new NodeArray<Node>(); createMissingList<ClassElement>();
    //    }


    //    return addJSDocComment(finishNode(node));
    //}


    public Identifier ParseNameOfClassDeclarationOrExpression()
    {

        // implements is a future reserved word so
        // 'class implements' might mean either
        // - class expression with omitted name, 'implements' starts heritage clause
        // - class with name 'implements'
        // 'isImplementsClause' helps to disambiguate between these two cases
        return IsIdentifier() && !IsImplementsClause()
            ? ParseIdentifier()
            : null;
    }


    public bool IsImplementsClause()
    {

        return Token() == SyntaxKind.ImplementsKeyword && LookAhead(NextTokenIsIdentifierOrKeyword);
    }


    public NodeArray<HeritageClause> ParseHeritageClauses()
    {
        if (IsHeritageClause())
        {

            return ParseList(TsTypes.ParsingContext.HeritageClauses, ParseHeritageClause);
        }


        return null;
    }


    public HeritageClause ParseHeritageClause()
    {
        var tok = Token();
        if (tok == SyntaxKind.ExtendsKeyword || tok == SyntaxKind.ImplementsKeyword)
        {
            var node = new HeritageClause
            {
                Pos = Scanner.StartPos,
                Token = tok
            };

            NextToken();

            node.Types = ParseDelimitedList(TsTypes.ParsingContext.HeritageClauseElement, ParseExpressionWithTypeArguments);

            return FinishNode(node);
        }


        return null;
    }


    public ExpressionWithTypeArguments ParseExpressionWithTypeArguments()
    {
        var node = new ExpressionWithTypeArguments
        {
            Pos = Scanner.StartPos,
            Expression = ParseLeftHandSideExpressionOrHigher()
        };
        if (Token() == SyntaxKind.LessThanToken)
        {

            node.TypeArguments = ParseBracketedList(TsTypes.ParsingContext.TypeArguments, ParseType, SyntaxKind.LessThanToken, SyntaxKind.GreaterThanToken);
        }


        return FinishNode(node);
    }


    public bool IsHeritageClause()
    {

        return Token() == SyntaxKind.ExtendsKeyword || Token() == SyntaxKind.ImplementsKeyword;
    }


    public NodeArray<IClassElement> ParseClassMembers()
    {

        return ParseList2(TsTypes.ParsingContext.ClassMembers, ParseClassElement);
    }


    public InterfaceDeclaration ParseInterfaceDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new InterfaceDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };

        ParseExpected(SyntaxKind.InterfaceKeyword);

        node.Name = ParseIdentifier();

        node.TypeParameters = ParseTypeParameters();

        node.HeritageClauses = ParseHeritageClauses();

        node.Members = ParseObjectTypeMembers();

        return AddJsDocComment(FinishNode(node));
    }


    public TypeAliasDeclaration ParseTypeAliasDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new TypeAliasDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };

        ParseExpected(SyntaxKind.TypeKeyword);

        node.Name = ParseIdentifier();

        node.TypeParameters = ParseTypeParameters();

        ParseExpected(SyntaxKind.EqualsToken);

        node.Type = ParseType();

        ParseSemicolon();

        return AddJsDocComment(FinishNode(node));
    }


    public EnumMember ParseEnumMember()
    {
        var node = new EnumMember
        {
            Pos = Scanner.StartPos,
            Name = ParsePropertyName(),

            Initializer = AllowInAnd(ParseNonParameterInitializer)
        };

        return AddJsDocComment(FinishNode(node));
    }


    public EnumDeclaration ParseEnumDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new EnumDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };

        ParseExpected(SyntaxKind.EnumKeyword);

        node.Name = ParseIdentifier();
        if (ParseExpected(SyntaxKind.OpenBraceToken))
        {

            node.Members = ParseDelimitedList(TsTypes.ParsingContext.EnumMembers, ParseEnumMember);

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            node.Members = CreateMissingList<EnumMember>();
        }

        return AddJsDocComment(FinishNode(node));
    }


    public ModuleBlock ParseModuleBlock()
    {
        var node = new ModuleBlock() { Pos = Scanner.StartPos };
        if (ParseExpected(SyntaxKind.OpenBraceToken))
        {

            node.Statements = ParseList2(TsTypes.ParsingContext.BlockStatements, ParseStatement);

            ParseExpected(SyntaxKind.CloseBraceToken);
        }
        else
        {

            node.Statements = new NodeArray<IStatement>(); // createMissingList<Statement>();
        }

        return FinishNode(node);
    }


    public ModuleDeclaration ParseModuleOrNamespaceDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers, NodeFlags flags)
    {
        var node = new ModuleDeclaration() { Pos = fullStart };
        var namespaceFlag = flags & NodeFlags.Namespace;

        node.Decorators = decorators;

        node.Modifiers = modifiers;

        node.Flags |= flags;

        node.Name = ParseIdentifier();

        node.Body = ParseOptional(SyntaxKind.DotToken)
            ? (/*NamespaceDeclaration*/Node)ParseModuleOrNamespaceDeclaration(GetNodePos(), /*decorators*/ null, /*modifiers*/ null, NodeFlags.NestedNamespace | namespaceFlag)
            : ParseModuleBlock();

        return AddJsDocComment(FinishNode(node));
    }


    public ModuleDeclaration ParseAmbientExternalModuleDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new ModuleDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };
        if (Token() == SyntaxKind.GlobalKeyword)
        {

            // parse 'global' as name of global scope augmentation
            node.Name = ParseIdentifier();

            node.Flags |= NodeFlags.GlobalAugmentation;
        }
        else
        {

            node.Name = (StringLiteral)ParseLiteralNode(/*internName*/ true);
        }
        if (Token() == SyntaxKind.OpenBraceToken)
        {

            node.Body = ParseModuleBlock();
        }
        else
        {

            ParseSemicolon();
        }


        return FinishNode(node);
    }


    public ModuleDeclaration ParseModuleDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        NodeFlags flags = 0;
        if (Token() == SyntaxKind.GlobalKeyword)
        {

            // global augmentation
            return ParseAmbientExternalModuleDeclaration(fullStart, decorators, modifiers);
        }
        else
    if (ParseOptional(SyntaxKind.NamespaceKeyword))
        {

            flags |= NodeFlags.Namespace;
        }
        else
        {

            ParseExpected(SyntaxKind.ModuleKeyword);
            if (Token() == SyntaxKind.StringLiteral)
            {

                return ParseAmbientExternalModuleDeclaration(fullStart, decorators, modifiers);
            }
        }

        return ParseModuleOrNamespaceDeclaration(fullStart, decorators, modifiers, flags);
    }


    public bool IsExternalModuleReference()
    {

        return Token() == SyntaxKind.RequireKeyword &&
            LookAhead(NextTokenIsOpenParen);
    }


    public bool NextTokenIsOpenParen()
    {

        return NextToken() == SyntaxKind.OpenParenToken;
    }


    public bool NextTokenIsSlash()
    {

        return NextToken() == SyntaxKind.SlashToken;
    }


    public NamespaceExportDeclaration ParseNamespaceExportDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var exportDeclaration = new NamespaceExportDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };

        ParseExpected(SyntaxKind.AsKeyword);

        ParseExpected(SyntaxKind.NamespaceKeyword);


        exportDeclaration.Name = ParseIdentifier();


        ParseSemicolon();


        return FinishNode(exportDeclaration);
    }


    public /*ImportEqualsDeclaration | ImportDeclaration*/IStatement ParseImportDeclarationOrImportEqualsDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {

        ParseExpected(SyntaxKind.ImportKeyword);
        var afterImportPos = Scanner.StartPos;
        Identifier identifier = null;
        if (IsIdentifier())
        {

            identifier = ParseIdentifier();
            if (Token() != SyntaxKind.CommaToken && Token() != SyntaxKind.FromKeyword)
            {

                return ParseImportEqualsDeclaration(fullStart, decorators, modifiers, identifier);
            }
        }
        var importDeclaration = new ImportDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };
        if (identifier != null || // import id
                        Token() == SyntaxKind.AsteriskToken || // import *
                        Token() == SyntaxKind.OpenBraceToken)
        {
            // import {
            importDeclaration.ImportClause = ParseImportClause(identifier, afterImportPos);

            ParseExpected(SyntaxKind.FromKeyword);
        }


        importDeclaration.ModuleSpecifier = ParseModuleSpecifier();

        ParseSemicolon();

        return FinishNode(importDeclaration);
    }


    public ImportEqualsDeclaration ParseImportEqualsDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers, Identifier identifier)
    {
        var importEqualsDeclaration = new ImportEqualsDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers,

            Name = identifier
        };

        ParseExpected(SyntaxKind.EqualsToken);

        importEqualsDeclaration.ModuleReference = ParseModuleReference();

        ParseSemicolon();

        return AddJsDocComment(FinishNode(importEqualsDeclaration));
    }


    public ImportClause ParseImportClause(Identifier identifier, int fullStart)
    {
        var importClause = new ImportClause() { Pos = fullStart };
        if (identifier != null)
        {

            // ImportedDefaultBinding:
            //  ImportedBinding
            importClause.Name = identifier;
        }
        if (importClause.Name == null ||
                        ParseOptional(SyntaxKind.CommaToken))
        {

            importClause.NamedBindings = Token() == SyntaxKind.AsteriskToken ? ParseNamespaceImport() : (INamedImportBindings)ParseNamedImportsOrExports(SyntaxKind.NamedImports);
        }


        return FinishNode(importClause);
    }


    public INode ParseModuleReference()
    {

        return IsExternalModuleReference()
            ? ParseExternalModuleReference()
            : ParseEntityName(/*allowReservedWords*/ false);
    }


    public ExternalModuleReference ParseExternalModuleReference()
    {
        var node = new ExternalModuleReference() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.RequireKeyword);

        ParseExpected(SyntaxKind.OpenParenToken);

        node.Expression = ParseModuleSpecifier();

        ParseExpected(SyntaxKind.CloseParenToken);

        return FinishNode(node);
    }


    public IExpression ParseModuleSpecifier()
    {
        if (Token() == SyntaxKind.StringLiteral)
        {
            var result = ParseLiteralNode();
            return result;
        }
        else
        {

            // We allow arbitrary expressions here, even though the grammar only allows string
            // literals.  We check to ensure that it is only a string literal later in the grammar
            // check pass.
            return ParseExpression();
        }
    }


    public NamespaceImport ParseNamespaceImport()
    {
        var namespaceImport = new NamespaceImport() { Pos = Scanner.StartPos };

        ParseExpected(SyntaxKind.AsteriskToken);

        ParseExpected(SyntaxKind.AsKeyword);

        namespaceImport.Name = ParseIdentifier();

        return FinishNode(namespaceImport);
    }


    //public NamedImports parseNamedImportsOrExports(SyntaxKind.NamedImports kind)
    //{
    //}


    //public NamedExports parseNamedImportsOrExports(SyntaxKind.NamedExports kind)
    //{
    //}


    public INamedImportsOrExports ParseNamedImportsOrExports(SyntaxKind kind)
    {
        if (kind == SyntaxKind.NamedImports)
        {
            var node = new NamedImports
            {
                Pos = Scanner.StartPos,
                Elements = ParseBracketedList<ImportSpecifier>(TsTypes.ParsingContext.ImportOrExportSpecifiers, ParseImportSpecifier,
               SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken)
            };

            return FinishNode(node);
        }
        else
        {
            var node = new NamedExports
            {
                Pos = Scanner.StartPos,
                Elements = ParseBracketedList<ExportSpecifier>(TsTypes.ParsingContext.ImportOrExportSpecifiers, ParseExportSpecifier,
               SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken)
            };

            return FinishNode(node);
        }
        //var node = new NamedImports | NamedExports();


        //// NamedImports:
        ////  { }
        ////  { ImportsList }
        ////  { ImportsList, }

        //// ImportsList:
        ////  ImportSpecifier
        ////  ImportsList, ImportSpecifier
        //node.elements = (List<ImportSpecifier> | List<ExportSpecifier>)parseBracketedList(ParsingContext.ImportOrExportSpecifiers,
        //    kind == SyntaxKind.NamedImports ? parseImportSpecifier : parseExportSpecifier,
        //    SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken);

        //return finishNode(node);
    }


    public ExportSpecifier ParseExportSpecifier()
    {
        var node = new ExportSpecifier { Pos = Scanner.StartPos };
        var identifierName = ParseIdentifierName();
        if (Token() == SyntaxKind.AsKeyword)
        {
            node.PropertyName = identifierName;
            ParseExpected(SyntaxKind.AsKeyword);
            node.Name = ParseIdentifierName();
        }
        else
        {
            node.Name = identifierName;
        }

        return FinishNode(node);
        //return parseImportOrExportSpecifier(SyntaxKind.ExportSpecifier);
    }


    public ImportSpecifier ParseImportSpecifier()
    {
        var node = new ImportSpecifier() { Pos = Scanner.StartPos };
        var checkIdentifierIsKeyword = IsKeyword(Token()) && !IsIdentifier();
        var checkIdentifierStart = Scanner.TokenPos;
        var checkIdentifierEnd = Scanner.TextPos;
        var identifierName = ParseIdentifierName();
        if (Token() == SyntaxKind.AsKeyword)
        {

            node.PropertyName = identifierName;

            ParseExpected(SyntaxKind.AsKeyword);

            checkIdentifierIsKeyword = IsKeyword(Token()) && !IsIdentifier();

            checkIdentifierStart = Scanner.TokenPos;

            checkIdentifierEnd = Scanner.TextPos;

            node.Name = ParseIdentifierName();
        }
        else
        {

            node.Name = identifierName;
        }
        if (/*kind == SyntaxKind.ImportSpecifier && */checkIdentifierIsKeyword)
        {

            // Report error identifier expected
            ParseErrorAtPosition(checkIdentifierStart, checkIdentifierEnd - checkIdentifierStart, Diagnostics.Identifier_expected);
        }

        return FinishNode(node);

        //return parseImportOrExportSpecifier(SyntaxKind.ImportSpecifier);
    }


    //public ImportOrExportSpecifier parseImportOrExportSpecifier(SyntaxKind kind)
    //{
    //    var node = new ImportSpecifier { pos = scanner.getStartPos() };
    //    var checkIdentifierIsKeyword = isKeyword(token()) && !isIdentifier();
    //    var checkIdentifierStart = scanner.getTokenPos();
    //    var checkIdentifierEnd = scanner.getTextPos();
    //    var identifierName = parseIdentifierName();
    //    if (token() == SyntaxKind.AsKeyword)
    //    {

    //        node.propertyName = identifierName;

    //        parseExpected(SyntaxKind.AsKeyword);

    //        checkIdentifierIsKeyword = isKeyword(token()) && !isIdentifier();

    //        checkIdentifierStart = scanner.getTokenPos();

    //        checkIdentifierEnd = scanner.getTextPos();

    //        node.name = parseIdentifierName();
    //    }
    //    else
    //    {

    //        node.name = identifierName;
    //    }
    //    if (kind == SyntaxKind.ImportSpecifier && checkIdentifierIsKeyword)
    //    {

    //        // Report error identifier expected
    //        parseErrorAtPosition(checkIdentifierStart, checkIdentifierEnd - checkIdentifierStart, Diagnostics.Identifier_expected);
    //    }

    //    return finishNode(node);
    //}


    public ExportDeclaration ParseExportDeclaration(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new ExportDeclaration
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };
        if (ParseOptional(SyntaxKind.AsteriskToken))
        {

            ParseExpected(SyntaxKind.FromKeyword);

            node.ModuleSpecifier = ParseModuleSpecifier();
        }
        else
        {

            node.ExportClause = (NamedExports)ParseNamedImportsOrExports(SyntaxKind.NamedExports);
            if (Token() == SyntaxKind.FromKeyword || (Token() == SyntaxKind.StringLiteral && !Scanner.HasPrecedingLineBreak))
            {

                ParseExpected(SyntaxKind.FromKeyword);

                node.ModuleSpecifier = ParseModuleSpecifier();
            }
        }

        ParseSemicolon();

        return FinishNode(node);
    }


    public ExportAssignment ParseExportAssignment(int fullStart, NodeArray<Decorator> decorators, NodeArray<Modifier> modifiers)
    {
        var node = new ExportAssignment
        {
            Pos = fullStart,
            Decorators = decorators,

            Modifiers = modifiers
        };
        if (ParseOptional(SyntaxKind.EqualsToken))
        {

            node.IsExportEquals = true;
        }
        else
        {

            ParseExpected(SyntaxKind.DefaultKeyword);
        }

        node.Expression = ParseAssignmentExpressionOrHigher();

        ParseSemicolon();

        return FinishNode(node);
    }

    public void SetExternalModuleIndicator(SourceFile sourceFile)
    {

        sourceFile.ExternalModuleIndicator = sourceFile.Statements./*ForEach*/FirstOrDefault(node =>
        {

            return HasModifier(node, ModifierFlags.Export)
                || node.Kind == SyntaxKind.ImportEqualsDeclaration && (node as ImportEqualsDeclaration)?.ModuleReference?.Kind == SyntaxKind.ExternalModuleReference
                || node.Kind == SyntaxKind.ImportDeclaration
                || node.Kind == SyntaxKind.ExportAssignment
                || node.Kind == SyntaxKind.ExportDeclaration;
        });
    }
}
