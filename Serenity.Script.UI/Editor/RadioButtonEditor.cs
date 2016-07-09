using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Radio Button")]
    [Element("<div/>")]
    public class RadioButtonEditor : Widget<RadioButtonEditorOptions>, IStringValue
    {
        static RadioButtonEditor()
        {
            Q.Prop(typeof(RadioButtonEditor), "value");
        }

        public RadioButtonEditor(jQueryObject input, RadioButtonEditorOptions opt)
            : base(input, opt)
        {
            if (options.EnumKey.IsEmptyOrNull() &&
                options.EnumType == null &&
                options.LookupKey.IsEmptyOrNull())
                return;

            if (!options.LookupKey.IsEmptyOrNull())
            {
                var lookup = Q.GetLookup<object>(options.LookupKey);
                foreach (dynamic item in lookup.Items)
                {
                    object textValue = ((dynamic)item)[lookup.TextField];
                    var text = textValue == null ? "" : textValue.ToString();
                    object idValue = item[lookup.IdField];
                    string id = idValue == null ? "" : idValue.ToString();
                    AddRadio(id, text);
                }
            }
            else
            {
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
                    AddRadio(((int)x).ToString(), Q.TryGetText("Enums." + enumKey + "." + name) ?? name);
                }
            }
        }

        private void AddRadio(string value, string text)
        {
            var label = J("<label/>").Text(text);
            J("<input type=\"radio\"/>")
                .Attribute("name", this.uniqueName)
                .Attribute("id", this.uniqueName + "_" + value)
                .Attribute("value", value)
                .PrependTo(label);
            label.AppendTo(this.element);
        }

        public string Value
        {
            get
            {
                return this.element.Find("input:checked").First().GetValue();
            }
            set
            {
                if (value != this.Value)
                {
                    var inputs = this.element.Find("input");
                    inputs.Filter(":checked").RemoveAttr("checked");
                    if (!string.IsNullOrEmpty(value))
                        inputs.Filter("[value=" + value + "]").Attribute("checked", "checked");
                }
            }
        }
    }

    [Serializable, Reflectable]
    public class RadioButtonEditorOptions
    {
        public RadioButtonEditorOptions()
        {
        }

        public String EnumKey { get; set; }
        public Type EnumType { get; set; }
        public String LookupKey { get; set; }
    }
}