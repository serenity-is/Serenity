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
            var parser = new Parser(ExceptionErrorProvider.Instance);
            var node = parser.ParseExpression(expression);
            var locator = new JoinAliasLocator();
            locator.Visit(node);
            return locator.Aliases;
        }
    }
}