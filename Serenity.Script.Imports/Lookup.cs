using System.Html;
using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Class that holds a list of items and a dictionary of them by Id
    /// </summary>
    [ScriptName("Lookup"), ScriptNamespace("Q"), IncludeGenericArguments(false)]
    [Imported]
    public class Lookup<TItem>
    {
        public Lookup(LookupOptions<TItem> options,
            IEnumerable<TItem> items = null)
        {
        }

        public void Update(IEnumerable<TItem> newItems)
        {
        }

        [IntrinsicProperty]
        public string IdField
        {
            get { return null; }
        }

        [IntrinsicProperty]
        public string ParentIdField
        {
            get { return null; }
        }

        [IntrinsicProperty]
        public string TextField
        {
            get { return null; }
        }

        [IntrinsicProperty]
        public Func<TItem, string> TextFormatter
        {
            get { return null; }
        }

        [IntrinsicProperty]
        public JsDictionary<object, TItem> ItemById
        {
            [InlineCode("{this}.itemById")]
            get { return null; }
        }

        [IntrinsicProperty]
        public List<TItem> Items
        {
            get { return null; }
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