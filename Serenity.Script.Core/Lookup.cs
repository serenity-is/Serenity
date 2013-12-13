using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Class that holds a list of items and a dictionary of them by Id
    /// </summary>
    [ScriptName("Q$Lookup"), IgnoreNamespace, IncludeGenericArguments(false)]
    public class Lookup<TItem>
    {
        private List<TItem> items;
        private JsDictionary<object, TItem> itemById;
        private LookupOptions<TItem> options;

        public Lookup(LookupOptions<TItem> options,
            IEnumerable<TItem> items = null)
        {
            this.items = new List<TItem>();
            this.itemById = new JsDictionary<object, TItem>();
            this.options = options ?? new LookupOptions<TItem>();

            if (items != null)
                Update(items);
        }

        public void Update(IEnumerable<TItem> newItems)
        {
            items = new List<TItem>();
            itemById = new JsDictionary<object, TItem>();

            if (newItems != null)
                items.AddRange(newItems);

            var idField = options.IdField;

            if (!idField.IsEmptyOrNull())
            {
                for (var i = 0; i < this.items.Count; i++)
                {
                    var r = this.items[i];
                    var v = Type.GetField(r, idField) ?? Type.GetProperty(r, idField);
                    if (v != null)
                        this.itemById[v] = r;
                }
            }

            RaiseChange();
        }

        public string IdField
        {
            get { return options.IdField; }
        }

        public string ParentIdField
        {
            get { return options.ParentIdField; }
        }

        public string TextField
        {
            get { return options.TextField; }
        }

        public Func<TItem, string> TextFormatter
        {
            get { return options.TextFormatter; }
        }

        public void RaiseChange()
        {
            jQuery.FromObject(this).Trigger("change");
        }

        public JsDictionary<object, TItem> ItemById
        {
            get { return itemById; }
        }

        public List<TItem> Items
        {
            get { return items; }
        }
    }

    [Imported, Serializable, IncludeGenericArguments(false)]
    public class LookupOptions<TItem>
    {
        public string IdField { get; set; }
        public string TextField { get; set; }
        public Func<TItem, string> TextFormatter { get; set; }
        public string ParentIdField { get; set; }
    }
}