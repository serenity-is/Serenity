using System.Collections.Generic;
using System.Linq;
using Sdcb.TypeScript.TsTypes;
using static Sdcb.TypeScript.TsParser.Factory;
using static Sdcb.TypeScript.TsParser.Scanner;
using static Sdcb.TypeScript.TsParser.Ts;

namespace Sdcb.TypeScript.TsParser
{
    public class Utilities
    {
        public static int GetFullWidth(INode node)
        {
            return (node.End ?? 0) - (node.Pos ?? 0);
        }


        public static INode ContainsParseError(INode node)
        {
            AggregateChildData(node);

            return (node.Flags & NodeFlags.ThisNodeOrAnySubNodesHasError) != 0 ? node : null;
        }


        public static void AggregateChildData(INode node)
        {
            if ((node.Flags & NodeFlags.HasAggregatedChildData) != 0)
            {
                var thisNodeOrAnySubNodesHasError = (node.Flags & NodeFlags.ThisNodeHasError) != 0 ||
                                                    ForEachChild(node, ContainsParseError) != null;
                if (thisNodeOrAnySubNodesHasError)
                    node.Flags |= NodeFlags.ThisNodeOrAnySubNodesHasError;


                node.Flags |= NodeFlags.HasAggregatedChildData;
            }
        }


        public static bool NodeIsMissing(INode node)
        {
            if (node == null)
                return true;


            return node.Pos == node.End && node.Pos >= 0 && node.Kind != SyntaxKind.EndOfFileToken;
        }


        public static string GetTextOfNodeFromSourceText(string sourceText, INode node)
        {
            if (NodeIsMissing(node))
                return "";


            return sourceText.substring(SkipTriviaM(sourceText, node.Pos ?? 0), node.End);
        }


        public static string EscapeIdentifier(string identifier)
        {
            return identifier.Length >= 2 && identifier.charCodeAt(0) == (int) CharacterCodes._ &&
                   identifier.charCodeAt(1) == (int) CharacterCodes._
                ? "_" + identifier
                : identifier;
        }


        public static List<CommentRange> GetLeadingCommentRangesOfNodeFromText(INode node, string text)
        {
            return GetLeadingCommentRanges(text, node.Pos ?? 0);
        }


        public static List<CommentRange> GetJsDocCommentRanges(INode node, string text)
        {
            var commentRanges = node.Kind == SyntaxKind.Parameter ||
                                node.Kind == SyntaxKind.TypeParameter ||
                                node.Kind == SyntaxKind.FunctionExpression ||
                                node.Kind == SyntaxKind.ArrowFunction
                ? GetTrailingCommentRanges(text, node.Pos ?? 0).Concat(GetLeadingCommentRanges(text, node.Pos ?? 0))
                : GetLeadingCommentRangesOfNodeFromText(node, text);
            if (commentRanges == null) commentRanges = new List<CommentRange>();
            return commentRanges.Where(comment =>
                    text.charCodeAt((comment.Pos ?? 0) + 1) == (int) CharacterCodes.Asterisk &&
                    text.charCodeAt((comment.Pos ?? 0) + 2) == (int) CharacterCodes.Asterisk &&
                    text.charCodeAt((comment.Pos ?? 0) + 3) != (int) CharacterCodes.Slash)
                .ToList();
        }


        public static bool IsKeyword(SyntaxKind token)
        {
            return SyntaxKind.FirstKeyword <= token && token <= SyntaxKind.LastKeyword;
        }


        public static bool IsTrivia(SyntaxKind token)
        {
            return SyntaxKind.FirstTriviaToken <= token && token <= SyntaxKind.LastTriviaToken;
        }


        public static bool IsModifierKind(SyntaxKind token)
        {
            switch (token)
            {
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.AsyncKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.DeclareKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.ExportKeyword:
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.ReadonlyKeyword:
                case SyntaxKind.StaticKeyword:

                    return true;
            }

            return false;
        }


        public static bool IsParameterDeclaration(IVariableLikeDeclaration node)
        {
            var root = GetRootDeclaration(node);

            return root.Kind == SyntaxKind.Parameter;
        }


        public static INode GetRootDeclaration(INode node)
        {
            while (node.Kind == SyntaxKind.BindingElement)
                node = node.Parent.Parent;

            return node;
        }


        public static bool HasModifiers(Node node)
        {
            return GetModifierFlags(node) != ModifierFlags.None;
        }


        public static bool HasModifier(INode node, ModifierFlags flags)
        {
            return (GetModifierFlags(node) & flags) != 0;
        }


        public static ModifierFlags GetModifierFlags(INode node)
        {
            if ((node.ModifierFlagsCache & ModifierFlags.HasComputedFlags) != 0)
                return node.ModifierFlagsCache & ~ModifierFlags.HasComputedFlags;
            var flags = ModifierFlags.None;
            if (node.Modifiers != null)
                foreach (var modifier in node.Modifiers)
                    flags |= ModifierToFlag(modifier.Kind);
            if (node.Flags.HasFlag(NodeFlags.NestedNamespace) || node.Kind == SyntaxKind.Identifier &&
                ((Identifier) node).IsInJsDocNamespace)
                flags |= ModifierFlags.Export;


            node.ModifierFlagsCache = flags | ModifierFlags.HasComputedFlags;

            return flags;
        }


        public static ModifierFlags ModifierToFlag(SyntaxKind token)
        {
            switch (token)
            {
                case SyntaxKind.StaticKeyword:
                    return ModifierFlags.Static;
                case SyntaxKind.PublicKeyword:
                    return ModifierFlags.Public;
                case SyntaxKind.ProtectedKeyword:
                    return ModifierFlags.Protected;
                case SyntaxKind.PrivateKeyword:
                    return ModifierFlags.Private;
                case SyntaxKind.AbstractKeyword:
                    return ModifierFlags.Abstract;
                case SyntaxKind.ExportKeyword:
                    return ModifierFlags.Export;
                case SyntaxKind.DeclareKeyword:
                    return ModifierFlags.Ambient;
                case SyntaxKind.ConstKeyword:
                    return ModifierFlags.Const;
                case SyntaxKind.DefaultKeyword:
                    return ModifierFlags.Default;
                case SyntaxKind.AsyncKeyword:
                    return ModifierFlags.Async;
                case SyntaxKind.ReadonlyKeyword:
                    return ModifierFlags.Readonly;
            }

            return ModifierFlags.None;
        }


        public static bool IsLogicalOperator(SyntaxKind token)
        {
            return token == SyntaxKind.BarBarToken
                   || token == SyntaxKind.AmpersandAmpersandToken
                   || token == SyntaxKind.ExclamationToken;
        }


        public static bool IsAssignmentOperator(SyntaxKind token)
        {
            return token >= SyntaxKind.FirstAssignment && token <= SyntaxKind.LastAssignment;
        }


        public static bool IsLeftHandSideExpressionKind(SyntaxKind kind)
        {
            return kind == SyntaxKind.PropertyAccessExpression
                   || kind == SyntaxKind.ElementAccessExpression
                   || kind == SyntaxKind.NewExpression
                   || kind == SyntaxKind.CallExpression
                   || kind == SyntaxKind.JsxElement
                   || kind == SyntaxKind.JsxSelfClosingElement
                   || kind == SyntaxKind.TaggedTemplateExpression
                   || kind == SyntaxKind.ArrayLiteralExpression
                   || kind == SyntaxKind.ParenthesizedExpression
                   || kind == SyntaxKind.ObjectLiteralExpression
                   || kind == SyntaxKind.ClassExpression
                   || kind == SyntaxKind.FunctionExpression
                   || kind == SyntaxKind.Identifier
                   || kind == SyntaxKind.RegularExpressionLiteral
                   || kind == SyntaxKind.NumericLiteral
                   || kind == SyntaxKind.StringLiteral
                   || kind == SyntaxKind.NoSubstitutionTemplateLiteral
                   || kind == SyntaxKind.TemplateExpression
                   || kind == SyntaxKind.FalseKeyword
                   || kind == SyntaxKind.NullKeyword
                   || kind == SyntaxKind.ThisKeyword
                   || kind == SyntaxKind.TrueKeyword
                   || kind == SyntaxKind.SuperKeyword
                   || kind == SyntaxKind.NonNullExpression
                   || kind == SyntaxKind.MetaProperty;
        }


        public static bool IsLeftHandSideExpression(IExpression node)
        {
            return IsLeftHandSideExpressionKind(SkipPartiallyEmittedExpressions(node).Kind);
        }
    }
}