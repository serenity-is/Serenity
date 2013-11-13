using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Açılır Liste", typeof(SelectEditorOptions))]
    [Element("<select/>")]
    public abstract class SelectEditor : Widget<SelectEditorOptions>, IStringValue
    {
        public SelectEditor(jQueryObject select, SelectEditorOptions opt)
            : base(select, opt)
        {
            select.Select2();

            UpdateItems();
        }

        protected override SelectEditorOptions GetDefaults()
        {
            return new SelectEditorOptions
            {
                EmptyOptionText = "--seçiniz--",
                Items = new List<object>()
            };
        }

        protected virtual List<object> GetItems()
        {
            return options.Items ?? new List<object>();
        }

        protected virtual string EmptyItemText()
        {
            return options.EmptyOptionText;
        }

        protected virtual void UpdateItems()
        {
            var items = GetItems();
            
            Q.ClearOptions(element);

            var emptyItemText = EmptyItemText();
            if (emptyItemText != null)
                Q.AddOption(element, "", emptyItemText);

            if (items.Count > 0)
            {
                bool isStrings = Type.GetScriptType(items[0]) == "string";

                foreach (dynamic item in items)
                {
                    string key = isStrings ? item : item[0];
                    string text = isStrings ? item : item[1] ?? item[0];
                    Q.AddOption(element, key, text);
                }
            }
        }

        public string Value
        {
            get 
            { 
                return this.element.GetValue(); 
            }
            set
            {
                if (value != Value)
                    this.element.Value(value).TriggerHandler("change");
            }
        }
    }

    [Serializable, Reflectable]
    public class SelectEditorOptions
    {
        [Hidden]
        public List<object> Items { get; set; }
        [DisplayName("Boş Eleman Metni")]
        public string EmptyOptionText { get; set; }
    }
}