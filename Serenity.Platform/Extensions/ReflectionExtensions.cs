using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Serenity.Data
{
    public static partial class ReflectionExtensions
    {
        public static string MemberName<T, R>(this T obj, Expression<Func<T, R>> expr)
        {
            var node = expr.Body as MemberExpression;
            if (object.ReferenceEquals(null, node))
                throw new InvalidOperationException("Expression must be of member access");
            return node.Member.Name;
        }
    }

    public static class MembersOf<T>
    {
        public static string GetName<R>(Expression<Func<T, R>> expr)
        {
            var node = expr.Body as MemberExpression;
            if (object.ReferenceEquals(null, node))
                throw new InvalidOperationException("Expression must be of member access");
            return node.Member.Name;
        }
    }
}
