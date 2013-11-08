using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Element("<select/>")]
    public abstract class LookupEditorBase<TOptions, TItem> : Widget<TOptions>, IStringValue
        where TOptions: class, new()
    {
        private Lookup<TItem> lookup;

        public LookupEditorBase(jQueryObject select, TOptions opt)
            : base(select, opt)
        {
            select.Select2();

            UpdateItems();

            lookup = GetLookup();
            var self = this;
            if (lookup != null)
                jQuery.FromObject(lookup).As<dynamic>().bind("change." + this.uniqueName, new jQueryEventHandler(e => {
                    self.UpdateItems();
                }));
        }

        public override void Destroy()
        {
            if (lookup != null)
            {
                jQuery.FromObject(lookup).Unbind("change." + this.uniqueName);
                lookup = null;
            }

            base.Destroy();
        }

        protected virtual Lookup<TItem> GetLookup()
        {
            var key = this.GetType().FullName;
            var idx = key.IndexOf(".");
            if (idx >= 0)
                key = key.Substring(idx + 1);

            if (key.EndsWith("Editor"))
                key = key.Substring(0, key.Length - 6);

            return Q.GetLookup<TItem>(key);
        }

        protected virtual string EmptyItemText()
        {
            return "--seçiniz--";
        }

        protected virtual IEnumerable<TItem> GetItems(Lookup<TItem> lookup)
        {
            return lookup.Items;
        }

        protected virtual void UpdateItems()
        {
            var lookup = GetLookup();
            Q.ClearOptions(element);

            var emptyItemText = EmptyItemText();
            if (emptyItemText != null)
                Q.AddOption(element, "", emptyItemText);

            var items = GetItems(lookup);
            foreach (dynamic item in items)
            {
                object textValue = (lookup.TextFormatter != null ? lookup.TextFormatter(item) : item[lookup.TextField]);
                var text = textValue == null ? "" : textValue.ToString();

                object idValue = item[lookup.IdField];
                string id = idValue == null ? "" : idValue.ToString();

                Q.AddOption(element, id, text);
            }
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set 
            {
                if (value != Value)
                    this.element.Value(value).TriggerHandler("change");
            }
        }
    }
}