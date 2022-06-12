using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Serenity.SourceGenerator;

public static class GeneratorExtensions
{
    public static string GetFullName(this ISymbol symbol)
    {
        var ns = GetNamespace(symbol);
        if (string.IsNullOrEmpty(ns))
            return symbol.Name;

        return ns + "." + symbol.Name;
    }

    public static string GetNamespace(this ISymbol symbol)
    {
        if (symbol.ContainingNamespace == null ||
            string.IsNullOrEmpty(symbol.ContainingNamespace.Name))
            return null;

        string restOfResult = symbol.ContainingNamespace.GetNamespace();
        string result = symbol.ContainingNamespace.Name;

        if (restOfResult != null)
            result = restOfResult + '.' + result;

        return result;
    }

    public static bool InheritsFrom(this ITypeSymbol symbol, ITypeSymbol type)
    {
        var baseType = symbol.BaseType;

        while (baseType != null)
        {
            if (type.Equals(baseType, SymbolEqualityComparer.Default))
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }

    public static string ToPascalCase(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));

        if (str.Length > 0 && !char.IsUpper(str[0]))
            return char.ToUpperInvariant(str[0]) + str.Substring(1);

        return str;
    }

    public static string ToTypeRef(this ITypeSymbol type, CodeWriter cw,
        bool baseType = false)
    {
        return ToTypeRef(type, cw.CurrentNamespace, cw.Using, baseType);
    }

    public static string ToTypeRef(this ITypeSymbol type,
        string nsCode = null, Func<string, bool> uses = null, bool baseType = false)
    {
        var ns = type.GetNamespace();

        if (type is IArrayTypeSymbol arrayType)
        {
            if (baseType)
                return ShortenNS(ns, nsCode, uses) + "Array";

            var name = ToTypeRef(arrayType.ElementType, nsCode, uses, baseType: false);

            if (arrayType.Rank > 1)
                return name + string.Join("", Enumerable.Range(0, arrayType.Rank).Select(x => "[]"));

            return name + "[]";
        }

        if (type is INamedTypeSymbol namedType)
        {
            if (namedType.IsGenericType &&
                namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                var name = ToTypeRef(namedType.TypeArguments[0], nsCode, uses, baseType: false);

                if (baseType)
                    return ShortenNS(ns, nsCode, uses) + "Nullable<" +
                        name + ">";

                return name + "?";
            }

            if (namedType.IsGenericType)
            {
                var sb = new StringBuilder();
                sb.Append(ShortenNS(ns, nsCode, uses));
                sb.Append(namedType.Name);
                sb.Append('<');

                for (var i = 0; i < namedType.TypeArguments.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");

                    var arg = namedType.TypeArguments[i];

                    sb.Append(ToTypeRef(arg, nsCode, uses, baseType: false));
                }

                sb.Append('>');
                return sb.ToString();
            }
        }

        if (!baseType &&
            type.SpecialType >= SpecialType.System_Boolean &&
            type.SpecialType <= SpecialType.System_String)
        {
            var keyword = ToCSKeyword(type.SpecialType);
            if (keyword != null)
                return keyword;
        }

        return ShortenNS(ns, nsCode, uses) + type.Name;
    }

    public static string ShortenNS(this CodeWriter cw, string ns)
    {
        return ShortenNS(ns, cw.CurrentNamespace, cw.Using);
    }

    public static string ShortenNS(this string ns, string nsCode,
        Func<string, bool> uses = null)
    {
        if (string.IsNullOrEmpty(ns) ||
            (!string.IsNullOrEmpty(nsCode) &&
             (nsCode == ns ||
              nsCode.StartsWith(ns + ".", StringComparison.Ordinal))) ||
            (uses != null && uses(ns)))
            return string.Empty;

        if (!string.IsNullOrEmpty(nsCode))
        {
            var idx = nsCode.IndexOf('.');
            if (idx >= 0 && ns.StartsWith(nsCode.Substring(0, idx + 1), StringComparison.Ordinal))
                return ns.Substring(idx + 1) + ".";
        }

        return ns + ".";
    }

    public static string ToCSKeyword(SpecialType type)
    {
        return type switch
        {
            SpecialType.System_String => "string",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Byte => "byte",
            SpecialType.System_Char => "char",
            SpecialType.System_Decimal => "decimal",
            SpecialType.System_Double => "double",
            SpecialType.System_Int16 => "short",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Object => "object",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_Single => "float",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_UInt64 => "ulong",
            _ => null
        };
    }

    public static string ToCSharpValue(this object value)
    {
        if (value == null)
            return "null";

        var invariant = CultureInfo.InvariantCulture;

        return value switch
        {
            string s => s != null && s.Contains('\n') ?
                SyntaxFactory.Literal("@\"" + s.Replace("\"", "\"\"") + "\"", s).ToFullString() :
                SyntaxFactory.Literal(s).ToFullString(),
            int i => i.ToString(invariant),
            byte i => i.ToString(invariant),
            sbyte i => i.ToString(invariant),
            ushort i => i.ToString(invariant),
            short i => i.ToString(invariant),
            long l => SyntaxFactory.Literal(l).ToFullString(),
            bool b => b ? "true" : "false",
            uint u => SyntaxFactory.Literal(u).ToFullString(),
            ulong u => SyntaxFactory.Literal(u).ToFullString(),
            float f => SyntaxFactory.Literal(f).ToFullString(),
            double d => SyntaxFactory.Literal(d).ToFullString(),
            decimal m => SyntaxFactory.Literal(m).ToFullString(),
            char c => SyntaxFactory.Literal(c).ToFullString(),
            _ => null
        };
    }

    public static HashSet<string> GetGlobalUsings(this Compilation compilation)
    {
        var globalUsings = new HashSet<string>(StringComparer.Ordinal);

        foreach (var gusTree in compilation.SyntaxTrees)
        {
            if (!gusTree.TryGetRoot(out var syntaxRoot))
                continue;

            foreach (var ud in syntaxRoot.ChildNodes().OfType<UsingDirectiveSyntax>())
                if (ud.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
                    globalUsings.Add(ud.Name.ToFullString());
        }

        return globalUsings;
    }
}