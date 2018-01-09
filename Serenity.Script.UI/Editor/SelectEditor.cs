using jQueryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    [Editor, DisplayName("Açılır Liste"), OptionsType(typeof(SelectEditorOptions))]
    [Element("<input type=\"hidden\"/>")]
    public class SelectEditor : Select2Editor<SelectEditorOptions, Select2Item>, IStringValue
    {
        public SelectEditor(jQueryObject hidden, SelectEditorOptions opt)
            : base(hidden, opt)
        {
            UpdateItems();
        }

        protected virtual List<object> GetItems()
        {
            return options.Items ?? new List<object>();
        }

        protected override string EmptyItemText()
        {
            if (!string.IsNullOrEmpty(options.EmptyOptionText))
                return options.EmptyOptionText;

            return base.EmptyItemText();
        }

        protected virtual void UpdateItems()
        {
            var items = GetItems();
            
            ClearItems();

            if (items.Count > 0)
            {
                bool isStrings = Script.TypeOf(items[0]) == "string";

                foreach (dynamic item in items)
                {
                    string key = isStrings ? item : item[0];
                    string text = isStrings ? item : item[1] ?? item[0];
                    AddItem(key, text, item);
                }
            }
        }
    }

    [Serializable, Reflectable]
    public class SelectEditorOptions
    {
        public SelectEditorOptions()
        {
            Items = new List<object>();
        }

        [Hidden]
        public List<object> Items { get; set; }
        [DisplayName("Boş Eleman Metni")]
        public string EmptyOptionText { get; set; }
    }
}