using System.Reflection.Metadata;
using static Serenity.TypeScript.NodeVisitor;
using static Serenity.TypeScript.Scanner;

namespace Serenity.TypeScript;

internal class NodeTests
{
    // Literals

    internal static bool IsNumericLiteral(INode node)
    {
        return node.Kind == SyntaxKind.NumericLiteral;
    }

    internal static bool IsBigIntLiteral(INode node)
    {
        return node.Kind == SyntaxKind.BigIntLiteral;
    }

    internal static bool IsStringLiteral(INode node)
    {
        return node.Kind == SyntaxKind.StringLiteral;
    }

    internal static bool IsJsxText(INode node)
    {
        return node.Kind == SyntaxKind.JsxText;
    }

    internal static bool IsRegularExpressionLiteral(INode node)
    {
        return node.Kind == SyntaxKind.RegularExpressionLiteral;
    }

    internal static bool IsNoSubstitutionTemplateLiteral(INode node)
    {
        return node.Kind == SyntaxKind.NoSubstitutionTemplateLiteral;
    }

    // Pseudo-literals

    internal static bool IsTemplateHead(INode node)
    {
        return node.Kind == SyntaxKind.TemplateHead;
    }

    internal static bool IsTemplateMiddle(INode node)
    {
        return node.Kind == SyntaxKind.TemplateMiddle;
    }

    internal static bool IsTemplateTail(INode node)
    {
        return node.Kind == SyntaxKind.TemplateTail;
    }

    // Punctuation

    internal static bool IsDotDotDotToken(INode node)
    {
        return node.Kind == SyntaxKind.DotDotDotToken;
    }

    /** @internal */
    internal static bool IsCommaToken(INode node)
    {
        return node.Kind == SyntaxKind.CommaToken;
    }

    internal static bool IsPlusToken(INode node)
    {
        return node.Kind == SyntaxKind.PlusToken;
    }

    internal static bool IsMinusToken(INode node)
    {
        return node.Kind == SyntaxKind.MinusToken;
    }

    internal static bool IsAsteriskToken(INode node)
    {
        return node.Kind == SyntaxKind.AsteriskToken;
    }

    internal static bool IsExclamationToken(INode node)
    {
        return node.Kind == SyntaxKind.ExclamationToken;
    }

    internal static bool IsQuestionToken(INode node)
    {
        return node.Kind == SyntaxKind.QuestionToken;
    }

    internal static bool IsColonToken(INode node)
    {
        return node.Kind == SyntaxKind.ColonToken;
    }

    internal static bool IsQuestionDotToken(INode node)
    {
        return node.Kind == SyntaxKind.QuestionDotToken;
    }

    internal static bool IsEqualsGreaterThanToken(INode node)
    {
        return node.Kind == SyntaxKind.EqualsGreaterThanToken;
    }

    // Identifiers

    internal static bool IsIdentifier(INode node)
    {
        return node.Kind == SyntaxKind.Identifier;
    }

    internal static bool IsPrivateIdentifier(INode node)
    {
        return node.Kind == SyntaxKind.PrivateIdentifier;
    }

    // Reserved Words

    /** @internal */
    internal static bool IsExportModifier(INode node)
    {
        return node.Kind == SyntaxKind.ExportKeyword;
    }

    /** @internal */
    internal static bool IsDefaultModifier(INode node)
    {
        return node.Kind == SyntaxKind.DefaultKeyword;
    }

    /** @internal */
    internal static bool IsAsyncModifier(INode node)
    {
        return node.Kind == SyntaxKind.AsyncKeyword;
    }

    internal static bool IsAssertsKeyword(INode node)
    {
        return node.Kind == SyntaxKind.AssertsKeyword;
    }

    internal static bool IsAwaitKeyword(INode node)
    {
        return node.Kind == SyntaxKind.AwaitKeyword;
    }

    /** @internal */
    internal static bool IsReadonlyKeyword(INode node)
    {
        return node.Kind == SyntaxKind.ReadonlyKeyword;
    }

    /** @internal */
    internal static bool IsStaticModifier(INode node)
    {
        return node.Kind == SyntaxKind.StaticKeyword;
    }

    /** @internal */
    internal static bool IsAbstractModifier(INode node)
    {
        return node.Kind == SyntaxKind.AbstractKeyword;
    }

    /** @internal */
    internal static bool IsOverrideModifier(INode node)
    {
        return node.Kind == SyntaxKind.OverrideKeyword;
    }

    /** @internal */
    internal static bool IsAccessorModifier(INode node)
    {
        return node.Kind == SyntaxKind.AccessorKeyword;
    }

    /** @internal */
    internal static bool IsSuperKeyword(INode node)
    {
        return node.Kind == SyntaxKind.SuperKeyword;
    }

    /** @internal */
    internal static bool IsImportKeyword(INode node)
    {
        return node.Kind == SyntaxKind.ImportKeyword;
    }

    /** @internal */
    internal static bool IsCaseKeyword(INode node)
    {
        return node.Kind == SyntaxKind.CaseKeyword;
    }

    // Names

    internal static bool IsQualifiedName(INode node)
    {
        return node.Kind == SyntaxKind.QualifiedName;
    }

    internal static bool IsComputedPropertyName(INode node)
    {
        return node.Kind == SyntaxKind.ComputedPropertyName;
    }

    // Signature elements

    internal static bool IsTypeParameterDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.TypeParameter;
    }

    // TODO(rbuckton): Rename to 'isParameterDeclaration'
    internal static bool IsParameter(INode node)
    {
        return node.Kind == SyntaxKind.Parameter;
    }

    internal static bool IsDecorator(INode node)
    {
        return node.Kind == SyntaxKind.Decorator;
    }

    // TypeMember

    internal static bool IsPropertySignature(INode node)
    {
        return node.Kind == SyntaxKind.PropertySignature;
    }

    internal static bool IsPropertyDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.PropertyDeclaration;
    }

    internal static bool IsMethodSignature(INode node)
    {
        return node.Kind == SyntaxKind.MethodSignature;
    }

    internal static bool IsMethodDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.MethodDeclaration;
    }

    internal static bool IsClassStaticBlockDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ClassStaticBlockDeclaration;
    }

    internal static bool IsConstructorDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.Constructor;
    }

    internal static bool IsGetAccessorDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.GetAccessor;
    }

    internal static bool IsSetAccessorDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.SetAccessor;
    }

    internal static bool IsCallSignatureDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.CallSignature;
    }

    internal static bool IsConstructSignatureDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ConstructSignature;
    }

    internal static bool IsIndexSignatureDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.IndexSignature;
    }

    // Type

    internal static bool IsTypePredicateNode(INode node)
    {
        return node.Kind == SyntaxKind.TypePredicate;
    }

    internal static bool IsTypeReferenceNode(INode node)
    {
        return node.Kind == SyntaxKind.TypeReference;
    }

    internal static bool IsFunctionTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.FunctionType;
    }

    internal static bool IsConstructorTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.ConstructorType;
    }

    internal static bool IsTypeQueryNode(INode node)
    {
        return node.Kind == SyntaxKind.TypeQuery;
    }

    internal static bool IsTypeLiteralNode(INode node)
    {
        return node.Kind == SyntaxKind.TypeLiteral;
    }

    internal static bool IsArrayTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.ArrayType;
    }

    internal static bool IsTupleTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.TupleType;
    }

    internal static bool IsNamedTupleMember(INode node)
    {
        return node.Kind == SyntaxKind.NamedTupleMember;
    }

    internal static bool IsOptionalTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.OptionalType;
    }

    internal static bool IsRestTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.RestType;
    }

    internal static bool IsUnionTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.UnionType;
    }

    internal static bool IsIntersectionTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.IntersectionType;
    }

    internal static bool IsConditionalTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.ConditionalType;
    }

    internal static bool IsInferTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.InferType;
    }

    internal static bool IsParenthesizedTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.ParenthesizedType;
    }

    internal static bool IsThisTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.ThisType;
    }

    internal static bool IsTypeOperatorNode(INode node)
    {
        return node.Kind == SyntaxKind.TypeOperator;
    }

    internal static bool IsIndexedAccessTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.IndexedAccessType;
    }

    internal static bool IsMappedTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.MappedType;
    }

    internal static bool IsLiteralTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.LiteralType;
    }

    internal static bool IsImportTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.ImportType;
    }

    internal static bool IsTemplateLiteralTypeSpan(INode node)
    {
        return node.Kind == SyntaxKind.TemplateLiteralTypeSpan;
    }

    internal static bool IsTemplateLiteralTypeNode(INode node)
    {
        return node.Kind == SyntaxKind.TemplateLiteralType;
    }

    // Binding patterns

    internal static bool IsObjectBindingPattern(INode node)
    {
        return node.Kind == SyntaxKind.ObjectBindingPattern;
    }

    internal static bool IsArrayBindingPattern(INode node)
    {
        return node.Kind == SyntaxKind.ArrayBindingPattern;
    }

    internal static bool IsBindingElement(INode node)
    {
        return node.Kind == SyntaxKind.BindingElement;
    }

    // Expression

    internal static bool IsArrayLiteralExpression(INode node)
    {
        return node.Kind == SyntaxKind.ArrayLiteralExpression;
    }

    internal static bool IsObjectLiteralExpression(INode node)
    {
        return node.Kind == SyntaxKind.ObjectLiteralExpression;
    }

    internal static bool IsPropertyAccessExpression(INode node)
    {
        return node.Kind == SyntaxKind.PropertyAccessExpression;
    }

    internal static bool IsElementAccessExpression(INode node)
    {
        return node.Kind == SyntaxKind.ElementAccessExpression;
    }

    internal static bool IsCallExpression(INode node)
    {
        return node.Kind == SyntaxKind.CallExpression;
    }

    internal static bool IsNewExpression(INode node)
    {
        return node.Kind == SyntaxKind.NewExpression;
    }

    internal static bool IsTaggedTemplateExpression(INode node)
    {
        return node.Kind == SyntaxKind.TaggedTemplateExpression;
    }

    internal static bool IsTypeAssertionExpression(INode node)
    {
        return node.Kind == SyntaxKind.TypeAssertionExpression;
    }

    internal static bool IsParenthesizedExpression(INode node)
    {
        return node.Kind == SyntaxKind.ParenthesizedExpression;
    }

    internal static bool IsFunctionExpression(INode node)
    {
        return node.Kind == SyntaxKind.FunctionExpression;
    }

    internal static bool IsArrowFunction(INode node)
    {
        return node.Kind == SyntaxKind.ArrowFunction;
    }

    internal static bool IsDeleteExpression(INode node)
    {
        return node.Kind == SyntaxKind.DeleteExpression;
    }

    internal static bool IsTypeOfExpression(INode node)
    {
        return node.Kind == SyntaxKind.TypeOfExpression;
    }

    internal static bool IsVoidExpression(INode node)
    {
        return node.Kind == SyntaxKind.VoidExpression;
    }

    internal static bool IsAwaitExpression(INode node)
    {
        return node.Kind == SyntaxKind.AwaitExpression;
    }

    internal static bool IsPrefixUnaryExpression(INode node)
    {
        return node.Kind == SyntaxKind.PrefixUnaryExpression;
    }

    internal static bool IsPostfixUnaryExpression(INode node)
    {
        return node.Kind == SyntaxKind.PostfixUnaryExpression;
    }

    internal static bool IsBinaryExpression(INode node)
    {
        return node.Kind == SyntaxKind.BinaryExpression;
    }

    internal static bool IsConditionalExpression(INode node)
    {
        return node.Kind == SyntaxKind.ConditionalExpression;
    }

    internal static bool IsTemplateExpression(INode node)
    {
        return node.Kind == SyntaxKind.TemplateExpression;
    }

    internal static bool IsYieldExpression(INode node)
    {
        return node.Kind == SyntaxKind.YieldExpression;
    }

    internal static bool IsSpreadElement(INode node)
    {
        return node.Kind == SyntaxKind.SpreadElement;
    }

    internal static bool IsClassExpression(INode node)
    {
        return node.Kind == SyntaxKind.ClassExpression;
    }

    internal static bool IsOmittedExpression(INode node)
    {
        return node.Kind == SyntaxKind.OmittedExpression;
    }

    internal static bool IsExpressionWithTypeArguments(INode node)
    {
        return node.Kind == SyntaxKind.ExpressionWithTypeArguments;
    }

    internal static bool IsAsExpression(INode node)
    {
        return node.Kind == SyntaxKind.AsExpression;
    }

    internal static bool IsSatisfiesExpression(INode node)
    {
        return node.Kind == SyntaxKind.SatisfiesExpression;
    }

    internal static bool IsNonNullExpression(INode node)
    {
        return node.Kind == SyntaxKind.NonNullExpression;
    }

    internal static bool IsMetaProperty(INode node)
    {
        return node.Kind == SyntaxKind.MetaProperty;
    }

    internal static bool IsSyntheticExpression(INode node)
    {
        return node.Kind == SyntaxKind.SyntheticExpression;
    }

    internal static bool IsPartiallyEmittedExpression(INode node)
    {
        return node.Kind == SyntaxKind.PartiallyEmittedExpression;
    }

    internal static bool IsCommaListExpression(INode node)
    {
        return node.Kind == SyntaxKind.CommaListExpression;
    }

    // Misc

    internal static bool IsTemplateSpan(INode node)
    {
        return node.Kind == SyntaxKind.TemplateSpan;
    }

    internal static bool IsSemicolonClassElement(INode node)
    {
        return node.Kind == SyntaxKind.SemicolonClassElement;
    }

    // Elements

    internal static bool IsBlock(INode node)
    {
        return node.Kind == SyntaxKind.Block;
    }

    internal static bool IsVariableStatement(INode node)
    {
        return node.Kind == SyntaxKind.VariableStatement;
    }

    internal static bool IsEmptyStatement(INode node)
    {
        return node.Kind == SyntaxKind.EmptyStatement;
    }

    internal static bool IsExpressionStatement(INode node)
    {
        return node.Kind == SyntaxKind.ExpressionStatement;
    }

    internal static bool IsIfStatement(INode node)
    {
        return node.Kind == SyntaxKind.IfStatement;
    }

    internal static bool IsDoStatement(INode node)
    {
        return node.Kind == SyntaxKind.DoStatement;
    }

    internal static bool IsWhileStatement(INode node)
    {
        return node.Kind == SyntaxKind.WhileStatement;
    }

    internal static bool IsForStatement(INode node)
    {
        return node.Kind == SyntaxKind.ForStatement;
    }

    internal static bool IsForInStatement(INode node)
    {
        return node.Kind == SyntaxKind.ForInStatement;
    }

    internal static bool IsForOfStatement(INode node)
    {
        return node.Kind == SyntaxKind.ForOfStatement;
    }

    internal static bool IsContinueStatement(INode node)
    {
        return node.Kind == SyntaxKind.ContinueStatement;
    }

    internal static bool IsBreakStatement(INode node)
    {
        return node.Kind == SyntaxKind.BreakStatement;
    }

    internal static bool IsReturnStatement(INode node)
    {
        return node.Kind == SyntaxKind.ReturnStatement;
    }

    internal static bool IsWithStatement(INode node)
    {
        return node.Kind == SyntaxKind.WithStatement;
    }

    internal static bool IsSwitchStatement(INode node)
    {
        return node.Kind == SyntaxKind.SwitchStatement;
    }

    internal static bool IsLabeledStatement(INode node)
    {
        return node.Kind == SyntaxKind.LabeledStatement;
    }

    internal static bool IsThrowStatement(INode node)
    {
        return node.Kind == SyntaxKind.ThrowStatement;
    }

    internal static bool IsTryStatement(INode node)
    {
        return node.Kind == SyntaxKind.TryStatement;
    }

    internal static bool IsDebuggerStatement(INode node)
    {
        return node.Kind == SyntaxKind.DebuggerStatement;
    }

    internal static bool IsVariableDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.VariableDeclaration;
    }

    internal static bool IsVariableDeclarationList(INode node)
    {
        return node.Kind == SyntaxKind.VariableDeclarationList;
    }

    internal static bool IsFunctionDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.FunctionDeclaration;
    }

    internal static bool IsClassDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ClassDeclaration;
    }

    internal static bool IsInterfaceDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.InterfaceDeclaration;
    }

    internal static bool IsTypeAliasDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.TypeAliasDeclaration;
    }

    internal static bool IsEnumDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.EnumDeclaration;
    }

    internal static bool IsModuleDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ModuleDeclaration;
    }

    internal static bool IsModuleBlock(INode node)
    {
        return node.Kind == SyntaxKind.ModuleBlock;
    }

    internal static bool IsCaseBlock(INode node)
    {
        return node.Kind == SyntaxKind.CaseBlock;
    }

    internal static bool IsNamespaceExportDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.NamespaceExportDeclaration;
    }

    internal static bool IsImportEqualsDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ImportEqualsDeclaration;
    }

    internal static bool IsImportDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ImportDeclaration;
    }

    internal static bool IsImportClause(INode node)
    {
        return node.Kind == SyntaxKind.ImportClause;
    }

    internal static bool IsImportTypeAssertionContainer(INode node)
    {
        return node.Kind == SyntaxKind.ImportTypeAssertionContainer;
    }

    /** @deprecated */
    internal static bool IsAssertClause(INode node)
    {
        return node.Kind == SyntaxKind.AssertClause;
    }

    /** @deprecated */
    internal static bool IsAssertEntry(INode node)
    {
        return node.Kind == SyntaxKind.AssertEntry;
    }

    internal static bool IsImportAttributes(INode node)
    {
        return node.Kind == SyntaxKind.ImportAttributes;
    }

    internal static bool IsImportAttribute(INode node)
    {
        return node.Kind == SyntaxKind.ImportAttribute;
    }

    internal static bool IsNamespaceImport(INode node)
    {
        return node.Kind == SyntaxKind.NamespaceImport;
    }

    internal static bool IsNamespaceExport(INode node)
    {
        return node.Kind == SyntaxKind.NamespaceExport;
    }

    internal static bool IsNamedImports(INode node)
    {
        return node.Kind == SyntaxKind.NamedImports;
    }

    internal static bool IsImportSpecifier(INode node)
    {
        return node.Kind == SyntaxKind.ImportSpecifier;
    }

    internal static bool IsExportAssignment(INode node)
    {
        return node.Kind == SyntaxKind.ExportAssignment;
    }

    internal static bool IsExportDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.ExportDeclaration;
    }

    internal static bool IsNamedExports(INode node)
    {
        return node.Kind == SyntaxKind.NamedExports;
    }

    internal static bool IsExportSpecifier(INode node)
    {
        return node.Kind == SyntaxKind.ExportSpecifier;
    }

    internal static bool IsMissingDeclaration(INode node)
    {
        return node.Kind == SyntaxKind.MissingDeclaration;
    }

    internal static bool IsNotEmittedStatement(INode node)
    {
        return node.Kind == SyntaxKind.NotEmittedStatement;
    }

    /** @internal */
    internal static bool IsSyntheticReference(INode node)
    {
        return node.Kind == SyntaxKind.SyntheticReferenceExpression;
    }

    // Module References

    internal static bool IsExternalModuleReference(INode node)
    {
        return node.Kind == SyntaxKind.ExternalModuleReference;
    }

    // JSX

    internal static bool IsJsxElement(INode node)
    {
        return node.Kind == SyntaxKind.JsxElement;
    }

    internal static bool IsJsxSelfClosingElement(INode node)
    {
        return node.Kind == SyntaxKind.JsxSelfClosingElement;
    }

    internal static bool IsJsxOpeningElement(INode node)
    {
        return node.Kind == SyntaxKind.JsxOpeningElement;
    }

    internal static bool IsJsxClosingElement(INode node)
    {
        return node.Kind == SyntaxKind.JsxClosingElement;
    }

    internal static bool IsJsxFragment(INode node)
    {
        return node.Kind == SyntaxKind.JsxFragment;
    }

    internal static bool IsJsxOpeningFragment(INode node)
    {
        return node.Kind == SyntaxKind.JsxOpeningFragment;
    }

    internal static bool IsJsxClosingFragment(INode node)
    {
        return node.Kind == SyntaxKind.JsxClosingFragment;
    }

    internal static bool IsJsxAttribute(INode node)
    {
        return node.Kind == SyntaxKind.JsxAttribute;
    }

    internal static bool IsJsxAttributes(INode node)
    {
        return node.Kind == SyntaxKind.JsxAttributes;
    }

    internal static bool IsJsxSpreadAttribute(INode node)
    {
        return node.Kind == SyntaxKind.JsxSpreadAttribute;
    }

    internal static bool IsJsxExpression(INode node)
    {
        return node.Kind == SyntaxKind.JsxExpression;
    }

    internal static bool IsJsxNamespacedName(INode node)
    {
        return node.Kind == SyntaxKind.JsxNamespacedName;
    }

    // Clauses

    internal static bool IsCaseClause(INode node)
    {
        return node.Kind == SyntaxKind.CaseClause;
    }

    internal static bool IsDefaultClause(INode node)
    {
        return node.Kind == SyntaxKind.DefaultClause;
    }

    internal static bool IsHeritageClause(INode node)
    {
        return node.Kind == SyntaxKind.HeritageClause;
    }

    internal static bool IsCatchClause(INode node)
    {
        return node.Kind == SyntaxKind.CatchClause;
    }

    // Property assignments

    internal static bool IsPropertyAssignment(INode node)
    {
        return node.Kind == SyntaxKind.PropertyAssignment;
    }

    internal static bool IsShorthandPropertyAssignment(INode node)
    {
        return node.Kind == SyntaxKind.ShorthandPropertyAssignment;
    }

    internal static bool IsSpreadAssignment(INode node)
    {
        return node.Kind == SyntaxKind.SpreadAssignment;
    }

    // Enum

    internal static bool IsEnumMember(INode node)
    {
        return node.Kind == SyntaxKind.EnumMember;
    }

    // Unparsed

    // TODO(rbuckton): isUnparsedPrologue
    /** @deprecated */
    internal static bool IsUnparsedPrepend(INode node)
    {
        return node.Kind == SyntaxKind.UnparsedPrepend;
    }

    // TODO(rbuckton): isUnparsedText
    // TODO(rbuckton): isUnparsedInternalText
    // TODO(rbuckton): isUnparsedSyntheticReference

    // Top-level nodes
    internal static bool IsSourceFile(INode node)
    {
        return node.Kind == SyntaxKind.SourceFile;
    }

    internal static bool IsBundle(INode node)
    {
        return node.Kind == SyntaxKind.Bundle;
    }

    /** @deprecated */
    internal static bool IsUnparsedSource(INode node)
    {
        return node.Kind == SyntaxKind.UnparsedSource;
    }

    // TODO(rbuckton): isInputFiles

    // JSDoc Elements

    internal static bool IsJSDocTypeExpression(INode node)
    {
        return node.Kind == SyntaxKind.JSDocTypeExpression;
    }

    internal static bool IsJSDocNameReference(INode node)
    {
        return node.Kind == SyntaxKind.JSDocNameReference;
    }

    internal static bool IsJSDocMemberName(INode node)
    {
        return node.Kind == SyntaxKind.JSDocMemberName;
    }

    internal static bool IsJSDocLink(INode node)
    {
        return node.Kind == SyntaxKind.JSDocLink;
    }

    internal static bool IsJSDocLinkCode(INode node)
    {
        return node.Kind == SyntaxKind.JSDocLinkCode;
    }

    internal static bool IsJSDocLinkPlain(INode node)
    {
        return node.Kind == SyntaxKind.JSDocLinkPlain;
    }

    internal static bool IsJSDocAllType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocAllType;
    }

    internal static bool IsJSDocUnknownType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocUnknownType;
    }

    internal static bool IsJSDocNullableType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocNullableType;
    }

    internal static bool IsJSDocNonNullableType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocNonNullableType;
    }

    internal static bool IsJSDocOptionalType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocOptionalType;
    }

    internal static bool IsJSDocFunctionType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocFunctionType;
    }

    internal static bool IsJSDocVariadicType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocVariadicType;
    }

    internal static bool IsJSDocNamepathType(INode node)
    {
        return node.Kind == SyntaxKind.JSDocNamepathType;
    }

    internal static bool IsJSDoc(INode node)
    {
        return node.Kind == SyntaxKind.JSDoc;
    }

    internal static bool IsJSDocTypeLiteral(INode node)
    {
        return node.Kind == SyntaxKind.JSDocTypeLiteral;
    }

    internal static bool IsJSDocSignature(INode node)
    {
        return node.Kind == SyntaxKind.JSDocSignature;
    }

    // JSDoc Tags

    internal static bool IsJSDocAugmentsTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocAugmentsTag;
    }

    internal static bool IsJSDocAuthorTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocAuthorTag;
    }

    internal static bool IsJSDocClassTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocClassTag;
    }

    internal static bool IsJSDocCallbackTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocCallbackTag;
    }

    internal static bool IsJSDocPublicTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocPublicTag;
    }

    internal static bool IsJSDocPrivateTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocPrivateTag;
    }

    internal static bool IsJSDocProtectedTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocProtectedTag;
    }

    internal static bool IsJSDocReadonlyTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocReadonlyTag;
    }

    internal static bool IsJSDocOverrideTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocOverrideTag;
    }

    internal static bool IsJSDocOverloadTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocOverloadTag;
    }

    internal static bool IsJSDocDeprecatedTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocDeprecatedTag;
    }

    internal static bool IsJSDocSeeTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocSeeTag;
    }

    internal static bool IsJSDocEnumTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocEnumTag;
    }

    internal static bool IsJSDocParameterTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocParameterTag;
    }

    internal static bool IsJSDocReturnTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocReturnTag;
    }

    internal static bool IsJSDocThisTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocThisTag;
    }

    internal static bool IsJSDocTypeTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocTypeTag;
    }

    internal static bool IsJSDocTemplateTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocTemplateTag;
    }

    internal static bool IsJSDocTypedefTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocTypedefTag;
    }

    internal static bool IsJSDocUnknownTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocTag;
    }

    internal static bool IsJSDocPropertyTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocPropertyTag;
    }

    internal static bool IsJSDocImplementsTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocImplementsTag;
    }

    internal static bool IsJSDocSatisfiesTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocSatisfiesTag;
    }

    internal static bool IsJSDocThrowsTag(INode node)
    {
        return node.Kind == SyntaxKind.JSDocThrowsTag;
    }

    // Synthesized list

    /** @internal */
    internal static bool IsSyntaxList(INode node)
    {
        return node.Kind == SyntaxKind.SyntaxList;
    }
}
