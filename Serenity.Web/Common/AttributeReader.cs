using System;
using System.Linq;
using System.Reflection;

namespace Serenity.Web.Common
{
    internal class AttributeReader
    {
        public static T GetAttributeWithAssemblyVersionChecking<T>(MethodInfo method) where T : Attribute
        {
            var attribute = method.GetCustomAttributes<T>().FirstOrDefault();
#if ASPNETCORE
            if (attribute == null)
            {
                var requestedAttribute = typeof(T);
                var candidate = method.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType().FullName == requestedAttribute.FullName);
                if (candidate != null)
                    throw new Exception(GetExceptionMessage<T>("method " + method, candidate));
            }
#endif
            return attribute;
        }

        public static T GetAttributeWithAssemblyVersionChecking<T>(Type type) where T : Attribute
        {
            var attribute = type.GetCustomAttributes<T>().FirstOrDefault();
#if ASPNETCORE
            if (attribute == null)
            {
                var requestedAttribute = typeof(T);
                var candidate = type.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType().FullName == requestedAttribute.FullName);
                if (candidate != null)
                    throw new Exception(GetExceptionMessage<T>("type " + type, candidate));
            }
#endif
            return attribute;
        }

        private static string GetExceptionMessage<T>(string member, object candidate)
        {
            var requestedAttribute = typeof(T);
            return string.Format("controller {0} is decorated with {1} from {2} but expected is {1} from {3}",
                member, requestedAttribute, candidate.GetType().GetTypeInfo().Assembly,
                requestedAttribute.GetTypeInfo().Assembly);
        }
    }
}