using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.TsParser;

public class Ts
{
    public static INode VisitNode(Func<INode, INode> cbNode, INode node)
    {
        if (node != null)
            return cbNode(node);
        return null;
    }


    public static T VisitList<T>(Func<INode[], T> cbNodes, INode[] nodes)
    {
        if (nodes != null)
            return cbNodes(nodes);
        return default(T);
    }


    public static INode VisitNodeArray(Func<INode[], INode> cbNodes, INode[] nodes)
    {
        if (nodes != null)
            return cbNodes(nodes);
        return null;
    }

    public static INode VisitEachNode(Func<INode, INode> cbNode, List<INode> nodes)
    {
        if (nodes != null)
            foreach (var node in nodes)
            {
                var result = cbNode(node);
                if (result != null)
                    return result;
            }
        return null;
    }


    public static INode ForEachChild(INode node, Func<INode, INode> cbNode, Func<INode[], INode> cbNodeArray = null)
    {
        if (node == null)
            return null;
        Func<object, IEnumerable<INode>, INode> visitNodes = (o1, o2) =>
        {
            var list = o2?.Cast<INode>().ToList();
            if (list != null)
                if (cbNodeArray == null)
                    return VisitEachNode(cbNode, list);
                else
                    return cbNodeArray(list.ToArray());
            return null;
        };
        var cbNodes = cbNodeArray;
        switch (node.Kind)
        {
            case SyntaxKind.QualifiedName:

                return VisitNode(cbNode, (node as QualifiedName)?.Left) ??
                       VisitNode(cbNode, (node as QualifiedName)?.Right);
            case SyntaxKind.TypeParameter:

                return VisitNode(cbNode, (node as TypeParameterDeclaration)?.Name) ??
                       VisitNode(cbNode, (node as TypeParameterDeclaration)?.Constraint) ??
                       VisitNode(cbNode, (node as TypeParameterDeclaration)?.Default) ??
                       VisitNode(cbNode, (node as TypeParameterDeclaration)?.Expression);
            case SyntaxKind.ShorthandPropertyAssignment:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ShorthandPropertyAssignment)?.Name) ??
                       VisitNode(cbNode, (node as ShorthandPropertyAssignment)?.QuestionToken) ??
                       VisitNode(cbNode, (node as ShorthandPropertyAssignment)?.EqualsToken) ??
                       VisitNode(cbNode, (node as ShorthandPropertyAssignment)?.ObjectAssignmentInitializer);
            case SyntaxKind.SpreadAssignment:

                return VisitNode(cbNode, (node as SpreadAssignment)?.Expression);
            case SyntaxKind.Parameter:
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.PropertySignature:
            case SyntaxKind.PropertyAssignment:
            case SyntaxKind.VariableDeclaration:
            case SyntaxKind.BindingElement:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as IVariableLikeDeclaration)?.PropertyName) ??
                       VisitNode(cbNode, (node as IVariableLikeDeclaration)?.DotDotDotToken) ??
                       VisitNode(cbNode, (node as IVariableLikeDeclaration)?.Name) ??
                       VisitNode(cbNode, (node as IVariableLikeDeclaration)?.QuestionToken) ??
                       VisitNode(cbNode, (node as IVariableLikeDeclaration)?.Type) ??
                       VisitNode(cbNode, (node as IVariableLikeDeclaration)?.Initializer);
            case SyntaxKind.FunctionType:
            case SyntaxKind.ConstructorType:
            case SyntaxKind.CallSignature:
            case SyntaxKind.ConstructSignature:
            case SyntaxKind.IndexSignature:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       visitNodes(cbNodes, (node as ISignatureDeclaration)?.TypeParameters) ??
                       visitNodes(cbNodes, (node as ISignatureDeclaration)?.Parameters) ??
                       VisitNode(cbNode, (node as ISignatureDeclaration)?.Type);
            case SyntaxKind.MethodDeclaration:
            case SyntaxKind.MethodSignature:
            case SyntaxKind.Constructor:
            case SyntaxKind.GetAccessor:
            case SyntaxKind.SetAccessor:
            case SyntaxKind.FunctionExpression:
            case SyntaxKind.FunctionDeclaration:
            case SyntaxKind.ArrowFunction:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as IFunctionLikeDeclaration)?.AsteriskToken) ??
                       VisitNode(cbNode, (node as IFunctionLikeDeclaration)?.Name) ??
                       VisitNode(cbNode, (node as IFunctionLikeDeclaration)?.QuestionToken) ??
                       visitNodes(cbNodes, (node as IFunctionLikeDeclaration)?.TypeParameters) ??
                       visitNodes(cbNodes, (node as IFunctionLikeDeclaration)?.Parameters) ??
                       VisitNode(cbNode, (node as IFunctionLikeDeclaration)?.Type) ??
                       VisitNode(cbNode, (node as ArrowFunction)?.EqualsGreaterThanToken) ??
                       VisitNode(cbNode, (node as IFunctionLikeDeclaration)?.Body);
            case SyntaxKind.TypeReference:

                return VisitNode(cbNode, (node as TypeReferenceNode)?.TypeName) ??
                       visitNodes(cbNodes, (node as TypeReferenceNode)?.TypeArguments);
            case SyntaxKind.TypePredicate:

                return VisitNode(cbNode, (node as TypePredicateNode)?.ParameterName) ??
                       VisitNode(cbNode, (node as TypePredicateNode)?.Type);
            case SyntaxKind.TypeQuery:

                return VisitNode(cbNode, (node as TypeQueryNode)?.ExprName);
            case SyntaxKind.TypeLiteral:

                return visitNodes(cbNodes, (node as TypeLiteralNode)?.Members);
            case SyntaxKind.ArrayType:

                return VisitNode(cbNode, (node as ArrayTypeNode)?.ElementType);
            case SyntaxKind.TupleType:

                return visitNodes(cbNodes, (node as TupleTypeNode)?.ElementTypes);
            case SyntaxKind.UnionType:
            case SyntaxKind.IntersectionType:

                return visitNodes(cbNodes, (node as IUnionOrIntersectionTypeNode)?.Types);
            case SyntaxKind.ParenthesizedType:
            case SyntaxKind.TypeOperator:

                return VisitNode(cbNode, (node as ParenthesizedTypeNode)?.Type ?? (node as TypeOperatorNode)?.Type);
            case SyntaxKind.IndexedAccessType:

                return VisitNode(cbNode, (node as IndexedAccessTypeNode)?.ObjectType) ??
                       VisitNode(cbNode, (node as IndexedAccessTypeNode)?.IndexType);
            case SyntaxKind.MappedType:

                return VisitNode(cbNode, (node as MappedTypeNode)?.ReadonlyToken) ??
                       VisitNode(cbNode, (node as MappedTypeNode)?.TypeParameter) ??
                       VisitNode(cbNode, (node as MappedTypeNode)?.QuestionToken) ??
                       VisitNode(cbNode, (node as MappedTypeNode)?.Type);
            case SyntaxKind.LiteralType:

                return VisitNode(cbNode, (node as LiteralTypeNode)?.Literal);
            case SyntaxKind.ObjectBindingPattern:
            case SyntaxKind.ArrayBindingPattern:

                return visitNodes(cbNodes, ((IBindingPattern) node).Elements);
            case SyntaxKind.ArrayLiteralExpression:

                return visitNodes(cbNodes, (node as ArrayLiteralExpression)?.Elements);
            case SyntaxKind.ObjectLiteralExpression:

                return visitNodes(cbNodes, (node as ObjectLiteralExpression)?.Properties);
            case SyntaxKind.PropertyAccessExpression:

                return VisitNode(cbNode, (node as PropertyAccessExpression)?.Expression) ??
                       VisitNode(cbNode, (node as PropertyAccessExpression)?.Name);
            case SyntaxKind.ElementAccessExpression:

                return VisitNode(cbNode, (node as ElementAccessExpression)?.Expression) ??
                       VisitNode(cbNode, (node as ElementAccessExpression)?.ArgumentExpression);
            case SyntaxKind.CallExpression:
            case SyntaxKind.NewExpression:

                return VisitNode(cbNode, (node as CallExpression)?.Expression) ??
                       visitNodes(cbNodes, (node as CallExpression)?.TypeArguments) ??
                       visitNodes(cbNodes, (node as CallExpression)?.Arguments);
            case SyntaxKind.TaggedTemplateExpression:

                return VisitNode(cbNode, (node as TaggedTemplateExpression)?.Tag) ??
                       VisitNode(cbNode, (node as TaggedTemplateExpression)?.Template);
            case SyntaxKind.TypeAssertionExpression:

                return VisitNode(cbNode, (node as TypeAssertion)?.Type) ??
                       VisitNode(cbNode, (node as TypeAssertion)?.Expression);
            case SyntaxKind.ParenthesizedExpression:

                return VisitNode(cbNode, (node as ParenthesizedExpression)?.Expression);
            case SyntaxKind.DeleteExpression:

                return VisitNode(cbNode, (node as DeleteExpression)?.Expression);
            case SyntaxKind.TypeOfExpression:

                return VisitNode(cbNode, (node as TypeOfExpression)?.Expression);
            case SyntaxKind.VoidExpression:

                return VisitNode(cbNode, (node as VoidExpression)?.Expression);
            case SyntaxKind.PrefixUnaryExpression:

                return VisitNode(cbNode, (node as PrefixUnaryExpression)?.Operand);
            case SyntaxKind.YieldExpression:

                return VisitNode(cbNode, (node as YieldExpression)?.AsteriskToken) ??
                       VisitNode(cbNode, (node as YieldExpression)?.Expression);
            case SyntaxKind.AwaitExpression:

                return VisitNode(cbNode, (node as AwaitExpression)?.Expression);
            case SyntaxKind.PostfixUnaryExpression:

                return VisitNode(cbNode, (node as PostfixUnaryExpression)?.Operand);
            case SyntaxKind.BinaryExpression:

                return VisitNode(cbNode, (node as BinaryExpression)?.Left) ??
                       VisitNode(cbNode, (node as BinaryExpression)?.OperatorToken) ??
                       VisitNode(cbNode, (node as BinaryExpression)?.Right);
            case SyntaxKind.AsExpression:

                return VisitNode(cbNode, (node as AsExpression)?.Expression) ??
                       VisitNode(cbNode, (node as AsExpression)?.Type);
            case SyntaxKind.NonNullExpression:

                return VisitNode(cbNode, (node as NonNullExpression)?.Expression);
            case SyntaxKind.MetaProperty:

                return VisitNode(cbNode, (node as MetaProperty)?.Name);
            case SyntaxKind.ConditionalExpression:

                return VisitNode(cbNode, (node as ConditionalExpression)?.Condition) ??
                       VisitNode(cbNode, (node as ConditionalExpression)?.QuestionToken) ??
                       VisitNode(cbNode, (node as ConditionalExpression)?.WhenTrue) ??
                       VisitNode(cbNode, (node as ConditionalExpression)?.ColonToken) ??
                       VisitNode(cbNode, (node as ConditionalExpression)?.WhenFalse);
            case SyntaxKind.SpreadElement:

                return VisitNode(cbNode, (node as SpreadElement)?.Expression);
            case SyntaxKind.Block:
            case SyntaxKind.ModuleBlock:

                return visitNodes(cbNodes, (node as Block)?.Statements);
            case SyntaxKind.SourceFile:

                return visitNodes(cbNodes, (node as SourceFile)?.Statements) ??
                       VisitNode(cbNode, (node as SourceFile)?.EndOfFileToken);
            case SyntaxKind.VariableStatement:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as VariableStatement)?.DeclarationList);
            case SyntaxKind.VariableDeclarationList:

                return visitNodes(cbNodes, (node as VariableDeclarationList)?.Declarations);
            case SyntaxKind.ExpressionStatement:

                return VisitNode(cbNode, (node as ExpressionStatement)?.Expression);
            case SyntaxKind.IfStatement:

                return VisitNode(cbNode, (node as IfStatement)?.Expression) ??
                       VisitNode(cbNode, (node as IfStatement)?.ThenStatement) ??
                       VisitNode(cbNode, (node as IfStatement)?.ElseStatement);
            case SyntaxKind.DoStatement:

                return VisitNode(cbNode, (node as DoStatement)?.Statement) ??
                       VisitNode(cbNode, (node as DoStatement)?.Expression);
            case SyntaxKind.WhileStatement:

                return VisitNode(cbNode, (node as WhileStatement)?.Expression) ??
                       VisitNode(cbNode, (node as WhileStatement)?.Statement);
            case SyntaxKind.ForStatement:

                return VisitNode(cbNode, (node as ForStatement)?.Initializer) ??
                       VisitNode(cbNode, (node as ForStatement)?.Condition) ??
                       VisitNode(cbNode, (node as ForStatement)?.Incrementor) ??
                       VisitNode(cbNode, (node as ForStatement)?.Statement);
            case SyntaxKind.ForInStatement:

                return VisitNode(cbNode, (node as ForInStatement)?.Initializer) ??
                       VisitNode(cbNode, (node as ForInStatement)?.Expression) ??
                       VisitNode(cbNode, (node as ForInStatement)?.Statement);
            case SyntaxKind.ForOfStatement:

                return VisitNode(cbNode, (node as ForOfStatement)?.AwaitModifier) ??
                       VisitNode(cbNode, (node as ForOfStatement)?.Initializer) ??
                       VisitNode(cbNode, (node as ForOfStatement)?.Expression) ??
                       VisitNode(cbNode, (node as ForOfStatement)?.Statement);
            case SyntaxKind.ContinueStatement:
            case SyntaxKind.BreakStatement:

                return VisitNode(cbNode, (node as IBreakOrContinueStatement)?.Label);
            case SyntaxKind.ReturnStatement:

                return VisitNode(cbNode, (node as ReturnStatement)?.Expression);
            case SyntaxKind.WithStatement:

                return VisitNode(cbNode, (node as WithStatement)?.Expression) ??
                       VisitNode(cbNode, (node as WithStatement)?.Statement);
            case SyntaxKind.SwitchStatement:

                return VisitNode(cbNode, (node as SwitchStatement)?.Expression) ??
                       VisitNode(cbNode, (node as SwitchStatement)?.CaseBlock);
            case SyntaxKind.CaseBlock:

                return visitNodes(cbNodes, (node as CaseBlock)?.Clauses);
            case SyntaxKind.CaseClause:

                return VisitNode(cbNode, (node as CaseClause)?.Expression) ??
                       visitNodes(cbNodes, (node as CaseClause)?.Statements);
            case SyntaxKind.DefaultClause:

                return visitNodes(cbNodes, (node as DefaultClause)?.Statements);
            case SyntaxKind.LabeledStatement:

                return VisitNode(cbNode, (node as LabeledStatement)?.Label) ??
                       VisitNode(cbNode, (node as LabeledStatement)?.Statement);
            case SyntaxKind.ThrowStatement:

                return VisitNode(cbNode, (node as ThrowStatement)?.Expression);
            case SyntaxKind.TryStatement:

                return VisitNode(cbNode, (node as TryStatement)?.TryBlock) ??
                       VisitNode(cbNode, (node as TryStatement)?.CatchClause) ??
                       VisitNode(cbNode, (node as TryStatement)?.FinallyBlock);
            case SyntaxKind.CatchClause:

                return VisitNode(cbNode, (node as CatchClause)?.VariableDeclaration) ??
                       VisitNode(cbNode, (node as CatchClause)?.Block);
            case SyntaxKind.Decorator:

                return VisitNode(cbNode, (node as Decorator)?.Expression);
            case SyntaxKind.ClassDeclaration:
                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ClassDeclaration)?.Name) ??
                       visitNodes(cbNodes, (node as ClassDeclaration)?.TypeParameters) ??
                       visitNodes(cbNodes, (node as ClassDeclaration)?.HeritageClauses) ??
                       visitNodes(cbNodes, (node as ClassDeclaration)?.Members);
            case SyntaxKind.ClassExpression:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ClassExpression)?.Name) ??
                       visitNodes(cbNodes, (node as ClassExpression)?.TypeParameters) ??
                       visitNodes(cbNodes, (node as ClassExpression)?.HeritageClauses) ??
                       visitNodes(cbNodes, (node as ClassExpression)?.Members);
            case SyntaxKind.InterfaceDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as InterfaceDeclaration)?.Name) ??
                       visitNodes(cbNodes, (node as InterfaceDeclaration)?.TypeParameters) ??
                       visitNodes(cbNodes, (node as InterfaceDeclaration)?.HeritageClauses) ??
                       visitNodes(cbNodes, (node as InterfaceDeclaration)?.Members);
            case SyntaxKind.TypeAliasDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as TypeAliasDeclaration)?.Name) ??
                       visitNodes(cbNodes, (node as TypeAliasDeclaration)?.TypeParameters) ??
                       VisitNode(cbNode, (node as TypeAliasDeclaration)?.Type);
            case SyntaxKind.EnumDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as EnumDeclaration)?.Name) ??
                       visitNodes(cbNodes, (node as EnumDeclaration)?.Members);
            case SyntaxKind.EnumMember:

                return VisitNode(cbNode, (node as EnumMember)?.Name) ??
                       VisitNode(cbNode, (node as EnumMember)?.Initializer);
            case SyntaxKind.ModuleDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ModuleDeclaration)?.Name) ??
                       VisitNode(cbNode, (node as ModuleDeclaration)?.Body);
            case SyntaxKind.ImportEqualsDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ImportEqualsDeclaration)?.Name) ??
                       VisitNode(cbNode, (node as ImportEqualsDeclaration)?.ModuleReference);
            case SyntaxKind.ImportDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ImportDeclaration)?.ImportClause) ??
                       VisitNode(cbNode, (node as ImportDeclaration)?.ModuleSpecifier);
            case SyntaxKind.ImportClause:

                return VisitNode(cbNode, (node as ImportClause)?.Name) ??
                       VisitNode(cbNode, (node as ImportClause)?.NamedBindings);
            case SyntaxKind.NamespaceExportDeclaration:

                return VisitNode(cbNode, (node as NamespaceExportDeclaration)?.Name);
            case SyntaxKind.NamespaceImport:

                return VisitNode(cbNode, (node as NamespaceImport)?.Name);
            case SyntaxKind.NamedImports:
            case SyntaxKind.NamedExports:

                if (node is NamedImports) return visitNodes(cbNodes, (node as NamedImports)?.Elements);
                else return visitNodes(cbNodes, (node as NamedExports)?.Elements);
            case SyntaxKind.ExportDeclaration:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ExportDeclaration)?.ExportClause) ??
                       VisitNode(cbNode, (node as ExportDeclaration)?.ModuleSpecifier);
            case SyntaxKind.ImportSpecifier:
            case SyntaxKind.ExportSpecifier:
                return VisitNode(cbNode, (node as IImportOrExportSpecifier)?.PropertyName ??
                                         VisitNode(cbNode, (node as IImportOrExportSpecifier)?.Name));
            case SyntaxKind.ExportAssignment:

                return visitNodes(cbNodes, node.Decorators) ??
                       visitNodes(cbNodes, node.Modifiers) ??
                       VisitNode(cbNode, (node as ExportAssignment)?.Expression);
            case SyntaxKind.TemplateExpression:

                return VisitNode(cbNode, (node as TemplateExpression)?.Head) ??
                       visitNodes(cbNodes, (node as TemplateExpression)?.TemplateSpans);
            case SyntaxKind.TemplateSpan:

                return VisitNode(cbNode, (node as TemplateSpan)?.Expression) ??
                       VisitNode(cbNode, (node as TemplateSpan)?.Literal);
            case SyntaxKind.ComputedPropertyName:

                return VisitNode(cbNode, (node as ComputedPropertyName)?.Expression);
            case SyntaxKind.HeritageClause:

                return visitNodes(cbNodes, (node as HeritageClause)?.Types);
            case SyntaxKind.ExpressionWithTypeArguments:

                return VisitNode(cbNode, (node as ExpressionWithTypeArguments)?.Expression) ??
                       visitNodes(cbNodes, (node as ExpressionWithTypeArguments)?.TypeArguments);
            case SyntaxKind.ExternalModuleReference:

                return VisitNode(cbNode, (node as ExternalModuleReference)?.Expression);
            case SyntaxKind.MissingDeclaration:

                return visitNodes(cbNodes, node.Decorators);
            case SyntaxKind.JsxElement:

                return VisitNode(cbNode, (node as JsxElement)?.OpeningElement) ??
                       visitNodes(cbNodes, (node as JsxElement)?.JsxChildren) ??
                       VisitNode(cbNode, (node as JsxElement)?.ClosingElement);
            case SyntaxKind.JsxSelfClosingElement:
            case SyntaxKind.JsxOpeningElement:
                return VisitNode(cbNode,
                           (node as JsxSelfClosingElement)?.TagName ?? (node as JsxOpeningElement)?.TagName) ??
                       VisitNode(cbNode,
                           (node as JsxSelfClosingElement)?.Attributes ?? (node as JsxOpeningElement)?.Attributes);
            case SyntaxKind.JsxAttributes:

                return visitNodes(cbNodes, (node as JsxAttributes)?.Properties);
            case SyntaxKind.JsxAttribute:

                return VisitNode(cbNode, (node as JsxAttribute)?.Name) ??
                       VisitNode(cbNode, (node as JsxAttribute)?.Initializer);
            case SyntaxKind.JsxSpreadAttribute:

                return VisitNode(cbNode, (node as JsxSpreadAttribute)?.Expression);
            case SyntaxKind.JsxExpression:

                return VisitNode(cbNode, (node as JsxExpression).DotDotDotToken) ??
                       VisitNode(cbNode, (node as JsxExpression).Expression);
            case SyntaxKind.JsxClosingElement:

                return VisitNode(cbNode, (node as JsxClosingElement)?.TagName);
            case SyntaxKind.JsDocTypeExpression:

                return VisitNode(cbNode, (node as JsDocTypeExpression)?.Type);
            case SyntaxKind.JsDocUnionType:

                return visitNodes(cbNodes, (node as JsDocUnionType)?.Types);
            case SyntaxKind.JsDocTupleType:

                return visitNodes(cbNodes, (node as JsDocTupleType)?.Types);
            case SyntaxKind.JsDocArrayType:

                return VisitNode(cbNode, (node as JsDocArrayType)?.ElementType);
            case SyntaxKind.JsDocNonNullableType:

                return VisitNode(cbNode, (node as JsDocNonNullableType)?.Type);
            case SyntaxKind.JsDocNullableType:

                return VisitNode(cbNode, (node as JsDocNullableType)?.Type);
            case SyntaxKind.JsDocRecordType:

                return VisitNode(cbNode, (node as JsDocRecordType)?.Literal);
            case SyntaxKind.JsDocTypeReference:

                return VisitNode(cbNode, (node as JsDocTypeReference)?.Name) ??
                       visitNodes(cbNodes, (node as JsDocTypeReference)?.TypeArguments);
            case SyntaxKind.JsDocOptionalType:

                return VisitNode(cbNode, (node as JsDocOptionalType)?.Type);
            case SyntaxKind.JsDocFunctionType:

                return visitNodes(cbNodes, (node as JsDocFunctionType)?.Parameters) ??
                       VisitNode(cbNode, (node as JsDocFunctionType)?.Type);
            case SyntaxKind.JsDocVariadicType:

                return VisitNode(cbNode, (node as JsDocVariadicType)?.Type);
            case SyntaxKind.JsDocConstructorType:

                return VisitNode(cbNode, (node as JsDocConstructorType)?.Type);
            case SyntaxKind.JsDocThisType:

                return VisitNode(cbNode, (node as JsDocThisType)?.Type);
            case SyntaxKind.JsDocRecordMember:

                return VisitNode(cbNode, (node as JsDocRecordMember)?.Name) ??
                       VisitNode(cbNode, (node as JsDocRecordMember)?.Type);
            case SyntaxKind.JsDocComment:

                return visitNodes(cbNodes, (node as JsDoc)?.Tags);
            case SyntaxKind.JsDocParameterTag:

                return VisitNode(cbNode, (node as JsDocParameterTag)?.PreParameterName) ??
                       VisitNode(cbNode, (node as JsDocParameterTag)?.TypeExpression) ??
                       VisitNode(cbNode, (node as JsDocParameterTag)?.PostParameterName);
            case SyntaxKind.JsDocReturnTag:

                return VisitNode(cbNode, (node as JsDocReturnTag)?.TypeExpression);
            case SyntaxKind.JsDocTypeTag:

                return VisitNode(cbNode, (node as JsDocTypeTag)?.TypeExpression);
            case SyntaxKind.JsDocAugmentsTag:

                return VisitNode(cbNode, (node as JsDocAugmentsTag)?.TypeExpression);
            case SyntaxKind.JsDocTemplateTag:

                return visitNodes(cbNodes, (node as JsDocTemplateTag)?.TypeParameters);
            case SyntaxKind.JsDocTypedefTag:

                return VisitNode(cbNode, (node as JsDocTypedefTag)?.TypeExpression) ??
                       VisitNode(cbNode, (node as JsDocTypedefTag)?.FullName) ??
                       VisitNode(cbNode, (node as JsDocTypedefTag)?.Name) ??
                       VisitNode(cbNode, (node as JsDocTypedefTag)?.JsDocTypeLiteral);
            case SyntaxKind.JsDocTypeLiteral:

                return visitNodes(cbNodes, (node as JsDocTypeLiteral)?.JsDocPropertyTags);
            case SyntaxKind.JsDocPropertyTag:

                return VisitNode(cbNode, (node as JsDocPropertyTag)?.TypeExpression) ??
                       VisitNode(cbNode, (node as JsDocPropertyTag)?.Name);
            case SyntaxKind.PartiallyEmittedExpression:

                return VisitNode(cbNode, (node as PartiallyEmittedExpression)?.Expression);
            case SyntaxKind.JsDocLiteralType:

                return VisitNode(cbNode, (node as JsDocLiteralType)?.Literal);
        }
        return null;
    }

    public static void ForEachChildOptimized(INode node, Action<INode> visitor)
    {
        if (node == null)
            return;

        void visitNodes(IEnumerable<INode> nodes)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
                visitor(node);
        }

        switch (node.Kind)
        {
            case SyntaxKind.Parameter:
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.PropertySignature:

                visitNodes(node.Decorators);
                visitor((node as IVariableLikeDeclaration)?.PropertyName);
                visitor((node as IVariableLikeDeclaration)?.Name);
                visitor((node as IVariableLikeDeclaration)?.Type);
                break;
            
            case SyntaxKind.MethodDeclaration:
            case SyntaxKind.MethodSignature:
            case SyntaxKind.Constructor:
            case SyntaxKind.FunctionDeclaration:

                visitNodes(node.Decorators);
                visitor((node as IFunctionLikeDeclaration)?.Name);
                visitNodes((node as IFunctionLikeDeclaration)?.Parameters);
                visitor((node as IFunctionLikeDeclaration)?.Type);
                break;

            case SyntaxKind.TypeReference:
                visitor((node as TypeReferenceNode)?.TypeName);
                break;

            case SyntaxKind.PropertyAccessExpression:
                visitor((node as PropertyAccessExpression)?.Expression);
                visitor((node as PropertyAccessExpression)?.Name);
                break;
            
            case SyntaxKind.ElementAccessExpression:
            case SyntaxKind.CallExpression:
                visitor((node as CallExpression)?.Expression);
                visitNodes((node as CallExpression)?.TypeArguments);
                visitNodes((node as CallExpression)?.Arguments);
                break;
            
            case SyntaxKind.Block:
            case SyntaxKind.ModuleBlock:
                visitNodes((node as Block)?.Statements);
                break;
            
            case SyntaxKind.SourceFile:
                visitNodes((node as SourceFile)?.Statements);
                break;
            
            case SyntaxKind.Decorator:
                visitor((node as Decorator)?.Expression);
                break;
            
            case SyntaxKind.ClassDeclaration:
                visitNodes(node.Decorators);
                visitor((node as ClassDeclaration)?.Name);
                visitNodes((node as ClassDeclaration)?.TypeParameters);
                visitNodes((node as ClassDeclaration)?.HeritageClauses);
                visitNodes((node as ClassDeclaration)?.Members);
                break;
            
            case SyntaxKind.InterfaceDeclaration:
                visitNodes(node.Decorators);
                visitNodes(node.Modifiers);
                visitor((node as InterfaceDeclaration)?.Name);
                visitNodes((node as InterfaceDeclaration)?.TypeParameters);
                visitNodes((node as InterfaceDeclaration)?.HeritageClauses);
                visitNodes((node as InterfaceDeclaration)?.Members);
                break;

            case SyntaxKind.TypeAliasDeclaration:
                visitNodes(node.Decorators);
                visitor((node as TypeAliasDeclaration)?.Name);
                visitor((node as TypeAliasDeclaration)?.Type);
                break;

            case SyntaxKind.EnumDeclaration:
                visitNodes(node.Decorators);
                visitor((node as EnumDeclaration)?.Name);
                visitNodes((node as EnumDeclaration)?.Members);
                break;

            case SyntaxKind.EnumMember:
                visitor((node as EnumMember)?.Name);
                break;

            case SyntaxKind.ModuleDeclaration:
                visitNodes(node.Decorators);
                visitor((node as ModuleDeclaration)?.Name);
                visitor((node as ModuleDeclaration)?.Body);
                break;

            case SyntaxKind.ImportEqualsDeclaration:
                visitNodes(node.Decorators);
                visitor((node as ImportEqualsDeclaration)?.Name);
                visitor((node as ImportEqualsDeclaration)?.ModuleReference);
                break;

            case SyntaxKind.ImportDeclaration:
                visitNodes(node.Decorators);
                visitor((node as ImportDeclaration)?.ImportClause);
                visitor((node as ImportDeclaration)?.ModuleSpecifier);
                break;

            case SyntaxKind.ImportClause:
                visitor((node as ImportClause)?.Name);
                visitor((node as ImportClause)?.NamedBindings);
                break;

            case SyntaxKind.NamespaceExportDeclaration:
                visitor((node as NamespaceExportDeclaration)?.Name);
                break;
            
            case SyntaxKind.NamespaceImport:
                visitor((node as NamespaceImport)?.Name);
                break;
            
            case SyntaxKind.NamedImports:
            case SyntaxKind.NamedExports:
                if (node is NamedImports) 
                    visitNodes((node as NamedImports)?.Elements);
                else 
                    visitNodes((node as NamedExports)?.Elements);
                break;
            
            case SyntaxKind.ExportDeclaration:
                visitNodes(node.Decorators);
                visitNodes(node.Modifiers);
                visitor((node as ExportDeclaration)?.ExportClause);
                visitor((node as ExportDeclaration)?.ModuleSpecifier);
                break;
            
            case SyntaxKind.ImportSpecifier:
            case SyntaxKind.ExportSpecifier:
                visitor((node as IImportOrExportSpecifier)?.PropertyName);
                visitor((node as IImportOrExportSpecifier)?.Name);
                break;

            case SyntaxKind.ExportAssignment:
                visitNodes(node.Decorators);
                visitNodes(node.Modifiers);
                visitor((node as ExportAssignment)?.Expression);
                break;

            case SyntaxKind.TemplateExpression:

                visitor((node as TemplateExpression)?.Head);
                visitNodes((node as TemplateExpression)?.TemplateSpans);
                break;

            case SyntaxKind.TemplateSpan:
                visitor((node as TemplateSpan)?.Expression);
                visitor((node as TemplateSpan)?.Literal);
                break;

            case SyntaxKind.ComputedPropertyName:
                visitor((node as ComputedPropertyName)?.Expression);
                break;

            case SyntaxKind.HeritageClause:
                visitNodes((node as HeritageClause)?.Types);
                break;

            case SyntaxKind.ExpressionWithTypeArguments:
                visitor((node as ExpressionWithTypeArguments)?.Expression);
                visitNodes((node as ExpressionWithTypeArguments)?.TypeArguments);
                break;

            case SyntaxKind.ExternalModuleReference:
                visitor((node as ExternalModuleReference)?.Expression);
                break;

            case SyntaxKind.MissingDeclaration:
                visitNodes(node.Decorators);
                break;
        }
    }

    private static INode VisitNodes(Func<INode[], INode> cbNodes, List<Node> list)
    {
        throw new NotImplementedException();
    }


    public bool IsExternalModule(SourceFile file)
    {
        return file.ExternalModuleIndicator != null;
    }
}