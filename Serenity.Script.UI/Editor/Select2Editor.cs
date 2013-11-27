using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class Select2Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    [Element("<input type=\"hidden\"/>")]
    public abstract class Select2Editor<TOptions, TItem> : Widget<TOptions>, IStringValue
        where TOptions : class, new()
    {
        protected List<Select2Item> items;
        protected int pageSize = 100;

        [InlineCode("Select2.util.stripDiacritics({input})")]
        private static string StripDiacritics(string input)
        {
            return null;
        }

        public Select2Editor(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
            items = new List<Select2Item>();

            var emptyItemText = EmptyItemText();
            if (emptyItemText != null)
                hidden.Attribute("placeholder", emptyItemText);

            hidden.Select2(GetSelect2Options());

            hidden.Attribute("type", "text"); // jquery validate to work
        }

        protected virtual string EmptyItemText()
        {
            return "--seçiniz--";
        }

        protected virtual Select2Options GetSelect2Options()
        {
            var emptyItemText = EmptyItemText();

            return new Select2Options
            {
                Data = items,
                MinimumResultsForSearch = 10,
                PlaceHolder = !emptyItemText.IsEmptyOrNull() ? emptyItemText : null,
                AllowClear = emptyItemText != null,
                Query = delegate(Select2QueryOptions query)
                {
                    var term = query.Term.IsEmptyOrNull() ? "" : StripDiacritics(query.Term ?? "").ToUpperCase();
                    var results = this.items.Filter(item =>
                    {
                        return (term == null ||
                            StripDiacritics(item.Text ?? "").ToUpperCase().StartsWith(term));
                    });

                    results.AddRange(this.items.Filter(item =>
                    {
                        return term != null &&
                            !StripDiacritics(item.Text ?? "").ToUpperCase().StartsWith(term) &&
                            StripDiacritics(item.Text ?? "").ToUpperCase().IndexOf(term) >= 0;
                    }));

                    
                    query.Callback(new Select2Result
                    {
                        Results = results.Slice((query.Page - 1) * pageSize, query.Page * pageSize),
                        More = results.Count >= query.Page * pageSize
                    });
                }
            };
        }

        protected void ClearItems()
        {
            this.items.Clear();
        }

        protected void AddItem(string key, string text)
        {
            this.items.Add(new Select2Item
            {
                Id = key,
                Text = text
            });
        }

        public string Value
        {
            get
            {
                return this.element.Select2Get("val") as string;
            }
            set
            {
                if (value != Value)
                    this.element.Select2("val", value);
            }
        }
    }
}