using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Element("<input type=\"hidden\"/>")]
    public abstract class LookupEditorBase<TOptions, TItem> : Select2Editor<TOptions, TItem>
        where TOptions: class, new()
    {
        private Lookup<TItem> lookup;

        public LookupEditorBase(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
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

            element.Select2("destroy");

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

        protected virtual IEnumerable<TItem> GetItems(Lookup<TItem> lookup)
        {
            return lookup.Items;
        }

        protected virtual void UpdateItems()
        {
            var lookup = GetLookup();

            ClearItems();

            var items = GetItems(lookup);
            foreach (dynamic item in items)
            {
                object textValue = (lookup.TextFormatter != null ? lookup.TextFormatter(item) : item[lookup.TextField]);
                var text = textValue == null ? "" : textValue.ToString();

                object idValue = item[lookup.IdField];
                string id = idValue == null ? "" : idValue.ToString();

                AddItem(id, text);
            }
        }

        protected void AddInplaceCreate(string title)
        {
            var self = this;

            J("<a><b/></a>").AddClass("inplace-create")
                .Attribute("title", title)
                .InsertAfter(this.element)
                .Click(e =>
                {
                    self.InplaceCreateClick(e);
                });

            this.Select2Container.Add(this.element).AddClass("has-inplace-create");
        }

        protected virtual void InplaceCreateClick(jQueryEvent e)
        {
        }
    }
}