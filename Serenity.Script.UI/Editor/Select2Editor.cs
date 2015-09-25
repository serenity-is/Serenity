using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Serenity
{
    [Imported, Serializable]
    public class Select2Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public object Source { get; set; }
        public bool Disabled { get; set; }
    }

    [Element("<input type=\"hidden\"/>")]
    public abstract class Select2Editor<TOptions, TItem> : Widget<TOptions>, IStringValue
        where TOptions : class, new()
        where TItem: class
    {
        protected List<Select2Item> items;
        protected int pageSize = 100;
        protected string lastCreateTerm;

        public Select2Editor(jQueryObject hidden, TOptions opt)
            : base(hidden, opt)
        {
            items = new List<Select2Item>();

            var emptyItemText = EmptyItemText();
            if (emptyItemText != null)
                hidden.Attribute("placeholder", emptyItemText);

            hidden.Select2(GetSelect2Options());
            
            hidden.Attribute("type", "text"); // jquery validate to work
            hidden.Bind2("change." + this.uniqueName, (e, x) =>
            {
                if (e.HasOriginalEvent() || Q.IsFalse(x))
                {
                    if (hidden.GetValidator() != null)
                        hidden.Valid();
                }
            });
        }

        protected virtual string EmptyItemText()
        {
            return element.GetAttribute("placeholder") ?? Texts.Controls.SelectEditor.EmptyItemText;
        }

        protected virtual Select2Options GetSelect2Options()
        {
            var emptyItemText = EmptyItemText();

            return new Select2Options
            {
                Data = items,
                PlaceHolder = !emptyItemText.IsEmptyOrNull() ? emptyItemText : null,
                AllowClear = emptyItemText != null,
                CreateSearchChoicePosition = "bottom",
                Query = delegate(Select2QueryOptions query)
                {
                    var term = query.Term.IsEmptyOrNull() ? "" : Q.Externals.StripDiacritics(query.Term ?? "").ToUpperCase();
                    var results = this.items.Filter(item =>
                    {
                        return (term == null ||
                            Q.Externals.StripDiacritics(item.Text ?? "").ToUpperCase().StartsWith(term));
                    });

                    results.AddRange(this.items.Filter(item =>
                    {
                        return term != null &&
                            !Q.Externals.StripDiacritics(item.Text ?? "").ToUpperCase().StartsWith(term) &&
                            Q.Externals.StripDiacritics(item.Text ?? "").ToUpperCase().IndexOf(term) >= 0;
                    }));

                    query.Callback(new Select2Result
                    {
                        Results = results.Slice((query.Page - 1) * pageSize, query.Page * pageSize),
                        More = results.Count >= query.Page * pageSize
                    });
                },
                InitSelection = delegate(jQueryObject element, Action<object> callback)
                {
                    var val = element.GetValue();
                    Select2Item item = null;
                    for (var i = 0; i < this.items.Count; i++)
                    {
                        var x = items[i];
                        if (x.Id == val)
                        {
                            item = x;
                            break;
                        }
                    }

                    callback(item);
                }
            };
        }

        protected void ClearItems()
        {
            this.items.Clear();
        }

        protected void AddItem(string key, string text, TItem source = null, bool disabled = false)
        {
            this.items.Add(new Select2Item
            {
                Id = key,
                Text = text,
                Source = source,
                Disabled = disabled
            });
        }

        protected void AddInplaceCreate(string addTitle = null, string editTitle = null)
        {
            var self = this;

            addTitle = addTitle ?? Texts.Controls.SelectEditor.InplaceAdd;
            editTitle = editTitle ?? Texts.Controls.SelectEditor.InplaceEdit;

            var inplaceButton = J("<a><b/></a>").AddClass("inplace-button inplace-create")
                .Attribute("title", addTitle)
                .InsertAfter(this.element)
                .Click(e =>
                {
                    self.InplaceCreateClick(e);
                });

            this.Select2Container.Add(this.element).AddClass("has-inplace-button");

            this.Change(e =>
            {
                bool isNew = this.Value.IsEmptyOrNull();
                inplaceButton
                    .Attribute("title", isNew ? addTitle : editTitle)
                    .ToggleClass("edit", !isNew);
            });

            this.ChangeSelect2(e =>
            {
                if (this.Value == Int32.MinValue.ToString())
                {
                    this.Value = null;
                    InplaceCreateClick(null);
                }
            });
        }

        protected virtual void InplaceCreateClick(jQueryEvent e)
        {
        }

        public Func<string, object> GetCreateSearchChoice(
            Func<TItem, string> getName = null)
        {
            return s =>
            {
                s = (Q.Externals.StripDiacritics(s) ?? "").ToLower();
                lastCreateTerm = s;

                if (s.IsTrimmedEmpty())
                    return null;

                if (this.Items.Any(x =>
                {
                    var text = getName != null ? getName(x.Source.As<TItem>()) : x.Text;
                    return Q.Externals.StripDiacritics(text ?? "").ToLower() == s;
                }))
                {
                    return null;
                }

                if (!this.Items.Any(x => (Q.Externals.StripDiacritics(x.Text) ?? "").ToLower().Contains(s)))
                {
                    return new Select2Item
                    {
                        Id = Int32.MinValue.ToString(),
                        Text = Texts.Controls.SelectEditor.NoResultsClickToDefine
                    };
                }

                return new Select2Item
                {
                    Id = Int32.MinValue.ToString(),
                    Text = Texts.Controls.SelectEditor.ClickToDefine
                };
            };
        }

        protected jQueryObject Select2Container
        {
            get { return this.element.PrevAll(".select2-container"); }
        }

        public List<Select2Item> Items
        {
            get { return this.items; }
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
                    this.element.Select2("val", value).TriggerHandler("change", new object[] { true });
            }
        }

        public string Text
        {
            get
            {
                return ((dynamic)element.Select2Get("data") ?? new object()).text;
            }
        }
    }
}