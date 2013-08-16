using System.Collections.Generic;
using NQuery;
using NQuery.Compilation;
using System;

namespace Serenity.Data
{
    public class T0ReferenceRemover : StandardVisitor
    {
        public override ExpressionNode VisitPropertyAccessExpression(PropertyAccessExpression expression)
        {
            var name = expression.Target as NameExpression;
            if (name != null)
            {
                var identifier = name.Name as Identifier;
                if (identifier != null)
                {
                    if (String.Compare(identifier.Text, "T0", StringComparison.OrdinalIgnoreCase) == 0)
                        return new NameExpression { Name = expression.Name };
                }
            }

            return base.VisitPropertyAccessExpression(expression);
        }

        public static string RemoveT0(string expression)
        {
            var parser = new Parser(ExceptionErrorProvider.Instance);
            var node = parser.ParseExpression(expression);
            var locator = new T0ReferenceRemover();
            return locator.Visit(node).ToString();
        }
    }
}