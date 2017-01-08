using System;
using System.Linq;
using System.Reflection;

namespace Serenity
{
    public static class ReflectionExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member, bool inherit = false) where TAttribute : Attribute
        {
            var attrs = member.GetCustomAttributes(typeof(TAttribute), inherit);
            if (!attrs.Any())
                return null;

            if (attrs.Count() >  1)
                throw new InvalidOperationException(String.Format("{0} has more than 1 of {1} attribute", member.Name, typeof(TAttribute).Name));

            return (TAttribute)attrs.First();
        }
    }
}
