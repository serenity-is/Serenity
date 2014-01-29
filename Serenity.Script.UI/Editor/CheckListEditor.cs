using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Checkbox'lı Liste", typeof(CheckListEditorOptions))]
    [Element("<ul/>")]
    public abstract class CheckListEditor : Widget<CheckListEditorOptions>, IGetEditValue, ISetEditValue
    {
        private jQueryObject list;

        public CheckListEditor(jQueryObject div, CheckListEditorOptions opt)
            : base(div, opt)
        {
            this.list = J("<ul/>").AppendTo(div);

            UpdateItems();
        }

        protected virtual List<CheckListItem> GetItems()
        {
            return options.Items ?? new List<CheckListItem>();
        }

        protected virtual void UpdateItems()
        {
            var items = GetItems();

            /*ClearItems();

            if (items.Count > 0)
            {
                bool isStrings = Script.TypeOf(items[0]) == "string";

                foreach (dynamic item in items)
                {
                    string key = isStrings ? item : item[0];
                    string text = isStrings ? item : item[1] ?? item[0];
                    AddItem(key, text);
                }
            }*/
        }

        void IGetEditValue.GetEditValue(PropertyItem property, dynamic target)
        {
        }

        void ISetEditValue.SetEditValue(dynamic source, PropertyItem property)
        {
        }
    }

    [Imported, Serializable]
    public class CheckListItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string ParentId { get; set; }
    }

    [Serializable, Reflectable]
    public class CheckListEditorOptions
    {
        public CheckListEditorOptions()
        {
            SelectAllOptionText = "Tümünü Seç";
            Items = new List<CheckListItem>();
        }

        [Hidden]
        public List<CheckListItem> Items { get; set; }
        [DisplayName("Tümünü Seç Metni")]
        public string SelectAllOptionText { get; set; }
    }
}