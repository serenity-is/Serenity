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

        public static Type TryGet(string key)
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
                    return null;

                knownTypes[key] = dialogType;
                return dialogType;
            }

            return knownTypes[key];
        }

        public static Type Get(string key)
        {
            var type = TryGet(key);
            if (type == null)
            {
                var message = key + " dialog class is not found! Make sure there is a dialog class with this name, " +
                    "it is under your project root namespace, and your namespace parts start with capital letters, " +
                    "e.g. MyProject.Pascal.Cased namespace. If you got this error from an editor with InplaceAdd option " +
                    "check that lookup key and dialog type name matches (case sensitive, excluding Dialog suffix). " + 
                    "You need to change lookup key or specify DialogType property in LookupEditor attribute if that's not the case.";

                Q.NotifyError(message);
                throw new Exception(message);
            }

            return type;
        }
    }
}