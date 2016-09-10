using System;
using System.Collections.Generic;

namespace Serenity
{
    public static class DialogTypeRegistry
    {
        private static JsDictionary<string, Type> knownTypes = new JsDictionary<string, Type>();

        private static Type Search(string typeName)
        {
            var dialogType = Type.GetType(typeName);
            if (dialogType != null && typeof(IDialog).IsAssignableFrom(dialogType))
                return dialogType;

            foreach (var ns in Q.Config.RootNamespaces)
            {
                dialogType = Type.GetType(ns + "." + typeName);
                if (dialogType != null && typeof(IDialog).IsAssignableFrom(dialogType))
                    return dialogType;
            }

            return null;
        }

        public static Type Get(string key)
        {
            if (!knownTypes.ContainsKey(key))
            {
                var typeName = key;
                var dialogType = Search(typeName);

                if (dialogType == null && !key.EndsWith("Dialog"))
                {
                    typeName = key + "Dialog";
                    dialogType = Search(typeName);
                }

                if (dialogType == null)
                    throw new Exception(typeName + " dialog class is not found!");

                knownTypes[key] = dialogType;
            }

            return knownTypes[key];
        }
    }
}