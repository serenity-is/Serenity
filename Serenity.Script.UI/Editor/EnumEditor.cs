using jQueryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Enumeration"), OptionsType(typeof(EnumEditorOptions))]
    [Element("<input type=\"hidden\"/>")]
    public class EnumEditor : Select2Editor<EnumEditorOptions, Select2Item>, IStringValue
    {
        public EnumEditor(jQueryObject hidden, EnumEditorOptions opt)
            : base(hidden, opt)
        {
            UpdateItems();
        }

        protected virtual void UpdateItems()
        {
            ClearItems();

            var enumType = options.EnumType ?? EnumTypeRegistry.Get(options.EnumKey);

            var enumKey = options.EnumKey;
            if (enumKey == null && enumType != null)
            {
                var enumKeyAttr = enumType.GetCustomAttributes(typeof(EnumKeyAttribute), false);
                if (enumKeyAttr.Length > 0)
                    enumKey = enumKeyAttr[0].As<EnumKeyAttribute>().Value;
            }

            foreach (var x in Enum.GetValues(enumType))
            {
                var name = Enum.ToString(enumType, x.As<Enum>());
                AddItem(((int)x).ToString(), Q.TryGetText("Enums." + enumKey + "." + name) ?? name);
            }
        }
    }

    [Serializable, Reflectable]
    public class EnumEditorOptions
    {
        public EnumEditorOptions()
        {
        }

        [DisplayName("Enum Type Key")]
        public string EnumKey { get; set; }

        [DisplayName("Enum Type Key"), Hidden]
        public Type EnumType { get; set; }
    }
}