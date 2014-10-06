using System;
using System.Collections.Generic;

namespace Serenity
{
    public static class DialogTypeRegistry
    {
        private static JsDictionary<string, Type> knownTypes = new JsDictionary<string, Type>();

        public static Type Get(string key)
        {
            if (!knownTypes.ContainsKey(key))
            {
                string typeName = key + "Dialog";

                Type dialogType = null;
                foreach (var ns in Q.Config.RootNamespaces)
                {
                    dialogType = Type.GetType(ns + "." + typeName);
                    if (dialogType != null && typeof(IDialog).IsAssignableFrom(dialogType))
                        break;
                }

                if (dialogType == null)
                    throw new Exception(typeName + " dialog class is not found!");

                knownTypes[key] = dialogType;
            }

            return knownTypes[key];
        }
    }
}