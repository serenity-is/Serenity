using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Radio button")]
    [Element("<input type=\"radio\"/>")]
    public class RadioButtonEditor : Widget<RadioButtonEditorOptions>, IStringValue
    {
        private string radioGroupName;

        static RadioButtonEditor()
        {
            Q.Prop(typeof(RadioButtonEditor), "value");
        }

        public RadioButtonEditor(jQueryObject input, RadioButtonEditorOptions opt)
            : base(input, opt)
        {
            input.RemoveClass("flexify");
            radioGroupName = input.GetAttribute("name");
            var radioClass = input.GetAttribute("class");
            var radioClassNoEditor = radioClass.ReplaceFirst("editor ", string.Empty);
            var radioId = input.GetAttribute("id");

            if (options.Labels == null)
                return;

            if (options.Values == null)
            {
                options.Values = new string[options.Labels.Length];
                int i = 0;
                foreach (string label in options.Labels)
                {
                    options.Values[i] = (i + 1).ToString();
                    i++;
                }
            }

            input.RemoveAttr("value");
            input.Attribute("value", options.Values[0]);
            input.Attribute("id", radioId + options.Values[0]);
            input.Attribute("title", options.Labels[0]);

            J("<label for=\"" + radioId + options.Values[0] + "\">" + options.Labels[0] + "</label>")
                .Attribute("title", options.Labels[0])
                .AddClass(radioClassNoEditor + "-label")
                .InsertBefore(input);

            int x = 0;
            foreach (string label in options.Labels)
            {
                if (x > 0)
                {
                    input = J("<div>").Attribute("class", "vx").InsertAfter(input);
                    input = J("<input type=\"radio\"/>")
                        .Attribute("title", label)
                        .Attribute("name", radioGroupName)
                        .Attribute("class", radioClass)
                        .Attribute("id", radioId + options.Values[x])
                        .Attribute("value", options.Values[x])
                        .InsertAfter(input);
                    J("<label for=\"" + input.GetAttribute("id") + "\">" + label + "</label>")
                        .Attribute("title", label)
                        .AddClass(radioClassNoEditor + "-label")
                        .InsertBefore(input);
                }
                x++;
            }
        }

        string IStringValue.Value
        {
            get
            {
                var radios = J("input[name=" + radioGroupName + "]");
                return radios.Filter(":checked").GetValue();
            }
            set
            {
                var radios = J("input[name=" + radioGroupName + "]");
                var val = radios.Filter("[value=" + value + "]").Property("checked", true);
            }
        }
    }

    [Serializable, Reflectable]
    public class RadioButtonEditorOptions
    {
        public RadioButtonEditorOptions()
        {
            Labels = null;
            Values = null;
        }

        public String[] Labels { get; set; }
        public String[] Values { get; set; }
    }
}