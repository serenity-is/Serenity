namespace Serenity.TypeScript;

internal static class NodeVisitor
{

    //public static INode VisitNode(Func<INode, INode> cbNode, INode node)
    //{
    //    if (node != null)
    //        return cbNode(node);
    //    return null;
    //}

    //public static T VisitList<T>(Func<INode[], T> cbNodes, INode[] nodes)
    //{
    //    if (nodes != null)
    //        return cbNodes(nodes);
    //    return default;
    //}

    //public static INode VisitNodeArray(Func<INode[], INode> cbNodes, INode[] nodes)
    //{
    //    if (nodes != null)
    //        return cbNodes(nodes);
    //    return null;
    //}

    //public static INode VisitEachNode(Func<INode, INode> cbNode, List<INode> nodes)
    //{
    //    if (nodes != null)
    //        foreach (var node in nodes)
    //        {
    //            var result = cbNode(node);
    //            if (result != null)
    //                return result;
    //        }
    //    return null;
    //}

    //public static INode ForEachChild(INode node, Func<INode, INode> visitor)
    //{
    //    if (node == null)
    //        return null;
    //    INode visitNodes(object o1, IEnumerable<INode> o2)
    //    {
    //        var list = o2?.Cast<INode>().ToList();
    //        if (list != null)
    //            if (cbNodeArray == null)
    //                return VisitEachNode(visitor, list);
    //            else
    //                return cbNodeArray([.. list]);
    //        return null;
    //    }
    //    var cbNodes = cbNodeArray;

    //    if (node is IHasModifierLike hasModifiers && hasModifiers.Modifiers != null)
    //    {
    //        var result = visitNodes(cbNodes, hasModifiers.Modifiers);
    //        if (result != null)
    //            return result;
    //    }

    //    if (node is IHasName hasName && hasName.Name != null)
    //    {
    //        var result = VisitNode(visitor, hasName.Name);
    //        if (result != null)
    //            return result;
    //    }

    //    switch (node.Kind)
    //    {
    //        case SyntaxKind.QualifiedName:

    //            return VisitNode(visitor, (node as QualifiedName)?.Left) ??
    //                   VisitNode(visitor, (node as QualifiedName)?.Right);
    //        case SyntaxKind.TypeParameter:

    //            return VisitNode(visitor, (node as TypeParameterDeclaration)?.Constraint) ??
    //                   VisitNode(visitor, (node as TypeParameterDeclaration)?.Default) ??
    //                   VisitNode(visitor, (node as TypeParameterDeclaration)?.Expression);
    //        case SyntaxKind.ShorthandPropertyAssignment:

    //            return VisitNode(visitor, (node as ShorthandPropertyAssignment)?.QuestionToken) ??
    //                   VisitNode(visitor, (node as ShorthandPropertyAssignment)?.EqualsToken) ??
    //                   VisitNode(visitor, (node as ShorthandPropertyAssignment)?.ObjectAssignmentInitializer);
    //        case SyntaxKind.SpreadAssignment:

    //            return VisitNode(visitor, (node as SpreadAssignment)?.Expression);

    //        case SyntaxKind.Parameter:
    //            return VisitNode(visitor, (node as VariableDeclaration)?.ExclamationToken) ??
    //                   VisitNode(visitor, (node as VariableDeclaration)?.Type) ??
    //                   VisitNode(visitor, (node as VariableDeclaration)?.Initializer) ??
    //                   VisitNode(visitor, (node as VariableDeclaration)?.Name);

    //        case SyntaxKind.PropertyDeclaration:

    //            return VisitNode(visitor, (node as PropertyDeclaration)?.QuestionToken) ??
    //                   VisitNode(visitor, (node as PropertyDeclaration)?.ExclamationToken) ??
    //                   VisitNode(visitor, (node as PropertyDeclaration)?.QuestionToken) ??
    //                   VisitNode(visitor, (node as PropertyDeclaration)?.Type) ??
    //                   VisitNode(visitor, (node as PropertyDeclaration)?.Initializer);

    //        case SyntaxKind.PropertySignature:
    //            return VisitNode(visitor, (node as PropertySignature)?.QuestionToken) ??
    //                   VisitNode(visitor, (node as PropertySignature)?.Type);

    //        case SyntaxKind.PropertyAssignment:
    //            return VisitNode(visitor, (node as PropertyAssignment)?.Initializer);

    //        case SyntaxKind.VariableDeclaration:
    //            return VisitNode(visitor, (node as VariableDeclaration)?.ExclamationToken) ??
    //                   VisitNode(visitor, (node as VariableDeclaration)?.Type) ??
    //                   VisitNode(visitor, (node as VariableDeclaration)?.Initializer);

    //        case SyntaxKind.BindingElement:

    //            return VisitNode(visitor, (node as BindingElement)?.PropertyName) ??
    //                   VisitNode(visitor, (node as BindingElement)?.DotDotDotToken) ??
    //                   VisitNode(visitor, (node as BindingElement)?.Initializer);

    //        case SyntaxKind.FunctionType:
    //        case SyntaxKind.ConstructorType:
    //        case SyntaxKind.CallSignature:
    //        case SyntaxKind.ConstructSignature:
    //        case SyntaxKind.IndexSignature:

    //            return visitNodes(cbNodes, (node as ISignatureDeclaration)?.TypeParameters) ??
    //                   visitNodes(cbNodes, (node as ISignatureDeclaration)?.Parameters) ??
    //                   VisitNode(visitor, (node as ISignatureDeclaration)?.Type);

    //        case SyntaxKind.MethodDeclaration:
    //        case SyntaxKind.MethodSignature:
    //        case SyntaxKind.Constructor:
    //        case SyntaxKind.GetAccessor:
    //        case SyntaxKind.SetAccessor:
    //        case SyntaxKind.FunctionExpression:
    //        case SyntaxKind.FunctionDeclaration:
    //        case SyntaxKind.ArrowFunction:

    //            return VisitNode(visitor, (node as IFunctionLikeDeclaration)?.AsteriskToken) ??
    //                   VisitNode(visitor, (node as IFunctionLikeDeclaration)?.Name) ??
    //                   VisitNode(visitor, (node as IFunctionLikeDeclaration)?.QuestionToken) ??
    //                   visitNodes(cbNodes, (node as IFunctionLikeDeclaration)?.TypeParameters) ??
    //                   visitNodes(cbNodes, (node as IFunctionLikeDeclaration)?.Parameters) ??
    //                   VisitNode(visitor, (node as IFunctionLikeDeclaration)?.Type) ??
    //                   VisitNode(visitor, (node as ArrowFunction)?.EqualsGreaterThanToken) ??
    //                   VisitNode(visitor, (node as IFunctionLikeDeclaration)?.Body);
    //        case SyntaxKind.TypeReference:

    //            return VisitNode(visitor, (node as TypeReferenceNode)?.TypeName) ??
    //                   visitNodes(cbNodes, (node as TypeReferenceNode)?.TypeArguments);
    //        case SyntaxKind.TypePredicate:

    //            return VisitNode(visitor, (node as TypePredicateNode)?.ParameterName) ??
    //                   VisitNode(visitor, (node as TypePredicateNode)?.Type);
    //        case SyntaxKind.TypeQuery:

    //            return VisitNode(visitor, (node as TypeQueryNode)?.ExprName);
    //        case SyntaxKind.TypeLiteral:

    //            return visitNodes(cbNodes, (node as TypeLiteralNode)?.Members);
    //        case SyntaxKind.ArrayType:

    //            return VisitNode(visitor, (node as ArrayTypeNode)?.ElementType);
    //        case SyntaxKind.TupleType:

    //            return visitNodes(cbNodes, (node as TupleTypeNode)?.ElementTypes);
    //        case SyntaxKind.UnionType:
    //        case SyntaxKind.IntersectionType:

    //            return visitNodes(cbNodes, (node as IUnionOrIntersectionTypeNode)?.Types);
    //        case SyntaxKind.ParenthesizedType:
    //        case SyntaxKind.TypeOperator:

    //            return VisitNode(visitor, (node as ParenthesizedTypeNode)?.Type ?? (node as TypeOperatorNode)?.Type);
    //        case SyntaxKind.IndexedAccessType:

    //            return VisitNode(visitor, (node as IndexedAccessTypeNode)?.ObjectType) ??
    //                   VisitNode(visitor, (node as IndexedAccessTypeNode)?.IndexType);
    //        case SyntaxKind.MappedType:

    //            return VisitNode(visitor, (node as MappedTypeNode)?.ReadonlyToken) ??
    //                   VisitNode(visitor, (node as MappedTypeNode)?.TypeParameter) ??
    //                   VisitNode(visitor, (node as MappedTypeNode)?.QuestionToken) ??
    //                   VisitNode(visitor, (node as MappedTypeNode)?.Type);
    //        case SyntaxKind.LiteralType:

    //            return VisitNode(visitor, (node as LiteralTypeNode)?.Literal);
    //        case SyntaxKind.ObjectBindingPattern:
    //        case SyntaxKind.ArrayBindingPattern:

    //            return visitNodes(cbNodes, ((IBindingPattern)node).Elements);
    //        case SyntaxKind.ArrayLiteralExpression:

    //            return visitNodes(cbNodes, (node as ArrayLiteralExpression)?.Elements);
    //        case SyntaxKind.ObjectLiteralExpression:

    //            return visitNodes(cbNodes, (node as ObjectLiteralExpression)?.Properties);
    //        case SyntaxKind.PropertyAccessExpression:

    //            return VisitNode(visitor, (node as PropertyAccessExpression)?.Expression) ??
    //                   VisitNode(visitor, (node as PropertyAccessExpression)?.Name);
    //        case SyntaxKind.ElementAccessExpression:

    //            return VisitNode(visitor, (node as ElementAccessExpression)?.Expression) ??
    //                   VisitNode(visitor, (node as ElementAccessExpression)?.ArgumentExpression);
    //        case SyntaxKind.CallExpression:
    //        case SyntaxKind.NewExpression:

    //            return VisitNode(visitor, (node as CallExpression)?.Expression) ??
    //                   visitNodes(cbNodes, (node as CallExpression)?.TypeArguments) ??
    //                   visitNodes(cbNodes, (node as CallExpression)?.Arguments);
    //        case SyntaxKind.TaggedTemplateExpression:

    //            return VisitNode(visitor, (node as TaggedTemplateExpression)?.Tag) ??
    //                   VisitNode(visitor, (node as TaggedTemplateExpression)?.Template);
    //        case SyntaxKind.TypeAssertionExpression:

    //            return VisitNode(visitor, (node as TypeAssertion)?.Type) ??
    //                   VisitNode(visitor, (node as TypeAssertion)?.Expression);
    //        case SyntaxKind.ParenthesizedExpression:

    //            return VisitNode(visitor, (node as ParenthesizedExpression)?.Expression);
    //        case SyntaxKind.DeleteExpression:

    //            return VisitNode(visitor, (node as DeleteExpression)?.Expression);
    //        case SyntaxKind.TypeOfExpression:

    //            return VisitNode(visitor, (node as TypeOfExpression)?.Expression);
    //        case SyntaxKind.VoidExpression:

    //            return VisitNode(visitor, (node as VoidExpression)?.Expression);
    //        case SyntaxKind.PrefixUnaryExpression:

    //            return VisitNode(visitor, (node as PrefixUnaryExpression)?.Operand);
    //        case SyntaxKind.YieldExpression:

    //            return VisitNode(visitor, (node as YieldExpression)?.AsteriskToken) ??
    //                   VisitNode(visitor, (node as YieldExpression)?.Expression);
    //        case SyntaxKind.AwaitExpression:

    //            return VisitNode(visitor, (node as AwaitExpression)?.Expression);
    //        case SyntaxKind.PostfixUnaryExpression:

    //            return VisitNode(visitor, (node as PostfixUnaryExpression)?.Operand);
    //        case SyntaxKind.BinaryExpression:

    //            return VisitNode(visitor, (node as BinaryExpression)?.Left) ??
    //                   VisitNode(visitor, (node as BinaryExpression)?.OperatorToken) ??
    //                   VisitNode(visitor, (node as BinaryExpression)?.Right);
    //        case SyntaxKind.AsExpression:

    //            return VisitNode(visitor, (node as AsExpression)?.Expression) ??
    //                   VisitNode(visitor, (node as AsExpression)?.Type);
    //        case SyntaxKind.NonNullExpression:

    //            return VisitNode(visitor, (node as NonNullExpression)?.Expression);
    //        case SyntaxKind.MetaProperty:

    //            return VisitNode(visitor, (node as MetaProperty)?.Name);
    //        case SyntaxKind.ConditionalExpression:

    //            return VisitNode(visitor, (node as ConditionalExpression)?.Condition) ??
    //                   VisitNode(visitor, (node as ConditionalExpression)?.QuestionToken) ??
    //                   VisitNode(visitor, (node as ConditionalExpression)?.WhenTrue) ??
    //                   VisitNode(visitor, (node as ConditionalExpression)?.ColonToken) ??
    //                   VisitNode(visitor, (node as ConditionalExpression)?.WhenFalse);
    //        case SyntaxKind.SpreadElement:

    //            return VisitNode(visitor, (node as SpreadElement)?.Expression);
    //        case SyntaxKind.Block:
    //        case SyntaxKind.ModuleBlock:

    //            return visitNodes(cbNodes, (node as Block)?.Statements);
    //        case SyntaxKind.SourceFile:

    //            return visitNodes(cbNodes, (node as SourceFile)?.Statements) ??
    //                   VisitNode(visitor, (node as SourceFile)?.EndOfFileToken);
    //        case SyntaxKind.VariableStatement:

    //            return VisitNode(visitor, (node as VariableStatement)?.DeclarationList);
    //        case SyntaxKind.VariableDeclarationList:

    //            return visitNodes(cbNodes, (node as VariableDeclarationList)?.Declarations);
    //        case SyntaxKind.ExpressionStatement:

    //            return VisitNode(visitor, (node as ExpressionStatement)?.Expression);
    //        case SyntaxKind.IfStatement:

    //            return VisitNode(visitor, (node as IfStatement)?.Expression) ??
    //                   VisitNode(visitor, (node as IfStatement)?.ThenStatement) ??
    //                   VisitNode(visitor, (node as IfStatement)?.ElseStatement);
    //        case SyntaxKind.DoStatement:

    //            return VisitNode(visitor, (node as DoStatement)?.Statement) ??
    //                   VisitNode(visitor, (node as DoStatement)?.Expression);
    //        case SyntaxKind.WhileStatement:

    //            return VisitNode(visitor, (node as WhileStatement)?.Expression) ??
    //                   VisitNode(visitor, (node as WhileStatement)?.Statement);
    //        case SyntaxKind.ForStatement:

    //            return VisitNode(visitor, (node as ForStatement)?.Initializer) ??
    //                   VisitNode(visitor, (node as ForStatement)?.Condition) ??
    //                   VisitNode(visitor, (node as ForStatement)?.Incrementor) ??
    //                   VisitNode(visitor, (node as ForStatement)?.Statement);
    //        case SyntaxKind.ForInStatement:

    //            return VisitNode(visitor, (node as ForInStatement)?.Initializer) ??
    //                   VisitNode(visitor, (node as ForInStatement)?.Expression) ??
    //                   VisitNode(visitor, (node as ForInStatement)?.Statement);
    //        case SyntaxKind.ForOfStatement:

    //            return VisitNode(visitor, (node as ForOfStatement)?.AwaitModifier) ??
    //                   VisitNode(visitor, (node as ForOfStatement)?.Initializer) ??
    //                   VisitNode(visitor, (node as ForOfStatement)?.Expression) ??
    //                   VisitNode(visitor, (node as ForOfStatement)?.Statement);
    //        case SyntaxKind.ContinueStatement:
    //        case SyntaxKind.BreakStatement:

    //            return VisitNode(visitor, (node as IBreakOrContinueStatement)?.Label);
    //        case SyntaxKind.ReturnStatement:

    //            return VisitNode(visitor, (node as ReturnStatement)?.Expression);
    //        case SyntaxKind.WithStatement:

    //            return VisitNode(visitor, (node as WithStatement)?.Expression) ??
    //                   VisitNode(visitor, (node as WithStatement)?.Statement);
    //        case SyntaxKind.SwitchStatement:

    //            return VisitNode(visitor, (node as SwitchStatement)?.Expression) ??
    //                   VisitNode(visitor, (node as SwitchStatement)?.CaseBlock);
    //        case SyntaxKind.CaseBlock:

    //            return visitNodes(cbNodes, (node as CaseBlock)?.Clauses);
    //        case SyntaxKind.CaseClause:

    //            return VisitNode(visitor, (node as CaseClause)?.Expression) ??
    //                   visitNodes(cbNodes, (node as CaseClause)?.Statements);
    //        case SyntaxKind.DefaultClause:

    //            return visitNodes(cbNodes, (node as DefaultClause)?.Statements);
    //        case SyntaxKind.LabeledStatement:

    //            return VisitNode(visitor, (node as LabeledStatement)?.Label) ??
    //                   VisitNode(visitor, (node as LabeledStatement)?.Statement);
    //        case SyntaxKind.ThrowStatement:

    //            return VisitNode(visitor, (node as ThrowStatement)?.Expression);
    //        case SyntaxKind.TryStatement:

    //            return VisitNode(visitor, (node as TryStatement)?.TryBlock) ??
    //                   VisitNode(visitor, (node as TryStatement)?.CatchClause) ??
    //                   VisitNode(visitor, (node as TryStatement)?.FinallyBlock);
    //        case SyntaxKind.CatchClause:

    //            return VisitNode(visitor, (node as CatchClause)?.VariableDeclaration) ??
    //                   VisitNode(visitor, (node as CatchClause)?.Block);
    //        case SyntaxKind.Decorator:

    //            return VisitNode(visitor, (node as Decorator)?.Expression);
    //        case SyntaxKind.ClassDeclaration:
    //            return VisitNode(visitor, (node as ClassDeclaration)?.Name) ??
    //                   visitNodes(cbNodes, (node as ClassDeclaration)?.TypeParameters) ??
    //                   visitNodes(cbNodes, (node as ClassDeclaration)?.HeritageClauses) ??
    //                   visitNodes(cbNodes, (node as ClassDeclaration)?.Members);
    //        case SyntaxKind.ClassExpression:

    //            return VisitNode(visitor, (node as ClassExpression)?.Name) ??
    //                   visitNodes(cbNodes, (node as ClassExpression)?.TypeParameters) ??
    //                   visitNodes(cbNodes, (node as ClassExpression)?.HeritageClauses) ??
    //                   visitNodes(cbNodes, (node as ClassExpression)?.Members);
    //        case SyntaxKind.InterfaceDeclaration:

    //            return VisitNode(visitor, (node as InterfaceDeclaration)?.Name) ??
    //                   visitNodes(cbNodes, (node as InterfaceDeclaration)?.TypeParameters) ??
    //                   visitNodes(cbNodes, (node as InterfaceDeclaration)?.HeritageClauses) ??
    //                   visitNodes(cbNodes, (node as InterfaceDeclaration)?.Members);
    //        case SyntaxKind.TypeAliasDeclaration:

    //            return VisitNode(visitor, (node as TypeAliasDeclaration)?.Name) ??
    //                   visitNodes(cbNodes, (node as TypeAliasDeclaration)?.TypeParameters) ??
    //                   VisitNode(visitor, (node as TypeAliasDeclaration)?.Type);
    //        case SyntaxKind.EnumDeclaration:

    //            return VisitNode(visitor, (node as EnumDeclaration)?.Name) ??
    //                   visitNodes(cbNodes, (node as EnumDeclaration)?.Members);
    //        case SyntaxKind.EnumMember:

    //            return VisitNode(visitor, (node as EnumMember)?.Name) ??
    //                   VisitNode(visitor, (node as EnumMember)?.Initializer);
    //        case SyntaxKind.ModuleDeclaration:

    //            return VisitNode(visitor, (node as ModuleDeclaration)?.Name) ??
    //                   VisitNode(visitor, (node as ModuleDeclaration)?.Body);
    //        case SyntaxKind.ImportEqualsDeclaration:

    //            return VisitNode(visitor, (node as ImportEqualsDeclaration)?.Name) ??
    //                   VisitNode(visitor, (node as ImportEqualsDeclaration)?.ModuleReference);
    //        case SyntaxKind.ImportDeclaration:

    //            return VisitNode(visitor, (node as ImportDeclaration)?.ImportClause) ??
    //                   VisitNode(visitor, (node as ImportDeclaration)?.ModuleSpecifier);
    //        case SyntaxKind.ImportClause:

    //            return VisitNode(visitor, (node as ImportClause)?.Name) ??
    //                   VisitNode(visitor, (node as ImportClause)?.NamedBindings);
    //        case SyntaxKind.NamespaceExportDeclaration:

    //            return VisitNode(visitor, (node as NamespaceExportDeclaration)?.Name);
    //        case SyntaxKind.NamespaceImport:

    //            return VisitNode(visitor, (node as NamespaceImport)?.Name);
    //        case SyntaxKind.NamedImports:
    //        case SyntaxKind.NamedExports:

    //            if (node is NamedImports) return visitNodes(cbNodes, (node as NamedImports)?.Elements);
    //            else return visitNodes(cbNodes, (node as NamedExports)?.Elements);
    //        case SyntaxKind.ExportDeclaration:

    //            return VisitNode(visitor, (node as ExportDeclaration)?.ExportClause) ??
    //                   VisitNode(visitor, (node as ExportDeclaration)?.ModuleSpecifier);
    //        case SyntaxKind.ImportSpecifier:
    //        case SyntaxKind.ExportSpecifier:
    //            return VisitNode(visitor, (node as IImportOrExportSpecifier)?.PropertyName ??
    //                                     VisitNode(visitor, (node as IImportOrExportSpecifier)?.Name));
    //        case SyntaxKind.ExportAssignment:

    //            return VisitNode(visitor, (node as ExportAssignment)?.Expression);
    //        case SyntaxKind.TemplateExpression:

    //            return VisitNode(visitor, (node as TemplateExpression)?.Head) ??
    //                   visitNodes(cbNodes, (node as TemplateExpression)?.TemplateSpans);
    //        case SyntaxKind.TemplateSpan:

    //            return VisitNode(visitor, (node as TemplateSpan)?.Expression) ??
    //                   VisitNode(visitor, (node as TemplateSpan)?.Literal);
    //        case SyntaxKind.ComputedPropertyName:

    //            return VisitNode(visitor, (node as ComputedPropertyName)?.Expression);
    //        case SyntaxKind.HeritageClause:

    //            return visitNodes(cbNodes, (node as HeritageClause)?.Types);
    //        case SyntaxKind.ExpressionWithTypeArguments:

    //            return VisitNode(visitor, (node as ExpressionWithTypeArguments)?.Expression) ??
    //                   visitNodes(cbNodes, (node as ExpressionWithTypeArguments)?.TypeArguments);
    //        case SyntaxKind.ExternalModuleReference:

    //            return VisitNode(visitor, (node as ExternalModuleReference)?.Expression);
    //        case SyntaxKind.MissingDeclaration:

    //            return null;
    //        case SyntaxKind.JsxElement:

    //            return VisitNode(visitor, (node as JsxElement)?.OpeningElement) ??
    //                   visitNodes(cbNodes, (node as JsxElement)?.Children) ??
    //                   VisitNode(visitor, (node as JsxElement)?.ClosingElement);
    //        case SyntaxKind.JsxSelfClosingElement:
    //        case SyntaxKind.JsxOpeningElement:
    //            return VisitNode(visitor,
    //                       (node as JsxSelfClosingElement)?.TagName ?? (node as JsxOpeningElement)?.TagName) ??
    //                   VisitNode(visitor,
    //                       (node as JsxSelfClosingElement)?.Attributes ?? (node as JsxOpeningElement)?.Attributes);
    //        case SyntaxKind.JsxAttributes:

    //            return visitNodes(cbNodes, (node as JsxAttributes)?.Properties);
    //        case SyntaxKind.JsxAttribute:

    //            return VisitNode(visitor, (node as JsxAttribute)?.Name) ??
    //                   VisitNode(visitor, (node as JsxAttribute)?.Initializer);
    //        case SyntaxKind.JsxSpreadAttribute:

    //            return VisitNode(visitor, (node as JsxSpreadAttribute)?.Expression);
    //        case SyntaxKind.JsxExpression:

    //            return VisitNode(visitor, (node as JsxExpression).DotDotDotToken) ??
    //                   VisitNode(visitor, (node as JsxExpression).Expression);
    //        case SyntaxKind.JsxClosingElement:

    //            return VisitNode(visitor, (node as JsxClosingElement)?.TagName);
    //    }
    //    return null;
    //}

    //private static INode VisitNodes(Func<INode[], INode> cbNodes, List<INode> list)
    //{
    //    throw new NotImplementedException();
    //}

}