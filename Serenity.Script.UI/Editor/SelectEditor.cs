using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Açılır Liste", typeof(SelectEditorOptions))]
    [Element("<input type=\"hidden\"/>")]
    public abstract class SelectEditor : Select2Editor<SelectEditorOptions, Select2Item>, IStringValue
    {
        public SelectEditor(jQueryObject hidden, SelectEditorOptions opt)
            : base(hidden, opt)
        {
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

        protected override string EmptyItemText()
        {
            return options.EmptyOptionText;
        }

        protected virtual void UpdateItems()
        {
            var items = GetItems();
            
            ClearItems();

            if (items.Count > 0)
            {
                bool isStrings = Type.GetScriptType(items[0]) == "string";

                foreach (dynamic item in items)
                {
                    string key = isStrings ? item : item[0];
                    string text = isStrings ? item : item[1] ?? item[0];
                    AddItem(key, text);
                }
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