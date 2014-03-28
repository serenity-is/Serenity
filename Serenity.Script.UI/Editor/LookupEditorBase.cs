using System.Html;
using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Element("<input type=\"hidden\"/>")]
    public abstract class LookupEditorBase<TOptions, TItem> : Select2Editor<TOptions, TItem>
        where TOptions: class, new()
        where TItem: class, new()
    {
        public LookupEditorBase(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
            UpdateItems();

            string lookupKey = GetLookupKey();
            var self = this;
            jQuery.FromObject(Document.Body).As<dynamic>().bind("scriptdatachange." + this.uniqueName, new Action<jQueryEvent, string>((e, s) => {
                if (s == lookupKey)
                    self.UpdateItems();
            }));
        }

        public override void Destroy()
        {
            jQuery.FromObject(Document.Body).As<dynamic>().unbind("scriptdatachange." + this.uniqueName);

            element.Select2("destroy");

            base.Destroy();
        }

        protected virtual string GetLookupKey()
        {
            var key = this.GetType().FullName;
            var idx = key.IndexOf(".");
            if (idx >= 0)
                key = key.Substring(idx + 1);

            if (key.EndsWith("Editor"))
                key = key.Substring(0, key.Length - 6);

            return key;
        }

        protected virtual Lookup<TItem> GetLookup()
        {
            return Q.GetLookup<TItem>(GetLookupKey());
        }

        protected virtual IEnumerable<TItem> GetItems(Lookup<TItem> lookup)
        {
            return lookup.Items;
        }

        protected virtual string GetItemText(TItem item, Lookup<TItem> lookup)
        {
            object textValue = (lookup.TextFormatter != null ? lookup.TextFormatter(item) : ((dynamic)item)[lookup.TextField]);
            return textValue == null ? "" : textValue.ToString();
        }

        protected virtual void UpdateItems()
        {
            var lookup = GetLookup();

            ClearItems();

            var items = GetItems(lookup);
            foreach (dynamic item in items)
            {
                var text = GetItemText(item, lookup);

                object idValue = item[lookup.IdField];
                string id = idValue == null ? "" : idValue.ToString();

                AddItem(id, text, item);
            }
        }
    }

    public abstract class LookupEditorBase<TItem> : LookupEditorBase<object, TItem>
        where TItem: class, new()
    {
        public LookupEditorBase(jQueryObject hidden)
            : base(hidden, null)
        {
        }
    }
}