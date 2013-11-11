using System.Collections.Generic;
using NQuery;
using NQuery.Compilation;
using System;

namespace Serenity.Data
{
    public class JoinAliasLocator : StandardVisitor
    {
        private HashSet<string> aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public override ExpressionNode VisitPropertyAccessExpression(PropertyAccessExpression expression)
        {
            var name = expression.Target as NameExpression;
            if (name != null)
            {
                var identifier = name.Name as Identifier;
                if (identifier != null)
                {
                    aliases.Add(identifier.Text);
                }
            }

            return base.VisitPropertyAccessExpression(expression);
        }

        public HashSet<string> Aliases
        {
            get { return aliases; }
        }

        public static HashSet<string> Locate(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            HashSet<string> aliases = null;
            EnumerateAliases(expression, s =>
            {
                aliases = aliases ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                aliases.Add(s);
            });

            return aliases;
        }

        public static HashSet<string> LocateOptimized(string expression, out string singleAlias)
        {
            //var parser = new Parser(ExceptionErrorProvider.Instance);
            //var node = parser.ParseExpression(expression);
            //var locator = new JoinAliasLocator();
            //locator.Visit(node);
            if (expression == null)
                throw new ArgumentNullException("expression");

            HashSet<string> aliases = null;
            string alias = null;
            EnumerateAliases(expression, s =>
            {
                if (aliases == null && (alias == null || (aliases == null && alias == s)))
                    alias = s;
                else
                {
                    alias = null;
                    aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    aliases.Add(alias);
                    aliases.Add(s);
                }
            });

            singleAlias = alias;
            return aliases;
        }

        public static bool EnumerateAliases(string expression, Action<string> alias)
        {
            bool inQuote = false;
            int startIdent = -1;
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];

                if (inQuote)
                {
                    if (c == '\'')
                    {
                        inQuote = false;
                        continue;
                    }
                }
                else
                {
                    if (c == '\'')
                    {
                        inQuote = true;
                        startIdent = -1;
                    }
                    else if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        if (startIdent < 0)
                            startIdent = i;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                    }
                    else if (c == '.')
                    {
                        if (startIdent >= 0 && startIdent < i)
                        {
                            alias(expression.Substring(startIdent, i - startIdent));
                        }
                        startIdent = -1;
                    }
                    else
                        startIdent = -1;
                }
            }

            return true;
        }
    }
}