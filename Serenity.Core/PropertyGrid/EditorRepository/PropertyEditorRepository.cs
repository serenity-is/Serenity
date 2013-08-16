using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    public static class PropertyEditorRepository
    {
        private static Dictionary<string, Type> _editorTypeByKey = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        static PropertyEditorRepository()
        {
            const string SettingsSuffix = "Settings";
            const string EditorSuffix = "Editor";

            foreach (var editorType in ExtensibilityHelper.GetTypesWithInterface(typeof(IPropertyEditor)))
            {
                var name = editorType.Name;

                if (name.EndsWith(SettingsSuffix, StringComparison.OrdinalIgnoreCase))
                    name = name.Substring(0, name.Length - SettingsSuffix.Length);

                if (name.EndsWith(EditorSuffix, StringComparison.OrdinalIgnoreCase))
                    name = name.Substring(0, name.Length - EditorSuffix.Length);

                if (_editorTypeByKey.ContainsKey(name))
                    continue; // ignore

                _editorTypeByKey[name] = editorType;
            }
        }

        public static Type GetEditorTypeByKey(string key)
        {
            Type editorType;
            if (!_editorTypeByKey.TryGetValue(key, out editorType))
                return null;

            return editorType;
        }
    }
}
