using System;
using System.Collections.Generic;

namespace Serenity
{
    public static class FormatterTypeRegistry
    {
        private static JsDictionary<string, Type> knownTypes = new JsDictionary<string, Type>();

        public static Type Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (!knownTypes.ContainsKey(key))
            {
                Type formatterType = null;
                foreach (var ns in Q.Config.RootNamespaces)
                {
                    var withoutSuffix = Type.GetType(ns + "." + key);
                    var withSuffix = Type.GetType(ns + "." + key + "Formatter");
                    formatterType = withoutSuffix ?? withSuffix;
                    if (withoutSuffix != null &&
                        withSuffix != null &&
                        !typeof(ISlickFormatter).IsAssignableFrom(withoutSuffix) &&
                        typeof(ISlickFormatter).IsAssignableFrom(withSuffix))
                    {
                        formatterType = withSuffix;
                    }

                    if (formatterType != null)
                        break;
                }

                if (formatterType != null)
                {
                    if (!typeof(ISlickFormatter).IsAssignableFrom(formatterType))
                        throw new Exception(String.Format("{0} formatter type doesn't implement " +
                            "ISlickFormatter interface", formatterType.FullName));

                    knownTypes[key] = formatterType;

                    return formatterType;
                }
                    
                throw new Exception(String.Format("Can't find {0} formatter type!", key));
            }
               
            return knownTypes[key];
        }
    }
}